/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Graphics.OpenGL;
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
    private Gl.Pencil? pencil;
    private Gl.Skybox? skybox;
    private Gl.Mvp mvps;

    public List<Game.Object> Objects { get; private set; }

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
      pencil!.BindArray ();
      foreach (var object_ in Objects)
      {
        mvps.Model = object_.Model;
        var mvp = mvps.Full;

        GL.UniformMatrix4 (locMvp, false, ref mvp);
        object_.Draw (pencil!);
      }
    }

    private void DoRender ()
    {
      program!.Use ();
      if (update)
        {
          update = false;
          var jvp = mvps.Jvp;
          GL.UniformMatrix4 (locJvp, false, ref jvp);
          var mvp = mvps.Full;
          GL.UniformMatrix4 (locMvp, false, ref mvp);

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
      GL.Flush ();

      a.RetVal = true;
    }

    private void OnResize (object? o, Gtk.ResizeArgs a)
    {
      glarea1!.MakeCurrent ();
      if (glarea1!.Error != IntPtr.Zero)
        throw new Exception ("GL");

      update = true;
      mvps.Project (a.Width, a.Height, fov);
      this.QueueDraw ();
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

      var program = new Gl.Program ();
      var shaders = new (string, ShaderType) [2];
      var skybox = new Gl.Skybox (skyboxdir, "$s.dds");

      this.pencil = new Gl.Pencil ();
      this.program = program;
      this.skybox = skybox;

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

      program.Link (shaders);

      locJvp = program.Uniform ("aJvp");
      locMvp = program.Uniform ("aMvp");
      Gl.Model.BindUnits (program);

      clock = GLib.Timeout.Add
      (((uint) 1000 / targetFPS),
       () =>
        {
          return true;
        });
    }

    private void OnUnrealize (object? o, EventArgs a)
    {
      GLib.Source.Remove (clock);
      program = null;
      pencil = null;
      skybox = null;
    }

#endregion

#region Constructors


    public Window () : base (null)
    {
      Gtk.TemplateBuilder.InitTemplate (this);
      Objects = new List<Game.Object> ();
      mvps = new Gl.Mvp ();
    }

    public Window (Rule rule) : this () { }

    static Window ()
    {
      extensions = new Dictionary<string, bool> ();
    }

#endregion
  }
}
