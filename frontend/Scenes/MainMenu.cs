/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Numerics;
using Raylib_CsLo;

namespace frontend
{
  public class Menu : Facade.Scene
  {
    private const float padding = 20;
    public override void Draw (Facade facade, double deltaTime)
    {
      Rectangle rec;
      rec.x = (float) padding;
      rec.y = (float) padding;
      rec.width = ((float) Raylib.GetScreenWidth ()) - padding * 2;
      rec.height = ((float) Raylib.GetScreenHeight ()) - padding * 2;
      var pos = RayGui.GuiGrid (rec, 3, 3);
    }
  }

  public class MainMenu : Menu
  {
    public override void Draw (Facade facade, double deltaTime)
    {
      base.Draw (facade, deltaTime);
    }
  }
}
