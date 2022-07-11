/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Mathematics;

namespace frontend.Gl
{
  public class Light
  {
    public Vector3 Direction { get; set; }
    public Vector3 Ambient { get; set; }
    public Vector3 Diffuse { get; set; }
    public Vector3 Specular { get; set; }

    public Light ()
    {
      Direction = Vector3.Zero;
      Ambient = Vector3.Zero;
      Diffuse = Vector3.Zero;
      Specular = Vector3.Zero;
    }
  }
}
