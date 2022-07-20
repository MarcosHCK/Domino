/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */
using OpenTK.Mathematics;

namespace Frontend.Engine
{
  public abstract class Object : Gl.IDrawable, Gl.ILocalizable, Gl.IRotable, Gl.IScalable
  {
    protected Gl.IDrawable Drawable;

    private Matrix4 _Model;
    public Matrix4 Model
    {
      get => _Model;
      set => _Model = value;
    }

    public bool Visible { get; set; }

    private void UpdateModel ()
    {
      var trans = Matrix4.CreateTranslation (_Position);
      var rotat = Matrix4.CreateFromQuaternion (Quaternion);
      var scale = Matrix4.CreateScale (_Scale);
      var tmp = Matrix4.Mult (scale, rotat);
      Model = Matrix4.Mult (tmp, trans);
    }

    private Vector3 _Position;
    public Vector3 Position
    {
      get => _Position;
      set
      {
        _Position = value;
        UpdateModel ();
      }
    }

    private Quaternion _Quaternion;
    public Quaternion Quaternion
    {
      get => _Quaternion;
      set
      {
        _Quaternion = value;
        UpdateModel ();
      }
    }

    private Vector3 _Scale;
    public Vector3 Scale
    {
      get => _Scale;
      set
      {
        _Scale = value;
        UpdateModel ();
      }
    }

    public virtual void Draw (Gl gl)
    {
      gl.Model4 = _Model;
      Drawable.Draw (gl);
    }

#region Constructors

    protected Object (Gl.IDrawable Drawable)
    {
      this.Drawable = Drawable;
      this.Visible = true;
      _Position = new Vector3 (0, 0, 0);
      _Scale = new Vector3 (1, 1, 1);
      _Quaternion = Quaternion.Identity;
    }

#endregion
  }
}
