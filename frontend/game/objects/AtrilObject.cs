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
    static Vector3 displace;

#region Constructors

    private static string modelName;
    public AtrilObject ()
      : base (new Gl.SingleModel (modelName))
    {
      var irot = (Gl.IRotable) this;
      irot.Pitch = MathHelper.DegreesToRadians (-90);
      irot.Roll = MathHelper.DegreesToRadians (90);
      Position += displace;
    }

    static AtrilObject ()
    {
      var basedir = Engine.Gl.DataDir;
      var relative = "models/atril/scene.gltf";
      modelName = Path.Combine (basedir, relative);
      displace = new Vector3 (0, -3, 6.7f);
    }

#endregion
  }
}
