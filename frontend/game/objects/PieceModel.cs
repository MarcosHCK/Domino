/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using Cairo;

namespace frontend.Game.Objects
{
  public class PieceModel : Gl.SingleModel
  {
    static readonly string modelPath;

#region Constructors

    static readonly int topMost = 910;
    static readonly int bottomMost = 1330;
    static readonly int leftMost = 190;
    static readonly int rightMost = 670;
    static readonly int canvasHeight;
    static readonly int canvasWidth;

    public PieceModel (params int[] faces)
      : base (modelPath)
    {
      if (faces.Length != 2)
        {
          throw new Exception ("can't handle more than two faces");
        }

      var tex = textures.Single ();
      var surface1 = new ImageSurface (Format.Argb32, canvasWidth, canvasHeight);
      var cairo1 = new Cairo.Context (surface1);
      var surface2 = new ImageSurface (Format.Argb32, canvasWidth, canvasHeight);
      var cairo2 = new Cairo.Context (surface2);
    }

    static PieceModel ()
    {
      var parent = Application.DataDir;
      var relative = "models/piece/scene.gltf";
      modelPath = System.IO.Path.Combine (parent, relative);
      canvasHeight = bottomMost - topMost;
      canvasWidth = rightMost - leftMost;
    }

#endregion
  }
}
