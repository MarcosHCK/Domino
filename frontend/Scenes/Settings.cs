/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Numerics;
using Raylib_CsLo;

namespace frontend
{
  public class Settings : Menu
  {
    private Vector2 scrollState;

    public unsafe override void Draw (Facade facade)
    {
      base.Draw (facade);

      Rectangle rec = GetDefaultRectangle ();
      Vector2 cursor = RayGui.GuiGrid (rec, 0, 0);

      rec.width -= (float) margin * 2;
      rec.x += (float) margin;

      if (RayGui.GuiButton (GetNextControl (ref rec, rec.width, 30), "Graphics"))
        throw new NotImplementedException ();

      {
        var font = RayGui.GuiGetFont ();
        var fontProp = (int) Raylib_CsLo.GuiDefaultProperty.TEXT_SIZE;
        var default_ = (int) Raylib_CsLo.GuiControl.DEFAULT;
        var fontSize = RayGui.GuiGetStyle (default_, fontProp);
    
        {
          var lineRec = Menu.GetNextControl (ref rec, rec.width, fontSize);
          var lineStartX = (int) lineRec.x;
          var lineStartY = (int) lineRec.x;
          var lineStopX = (int) rec.width + (int) lineRec.x;
          var lineStopY = (int) lineRec.x;

          RayGui.GuiSetStyle (default_, fontProp, fontSize * 2);
          RayGui.GuiLine (lineRec, "Partidas");
          RayGui.GuiSetStyle (default_, fontProp, fontSize);
        }
      }

      if (RayGui.GuiButton (GetNextControl (ref rec, rec.width, 30), "Create new game"))
        facade.PushScene (new CreateRules ());

      var needed = rec.height;
      var remain = ((float) Raylib.GetScreenHeight ()) - rec.y - padding * 2 - 30;
      var rec_ = GetNextControl (ref rec, rec.width, remain);
      var content = new Rectangle (0, 0, rec.width - padding, needed );
      var scroll = new Vector2 ( scrollState.X, scrollState.Y );
      var view = RayGui.GuiScrollPanel (rec_, content, &scroll);
      var rect = new Rectangle (rec.x + scroll.X, rec.y + scroll.Y, content.width, content.height);

      Raylib.BeginScissorMode ((int) view.x, (int) view.y, (int) view.width, (int) view.height);
        scrollState.X = scroll.X;
        scrollState.Y = scroll.Y;

        rect.x += (float) padding;
        rect.y += (float) padding;
        rect.width -= (float) padding * 2;
        rect.height -= (float) padding * 2;
      Raylib.EndScissorMode ();

      rec.height = 30;
      rec.width = 50;
      rec.x = ((float) Raylib.GetScreenWidth ()) - padding - rec.width;
      rec.y = ((float) Raylib.GetScreenHeight ()) - padding - rec.height;
      if (RayGui.GuiButton (rec, "Back"))
        Running = false;
    }
  }
}
