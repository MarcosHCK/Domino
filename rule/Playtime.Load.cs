/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/rule.
 *
 */

namespace Rule
{
  public sealed partial class Playtime : GLib.Object, IFileBased
  {
    private static int[][]
    LoadStyle1 (GLib.IFile file, GLib.Cancellable? cancellable = null)
    {
      GLib.DataInputStream data;
      GLib.InputStream stream;
      List<int> verbs;
      ulong length;
      string line;

      stream = file.Read (cancellable);
      data = new GLib.DataInputStream (stream);
      var list = new List<int[]> ();

      while (true)
      {
        line = data.ReadLine (out length, cancellable);
        if (line == null)
          throw new Exception ("Unexpected end of file");
        else
        {
          if (Common.IsBreak (line))
            break;

          var two = line.Split ('\x20');
          if (2 > two.Length)
            throw new Exception ("Malformed line");
          else
          {
            verbs = new List<int> ();
            for (int i = 1; i < two.Length; i++)
              verbs.Add (int.Parse (two [i]));
            list.Add (verbs.ToArray ());
          }
        }
      }

      data.Close (cancellable);
      stream.Close (cancellable);
    return list.ToArray ();
    }

    private static (int, int[])[]
    LoadStyle2 (GLib.IFile file, GLib.Cancellable? cancellable = null)
    {
      GLib.DataInputStream data;
      GLib.InputStream stream;
      List<int> verbs;
      ulong length;
      string line;

      stream = file.Read (cancellable);
      data = new GLib.DataInputStream (stream);
      var list = new List<(int, int[])> ();

      while (true)
      {
        line = data.ReadLine (out length, cancellable);
        if (line == null)
          throw new Exception ("Unexpected end of file");
        else
        {
          if (Common.IsBreak (line))
            break;

          var two = line.Split ('\x20');
          if (2 > two.Length)
            throw new Exception ("Malformed line");
          else
          {
            verbs = new List<int> ();
            for (int i = 1; i < two.Length; i++)
              verbs.Add (int.Parse (two [i]));

            var first = int.Parse (two [0]);
            list.Add ((first, verbs.ToArray ()));
          }
        }
      }

      data.Close (cancellable);
      stream.Close (cancellable);
    return list.ToArray ();
    }

    private static (int, int, int, int, int[])[]
    LoadStyle3 (GLib.IFile file, GLib.Cancellable? cancellable = null)
    {
      GLib.DataInputStream data;
      GLib.InputStream stream;
      List<int> verbs;
      ulong length;
      string line;

      stream = file.Read (cancellable);
      data = new GLib.DataInputStream (stream);
      var list = new List<(int, int, int, int, int[])> ();

      while (true)
      {
        line = data.ReadLine (out length, cancellable);
        if (line == null)
          throw new Exception ("Unexpected end of file");
        else
        {
          if (Common.IsBreak (line))
            break;

          var four = line.Split ('\x20');
          if (four.Length != 4)
            throw new Exception ("Malformed line");
          else
          {
            var val1 = int.Parse (four [0]);
            var val2 = int.Parse (four [1]);
            var val3 = int.Parse (four [2]);
            var val4 = int.Parse (four [3]);
            list.Add ((val1, val2, val3, val4, null!));
          }
        }
      }

      var array = list.ToArray ();

      while (true)
      {
        line = data.ReadLine (out length, cancellable);
        if (line == null)
          throw new Exception ("Unexpected end of file");
        else
        {
          if (Common.IsBreak (line))
            break;

          var two = line.Split ('\x20');
          if (2 > two.Length)
            throw new Exception ("Malformed line");
          else
          {
            var idx = int.Parse (two [0]) - 1;
            if (idx < 0 || idx >= array.Length)
              throw new Exception ("Index out of bounds");
            else
            {
              verbs = new List<int> ();
              for (int i = 1; i < two.Length; i++)
                verbs.Add (int.Parse (two [i]));
              array [idx].Item5 = verbs.ToArray ();
            }
          }
        }
      }

      for (int i = 0; i < array.Length; i++)
        if (array [i].Item5 == null)
          array [i].Item5 = new int [0];

      data.Close (cancellable);
      stream.Close (cancellable);
    return array;
    }

    private static ((int, int), (int, int), int[])[]
    LoadStyle4 (GLib.IFile file, GLib.Cancellable? cancellable = null)
    {
      GLib.DataInputStream data;
      GLib.InputStream stream;
      List<int> verbs;
      ulong length;
      string line;

      stream = file.Read (cancellable);
      data = new GLib.DataInputStream (stream);

      ((int, int), (int, int), int[])[] array;
      var list = new List<(int, int)> ();

      while (true)
      {
        line = data.ReadLine (out length, cancellable);
        if (line == null)
          throw new Exception ("Unexpected end of file");
        else
        {
          if (Common.IsBreak (line))
            break;

          var two = line.Split ('\x20');
          if (two.Length != 2)
            throw new Exception ("Malformed line");
          else
          {
            var val1 = int.Parse (two [0]);
            var val2 = int.Parse (two [1]);
            list.Add ((val1, val2));
          }
        }
      }

      if (list.Count % 2 != 0)
        throw new Exception ("Unpaired condition");
      else
      {
        var count = list.Count / 2;
        var tmp = list.ToArray ();

        array = new ((int, int), (int, int), int[]) [count];
        for (int i = 0; i < count; i++)
          {
            var val1a = tmp [i * 2 + 0].Item1;
            var val1b = tmp [i * 2 + 0].Item2;
            var val2a = tmp [i * 2 + 1].Item1;
            var val2b = tmp [i * 2 + 1].Item2;
            array [i].Item1 = (val1a, val1b);
            array [i].Item2 = (val2a, val2b);
            array [i].Item3 = null!;
          }
      }

      while (true)
      {
        line = data.ReadLine (out length, cancellable);
        if (line == null)
          throw new Exception ("Unexpected end of file");
        else
        {
          if (Common.IsBreak (line))
            break;

          var two = line.Split ('\x20');
          if (2 > two.Length)
            throw new Exception ("Malformed line");
          else
          {
            var idx = int.Parse (two [0]) - 1;
            if (idx < 0 || idx >= array.Length)
              throw new Exception ("Index out of bounds");
            else
            {
              verbs = new List<int> ();
              for (int i = 1; i < two.Length; i++)
                verbs.Add (int.Parse (two [i]));
              array [idx].Item3 = verbs.ToArray ();
            }
          }
        }
      }

      for (int i = 0; i < array.Length; i++)
        if (array [i].Item3 == null)
          array [i].Item3 = new int [0];

      data.Close (cancellable);
      stream.Close (cancellable);
    return array;
    }

    public void Load (GLib.IFile savedir, GLib.Cancellable? cancellable = null)
    {
      GLib.DataInputStream data;
      GLib.InputStream stream;
      GLib.IFile file;
      ulong length;
      string line;

      /* Numeros.txt */
      file = savedir.GetChild ("Numeros.txt");
      stream = file.Read (cancellable);
      data = new GLib.DataInputStream (stream);
      this.HeadMaxNumber = int.Parse (data.ReadLine (out length, cancellable));
      this.HeadsNumber = int.Parse (data.ReadLine (out length, cancellable));
      this.HandSize = int.Parse (data.ReadLine (out length, cancellable));
      data.Close (cancellable);
      stream.Close (cancellable);

      /* Puntuador.txt */
      file = savedir.GetChild ("Puntuador.txt");
      stream = file.Read (cancellable);
      data = new GLib.DataInputStream (stream);
      this.TokenRating = int.Parse (data.ReadLine (out length, cancellable));
      this.PlayerRating = int.Parse (data.ReadLine (out length, cancellable));
      this.TeamRating = int.Parse (data.ReadLine (out length, cancellable));
      data.Close (cancellable);
      stream.Close (cancellable);

      /* Creador.txt */
      file = savedir.GetChild ("Creador.txt");
      stream = file.Read (cancellable);
      data = new GLib.DataInputStream (stream);
      line = data.ReadLine (out length, cancellable);
      if (line.StartsWith ("Si"))
        this.DoublesAppears = true;
      else if (line.StartsWith ("No"))
        this.DoublesAppears = false;
      else
        throw new Exception ($"Unkown value {line}");
      line = data.ReadLine (out length, cancellable);
      this.TokenRepeats = int.Parse (line);
      data.Close (cancellable);
      stream.Close (cancellable);

      /* Finisher.txt */
      file = savedir.GetChild ("Finisher.txt");
      this.FinishConditions = LoadStyle1 (file, cancellable);

      /* Emparejador.txt */
      file = savedir.GetChild ("Emparejador.txt");
      this.PairingConditions = LoadStyle2 (file, cancellable);

      /* Validador.txt */
      file = savedir.GetChild ("Validador.txt");
      this.ValidMoveConditions = LoadStyle2 (file, cancellable);

      /* MoverTurno.txt */
      file = savedir.GetChild ("MoverTurno.txt");
      this.NextPlayerConditions = LoadStyle2 (file, cancellable);

      /* Refrescador.txt */
      file = savedir.GetChild ("Refrescador.txt");
      this.ValidFlushes = LoadStyle3 (file, cancellable);

      /* Repartidor.txt */
      file = savedir.GetChild ("Repartidor.txt");
      this.ValidTakes = LoadStyle4 (file, cancellable);
    }
  }
}
