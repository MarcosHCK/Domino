/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Mathematics;

namespace frontend.Gl
{
  public abstract class Light
  {
    public Vector3 Ambient { get; set; }
    public Vector3 Diffuse { get; set; }
    public Vector3 Specular { get; set; }

    protected unsafe struct Layout
    {
      public fixed float Ambient [3];
      public fixed float Diffuse [3];
      public fixed float Specular [3];
    }
  }

  public class DirLight : Light
  {
    public Vector3 Direction { get; set; }

    protected new unsafe struct Layout
    {
      public fixed float Ambient [3];
      public fixed float Diffuse [3];
      public fixed float Specular [3];
      public fixed float Direction [3];
    }
  }

  public class PointLight : Light
  {
    public Vector3 Position { get; set; }
    public float Constant { get; set; }
    public float Linear { get; set; }
    public float Quadratic { get; set; }

    protected new unsafe struct Layout
    {
      public fixed float Ambient [3];
      public fixed float Diffuse [3];
      public fixed float Specular [3];
      public fixed float Position [3];
      public fixed float Constant [1];
      public fixed float Linear [1];
      public fixed float Quadratic [1];
    }
  }

  public class SpotLight : PointLight
  {
    public Vector3 Direction { get; set; }
    public float CutOff { get; set; }
    public float OuterCutOff { get; set; }

    protected new unsafe struct Layout
    {
      public fixed float Ambient [3];
      public fixed float Diffuse [3];
      public fixed float Specular [3];
      public fixed float Position [3];
      public fixed float Constant [1];
      public fixed float Linear [1];
      public fixed float Quadratic [1];
      public fixed float CutOff [1];
      public fixed float OuterCutOff [1];
    }
  }
}
