/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using System.Collections;

namespace frontend.Gl
{
  public sealed class Ssbo<T> : IList<T>
    where T: notnull
  {
    static BufferTarget target;
    static BufferRangeTarget range;
    static BufferUsageHint usage;
    static Stack<int> used;
    static int top;

    private IList<T> back;
    private bool freezed;
    private int binding;
    private int ssbo;

#region API

    public void Freeze () => freezed = true;
    public void Unfreeze () => freezed = false;
    public void Update ()
    {
      var array = back.ToArray ();
      var sizet = Marshal.SizeOf (typeof (T));
      var size = array.Length * sizet;
      var zone = new ReadOnlyMemory<T> (array);

      unsafe
      {
        var pointer = zone.Pin ().Pointer;
        var data = (IntPtr) pointer;

        GL.BindBuffer (target, ssbo);
        GL.BufferData (target, size, data, usage);
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
      if (!freezed)
        Update ();
    }

    public void Insert (int at, T e)
    {
      back.Insert (at, e);
      if (!freezed)
        Update ();
    }

    public void RemoveAt (int at)
    {
      back.RemoveAt (at);
      if (!freezed)
        Update ();
    }

    bool ICollection<T>.Remove (T e) => Remove (e);

    public bool Remove (T e)
    {
      var was =
      back.Remove (e);
      if (!freezed)
        Update ();
      return was;
    }

#endregion

#region Constructors

    public Ssbo ()
    {
      lock (used)
      {
        if (used.Count > 0)
          binding = used.Pop ();
        else
          binding = top++;
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
      top = 1;
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
