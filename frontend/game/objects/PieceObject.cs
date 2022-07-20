/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */
using OpenTK.Mathematics;

namespace Frontend.Game.Objects
{
  public class PieceObject : Engine.SingleObject
  {
    private static readonly Vector3 scale = new Vector3 (30, 30, 30);

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

    public PieceObject (Pieces pieces, params int[] faces)
      : base (pieces [faces])
    {
      if (faces.Length != 2)
        {
          throw new Exception ("can't handle more than two faces");
        }

      Head1 = faces [0];
      Head2 = faces [1];
      Scale = scale;
    }
#endregion
  }
}
