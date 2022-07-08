/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace frontend.Gl
{
  public sealed class Pencil
  {
    private int vao;
    private bool attrfmt;

#region Vertex

    public unsafe struct Vertex
    {
      public fixed float position [3];
      public fixed float normal [3];
      public fixed float uvw [3];
      public fixed float tangent [3];
      public fixed float bitangent [3];
    }

    public static int VertexSize { get; private set; }

    static readonly int [] offsets =
    new int[]
    {
      (int) Marshal.OffsetOf <Vertex> ("position"),
      (int) Marshal.OffsetOf <Vertex> ("normal"),
      (int) Marshal.OffsetOf <Vertex> ("uvw"),
      (int) Marshal.OffsetOf <Vertex> ("tangent"),
      (int) Marshal.OffsetOf <Vertex> ("bitangent"),
    };

    static readonly int [] floats =
    new int[]
    {
      3, 3, 3, 3, 3,
    };

#endregion

    public void BindArray () => GL.BindVertexArray (vao);

    public void BindBuffer (int vbo)
    {
      if (attrfmt)
        GL.BindVertexBuffer (0, vbo, IntPtr.Zero, VertexSize);
      else
        {
          GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);
          for (int i = 0; i < offsets.Length; i++)
          {
            var type = VertexAttribPointerType.Float;
            var offset = (IntPtr) offsets [i];

            GL.VertexAttribPointer (i, floats [i], type, false, VertexSize, offset);
          }
        }
    }

    public void UnbindBuffer ()
    {
      GL.BindBuffer (BufferTarget.ArrayBuffer, 0);
    }

#region Constructor

    public Pencil ()
    {
      vao =
      GL.GenVertexArray ();
      GL.BindVertexArray (vao);

      attrfmt  = GameWindow.CheckVersion (4, 3);
      attrfmt |= GameWindow.CheckExtension ("ARB_vertex_attrib_binding");

      for (int i = 0; i < offsets.Length; i++)
        {
          GL.EnableVertexAttribArray (i);
          if (attrfmt)
          {
            GL.VertexAttribFormat (i, floats [i], VertexAttribType.Float, false, offsets [i]);
            GL.VertexAttribBinding (i, 0);
          }
        }

      GL.BindVertexArray (0);
    }

    static Pencil ()
    {
      if (offsets.Length != floats.Length)
        throw new Exception ();
      if (typeof (Vertex).GetFields ().Length != floats.Length)
        throw new Exception ();

      unsafe
      {
        VertexSize = sizeof (Vertex);
      }
    }

    ~ Pencil ()
    {
      GL.DeleteVertexArray (vao);
    }

#endregion
  }
}
