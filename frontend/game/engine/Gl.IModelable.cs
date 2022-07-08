/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Mathematics;

namespace frontend.Gl
{
  public interface IModelable
  {
    public Matrix4 Model { get; set; }
  }
}
