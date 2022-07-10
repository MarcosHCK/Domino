/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace frontend.Game.Objects
{
  public class PieceBoard : Game.Object
  {
    public PieceObject? Head1 { get; private set; }
    public int Head1Value { get; private set; }
    public PieceObject? Head2 { get; private set; }
    public int Head2Value { get; private set; }
    private static readonly Vector3 displace = new Vector3 (0, -0.2f, 0);
    private static readonly Vector3 piece_scale = new Vector3 (30, 30, 30);
    private static readonly float piece_angle = MathHelper.DegreesToRadians (180);
    private static readonly Vector3 piece_width = new Vector3 (0.8f, 0, 0);
    private static readonly Vector3 piece_heigth = new Vector3 (1.6f, 0, 0);

#region API

    private void AppendHead (int[] faces, int tail, ref PieceObject head, ref int value)
    {
      PieceObject piece;
      if (faces [0] == value)
        {
          if (tail < 0)
          {
            var tmp = faces [1];
            faces [1] = faces [0];
            faces [0] = tmp;
  
            piece = new PieceObject (faces);
            value = tmp;
          }
          else
          {
            piece = new PieceObject (faces);
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
  
            piece = new PieceObject (faces);
            value = tmp;
          }
          else
          {
            piece = new PieceObject (faces);
            value = faces [0];
          }
        }

      piece.Scale = piece_scale;
      piece.Angle = piece_angle;
      piece.Direction = Vector3.UnitZ + Vector3.UnitX;
      piece.Position = head.Position + (tail * piece_heigth);
      piece.Visible = true;

      if (tail > 0)
        PieceObject.Append (head, piece);
      else
        PieceObject.Prepend (head, piece);
      head = piece;
    }

    public void Append (int where, params int[] faces)
    {
      if (faces.Length != 2)
        {
          throw new Exception ("can't handle more than two faces");
        }

      Console.WriteLine ("piece {0}-{1}", faces [0], faces [1]);

      if (Head1 == Head2 && Head1 == null)
        {
          if (where != -1)
            {
              throw new Exception ();
            }

          var
          piece = new PieceObject (faces);
          piece.Scale = piece_scale;
          piece.Angle = piece_angle;
          piece.Direction = Vector3.UnitZ + Vector3.UnitX;
          piece.Position = Vector3.Zero;
          piece.Visible = true;

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
              AppendHead (faces, -1, ref head, ref value);

              Head1Value = value;
              Head1 = head;
            }
          else
          if (where == Head2Value)
            {
              var head = Head2!;
              var value = Head2Value;
              AppendHead (faces,  1, ref head, ref value);

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
        var basedir = Application.DataDir;
        var relative = "models/table/scene.gltf";
        modelName = Path.Combine (basedir, relative);
      }
    }

    public override void Draw (Gl.Frame frame)
    {
      if (Visible)
        {
          var camera = frame.Camera;
          var target = camera.Target;
          var list = Head1;

          base.Position = target + displace;
          base.Draw (frame);

          while (list != null)
            {
              list.Draw (frame);
              list = list.Next;
            }
        }
    }

#endregion

#region Constructor

    public PieceBoard (int n_faces)
      : base (new Board ())
    {
      if (n_faces != 2)
        {
          throw new Exception ("can't handle more than two faces");
        }

      Scale = new Vector3 (10, 10, 10);
    }

#endregion
  }
}
