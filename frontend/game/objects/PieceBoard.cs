/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */
using OpenTK.Mathematics;
using Frontend.Engine;

namespace Frontend.Game.Objects
{
  public class PieceBoard : Engine.SingleObject
  {
    public PieceObject? Head1 { get; private set; }
    public int Head1Value { get; private set; }
    public PieceObject? Head2 { get; private set; }
    public int Head2Value { get; private set; }
    private Game.Pieces pieces;
    private static readonly Vector3 displace = new Vector3 (0, -0.2f, 0);
    private static readonly Vector3 piece_margin = new Vector3 (0.03f, 0, 0);
    private static readonly Vector3 table_scale = new Vector3 (7, 7, 7);
    private static readonly float piece_angle = MathHelper.DegreesToRadians (180);

#region API

    private void SetPosition (PieceObject piece, PieceObject head, int tail)
    {
      var
      position = head.Position;
      position.X += (head.Size.X / 2 + piece.Size.X / 2) * tail;
      position += piece_margin * tail;
      piece.Position = position;
    }

    private void AppendSimple (int[] faces, int by, int tail, ref PieceObject head, ref int value)
    {
      PieceObject piece;
      if (faces [0] == by)
        {
          if (tail < 0)
          {
            var tmp = faces [1];
            faces [1] = faces [0];
            faces [0] = tmp;
  
            piece = new PieceObject (pieces, faces);
            value = tmp;
          }
          else
          {
            piece = new PieceObject (pieces, faces);
            value = faces [1];
          }
        }
      else
        {
          if (tail > 0)
          {
            var tmp = faces [0];
            faces [0] = faces [1];
            faces [1] = tmp;
  
            piece = new PieceObject (pieces, faces);
            value = tmp;
          }
          else
          {
            piece = new PieceObject (pieces, faces);
            value = faces [0];
          }
        }

      ((Gl.IRotable) piece).Angle = piece_angle;
      ((Gl.IRotable) piece).Direction = Vector3.UnitZ + Vector3.UnitX;

      SetPosition (piece, head, tail);

      if (tail > 0)
        PieceObject.Append (head, piece);
      else
        PieceObject.Prepend (head, piece);
      head = piece;
    }

    private void AppendDouble (int[] faces, int by, int tail, ref PieceObject head, ref int value)
    {
      var
      piece = new PieceObject (pieces, faces);
      ((Gl.IRotable) piece).Angle = piece_angle;
      ((Gl.IRotable) piece).Direction = Vector3.UnitZ;

      SetPosition (piece, head, tail);

      if (tail > 0)
        PieceObject.Append (head, piece);
      else
        PieceObject.Prepend (head, piece);
      head = piece;
    }

    private void AppendHead (int[] faces, int by, int tail, ref PieceObject head, ref int value)
    {
      if (faces [0] != faces [1])
        AppendSimple (faces, by, tail, ref head, ref value);
      else
        AppendDouble (faces, by, tail, ref head, ref value);
    }

    public void Append (int by, int where, params int[] faces)
    {
      if (faces.Length != 2)
        {
          throw new Exception ("can't handle more than two faces");
        }

      if (Head1 == Head2 && Head1 == null)
        {
          if (where != -1)
            {
              throw new Exception ();
            }

          var piece = new PieceObject (pieces, faces);
          var irot = (Gl.IRotable) piece;

          piece.Position = Vector3.Zero;

            irot.Angle = piece_angle;
          if (piece.Head1 != piece.Head2)
            irot.Direction = Vector3.UnitZ + Vector3.UnitX;
          else
            irot.Direction = Vector3.UnitZ;

          Head1 = piece;
          Head1Value = piece.Head1;
          Head2 = piece;
          Head2Value = piece.Head2;
        }
      else
        {
          if (where == Head1Value)
            {
              var head = Head1!;
              var value = Head1Value;
              AppendHead (faces, by, -1, ref head, ref value);

              Head1Value = value;
              Head1 = head;
            }
          else
          if (where == Head2Value)
            {
              var head = Head2!;
              var value = Head2Value;
              AppendHead (faces, by,  1, ref head, ref value);

              Head2Value = value;
              Head2 = head;
            }
          else
          {
            throw new Exception ();
          }
        }
    }

#endregion

#region Gl.IDrawable

    public class Board : Gl.SingleModel
    {
      private static string modelName;
      public Board () : base (modelName) { }
      static Board ()
      {
        var basedir = Engine.Gl.DataDir;
        var relative = "models/table/scene.gltf";
        modelName = Path.Combine (basedir, relative);
      }
    }

    public override void Draw (Gl gl)
    {
      var list = Head1;
      if (Visible)
      {
        var position = Position;
        var current = position;
        var size = Size;
        base.Draw (gl);

        while (Head1 != null)
        {
          current.X -= size.X;
          Position = current;
          base.Draw (gl);

          if (Position.X < Head1!.Position.X)
            break;
        }

        Position = position;
        current = position;

        while (Head2 != null)
        {
          current.X += size.X;
          Position = current;
          base.Draw (gl);

          if (Position.X > Head2!.Position.X)
            break;
        }

        Position = position;

        while (list != null)
        {
          list.Draw (gl);
          list = list.Next;
        }
      }
    }

#endregion

#region Constructor

    public PieceBoard (Pieces pieces, int n_faces)
      : base (new Board ())
    {
      if (n_faces != 2)
        {
          throw new Exception ("can't handle more than two faces");
        }

      this.pieces = pieces;
      Scale = table_scale;
      Position = displace;
    }

#endregion
  }
}
