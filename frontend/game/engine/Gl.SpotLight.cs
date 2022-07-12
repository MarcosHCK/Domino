/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Mathematics;

namespace frontend.Gl
{
  public class SpotLight : PointLight, IPackable
  {
    public Vector3 Direction { get; set; }
    public float CutOff { get; set; }
    public float OuterCutOff { get; set; }

    public override void Pack(Stream stream)
    {
      base.Pack (stream);
      IPackable.Pack (stream, Direction);
      IPackable.Pack (stream, CutOff);
      IPackable.Pack (stream, OuterCutOff);
    }
  }
}
