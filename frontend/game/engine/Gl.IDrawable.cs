/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
namespace Engine;

public partial class Gl
{
  public interface IDrawable
  {
    public bool Visible { get; set; }
    public void Draw (Gl gl);
  }
}
