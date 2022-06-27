/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Numerics;
using Raylib_CsLo;

namespace frontend
{
  public class MainMenu : Menu
  {
    public override void Draw (Facade facade)
    {
      base.Draw (facade);

      Rectangle rec = GetDefaultRectangle ();
      Vector2 cursor = RayGui.GuiGrid (rec, 0, 0);

      rec.width -= (float) margin * 2;
      rec.x += (float) margin;

      if (RayGui.GuiButton (GetNextControl (ref rec, rec.width, 30), "Play!"))
        facade.PushScene (new Game ());

      if (RayGui.GuiButton (GetNextControl (ref rec, rec.width, 30), "Settings"))
        facade.PushScene (new Settings ());

      if (RayGui.GuiButton (GetNextControl (ref rec, rec.width, 30), "Exit"))
        Running = false;
    }
  }
}
