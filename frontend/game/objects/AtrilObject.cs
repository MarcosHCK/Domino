/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */
using OpenTK.Mathematics;
using Frontend.Engine;

namespace Frontend.Game.Objects
{
  public class AtrilObject : Engine.Object
  {
#region Constructors

    private static string modelName;
    public AtrilObject ()
      : base (new Gl.SingleModel (modelName))
    {
      Model = Matrix4.Identity;
    }

    static AtrilObject ()
    {
      var basedir = Engine.Gl.DataDir;
      var relative = "models/atril/scene.gltf";
      modelName = Path.Combine (basedir, relative);
    }

#endregion
  }
}
