/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
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
    private Gl.Program? program;
    private Gl.Frame? frame;
    private Gl.Skybox? skybox;
    private Vector3 defaultcamera;

    public List<Game.Object> Objects { get; private set; }
    public Game.Engine Engine { get; private set; }

    private const int targetFPS = 60;
    private const float fov = 45;
    private bool update = true;
    private int locJvp;
    private int locMvp;
    private uint clock;

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

    private void DoPipeline ()
    {
      frame!.Pencil!.BindArray ();
      foreach (var object_ in Objects)
      {
        object_.Draw (frame!);
      }
    }

    private void DoRender ()
    {
      program!.Use ();
      if (update)
        {
          update = false;
          var camera = frame!.Camera;
          var jvp = camera.Jvp;

          GL.UniformMatrix4 (locJvp, false, ref jvp);

          DoPipeline ();
          skybox!.Jvp = jvp;
          skybox!.Draw ();
        }
      else
        {
          DoPipeline ();
          skybox!.Draw ();
        }
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
      DoRender ();

      a.RetVal = true;
    }

    private void OnResize (object? o, Gtk.ResizeArgs a)
    {
      glarea1!.MakeCurrent ();
      if (glarea1!.Error != IntPtr.Zero)
        throw new Exception ("GL");

      var fovy = MathHelper.DegreesToRadians (fov);
      frame!.Camera.Project (a.Width, a.Height, fovy);
      update = true;

      this.QueueDraw ();
    }

    private void OnDeleteEvent (object? o, Gtk.DeleteEventArgs a)
    {
      Engine.StopAndWait ();
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
            update = true;
            glarea1!.QueueRender ();
            break;
          case Gdk.Key.D:
          case Gdk.Key.d:
            pos = frame!.Camera.Position;
            pos.X += 1;
            frame!.Camera.Position = pos;
            update = true;
            glarea1!.QueueRender ();
            break;
          case Gdk.Key.W:
          case Gdk.Key.w:
            pos = frame!.Camera.Position;
            pos.Y += 1;
            frame!.Camera.Position = pos;
            update = true;
            glarea1!.QueueRender ();
            break;
          case Gdk.Key.S:
          case Gdk.Key.s:
            pos = frame!.Camera.Position;
            pos.Y -= 1;
            frame!.Camera.Position = pos;
            update = true;
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
                  extensions.Add (extension, true);
                }
            }
        }

      GL.ClearColor (0, 0, 0, 1);
      GL.ClearDepth (1d);

      Console.WriteLine ("OpenGL context attached");
      Console.WriteLine ("Version: " + GL.GetString (StringName.Version));
      Console.WriteLine ("Renderer: " + GL.GetString (StringName.Renderer));

      var datadir = frontend.Application.DataDir;
      var glsldir = System.IO.Path.Combine (datadir, "glsl");
      var skyboxdir = System.IO.Path.Combine (datadir, "skybox");

      string LoadShaderCode (string name)
      {
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

      skybox = new Gl.Skybox (skyboxdir, "$s.dds");
      program = new Gl.Program ();
      program.Link (shaders);

      locJvp = program.Uniform ("aJvp");
      locMvp = program.Uniform ("aMvp");
      frame = new Gl.Frame (locMvp);
      Gl.Model.BindUnits (program);

      defaultcamera = new Vector3 (0, 7, 13);
      frame.Camera.Position = defaultcamera;
      frame.Camera.LookAt (0, 0, 0);

      var
      board = new Game.Objects.PieceBoard (2);
      board.Visible = true;
      board.Append (2, 5);

      Objects.Add (board);
      Engine.Start ();

      clock = GLib.Timeout.Add
      (((uint) 1000 / targetFPS),
       () =>
        {
          glarea1!.QueueRender ();
          return true;
        });
    }

    private void OnUnrealize (object? o, EventArgs a)
    {
      Console.WriteLine ("OpenGL context detached");
      GLib.Source.Remove (clock);

      Objects.Clear ();

      program = null;
      frame = null;
      skybox = null;
    }

#endregion

#region Constructors

    public Window (Engine engine) : base (null)
    {
      Gtk.TemplateBuilder.InitTemplate (this);
      Objects = new List<Game.Object> ();
      Engine = engine;
    }

    static Window ()
    {
      extensions = new Dictionary<string, bool> ();
    }

    ~Window ()
    {
      Engine.Stop ();
    }

#endregion
  }
}
