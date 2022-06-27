/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Numerics;
using Raylib_CsLo;

namespace frontend
{
  public class Facade
  {
    private Stack<Scene>? sceneQueue;
    private Camera3D camera;
    private const int targetFPS = 60;
    private const int screenWidth = 800;
    private const int screenHeight = 600;
    private const string windowTitle = "Domino";

    public abstract class Scene
    {
      public bool Running { get; protected set; }
      public abstract void Draw (Facade facade);

      public Scene ()
        {
          this.Running = true;
        }
    }

    public void BeginMode3D () => Raylib.BeginMode3D (camera);
    public void EndMode3D () => Raylib.EndMode3D ();
    public void PushScene (Scene scene) => sceneQueue!.Push (scene);

    public static int Main (string[] argv)
    {
      var facade = new Facade ();
      Stack<Scene> sceneQueue;
      Scene scene;
  
      facade.camera.position = new Vector3 (10.0f, 10.0f, 10.0f);
      facade.camera.target = new Vector3 (0.0f, 0.0f, 0.0f);
      facade.camera.up = new Vector3 (0.0f, 1.0f, 0.0f);
      facade.camera.fovy = 45.0f;
      facade.camera.projection = (int) CameraProjection.CAMERA_PERSPECTIVE;

      Raylib.InitWindow (screenWidth, screenHeight, windowTitle);
      Raylib.SetCameraMode (facade.camera, CameraMode.CAMERA_CUSTOM);
      Raylib.SetTargetFPS (targetFPS);
      Raylib.SetExitKey (0);

      sceneQueue = new Stack<Scene> ();
      sceneQueue.Push (new MainMenu ());
      sceneQueue.Push (new Introduction ());
      facade.sceneQueue = sceneQueue;

      while (!Raylib.WindowShouldClose ()
        && sceneQueue.Count > 0)
        {
          Raylib.BeginDrawing ();
          Raylib.UpdateCamera (ref facade.camera);

          do
          {
            if (sceneQueue.Count == 0)
              break;
            else
            {
              scene = sceneQueue.Peek ();
              if (scene.Running)
                scene.Draw (facade);
              else
              {
                sceneQueue.Pop ();
                continue;
              }
            }
          }
          while (false);

          Raylib.EndDrawing ();
        }

      Raylib.CloseWindow ();
    return 0;
    }
  }
}
