/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */

namespace frontend
{
  public abstract class Scene
  {
    public bool Running { get; protected set; }
    public abstract void Draw (Stack<Scene> sceneQueue, double deltaTime);

    public Scene ()
      {
        this.Running = true;
      }
  }
}
