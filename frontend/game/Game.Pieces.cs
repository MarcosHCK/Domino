/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */
using Frontend.Game.Objects;

namespace Frontend.Game
{
  public class Pieces
  {
    private Dictionary<Piece, PieceModel> pieces;
    private Piece tester = new Piece (0, 0);

#region Types

    class Piece : ICloneable
    {
      public int face1;
      public int face2;

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

      public object Clone ()
      {
        return new Piece (face1, face2);
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

    public PieceModel this[int[] piece_]
    {
      get
      {
        if (piece_.Length != 2)
        {
          throw new Exception ("Only two faced pieces handled!");
        }

        tester.face1 = piece_ [0];
        tester.face2 = piece_ [1];
        if (!pieces.ContainsKey (tester))
        {
          var piece = new PieceModel (piece_);
          var key = (Piece) tester.Clone();
          pieces.Add (key, piece);
        }

      return pieces [tester];
      }
    }

#endregion

#region Constructors

    public Pieces ()
    {
      pieces = new Dictionary<Piece, PieceModel> ();
    }

#endregion
  }
}
