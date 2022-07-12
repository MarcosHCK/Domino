/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Mathematics;

namespace Engine
{
  public class Skybox : Engine.Object
  {
    static string modelFile;

    public override void Draw (Gl gl)
    {
      var camera = gl.Viewport;
      var target = camera.Target;
      Position = target;
      base.Draw (gl);
    }

    public Skybox (Gl.IDrawable drawable) : base (drawable) { }
    public Skybox () : this (new Gl.SingleModel (modelFile)) { }
    static Skybox ()
    {
      var parent = Gl.DataDir;
      var relative = "models/skybox/scene.gltf";
      modelFile = System.IO.Path.Combine (parent, relative);
    }
  }
}
