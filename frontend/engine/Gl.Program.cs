/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace frontend.Gl
{
  public class Program
  {
    private int pid;

    public void Use () => GL.UseProgram (pid);
    public int Uniform (string name) => GL.GetUniformLocation (pid, name);
    public Program () => pid = GL.CreateProgram ();
    ~Program () => GL.DeleteProgram (pid);

    public void Link ()
    {
      var result = new int [1];
      var length = new int [1];

      GL.LinkProgram (pid);
      GL.GetProgram (pid, GetProgramParameterName.LinkStatus, result);
      GL.GetProgram (pid, GetProgramParameterName.InfoLogLength, length);

      if (result[0] == (int) All.False)
        {
          string infoLog;
          int Length;

          GL.GetProgramInfoLog (pid, length [0], out Length, out infoLog);
          throw new ProgramException ("can't link: " + infoLog);
        }
    }

    public void Link (params Shader[] shaders)
    {
      foreach (var shader in shaders)
        shader.Attach (this);

      try
        {
          Link ();
        } 
      catch (ProgramException)
        {
          foreach (var shader in shaders)
            shader.Detach (this);
            throw;
        }

      foreach (var shader in shaders)
        shader.Detach (this);
    }

    public void Link (params (string code, ShaderType type)[] descs)
    {
      var shaders = new Shader [descs.Count ()];
      uint i = 0;

      foreach (var desc in descs)
        {
          var code = desc.code;
          var type = desc.type;
          var shader = new Shader (type);

          shader.Load (code);
          shader.Compile ();
          shaders [i++] = shader;
        }

      Link (shaders);
    }

    [System.Serializable]
    public class ProgramException : System.Exception
    {
      public ProgramException () { }
      public ProgramException (string message) : base(message) { }
      public ProgramException (string message, System.Exception inner) : base(message, inner) { }
      protected ProgramException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class Shader
    {
      private int sid;

      public void Compile ()
      {
        var result = new int [1];
        var length = new int [1];

        GL.CompileShader (sid);
        GL.GetShader (sid, ShaderParameter.CompileStatus, result);
        GL.GetShader (sid, ShaderParameter.InfoLogLength, length);

        if (result[0] == (int) All.False)
          {
            string infoLog;
            int Length;

            GL.GetShaderInfoLog (sid, length [0], out Length, out infoLog);
            throw new Exception ("can't compile: " + infoLog);
          }
      }

      [System.Serializable]
      public class ShaderException : System.Exception
      {
        public ShaderException () { }
        public ShaderException (string message) : base(message) { }
        public ShaderException (string message, System.Exception inner) : base(message, inner) { }
        protected ShaderException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
      }

      public void Load (string code) => GL.ShaderSource (sid, 1, new string [] { code }, new int [] { code.Length });
      public void Attach (Program program) => GL.AttachShader (program.pid, sid);
      public void Detach (Program program) => GL.DetachShader (program.pid, sid);
      public Shader (ShaderType type) => sid = GL.CreateShader (type);
      ~Shader () => GL.DeleteShader (sid);
    }
  }
}
