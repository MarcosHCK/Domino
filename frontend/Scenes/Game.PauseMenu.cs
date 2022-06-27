/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Numerics;
using Raylib_CsLo;

namespace frontend
{
  public partial class Game
  {
    public class PauseMenu : Menu
    {
      private Game game;
      public PauseMenu (Game game) : base () => this.game = game;
      public override void Draw (Facade facade)
      {
        var escape = (int) KeyboardKey.KEY_ESCAPE;
        if (Raylib.IsKeyPressed (escape))
          Running = false;

        Rectangle rec = GetDefaultRectangle ();
        Vector2 cursor = RayGui.GuiGrid (rec, 0, 0);

        rec.width -= (float) margin * 2;
        rec.x += (float) margin;

        if (RayGui.GuiButton (GetNextControl (ref rec, rec.width, 30), "Back"))
          Running = false;

        if (RayGui.GuiButton (GetNextControl (ref rec, rec.width, 30), "Exit"))
          {
            this.Running = false;
            game.Running = false;
          }
      }
    }
  }
}
