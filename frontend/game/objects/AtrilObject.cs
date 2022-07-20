/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */
using OpenTK.Mathematics;
using Frontend.Engine;

namespace Frontend.Game.Objects
{
  public class AtrilObject : Engine.SingleObject
  {
    private Game.Pieces pieces;
    private float maxwidth = 0;
    private float padding = 0.5f;
    private float piece_spacing = 0.03f;
    private Vector3 draw_start = new Vector3 (0, 0, 0);
    private Dictionary<Piece, PieceObject> onboard;
    private static string modelName;
    private static Vector3 stand_start;
    private static Vector3 displace;

#region Types

    class Piece
    {
      private int face1;
      private int face2;

      public override bool Equals (object? obj)
      {
        if (obj is Piece)
        {
          var other = (Piece) obj;
          if (other.face1 == face1
           && other.face2 == face2)
            return true;
        }
      return false;
      }

      public override int GetHashCode ()
      {
        var a = face1.GetHashCode ();
        var b = face2.GetHashCode ();
      return a ^ b;
      }

      public override string ToString()
      {
        return $"{face1}-{face2}";
      }

      public Piece (params int[] piece)
      {
        if (piece.Length != 2)
          {
            throw new Exception ("can't handle more than two faces");
          }

        this.face1 = piece [0];
        this.face2 = piece [1];
      }
    }

#endregion

#region API

    public void ShowPieces (params int[][] pieces_)
    {
      PieceObject piece;
      Gl.IRotable irot;
      Vector3 size;
      var scale = Scale;
      var height = 0f;
      var width = 0f;

      foreach (var piece_ in onboard.Values)
        piece_.Visible = false;

      foreach (var piece_ in pieces_)
      {
        var key = new Piece (piece_);
        if (!onboard.ContainsKey (key))
        {
          piece = new PieceObject (pieces, piece_);
          onboard.Add (key, piece);

          irot = (Gl.IRotable) piece;
          irot.Pitch = MathHelper.DegreesToRadians (19);
          irot.Roll = MathHelper.DegreesToRadians (180);
        }

        piece = onboard [key];
        piece.Visible = true;
        size = piece.ScaledSize;

        height = Math.Max (height, size.Y);
        width += size.X;
      }

      width += piece_spacing * pieces_.Length;
      width = Math.Max (width, maxwidth);
      maxwidth = width;

      draw_start = Position + stand_start;
      draw_start.X -= (width / 2);
      draw_start.X += padding;

      scale.X = (width + padding * 2) / ((Gl.ISizeable) Drawable).Width;
      scale.Y = (height + padding * 2) / ((Gl.ISizeable) Drawable).Height;
      Scale = scale;
    }

#endregion

#region IDrawable

    public override void Draw (Gl gl)
    {
      if (Visible)
      {
        var position = draw_start;
        base.Draw (gl);

        foreach (var piece in onboard.Values)
        if (piece.Visible)
        {
          piece.Position = position;
          piece.Draw (gl);
          position.X += piece.Size.X + piece_spacing;
        }
      }
    }

#endregion

#region Constructors

    public AtrilObject (Pieces pieces, int maxfaces)
      : base (new Gl.SingleModel (modelName))
    {
      onboard = new Dictionary<Piece, PieceObject> ();
      this.pieces = pieces;
      Position += displace;
    }

    static AtrilObject ()
    {
      var basedir = Engine.Gl.DataDir;
      var relative = "models/atril/scene.gltf";
      modelName = Path.Combine (basedir, relative);

      stand_start = new Vector3 (0, 0.33f, 0);
      displace = new Vector3 (0, 0.1f, 6.0f);
    }

#endregion
  }
}
