/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Mathematics;

namespace frontend.Game
{
  public abstract class Object : Gl.IDrawable, Gl.ILocalizable, Gl.IRotable, Gl.IScalable
  {
    private Gl.IDrawable drawable;

    private Matrix4 _Model;
    public Matrix4 Model
    {
      get => _Model;
      set => _Model = value;
    }

    public bool Visible
    {
      get => drawable.Visible;
      set => drawable.Visible = value;
    }

    private void UpdateModel ()
    {
      var trans = Matrix4.CreateTranslation (_Position);
      var rotat = Matrix4.CreateFromAxisAngle (_Direction, _Angle);
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

    private float _Angle;
    public float Angle
    {
      get => _Angle;
      set
      {
        _Angle = value;
        UpdateModel ();
      }
    }

    private Vector3 _Direction;
    public Vector3 Direction
    {
      get => _Direction;
      set
      {
        _Direction = value;
        UpdateModel ();
      }
    }

    public virtual void Draw (Gl.Frame frame)
    {
      frame.Model = _Model;
      drawable.Draw (frame);
    }

#region Constructors

    protected Object (Gl.IDrawable drawable)
    {
      this.drawable = drawable;
      _Position = new Vector3 (0, 0, 0);
      _Scale = new Vector3 (1, 1, 1);
      _Direction = new Vector3 (1, 0, 0);
      UpdateModel ();
    }

#endregion
  }
}
