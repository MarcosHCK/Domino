/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */

namespace frontend.Game.Objects
{
  public class PieceObject : Game.Object
  {
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

    public PieceObject Append (PieceObject head, PieceObject link)
    {
      var last = Last (head);
      last.Next = link;
      link.Prev = last;
    return head;
    }

    public PieceObject Prepend (PieceObject head, PieceObject link)
    {
      head.Prev = link;
      link.Next = head;
    return link;
    }

    public PieceObject Last (PieceObject head)
    {
      while (head.Next != null)
        head = head.Next;
    return head;
    }

#endregion

#region Constructor
    public PieceObject (params int[] faces)
      : base (new PieceModel (faces))
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
