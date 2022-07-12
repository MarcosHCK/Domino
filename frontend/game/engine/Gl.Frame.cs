/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Text;

namespace frontend.Gl
{
  public sealed class Frame
  {
    /* vertex shader */
    private int locMvp;
    private int locJvp;
    private int locProjection;
    private int locView;
    private int locViewPosition;
    private int locModel;
    private int locProjectionInverse;
    private int locViewInverse;
    private int locModelInverse;

    /* Fragment shader */
    private Ssbo<Light> dirlights;
    private Ssbo<Light> pointlights;
    private Ssbo<Light> spotlights;
    private int locShininess;

    /* extensions */
    private bool multiple = false;
    private bool direct = false;

#region Properties

    public bool ShouldUpdate { get; set; }
    public Program Program { get; private set; }
    public Pencil Pencil { get; private set; }
    public Camera Camera { get; private set; }

    public Matrix4 Model
    {
      set
      {
        var mvp = Matrix4.Mult (value, Camera.Jvp);
        var inv = Matrix4.Invert (value);
        GL.UniformMatrix4 (locModel, false, ref value);
        GL.UniformMatrix4 (locModelInverse, false, ref inv);
        GL.UniformMatrix4 (locMvp, false, ref mvp);
      }
    }

    public Material Material
    {
      set
      {
        var material = value;
        var tios = material.Textures;
        GL.Uniform1 (locShininess, material.Shininess);

        if (multiple)
          GL.BindTextures (0, tios.Length, tios);
        else if (direct)
          for (int i = 0; i < tios.Length; i++)
          {
            GL.BindTextureUnit (i, tios [i]);
          }
        else
          for (int i = 0; i < tios.Length; i++)
          {
            GL.ActiveTexture (TextureUnit.Texture0 + i);
            GL.BindTexture (TextureTarget.Texture2DArray, tios [i]);
          }
      }
    }

#endregion

    public void Begin ()
    {
      Program.Use ();
      if (ShouldUpdate)
        {
          Matrix4 matrix;
          Vector3 vec3;

          matrix = Camera.Projection;
          GL.UniformMatrix4 (locProjection, false, ref matrix);
          matrix = Matrix4.Invert (Camera.Projection);
          GL.UniformMatrix4 (locProjectionInverse, false, ref matrix);
          matrix = Camera.View;
          GL.UniformMatrix4 (locView, false, ref matrix);
          vec3 = Camera.Position;
          GL.Uniform3 (locViewPosition, ref vec3);
          matrix = Matrix4.Invert (Camera.View);
          GL.UniformMatrix4 (locViewInverse, false, ref matrix);
          matrix = Camera.Jvp;
          GL.UniformMatrix4 (locJvp, false, ref matrix);
          ShouldUpdate = false;
        }

      Pencil.BindArray ();
    }

#region Constructors

    static string LoadShaderCode (string name)
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

    public Frame ()
    {
      Pencil = new Pencil ();
      Camera = new Camera ();
      Program = new Program ();

      /* check API extensions */
      multiple |= Game.Window.CheckVersion (4, 4);
      multiple |= Game.Window.CheckExtension ("ARB_multi_bind");
      direct |= Game.Window.CheckVersion (4, 5);
      direct |= Game.Window.CheckExtension ("ARB_direct_state_access");

      /* load shaders */
      var shaders = new List<(string, ShaderType)> ();
      shaders.Add ((LoadShaderCode ("model.vs.glsl"), ShaderType.VertexShader));
      shaders.Add ((LoadShaderCode ("model.fs.glsl"), ShaderType.FragmentShader));

      /* link and activate shaders */
      Program.Link (shaders.ToArray ());
      Program.Use ();

      /* clear screen base */
      GL.ClearColor (0, 0, 0, 1);
      GL.ClearDepth (1d);

      /* vertex shader */
      locMvp = Program.Uniform ("aMvp");
      locJvp = Program.Uniform ("aJvp");
      locProjection = Program.Uniform ("aProjection");
      locView = Program.Uniform ("aView");
      locViewPosition = Program.Uniform ("aViewPosition");
      locModel = Program.Uniform ("aModel");
      locProjectionInverse = Program.Uniform ("aProjectionInverse");
      locViewInverse = Program.Uniform ("aViewInverse");
      locModelInverse = Program.Uniform ("aModelInverse");

      dirlights = new Ssbo<Light> ();
      pointlights = new Ssbo<Light> ();
      spotlights = new Ssbo<Light> ();

      int loc;

      loc = GL.GetProgramResourceIndex (Program.Pid, ProgramInterface.ShaderStorageBlock, "bDirLights");
      GL.ShaderStorageBlockBinding (Program.Pid, loc, dirlights.Binding);
      loc = GL.GetProgramResourceIndex (Program.Pid, ProgramInterface.ShaderStorageBlock, "bPointLights");
      GL.ShaderStorageBlockBinding (Program.Pid, loc, pointlights.Binding);
      loc = GL.GetProgramResourceIndex (Program.Pid, ProgramInterface.ShaderStorageBlock, "bSpotLights");
      GL.ShaderStorageBlockBinding (Program.Pid, loc, spotlights.Binding);

      DirLight ambient;
      ambient = new DirLight ();
      ambient.Ambient = new Vector3 (0.2f, 0.2f, 0.2f);
      ambient.Diffuse = new Vector3 (0.5f, 0.5f, 0.5f);
      ambient.Specular = new Vector3 (1.0f, 1.0f, 1.0f);
      ambient.Direction = new Vector3 (-0.2f, -1.0f, -0.3f);
      dirlights.Add (ambient);

      /* fragment shader */
      locShininess = Program.Uniform ("aShininess");

      int unit = 0;
      foreach (var name in Gl.Material.samplerNames)
       GL.Uniform1 (Program.Uniform (name), unit++);
    }

#endregion
  }
}
