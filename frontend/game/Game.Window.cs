/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Text;

namespace frontend.Game
{
  [GLib.TypeName ("DominoGameWindow")]
  [Gtk.Template (ResourceName = "gamewindow.ui")]
  public sealed class Window : Gtk.Window
  {
    [Gtk.Builder.Object]
    private Gtk.GLArea? glarea1;
    [Gtk.Builder.Object]
    private Gtk.ToggleButton? keep1;
    [Gtk.Builder.Object]
    private Gtk.Button? forward1;

    private List<(int, int[])> boarded;
    private List<Game.Object> objects;
    private Gl.Frame? frame;

    private Game.Engine.ActionHandler? actionHandler;
    private Game.Engine engine;

    private const int targetFPS = 60;
    private const float fov = 45;

    private static Dictionary<string, bool> extensions;
    private static bool opentk_done = false;
    private static int opengl_major;
    private static int opengl_minor;

    public static bool CheckVersion (int major, int minor) => (opengl_major > major || (opengl_major == major && opengl_minor >= minor));
    public static bool CheckExtension (string name) => extensions.ContainsKey ("GL_" + name);

#region Callbacks

    private void OnCreateContext (object? o, Gtk.CreateContextArgs a)
    {
      var
      context = Window.CreateGlContext ();
      context.DebugEnabled = true;
      context.SetRequiredVersion (3, 3);
      context.SetUseEs (glarea1!.UseEs ? 1 : 0);
      a.RetVal = context;
    }

    private void OnRender (object? o, Gtk.RenderArgs a)
    {
      glarea1!.MakeCurrent ();
      if (glarea1!.Error != IntPtr.Zero)
        {
          a.RetVal = false;
          return;
        }

      GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      frame!.Begin ();

      foreach (var object_ in objects)
        object_.Draw (frame!);

      a.RetVal = true;
    }

    private void OnResize (object? o, Gtk.ResizeArgs a)
    {
      glarea1!.MakeCurrent ();
      if (glarea1!.Error != IntPtr.Zero)
        throw new Exception ("GL");

      var fovy = MathHelper.DegreesToRadians (fov);
      frame!.Camera.Project (a.Width, a.Height, fovy);
      frame!.ShouldUpdate = true;

      this.QueueDraw ();
    }

    private void OnDeleteEvent (object? o, Gtk.DeleteEventArgs a)
    {
      a.RetVal = false;
    }

    private void OnKeyPressed (object? o, Gtk.KeyPressEventArgs a)
    {
      var press = a.Event;
      var mods = press.State;
      Vector3 pos;

      if (!mods.HasFlag (Gdk.ModifierType.ReleaseMask))
      {
        switch (a.Event.Key)
        {
          case Gdk.Key.A:
          case Gdk.Key.a:
            pos = frame!.Camera.Position;
            pos.X -= 1;
            frame!.Camera.Position = pos;
            frame!.Camera.LookAt (pos.X, 0, 0);
            frame!.ShouldUpdate = true;
            glarea1!.QueueRender ();
            break;
          case Gdk.Key.D:
          case Gdk.Key.d:
            pos = frame!.Camera.Position;
            pos.X += 1;
            frame!.Camera.Position = pos;
            frame!.Camera.LookAt (pos.X, 0, 0);
            frame!.ShouldUpdate = true;
            glarea1!.QueueRender ();
            break;
          case Gdk.Key.W:
          case Gdk.Key.w:
            pos = frame!.Camera.Position;
            pos.Y += 1;
            frame!.Camera.Position = pos;
            frame!.ShouldUpdate = true;
            glarea1!.QueueRender ();
            break;
          case Gdk.Key.S:
          case Gdk.Key.s:
            pos = frame!.Camera.Position;
            pos.Y -= 1;
            frame!.Camera.Position = pos;
            frame!.ShouldUpdate = true;
            glarea1!.QueueRender ();
            break;
        }
      }
    }

    private void OnRealize (object? o, EventArgs a)
    {
      glarea1!.MakeCurrent ();
      if (glarea1!.Error != IntPtr.Zero)
        throw new Exception ("GL");

      if (opentk_done == false)
        {
          opentk_done = true;
          GL.LoadBindings (new Gl.Loader ());
          opengl_major = GL.GetInteger (GetPName.MajorVersion);
          opengl_minor = GL.GetInteger (GetPName.MinorVersion);

          if (! CheckVersion (3, 3))
            throw new Exception ("OpenGL 3.3 or higher required");
          else
            {
              var available = GL.GetInteger (GetPName.NumExtensions);
              for (int i = 0; i < available; i++)
                {
                  var extension = GL.GetString (StringNameIndexed.Extensions, i);
                  extensions.TryAdd (extension, true);
                }
            }
        }

      Console.WriteLine ("OpenGL context attached");
      Console.WriteLine ("Version: " + GL.GetString (StringName.Version));
      Console.WriteLine ("Renderer: " + GL.GetString (StringName.Renderer));

      if (CheckVersion (4, 3)
        || CheckExtension ("KHR_debug"))
        {
          var sources = DebugSourceControl.DontCare;
          var types = DebugTypeControl.DontCare;
          var severities = DebugSeverityControl.DontCare;

          GL.DebugMessageControl (sources, types, severities, 0, (int[]) null!, true);

          GL.DebugMessageCallback (
          (source, type, id, severity, length, message, userParam) =>
          {
            var content = Marshal.PtrToStringUTF8 (message, length);
            if (severity != DebugSeverity.DebugSeverityNotification
              && severity != DebugSeverity.DontCare)
              Console.Error.WriteLine ("{0} {2}, {1} {3}: {4}", source, type, id, severity, content);
            else
              Console.WriteLine ("{0} {2}, {1} {3}: {4}", source, type, id, severity, content);
          }, IntPtr.Zero);
        }

      string LoadShaderCode (string name)
      {
        var datadir = frontend.Application.DataDir;
        var glsldir = System.IO.Path.Combine (datadir, "glsl");
        var fullpath = System.IO.Path.Combine (glsldir, name);
        using (var stream = new FileStream (fullpath, FileMode.Open))
          {
            if (stream == null)
              throw new Exception ("can't find resource " + name);
            else
              {
                var length = (int) stream.Length;
                var bytes = new byte [length];
                stream.Read (bytes, 0, length);
                stream.Close ();

                return Encoding.UTF8.GetString (bytes);
              }
          }
      }

      var shaders = new (string, ShaderType) [2];

      if (glarea1!.UseEs)
        {
          shaders [0] = (LoadShaderCode ("model-es.vs.glsl"), ShaderType.VertexShader);
          shaders [1] = (LoadShaderCode ("model-es.fs.glsl"), ShaderType.FragmentShader);
        }
      else
        {
          shaders [0] = (LoadShaderCode ("model.vs.glsl"), ShaderType.VertexShader);
          shaders [1] = (LoadShaderCode ("model.fs.glsl"), ShaderType.FragmentShader);
        }

      frame = new Gl.Frame ();

      var camera_at = new Vector3 (0, 7, 13);
      frame.Camera.Position = camera_at;
      frame.Camera.LookAt (0, 0, 0);
      frame.ShouldUpdate = true;

      var
      board = new Objects.PieceBoard (2);
      board.Visible = true;
      objects.Add (board);

      var
      skybox = new Game.Skybox ();
      skybox.Direction = Vector3.UnitY;
      skybox.Angle = MathHelper.DegreesToRadians (180);
      skybox.Position = Vector3.Zero;
      skybox.Scale = Vector3.One * 60;
      skybox.Visible = true;
      objects.Add (skybox);

      foreach (var tuple in boarded)
        {
          board.Append (tuple.Item1, tuple.Item2);
        }

      actionHandler = (o, arg) =>
        {
          if (arg is Engine.MoveArgs)
          {
            var a = (Engine.MoveArgs) arg;
            var putat = a.PutAt;
            var piece = a.Piece;
            if (piece != null)
            {
              GLib.Idle.Add (() =>
              {
                boarded.Add ((putat, piece));
                board.Append (putat, piece);
                glarea1!.QueueRender ();
                return false;
              });
            }
          }
        };

      engine.Action += actionHandler;
    }

    private void OnUnrealize (object? o, EventArgs a)
    {
      engine.Action -= actionHandler;

      objects.Clear ();
      frame = null;

      Console.WriteLine ("OpenGL context detached");
    }

#endregion

#region Constructors

    public Window (Engine engine) : base (null)
    {
      Gtk.TemplateBuilder.InitTemplate (this);

      this.boarded = new List<(int, int[])> ();
      this.objects = new List<Game.Object> ();
      this.engine = engine;

      forward1!.Clicked += (o, a) =>
      {
        if (frame != null)
        {
          engine.PollNext ();
        }
      };

      var clock = (uint) 0;
      keep1!.AddNotification ("active", (o, a) =>
        {
          var keep1 = (Gtk.ToggleButton) o!;
          if (keep1.Active && clock == 0)
          {
            Console.WriteLine ("activating");
            clock = GLib.Timeout.Add (400, () =>
            {
              engine.PollNext ();
              return true;
            });
          }
          else
          if (!keep1.Active && clock != 0)
          {
            Console.WriteLine ("deactivating");
            GLib.Timeout.Remove (clock);
            clock = 0;
          }
        });
    }

    static Window ()
    {
      extensions = new Dictionary<string, bool> ();
    }

#endregion
  }
}
