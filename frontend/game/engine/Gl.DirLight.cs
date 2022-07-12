/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Mathematics;
namespace Engine;

public partial class Gl
{
  public class DirLight : Light, IPackable
  {
    public Vector3 Direction { get; set; }

    public override void Pack (Stream stream)
    {
      base.Pack (stream);
      IPackable.Pack (stream, Direction);
    }
  }
}
