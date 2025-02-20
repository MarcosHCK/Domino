/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */
namespace Frontend.Engine;

public partial class Gl
{
  public interface IDrawable
  {
    public bool Visible { get; set; }
    public void Draw (Gl gl);
  }
}
