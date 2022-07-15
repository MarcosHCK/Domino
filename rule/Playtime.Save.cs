/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/rule.
 *
 */

namespace Rule
{
  public sealed partial class Playtime : GLib.Object, IFileBased
  {
    public void Save (GLib.IFile savedir, GLib.Cancellable? cancellable = null)
    {
      GLib.DataOutputStream data;
      GLib.OutputStream stream;
      GLib.IFile file;

      if (!savedir.QueryExists (cancellable))
        savedir.MakeDirectory (cancellable);

#region Numeros.txt

      file = savedir.GetChild ("Numeros.txt");
      stream = file.Replace (null, false, GLib.FileCreateFlags.None, cancellable);
      data = new GLib.DataOutputStream (stream);
      data.PutString ($"{this.HeadMaxNumber}\r\n", cancellable);
      data.PutString ($"{this.HeadsNumber}\r\n", cancellable);
      data.PutString ($"{this.HandSize}\r\n", cancellable);
      data.Close (cancellable);
      stream.Close (cancellable);

#endregion

#region Puntuador.txt

      file = savedir.GetChild ("Puntuador.txt");
      stream = file.Replace (null, false, GLib.FileCreateFlags.None, cancellable);
      data = new GLib.DataOutputStream (stream);
      data.PutString ($"{this.TokenRating}\r\n", cancellable);
      data.PutString ($"{this.PlayerRating}\r\n", cancellable);
      data.PutString ($"{this.TeamRating}\r\n", cancellable);
      data.Close (cancellable);
      stream.Close (cancellable);

#endregion

#region Creador.txt

      file = savedir.GetChild ("Creador.txt");
      stream = file.Replace (null, false, GLib.FileCreateFlags.None, cancellable);
      data = new GLib.DataOutputStream (stream);
      data.PutString ((this.DoublesAppears ? "Si" : "No") + "\r\n", cancellable);
      data.PutString ($"{this.TokenRepeats}\r\n", cancellable);
      data.Close (cancellable);
      stream.Close (cancellable);

#endregion

#region Finisher.txt

      file = savedir.GetChild ("Finisher.txt");
      stream = file.Replace (null, false, GLib.FileCreateFlags.None, cancellable);
      data = new GLib.DataOutputStream (stream);

      foreach (var array in FinishConditions)
      {
        data.PutString ("1 ", cancellable);
        for (int i = 0; i < array.Length; i++)
        if (i == 0)
          data.PutString (array [i].ToString (), cancellable);
        else
        {
          data.PutString (" " + array [i], cancellable);
        }

        data.PutString ("\r\n", cancellable);
      }

      data.PutString ("break\r\n", cancellable);
      data.Close (cancellable);
      stream.Close (cancellable);

#endregion

#region Emparejador.txt

      file = savedir.GetChild ("Emparejador.txt");
      stream = file.Replace (null, false, GLib.FileCreateFlags.None, cancellable);
      data = new GLib.DataOutputStream (stream);

      foreach (var tuple in PairingConditions)
      {
        data.PutString (tuple.Item1 + " ", cancellable);
        var array = tuple.Item2;

        for (int i = 0; i < array.Length; i++)
        if (i == 0)
          data.PutString (array [i].ToString (), cancellable);
        else
        {
          data.PutString (" " + array [i], cancellable);
        }

        data.PutString ("\r\n", cancellable);
      }

      data.PutString ("break\r\n", cancellable);
      data.Close (cancellable);
      stream.Close (cancellable);

#endregion

#region Validador.txt

      file = savedir.GetChild ("Validador.txt");
      stream = file.Replace (null, false, GLib.FileCreateFlags.None, cancellable);
      data = new GLib.DataOutputStream (stream);

      foreach (var tuple in ValidMoveConditions)
      {
        data.PutString (tuple.Item1 + " ", cancellable);
        var array = tuple.Item2;

        for (int i = 0; i < array.Length; i++)
        if (i == 0)
          data.PutString (array [i].ToString (), cancellable);
        else
        {
          data.PutString (" " + array [i], cancellable);
        }

        data.PutString ("\r\n", cancellable);
      }

      data.PutString ("break\r\n", cancellable);
      data.Close (cancellable);
      stream.Close (cancellable);

#endregion

#region MoverTurno.txt

      file = savedir.GetChild ("MoverTurno.txt");
      stream = file.Replace (null, false, GLib.FileCreateFlags.None, cancellable);
      data = new GLib.DataOutputStream (stream);

      foreach (var tuple in NextPlayerConditions)
      {
        data.PutString (tuple.Item1 + " ", cancellable);
        var array = tuple.Item2;

        for (int i = 0; i < array.Length; i++)
        if (i == 0)
          data.PutString (array [i].ToString (), cancellable);
        else
        {
          data.PutString (" " + array [i], cancellable);
        }

        data.PutString ("\r\n", cancellable);
      }

      data.PutString ("break\r\n", cancellable);
      data.Close (cancellable);
      stream.Close (cancellable);

#endregion

      int idx = 0;

#region Refrescador.txt

      file = savedir.GetChild ("Refrescador.txt");
      stream = file.Replace (null, false, GLib.FileCreateFlags.None, cancellable);
      data = new GLib.DataOutputStream (stream);

      foreach (var tuple in ValidFlushes)
      {
        data.PutString (tuple.Item1 + " ", cancellable);
        data.PutString (tuple.Item2 + " ", cancellable);
        data.PutString (tuple.Item3 + " ", cancellable);
        data.PutString (tuple.Item4 + "\r\n", cancellable);
      }

      data.PutString ("break\r\n", cancellable);

      idx = 0;
      foreach (var tuple in ValidFlushes)
      {
        data.PutString ((++idx) + " ", cancellable);
        var array = tuple.Item5;

        for (int i = 0; i < array.Length; i++)
        if (i == 0)
          data.PutString (array [i].ToString (), cancellable);
        else
        {
          data.PutString (" " + array [i], cancellable);
        }

        data.PutString ("\r\n", cancellable);
      }

      data.PutString ("break\r\n", cancellable);

      data.Close (cancellable);
      stream.Close (cancellable);

#endregion

#region Repartidor.txt

      file = savedir.GetChild ("Repartidor.txt");
      stream = file.Replace (null, false, GLib.FileCreateFlags.None, cancellable);
      data = new GLib.DataOutputStream (stream);

      foreach (var tuple in ValidTakes)
      {
        data.PutString (tuple.Item1.Item1 + " ", cancellable);
        data.PutString (tuple.Item1.Item2 + "\r\n", cancellable);
        data.PutString (tuple.Item2.Item1 + " ", cancellable);
        data.PutString (tuple.Item2.Item2 + "\r\n", cancellable);
      }

      data.PutString ("break\r\n", cancellable);

      idx = 0;
      foreach (var tuple in ValidFlushes)
      {
        data.PutString ((++idx) + " ", cancellable);
        var array = tuple.Item5;

        for (int i = 0; i < array.Length; i++)
        if (i == 0)
          data.PutString (array [i].ToString (), cancellable);
        else
        {
          data.PutString (" " + array [i], cancellable);
        }

        data.PutString ("\r\n", cancellable);
      }

      data.PutString ("break\r\n", cancellable);

      data.Close (cancellable);
      stream.Close (cancellable);

#endregion
    }
  }
}
