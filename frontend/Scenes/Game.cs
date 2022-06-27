/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using Raylib_cs;

namespace frontend
{
  public partial class Game : Facade.Scene
  {
    public override void Draw (Facade facade)
    {
      Raylib.ClearBackground (Color.BLACK);
      if (Raylib.IsKeyPressed (KeyboardKey.KEY_ESCAPE))
        facade.PushScene (new PauseMenu (this));

      facade.BeginMode3D ();
      DrawBoard ();
      facade.EndMode3D ();
    }

    private void DrawBoard ()
    {
      Raylib.DrawGrid (10, 2.0f);
    }
  }
}
