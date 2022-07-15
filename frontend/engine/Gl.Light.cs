/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */
using OpenTK.Mathematics;
namespace Frontend.Engine;

public partial class Gl
{
  public abstract class Light : IPackable
  {
    public Vector3 Ambient { get; set; }
    public Vector3 Diffuse { get; set; }
    public Vector3 Specular { get; set; }

    public virtual void Pack (Stream stream)
    {
      IPackable.Pack (stream, Ambient);
      IPackable.Pack (stream, Diffuse);
      IPackable.Pack (stream, Specular);
    }
  }
}
