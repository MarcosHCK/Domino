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
    private static readonly Vector3 displace;

#region API

    public void Append (params int[] faces)
    {
      var
      piece = new PieceObject (faces);
      piece.Scale = new Vector3 (30, 30, 30);
      piece.Visible = true;

      if (Head1 == Head2 && Head1 == null)
        {
          piece.Direction = Vector3.UnitZ + Vector3.UnitX;
          piece.Angle = MathHelper.DegreesToRadians (180);
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

    static PieceBoard ()
    {
      displace = new Vector3 (0, -0.2f, 0);
    }

#endregion
  }
}
