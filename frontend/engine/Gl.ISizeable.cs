/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */
using OpenTK.Mathematics;
namespace Frontend.Engine;

public partial class Gl
{
  public interface ISizeable
  {
    public float Width { get; }
    public float Height { get; }
    public float Depth { get; }
  }
}
