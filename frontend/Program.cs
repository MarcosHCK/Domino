/* Copyright 2021-2025 MarcosHCK
 * This file is part of deusexmakina3.
 *
 */
using System.Numerics;
using Raylib_cs;

namespace frontend
{
  public static class Program
  {
    private const int targetFPS = 60;
    private const int screenWidth = 800;
    private const int screenHeight = 600;
    private const string windowTitle = "Domino";
    private static Scene? scene = null;

    private static void DrawScene ()
    {
      if (scene != null)
        scene.Draw ();
    }

    public static int Main (string[] argv)
    {
      var previousTime = 0.0d;
      var currentTime = 0.0d;
      var deltaTime = 0.0f;
      var updateTime = 0.0d;
      var waitTime = 0.0d;
      Camera3D camera;

      previousTime = Raylib.GetTime ();
  
      camera.position = new Vector3 (10.0f, 10.0f, 10.0f);
      camera.target = new Vector3 (0.0f, 0.0f, 0.0f);
      camera.up = new Vector3 (0.0f, 1.0f, 0.0f);
      camera.fovy = 45.0f;
      camera.projection = CameraProjection.CAMERA_PERSPECTIVE;

      Raylib.InitWindow (screenWidth, screenHeight, windowTitle);
      Raylib.SetCameraMode (camera, CameraMode.CAMERA_CUSTOM);

      while (!Raylib.WindowShouldClose ())
        {
          Raylib.PollInputEvents ();
          Raylib.UpdateCamera (ref camera);

          Raylib.BeginDrawing ();
          Raylib.ClearBackground (Color.BLACK);

          DrawScene ();

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
