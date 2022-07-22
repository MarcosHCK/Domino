/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Text;

namespace Frontend.Engine
{
  public partial class Gl
  {
    public static string DataDir = ".";
    private static bool opentk_done = false;

    private Program normal;
    private Program solid;
    private Pencil pencil;
    private Camera camera;
    private List<IDrawable> objects;

    public bool ShouldUpdate { get; set; }
    public Camera Viewport { get => camera; }
    public List<IDrawable> Objects { get => objects; }

    public Matrix4 Model4
    {
      set
      {
        var mvp = Matrix4.Mult (value, camera.Jvp);
        var inv = Matrix4.Invert (value);
        matrices [(int) Matrices.aModel] = value;
        matrices [(int) Matrices.aModelInverse] = inv;
        matrices [(int) Matrices.aMvp] = mvp;
      }
    }

#region Vertex shader

    private Ubo<Matrix4> matrices;
    private int locViewPosition;

    enum Matrices
    {
      aProjection = 0,
      aProjectionInverse,
      aView,
      aViewInverse,
      aModel,
      aModelInverse,
      aJvp,
      aMvp,
      NUMBER,
    }

#endregion

#region Fragment shader

    private Ssbo<Light> dirlights;
    private Ssbo<Light> pointlights;
    private Ssbo<Light> spotlights;
    private int locShininess;

#endregion

#region OpenGL API control

    private static Dictionary<string, bool> extensions;
    private static int opengl_major;
    private static int opengl_minor;

    public static bool CheckVersion (int major, int minor)
    {
      return (opengl_major > major
        || (opengl_major == major
          && opengl_minor >= minor));
    }

    public static bool CheckExtension (string name)
    {
      bool value;
      if (extensions.TryGetValue ("GL_" + name, out value))
        return value;
    return false;
    }

#endregion

#region API

    public void Render ()
    {
      GL.Clear
      (ClearBufferMask.ColorBufferBit
      | ClearBufferMask.DepthBufferBit
      | ClearBufferMask.StencilBufferBit);
  
      normal.Use ();

      if (ShouldUpdate)
        {
          Matrix4 matrix;
          Vector3 vec3;

          matrix = camera.Projection;
          matrices [(int) Matrices.aProjection] = matrix;
          matrix = Matrix4.Invert (camera.Projection);
          matrices [(int) Matrices.aProjectionInverse] = matrix;
          matrix = camera.View;
          matrices [(int) Matrices.aView] = matrix;
          matrix = Matrix4.Invert (camera.View);
          matrices [(int) Matrices.aViewInverse] = matrix;
          matrix = camera.Jvp;
          matrices [(int) Matrices.aJvp] = matrix;
          vec3 = camera.Position;
          GL.Uniform3 (locViewPosition, ref vec3);
          ShouldUpdate = false;
        }

      pencil.BindArray ();

      foreach (var object_ in Objects)
        object_.Draw (this);
    }

#endregion

#region Constructors

    static string LoadShaderCode (string name)
    {
      var glsldir = System.IO.Path.Combine (DataDir, "glsl");
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

    private void BindBOs (Program prog)
    {
      int pid = prog.Pid;
      int loc;

      /* UBOs */
      loc = GL.GetUniformBlockIndex (pid, "aMatrices");
      GL.UniformBlockBinding (pid, loc, matrices.Binding);

      /* SSBOs */
      loc = GL.GetProgramResourceIndex (pid, ProgramInterface.ShaderStorageBlock, "bDirLights");
      GL.ShaderStorageBlockBinding (pid, loc, dirlights.Binding);
      loc = GL.GetProgramResourceIndex (pid, ProgramInterface.ShaderStorageBlock, "bPointLights");
      GL.ShaderStorageBlockBinding (pid, loc, pointlights.Binding);
      loc = GL.GetProgramResourceIndex (pid, ProgramInterface.ShaderStorageBlock, "bSpotLights");
      GL.ShaderStorageBlockBinding (pid, loc, spotlights.Binding);
    }

    public Gl ()
    {
      if (opentk_done == false)
        {
          opentk_done = true;
          OpenTK.Graphics.OpenGL.GL.LoadBindings (new Gl.Loader ());
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

              if (CheckVersion (4, 0))
              {
                OpenTK.Graphics.OpenGL4.GL.LoadBindings (new Gl.Loader ());
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

      /* buffers default */
      GL.ClearColor (0, 0, 0, 1);
      GL.ClearDepth (1d);

      /* tune OpenGL */
      GL.Enable (EnableCap.DepthTest);
      GL.Enable (EnableCap.CullFace);
      GL.CullFace (CullFaceMode.Back);
      GL.FrontFace (FrontFaceDirection.Ccw);

      normal = new Program ();
      solid = new Program ();
      pencil = new Pencil ();
      camera = new Camera ();
      matrices = new Ubo<Matrix4> ((int) Matrices.NUMBER);
      dirlights = new Ssbo<Light> ();
      pointlights = new Ssbo<Light> ();
      spotlights = new Ssbo<Light> ();
      objects = new List<IDrawable> ();

      /* camera initial state */
      camera.Position = Vector3.Zero;
      camera.LookAt (0, 0, 0);
      ShouldUpdate = true;

      /* shaders list */
      var shaders = new List<(string, ShaderType)> ();

      /* solid color draw shader */
      shaders.Add ((LoadShaderCode ("model.vs.glsl"), ShaderType.VertexShader));
      shaders.Add ((LoadShaderCode ("solid.fs.glsl"), ShaderType.FragmentShader));
      solid.Link (shaders.ToArray ());
      shaders.Clear ();

      locSolidColor = solid.Uniform ("aColor");
      BindBOs (solid);

      /* normal program (used to draw everything) */
      shaders.Add ((LoadShaderCode ("model.vs.glsl"), ShaderType.VertexShader));
      shaders.Add ((LoadShaderCode ("model.fs.glsl"), ShaderType.FragmentShader));
      normal.Link (shaders.ToArray ());
      shaders.Clear ();

        /* location TODO: put in uniform buffer */
      locViewPosition = normal.Uniform ("aViewPosition");
      locShininess = normal.Uniform ("aShininess");
      BindBOs (normal);

      /* Create lights */

      DirLight ambient;
      ambient = new DirLight ();
      ambient.Ambient = new Vector3 (0.2f, 0.2f, 0.2f);
      ambient.Diffuse = new Vector3 (0.5f, 0.5f, 0.5f);
      ambient.Specular = new Vector3 (1.0f, 1.0f, 1.0f);
      ambient.Direction = new Vector3 (-0.2f, -1.0f, -0.3f);
      dirlights.Add (ambient);

      /* bind texture units */
      normal.Use ();

      var unit = 0;
      foreach (var name in samplerNames)
        GL.Uniform1 (normal.Uniform (name), unit++);
    }

    static Gl ()
    {
      extensions = new Dictionary<string, bool> ();
    }

#endregion
  }
}
