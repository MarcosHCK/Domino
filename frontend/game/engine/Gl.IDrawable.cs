/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */

namespace frontend.Gl
{
  public interface IDrawable
  {
    public bool Visible { get; set; }
    public void Draw (Pencil pencil);
  }
}
