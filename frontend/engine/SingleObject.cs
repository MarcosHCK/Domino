/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */
using OpenTK.Mathematics;

namespace Frontend.Engine
{
  public abstract class SingleObject : Object
  {
    public Vector3 RawSize
    {
      get
      {
        var x = ((Gl.SingleModel) Drawable).Width;
        var y = ((Gl.SingleModel) Drawable).Height;
        var z = ((Gl.SingleModel) Drawable).Depth;
        return new Vector3 (x, y, z);
      }
    }

    public Vector3 ScaledSize
    {
      get
      {
        var v2 = RawSize;
        var mat = Matrix3.CreateScale (Scale);
        return v2 * mat;
      }
    }

    public Vector3 Size
    {
      get
      {
        var v2 = Vector3.TransformVector (RawSize, Model);
        v2.X = Math.Abs (v2.X);
        v2.Y = Math.Abs (v2.Y);
        v2.Z = Math.Abs (v2.Z);
        return v2;
      }
    }

    protected SingleObject (Gl.SingleModel model)
      : base (model) { }
  }
}
