/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */
using OpenTK.Mathematics;

namespace Frontend.Game.Objects
{
  public class PieceObject : Engine.Object
  {
    private PieceModel model;

#region ISizable

    public Vector3 Size
    {
      get
      {
        var x = model.Width;
        var y = model.Height;
        var z = model.Depth;
        var v = new Vector3 (x, y, z);
        var v2 = Vector3.TransformVector(v, Model);
        v2.X = Math.Abs (v2.X);
        v2.Y = Math.Abs (v2.Y);
        v2.Z = Math.Abs (v2.Z);
      return v2;
      }
    }

#endregion

#region List

    public PieceObject? Next { get; private set; }
    public PieceObject? Prev { get; private set; }
    public int Head1 { get; private set; }
    public int Head2 { get; private set; }

    [System.Serializable]
    public class PieceObjectListException : System.Exception
    {
      public PieceObjectListException () { }
      public PieceObjectListException (string message) : base(message) { }
      public PieceObjectListException (string message, System.Exception inner) : base(message, inner) { }
      protected PieceObjectListException (
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public static PieceObject Append (PieceObject head, PieceObject link)
    {
      var last = Last (head);
      last.Next = link;
      link.Prev = last;
    return head;
    }

    public static PieceObject Prepend (PieceObject head, PieceObject link)
    {
      head.Prev = link;
      link.Next = head;
    return link;
    }

    public static PieceObject Last (PieceObject head)
    {
      while (head.Next != null)
        head = head.Next;
    return head;
    }

#endregion

#region Constructor

    public PieceObject (PieceModel model)
      : base (model)
    {
      this.model = model;
    }

    public PieceObject (params int[] faces)
      : this (new PieceModel (faces))
    {
      if (faces.Length != 2)
        {
          throw new Exception ("can't handle more than two faces");
        }

      Head1 = faces [0];
      Head2 = faces [1];
    }
#endregion
  }
}
