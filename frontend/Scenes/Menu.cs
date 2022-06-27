/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Numerics;
using Raylib_CsLo;

namespace frontend
{
  public abstract class Menu : Facade.Scene
  {
    protected const float padding = 20;
    protected const float margin = 20;

    public override void Draw (Facade facade)
    {
      var ctrl = (int) GuiControl.DEFAULT;
      var prop = (int) GuiDefaultProperty.BACKGROUND_COLOR;
      var code = RayGui.GuiGetStyle (ctrl, prop);
      var back = Raylib.GetColor ((uint) code);
      Raylib.ClearBackground (back);
    }

    public static Rectangle GetDefaultRectangle ()
    {
      Rectangle rec;
      rec.x = (float) padding;
      rec.y = (float) padding;
      rec.width = ((float) Raylib.GetScreenWidth ()) - padding * 2;
      rec.height = ((float) Raylib.GetScreenHeight ()) - padding * 2;
    return rec;
    }

    public static Rectangle GetNextControl (ref Rectangle rec, float width, float height)
    {
      var nrec = new Rectangle (rec.x, rec.y, width, height);

      rec.y += height;
      rec.y += (float) margin;
    return nrec;
    }
  }
}
