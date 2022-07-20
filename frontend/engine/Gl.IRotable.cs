/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */
using OpenTK.Mathematics;
namespace Frontend.Engine;

public partial class Gl
{
  public interface IRotable
  {
    public Quaternion Quaternion { get; set; }

#region Axis-Angle rotation

    public virtual float Angle
    {
      get
      {
        float angle;
        Vector3 vector;
        Quaternion.ToAxisAngle (out vector, out angle);
        return angle;
      }

      set
      {
        float angle;
        Vector3 vector;
        Quaternion.ToAxisAngle (out vector, out angle);
        Quaternion = Quaternion.FromAxisAngle (vector, value);
      }
    }

    public virtual Vector3 Direction
    {
      get
      {
        float angle;
        Vector3 vector;
        Quaternion.ToAxisAngle (out vector, out angle);
        return vector;
      }

      set
      {
        float angle;
        Vector3 vector;
        Quaternion.ToAxisAngle (out vector, out angle);
        Quaternion = Quaternion.FromAxisAngle (value, angle);
      }
    }

#endregion

#region XYZ-Euler rotation

    public virtual float Pitch
    {
      get
      {
        Vector3 angles;
        Quaternion.ToEulerAngles (out angles);
        return angles.X;
      }

      set
      {
        Vector3 angles;
        Quaternion.ToEulerAngles (out angles);
        angles.X = value;
        Quaternion = Quaternion.FromEulerAngles (angles);
      }
    }

    public virtual float Yaw
    {
      get
      {
        Vector3 angles;
        Quaternion.ToEulerAngles (out angles);
        return angles.Y;
      }

      set
      {
        Vector3 angles;
        Quaternion.ToEulerAngles (out angles);
        angles.Y = value;
        Quaternion = Quaternion.FromEulerAngles (angles);
      }
    }

    public virtual float Roll
    {
      get
      {
        Vector3 angles;
        Quaternion.ToEulerAngles (out angles);
        return angles.Z;
      }

      set
      {
        Vector3 angles;
        Quaternion.ToEulerAngles (out angles);
        angles.Z = value;
        Quaternion = Quaternion.FromEulerAngles (angles);
      }
    }

#endregion
  }
}
