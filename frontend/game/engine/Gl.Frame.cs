/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace frontend.Gl
{
  public sealed class Frame
  {
    private int locMvp;
    public Pencil Pencil { get; private set; }
    public Camera Camera { get; private set; }

    public Matrix4 Model
    {
      set
      {
        var mvp = Matrix4.Mult (value, Camera.Jvp);
        GL.UniformMatrix4 (locMvp, false, ref mvp);
      }
    }

    public Frame (int locMvp)
    {
      Pencil = new Pencil ();
      Camera = new Camera ();
      this.locMvp = locMvp;
    }
  }
}
