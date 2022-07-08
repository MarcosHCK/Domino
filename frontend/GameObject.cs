/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Mathematics;

namespace frontend
{
  public class GameObject : Gl.IDrawable, Gl.ILocalizable, Gl.IRotable, Gl.IScalable, Gl.IModelable
  {
    private Gl.IDrawable model;
    public Matrix4 Model { get; set; }

    private bool _Visible;
    public bool Visible
    {
      get => _Visible;
      set
      {
        _Visible = value;
        model.Visible = value;
      }
    }

    private void UpdateModel ()
    {
      var trans = Matrix4.CreateTranslation (_Position);
      var rotat = Matrix4.CreateFromAxisAngle (_Direction, _Angle);
      var scale = Matrix4.CreateScale (_Scale);
      var tmp = Matrix4.Mult (rotat, scale);
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

    public void Draw (Gl.Pencil pencil)
    {
      if (Visible)
      {
        model.Draw (pencil);
      }
    }

#region Constructors

    protected GameObject (Gl.IDrawable model)
    {
      this.model = model;
      _Position = new Vector3 (0, 0, 0);
      _Scale = new Vector3 (1, 1, 1);
      Direction = new Vector3 (1, 0, 0);
    }

    public GameObject (string model) : this (new Gl.SingleModel (model)) { }

#endregion
  }
}
