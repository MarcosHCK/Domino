/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */
using OpenTK.Graphics.OpenGL;
namespace Frontend.Engine;

public partial class Gl
{
  public sealed class Program
  {
    private int _Pid;
    public int Pid { get => _Pid; }
    private bool separate;

    public void Use () => GL.UseProgram (_Pid);
    public int Uniform (string name) => GL.GetUniformLocation (_Pid, name);

    public void Link ()
    {
      var result = new int [1];
      var length = new int [1];

      GL.LinkProgram (_Pid);
      GL.GetProgram (_Pid, GetProgramParameterName.LinkStatus, result);
      GL.GetProgram (_Pid, GetProgramParameterName.InfoLogLength, length);

      if (result[0] == (int) All.False)
        {
          string infoLog;
          int Length;

          GL.GetProgramInfoLog (_Pid, length [0], out Length, out infoLog);
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

    public void SetUniform (int loc, int value)
    {
      if (!separate) throw new NotSupportedException ();
      GL.ProgramUniform1 (_Pid, loc, value);
    }

    public void SetUniform (int loc, uint value)
    {
      if (!separate) throw new NotSupportedException ();
      GL.ProgramUniform1 (_Pid, loc, value);
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

    public sealed class Shader
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
            GL.GetShader (sid, ShaderParameter.ShaderType, result);
            var message = $"can't compile {(ShaderType) result [0]}: {infoLog}";
            throw new ShaderException (message);
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
      public void Attach (Program program) => GL.AttachShader (program._Pid, sid);
      public void Detach (Program program) => GL.DetachShader (program._Pid, sid);
      public Shader (ShaderType type) => sid = GL.CreateShader (type);
      ~Shader () => GL.DeleteShader (sid);
    }

#region Constructors

    public Program()
    {
      _Pid = GL.CreateProgram();

      separate = false;
      separate |= CheckVersion (4, 1);
      separate |= CheckExtension ("ARB_separate_shader_objects");
    }

    ~Program()
    {
      GL.DeleteProgram(_Pid);
    }

#endregion
  }
}
