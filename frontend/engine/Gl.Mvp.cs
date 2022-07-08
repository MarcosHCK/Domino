/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Mathematics;

namespace frontend.Gl
{
  public sealed class Mvp : ILocalizable
  {
    private Matrix4 _Model;
    public Matrix4 Model
    {
      get => _Model;
      set
      {
        _Model = value;
        Full = Matrix4.Mult (_Model, _Jvp);
      }
    }

    private Matrix4 _View;
    public Matrix4 View
    { get => _View;
      set
      {
        _View = value;
        Jvp = Matrix4.Mult (_View, _Projection);
      }
    }

    private Matrix4 _Projection;
    public Matrix4 Projection
    {
      get => _Projection;
      set
      {
        _Projection = value;
        Jvp = Matrix4.Mult (_View, _Projection);
      }
    }

    private Vector3 _Position;
    public Vector3 Position
    {
      get => _Position;
      set
      {
        _Position = value;
        Jvp = Matrix4.Mult (_View, _Projection);
      }
    }

    private Matrix4 _Jvp;
    public Matrix4 Jvp
    {
      get => _Jvp;
      private set
      {
        _Jvp = value;
        Full = Matrix4.Mult (_Model, _Jvp);
      }
    }

    private Matrix4 _Full;
    public Matrix4 Full
    {
      get => _Full;
      private set
      {
        _Full = value;
      }
    }

    public void Project (int width, int height, float fov)
    {
      var fovy = MathHelper.DegreesToRadians (fov);
      var aspect = ((float) width) / ((float) height);
      Projection = Matrix4.CreatePerspectiveFieldOfView (fovy, aspect, 0.1f, 100);
    }

    private static float max_pitch = MathHelper.DegreesToRadians (89);
    private static float min_pitch = MathHelper.DegreesToRadians (-89);
    private static float angle_center = MathHelper.DegreesToRadians (0);

    public void LookAt (float yaw, float pitch)
    {
      if (pitch > max_pitch)
        pitch = max_pitch;
      else
      if (pitch < min_pitch)
        pitch = min_pitch;

      var worldup = new Vector3 (0, 1, 0);
      var front = new Vector3 ();

      front.X = (float) (MathHelper.Cos (yaw) * MathHelper.Cos (pitch));
      front.Y = (float) (MathHelper.Sin (pitch));
      front.Z = (float) (MathHelper.Sin (yaw) * MathHelper.Cos (pitch));

      var right = Vector3.Cross (worldup, front);
      var up = Vector3.Cross (front, right);
      var center = Vector3.Add (Position, front);
      View = Matrix4.LookAt (Position, center, up);
    }

    public void LookAt (float x, float y, float z)
    {
      var center = new Vector3 (x, y, z);
      var up = new Vector3 (0, 1, 0);
      View = Matrix4.LookAt (Position, center, up);
    }

    public void MoveAt (float x, float y, float z)
    {
      Position = new Vector3 (x, y, z);
    }

    public Mvp ()
    {
      MoveAt (0, 0, 0);
      LookAt (angle_center, angle_center);
    }
  }
}
