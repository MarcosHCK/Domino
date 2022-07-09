/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace frontend.Gl
{
  public sealed class Camera : ILocalizable
  {
    private Vector3 _Position;
    public Vector3 Position
    {
      get => _Position;
      set
      {
        _Position = value;
        View = Matrix4.LookAt (_Position, _Target, worldup);
      }
    }

    private Vector3 _Target;
    public Vector3 Target
    {
      get => _Target;
      set
      {
        _Target = value;
        View = Matrix4.LookAt (_Position, _Target, worldup);
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

    private Matrix4 _Jvp;
    public Matrix4 Jvp
    {
      get => _Jvp;
      private set
      {
        _Jvp = value;
      }
    }

    private static Vector3 worldup = new Vector3 (0, 1, 0);
    private static float max_pitch = MathHelper.DegreesToRadians (89);
    private static float min_pitch = MathHelper.DegreesToRadians (-89);
    private static float angle_center = MathHelper.DegreesToRadians (0);

    public void Project (int width, int height, float fovy)
    {
      var aspect = ((float) width) / ((float) height);
      Projection = Matrix4.CreatePerspectiveFieldOfView (fovy, aspect, 0.1f, 100f);
    }

    public void LookAt (float yaw, float pitch)
    {
      if (pitch > max_pitch)
        pitch = max_pitch;
      else
      if (pitch < min_pitch)
        pitch = min_pitch;

      Vector3 front;
      front.X = (float) (MathHelper.Cos (yaw) * MathHelper.Cos (pitch));
      front.Y = (float) (MathHelper.Sin (pitch));
      front.Z = (float) (MathHelper.Sin (yaw) * MathHelper.Cos (pitch));
      front.Normalize ();

      var right = Vector3.Cross (worldup, front);
          right.Normalize ();
      var up = Vector3.Cross (front, right);
          up.Normalize ();

      _Target = _Position + front;
      View = Matrix4.LookAt (_Position, _Target, up);
    }

    public void LookAt (float x, float y, float z)
    {
      Target = new Vector3 (x, y, z);
    }

    public void MoveAt (float x, float y, float z)
    {
      Position = new Vector3 (x, y, z);
    }
  }
}
