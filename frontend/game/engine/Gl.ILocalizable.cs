/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Mathematics;
namespace Engine;

public partial class Gl
{
  public interface ILocalizable
  {
    public Vector3 Position { get; set; }
  }
}
