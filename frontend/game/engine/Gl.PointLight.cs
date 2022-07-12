/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Mathematics;
namespace Engine;

public partial class Gl
{
  public class PointLight : Light, IPackable
  {
    public Vector3 Position { get; set; }
    public float Constant { get; set; }
    public float Linear { get; set; }
    public float Quadratic { get; set; }

    public override void Pack(Stream stream)
    {
      base.Pack (stream);
      IPackable.Pack (stream, Position);
      IPackable.Pack (stream, Constant);
      IPackable.Pack (stream, Linear);
      IPackable.Pack (stream, Quadratic);
    }
  }
}
