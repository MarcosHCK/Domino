/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Numerics;
using Raylib_cs;

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
      public abstract void Draw (Facade facade, double deltaTime);

      public Scene ()
        {
          this.Running = true;
        }
    }

    private void BeginMode3D () => Raylib.BeginMode3D (camera);
    private void EndMode3D () => Raylib.EndMode3D ();
    private void PushScene (Scene scene) => sceneQueue!.Push (scene);

    public static int Main (string[] argv)
    {
      var facade = new Facade ();
      var previousTime = 0.0d;
      var currentTime = 0.0d;
      var deltaTime = 0.0f;
      var updateTime = 0.0d;
      var waitTime = 0.0d;
      Stack<Scene> sceneQueue;
      Scene scene;
  
      facade.camera.position = new Vector3 (10.0f, 10.0f, 10.0f);
      facade.camera.target = new Vector3 (0.0f, 0.0f, 0.0f);
      facade.camera.up = new Vector3 (0.0f, 1.0f, 0.0f);
      facade.camera.fovy = 45.0f;
      facade.camera.projection = CameraProjection.CAMERA_PERSPECTIVE;

      Raylib.InitWindow (screenWidth, screenHeight, windowTitle);
      Raylib.SetCameraMode (facade.camera, CameraMode.CAMERA_CUSTOM);

      previousTime = Raylib.GetTime ();
      sceneQueue = new Stack<Scene> ();
      sceneQueue.Push (new MainMenu ());
      sceneQueue.Push (new Introduction ());
      facade.sceneQueue = sceneQueue;

      while (!Raylib.WindowShouldClose ()
        && sceneQueue.Count > 0)
        {
          Raylib.PollInputEvents ();
          Raylib.UpdateCamera (ref facade.camera);

          Raylib.BeginDrawing ();
          Raylib.ClearBackground (Color.BLACK);

          do
          {
            if (sceneQueue.Count == 0)
              break;
            else
            {
              scene = sceneQueue.Peek ();
              if (scene.Running)
                scene.Draw (facade, deltaTime);
              else
              {
                sceneQueue.Pop ();
                continue;
              }
            }
          }
          while (false);

          Raylib.EndDrawing ();
          Raylib.SwapScreenBuffer ();

          currentTime = Raylib.GetTime ();
          updateTime = currentTime - previousTime;

          if (targetFPS > 0)
            {
              waitTime = (1.0f / (float) targetFPS) - updateTime;
              if (waitTime > 0)
                {
                  Raylib.WaitTime (1000.0f * (float) waitTime);
                  currentTime = Raylib.GetTime ();
                  deltaTime = (float) (currentTime - previousTime);
                }
            }
          else
            {
              deltaTime = (float) updateTime;
            }

          previousTime = currentTime;
        }

      Raylib.CloseWindow ();
    return 0;
    }
  }
}
