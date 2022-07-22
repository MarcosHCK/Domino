/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */

namespace Frontend.Engine;
public partial class Gl
{
  public interface ISelectable : IDrawable, IScalable
  {
    public bool Selected { get; set; }
  }
}
