/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace frontend.Gl
{
  public class Mvp
  {
    public Matrix4 Model { get; private set; }
    public Matrix4 View { get; private set; }
    public Matrix4 Projection { get; private set; }
    public Vector3 Position { get; private set; }
    public Matrix4 Jvp { get; private set; }
    public Matrix4 Full { get; private set; }

    public void Update ()
    {
      Jvp = Matrix4.Mult (View, Projection);
      Full = Matrix4.Mult (Model, Jvp);
    }

    public void Project (int width, int height, float fov)
    {
      var fovy = MathHelper.DegreesToRadians (fov);
      var aspect = ((float) width) / ((float) height);
      Projection = Matrix4.CreatePerspectiveFieldOfView (fovy, aspect, 0.1f, 100);
      Update ();
    }

    public void LookAt (float yaw_, float pitch_)
    {
      if (pitch_ > 89) pitch_ = 89;
      else if (pitch_ < -89) pitch_ = -89;

      var yaw = MathHelper.DegreesToRadians (yaw_);
      var pitch = MathHelper.DegreesToRadians (pitch_);
      var worldup = new Vector3 (0, 1, 0);
      var front = new Vector3 ();

      front.X = (float) (MathHelper.Cos (yaw) * MathHelper.Cos (pitch));
      front.Y = (float) (MathHelper.Sin (pitch));
      front.Z = (float) (MathHelper.Cos (yaw) * MathHelper.Sin (pitch));

      var right = Vector3.Cross (worldup, front);
      var up = Vector3.Cross (front, right);
      var center = Vector3.Add (Position, front);
      View = Matrix4.LookAt (Position, center, up);
      Update ();
    }

    public void LookAt (float x, float y, float z)
    {
      var center = new Vector3 (x, y, z);
      var up = new Vector3 (0, 1, 0);
      View = Matrix4.LookAt (Position, center, up);
      Update ();
    }

    public void MoveAt (float x, float y, float z)
    {
      Position = new Vector3 (x, y, z);
      Update ();
    }

    public Mvp ()
    {
      MoveAt (0, 0, 0);
      LookAt (0, 0);
    }
  }
}
