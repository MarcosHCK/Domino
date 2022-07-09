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

#region API

    public void Append (params int[] faces)
    {
      var
      piece = new PieceObject (faces);
      piece.Scale = new Vector3 (30, 30, 30);
      piece.Visible = true;

      if (Head1 == Head2 && Head1 == null)
        {
          piece.Direction = Vector3.UnitY + Vector3.UnitZ;
          piece.Angle = MathHelper.DegreesToRadians (90);
          piece.Position = Vector3.Zero;

          Head1 = piece;
          Head1Value = piece.Head1;
          Head2 = piece;
          Head2Value = piece.Head2;
        }
      else
        {
          throw new NotImplementedException ();
        }
    }

#endregion

#region Gl.IDrawable

    public class Board : Gl.IDrawable
    {
      public bool Visible { get; set; }
      public void Draw (Gl.Pencil pencil) { }
    }

    private int locMvp = -1;
    public override void Draw (Gl.Pencil pencil)
    {
      if (locMvp == -1)
        {
          var pid = GL.GetInteger (GetPName.CurrentProgram);
          locMvp = GL.GetUniformLocation (pid, "aMvp");
        }

      if (Visible)
        {
          base.Draw (pencil);
          var list = Head1;
          while (list != null)
            {
              var mvp = list.Model;
              GL.UniformMatrix4 (locMvp, false, ref mvp);

              list.Draw (pencil);
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
    }

#endregion
  }
}
