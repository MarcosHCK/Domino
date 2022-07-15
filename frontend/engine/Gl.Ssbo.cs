/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */
using OpenTK.Graphics.OpenGL4;
using System.Collections;
namespace Frontend.Engine;

public partial class Gl
{
  public sealed class Ssbo<T> : IList<T>, IBindable
    where T: IPackable
  {
    static BufferTarget target;
    static BufferRangeTarget range;
    static BufferUsageHint usage;
    static Stack<int> used;
    static int max = -1;
    static int top = 0;

    public int Binding { get => binding; }

    private IList<T> back;
    private int freezed;
    private int binding;
    private int ssbo;

#region API

    public void Freeze () => ++freezed;
    public void Thaw () => freezed = 0;
    public void Unfreeze ()
    {
      if (0 == --freezed)
        Update ();
      if (freezed == -1)
        freezed = 0;
    }

    public void Update ()
    {
      var stream = new MemoryStream ();
      foreach (var packable in back)
        packable.Pack (stream);

      unsafe
      {
        stream.Flush ();
        stream.Close ();
        var array = stream.ToArray ();
        var size = array.Length;

        GL.BindBuffer (target, ssbo);
        GL.BufferData<byte> (target, size, array, usage);
        GL.BindBuffer (target, 0);
      }
    }

    public int IndexOf (T e) => back.IndexOf (e);
    public void Clear () => back.Clear ();
    public bool Contains (T e) => back.Contains (e);
    public void CopyTo (T[] array, int arrayIndex) => back.CopyTo (array, arrayIndex);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator ();
    public IEnumerator<T> GetEnumerator () => back.GetEnumerator ();

    public T this[int i] { get => back [i]; set => back [i] = value; }
    public int Count { get => back.Count; }
    public bool IsReadOnly { get => back.IsReadOnly; }

    public void Add (T e)
    {
      back.Add (e);
      if (freezed == 0)
        Update ();
    }

    public void Insert (int at, T e)
    {
      back.Insert (at, e);
      if (freezed == 0)
        Update ();
    }

    public void RemoveAt (int at)
    {
      back.RemoveAt (at);
      if (freezed == 0)
        Update ();
    }

    bool ICollection<T>.Remove (T e) => Remove (e);

    public bool Remove (T e)
    {
      var was =
      back.Remove (e);
      if (freezed == 0)
        Update ();
      return was;
    }

#endregion

#region Constructors

    public Ssbo ()
    {
      lock (used)
      {
        if (max < 0)
        {
          bool ssbos = false;
          ssbos |= CheckVersion (4, 3);
          ssbos |= CheckExtension ("ARB_shader_storage_buffer_object");
          if (!ssbos)
            {
              throw new Exception ();
            }

          var
          pname = All.MaxShaderStorageBufferBindings;
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
      back = new List<T> ();

      GL.BindBuffer (target, ssbo);
      GL.BindBufferBase (range, binding, ssbo);
      GL.BindBuffer (target, 0);
    }

    static Ssbo ()
    {
      target = BufferTarget.ShaderStorageBuffer;
      range = BufferRangeTarget.ShaderStorageBuffer;
      usage = BufferUsageHint.DynamicRead;
      used = new Stack<int> ();
    }

    ~Ssbo ()
    {
      lock (used)
      {
        GL.DeleteBuffer (ssbo);
        used.Push (binding);
      }
    }

#endregion
  }
}
