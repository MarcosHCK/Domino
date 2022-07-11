/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using frontend.Gl;
using OpenTK.Mathematics;

namespace frontend.Game
{
  public class Skybox : Game.Object
  {
    static string modelFile;

    public override void Draw (Frame frame)
    {
      var camera = frame.Camera;
      var target = camera.Target;
      Position = target;
      base.Draw (frame);
    }

    public Skybox (Gl.IDrawable drawable) : base (drawable) { }
    public Skybox () : this (new Gl.SingleModel (modelFile)) { }
    static Skybox ()
    {
      var parent = Application.DataDir;
      var relative = "models/skybox/scene.gltf";
      modelFile = System.IO.Path.Combine (parent, relative);
    }
  }
}
