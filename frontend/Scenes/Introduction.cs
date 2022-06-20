/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using Raylib_cs;

namespace frontend
{
  public class Introduction : Facade.Scene
  {
    private Stage stage;

    private enum Stage
    {
      COPYRIGHT,
      NUMBER,
    }

    public override void Draw (Facade facade, double daltaTime)
    {
      if (Raylib.GetKeyPressed () != (int) KeyboardKey.KEY_NULL)
        Running = false;
      Raylib.DrawText ("Intro", 20, 20, 10, Color.WHITE);
    }

    public Introduction () : base ()
    {
      this.stage = Stage.COPYRIGHT;
    }
  }
}
