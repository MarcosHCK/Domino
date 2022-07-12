/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
namespace Engine;

public partial class Gl
{
  public sealed class Ubo<T> : IBindable
    where T: struct
  {
    static BufferTarget target;
    static BufferRangeTarget range;
    static BufferUsageHint usage;
    static Stack<int> used;
    static int max = -1;
    static int top = 0;

    public int Binding { get => binding; }

    public T this [int i]
    {
      get
      {
        if (i < 0 || i >= length)
          throw new Exception ();
        else
        {
          return Marshal.PtrToStructure<T> (buffer + stride * i);
        }
      }

      set
      {
        if (i < 0 || i >= length)
          throw new Exception ();
        else
        {
          Marshal.StructureToPtr<T> (value, buffer + stride * i, true);
          Update (i);
        }
      }
    }

    private IntPtr buffer;
    private int stride;
    private int length;
    private int size;
    private int binding;
    private int ssbo;

#region API

    public void Update ()
    {
      GL.BindBuffer (target, ssbo);
      GL.BufferData (target, size, buffer, usage);
      GL.BindBuffer (target, 0);
    }

    public void Update (int index)
    {
      var offset = index * stride;
      var data = buffer + offset;
      GL.BindBuffer (target, ssbo);
      GL.BufferSubData (target, (IntPtr) offset, stride, data);
      GL.BindBuffer (target, 0);
    }

#endregion

#region Constructors

    public Ubo (int length)
    {
      lock (used)
      {
        if (max < 0)
        {
          var
          pname = All.MaxUniformBufferBindings;
          max = GL.GetInteger ((GetPName) pname);
        }

        if (used.Count > 0)
          binding = used.Pop ();
        else
          {
            binding = top++;
            if (binding > max)
            {
              throw new Exception ();
            }
          }
      }

      ssbo = GL.GenBuffer ();
      stride = Marshal.SizeOf (typeof (T));
      size = stride * length;
      buffer = Marshal.AllocHGlobal (size);
      this.length = length;

      unsafe
      {
        (new Span<T> ((void*) buffer, length)).Clear ();
      }

      GL.BindBuffer (target, ssbo);
      GL.BufferData (target, size, IntPtr.Zero, usage);
      GL.BindBufferBase (range, binding, ssbo);
      GL.BindBuffer (target, 0);
    }

    static Ubo ()
    {
      target = BufferTarget.UniformBuffer;
      range = BufferRangeTarget.UniformBuffer;
      usage = BufferUsageHint.DynamicRead;
      used = new Stack<int> ();
    }

    ~Ubo ()
    {
      lock (used)
      {
        Marshal.FreeHGlobal (buffer);
        GL.DeleteBuffer (ssbo);
        used.Push (binding);
      }
    }

#endregion
  }
}
