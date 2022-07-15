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
    public float Angle { get; set; }
    public Vector3 Direction { get; set; }
  }
}
