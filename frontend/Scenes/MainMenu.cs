/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Numerics;
using Raylib_cs;

namespace frontend
{
  public class MainMenu : Facade.Scene
  {
    private Vector3 position;
    private Mesh mesh;
    private Model model;
    private Image image;

    public override void Draw (Facade facade, double deltaTime)
    {
      if (Raylib.GetKeyPressed () != (int) KeyboardKey.KEY_NULL)
        Running = false;
      Raylib.DrawText ("Main menu", 20, 20, 10, Color.WHITE);
      Raylib.DrawModel (model, position, 1.0f, new Color (255, 255, 0, 255));
    }

    public MainMenu ()
    {
      var position = new Vector3 (10.0f, 10.0f, 0.0f);

      var width = (float) Raylib.GetScreenWidth ();
      var height = (float) Raylib.GetScreenHeight ();
      var mesh = Raylib.GenMeshPlane (width, height, 10, 10);
      this.mesh = mesh;

      var model = Raylib.LoadModelFromMesh (mesh);
      this.model = model;
    }

    ~MainMenu ()
    {
      Raylib.UnloadMesh (ref mesh);
      Raylib.UnloadModel (model);
    }
  }
}
