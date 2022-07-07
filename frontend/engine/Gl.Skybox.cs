/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Text.RegularExpressions;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Text;

namespace frontend.Gl
{
  public sealed class Skybox
  {
    private Program pid;
    private int locSkybox;
    private int locJvp;

    public bool Visible { get; set; }

    private bool ActiveTexUnit = false;
    public bool UseEs = false;

    private int vao;
    private int tio;
    private int vbo;
    private int ebo;

    private Matrix4 _Jvp;
    public Matrix4 Jvp
    {
      get => _Jvp;
      set
      {
        pid.Use ();
        _Jvp = value;
        GL.UniformMatrix4 (locJvp, false, ref _Jvp);
      }
    }

#region  Static fields

    private static Regex regex;

    static readonly float[] cubeVertices =
    new float []
    {
      -1.0f,  1.0f, -1.0f,
      -1.0f, -1.0f, -1.0f,
       1.0f, -1.0f, -1.0f,
       1.0f, -1.0f, -1.0f,
       1.0f,  1.0f, -1.0f,
      -1.0f,  1.0f, -1.0f,

      -1.0f, -1.0f,  1.0f,
      -1.0f, -1.0f, -1.0f,
      -1.0f,  1.0f, -1.0f,
      -1.0f,  1.0f, -1.0f,
      -1.0f,  1.0f,  1.0f,
      -1.0f, -1.0f,  1.0f,

       1.0f, -1.0f, -1.0f,
       1.0f, -1.0f,  1.0f,
       1.0f,  1.0f,  1.0f,
       1.0f,  1.0f,  1.0f,
       1.0f,  1.0f, -1.0f,
       1.0f, -1.0f, -1.0f,

      -1.0f, -1.0f,  1.0f,
      -1.0f,  1.0f,  1.0f,
       1.0f,  1.0f,  1.0f,
       1.0f,  1.0f,  1.0f,
       1.0f, -1.0f,  1.0f,
      -1.0f, -1.0f,  1.0f,

      -1.0f,  1.0f, -1.0f,
       1.0f,  1.0f, -1.0f,
       1.0f,  1.0f,  1.0f,
       1.0f,  1.0f,  1.0f,
      -1.0f,  1.0f,  1.0f,
      -1.0f,  1.0f, -1.0f,

      -1.0f, -1.0f, -1.0f,
      -1.0f, -1.0f,  1.0f,
       1.0f, -1.0f, -1.0f,
       1.0f, -1.0f, -1.0f,
      -1.0f, -1.0f,  1.0f,
       1.0f, -1.0f,  1.0f,
    };

    static readonly int[] cubeIndices =
    new int[]
    {
      0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0,
    };

    static readonly string[] cubeSides =
    new string[]
      {
        "right",
        "left",
        "top",
        "bottom",
        "front",
        "back",
      };

    static readonly TextureTarget[] cubeFaces =
    new TextureTarget[]
    {
      TextureTarget.TextureCubeMapPositiveX,
      TextureTarget.TextureCubeMapNegativeX,
      TextureTarget.TextureCubeMapPositiveY,
      TextureTarget.TextureCubeMapNegativeY,
      TextureTarget.TextureCubeMapPositiveZ,
      TextureTarget.TextureCubeMapNegativeZ,
    };

#endregion

    private Skybox (Program pid)
    {
      this.pid = pid;
      var shaders = new (string, ShaderType) [2];

      var datadir = frontend.Application.DataDir;
      var glsldir = System.IO.Path.Combine (datadir, "glsl");

      if (GameWindow.CheckVersion (4, 5))
        ActiveTexUnit = true;
      else
        {
          if (GameWindow.CheckExtension ("ARB_direct_state_access"))
            ActiveTexUnit = true;
        }

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

      if (UseEs)
        {
          shaders [0] = (LoadShaderCode ("skybox-es.vs.glsl"), ShaderType.VertexShader);
          shaders [1] = (LoadShaderCode ("skybox-es.fs.glsl"), ShaderType.FragmentShader);
        }
      else
        {
          shaders [0] = (LoadShaderCode ("skybox.vs.glsl"), ShaderType.VertexShader);
          shaders [1] = (LoadShaderCode ("skybox.fs.glsl"), ShaderType.FragmentShader);
        }

      pid.Link (shaders);
      locSkybox = pid.Uniform ("aSkybox");
      locJvp = pid.Uniform ("aJvp");

      pid.Use ();
      GL.Uniform1 (locSkybox, 0);
      GL.UniformMatrix4 (locJvp, false, ref _Jvp);
    }

    private Skybox () : this (new Program())
    {
      vao = GL.GenVertexArray ();
      tio = GL.GenTexture ();
      vbo = GL.GenBuffer ();
      ebo = GL.GenBuffer ();

      GL.BindVertexArray (vao);

      GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);
      GL.BufferData (BufferTarget.ArrayBuffer, cubeVertices.Length * sizeof (float), cubeVertices, BufferUsageHint.StaticDraw);
      GL.BindBuffer (BufferTarget.ElementArrayBuffer, ebo);
      GL.BufferData (BufferTarget.ElementArrayBuffer, cubeIndices.Length * sizeof (int), cubeIndices, BufferUsageHint.StaticDraw);

      GL.EnableVertexAttribArray (0);
      GL.VertexAttribPointer (0, 3, VertexAttribPointerType.Float, false, 3 * sizeof (float), 0);

      GL.BindBuffer (BufferTarget.ElementArrayBuffer, 0);
      GL.BindBuffer (BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray (0);
    }

    public Skybox (string sourceDir, string fmtString) : this ()
    {
      MatchEvaluator eval;
      string filename;
      string path;
      Gl.Dds dds;
      int i;

      GL.BindTexture (TextureTarget.TextureCubeMap, tio);
      Visible = true;

      for (i = 0; i < cubeSides.Length; i++)
        {
          eval = (match) =>
            {
              switch (match.Value [1])
              {
              case 'i': return i.ToString ();
              case 's': return cubeSides [i];
              default: return match.Value;
              }
            };

          filename = regex.Replace (fmtString, eval);
          path = Path.Combine (sourceDir, filename);

          dds = new Gl.Dds (path);
          dds.Load2D (cubeFaces [i], true);
        }

      GL.TexParameter (TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int) All.Linear);
      GL.TexParameter (TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int) All.Linear);
      GL.TexParameter (TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int) All.ClampToEdge);
      GL.TexParameter (TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int) All.ClampToEdge);
      GL.TexParameter (TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int) All.ClampToEdge);
      GL.BindTexture(TextureTarget.TextureCubeMap, 0);
    }

    static Skybox ()
    {
      regex = new Regex ("\\$[a-zA-Z]", RegexOptions.Compiled);

      if (cubeFaces.Length != cubeSides.Length)
        throw new Exception ();
      if (cubeVertices.Length % 3 != 0)
        throw new Exception ();
      if (cubeVertices.Length / 3 != cubeIndices.Length)
        throw new Exception ();
    }

    ~Skybox ()
    {
      GL.DeleteVertexArray (vao);
      GL.DeleteTexture (tio);
      GL.DeleteBuffer (vbo);
      GL.DeleteBuffer (ebo);
    }

    public void Draw ()
    {
      if (!Visible)
        return;

      pid.Use ();
      GL.DepthFunc (DepthFunction.Lequal);

      if (ActiveTexUnit)
        GL.BindTextureUnit (0, tio);
      else
        {
          GL.ActiveTexture (TextureUnit.Texture0);
          GL.BindTexture (TextureTarget.TextureCubeMap, tio);
        }

      GL.BindVertexArray (vao);
      GL.DrawArrays (PrimitiveType.Triangles, 0, cubeIndices.Length);
      GL.BindTexture (TextureTarget.TextureCubeMap, 0);
      GL.BindVertexArray (0);
      GL.DepthFunc (DepthFunction.Less);
    }
  }
}
