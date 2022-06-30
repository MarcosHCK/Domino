/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */

namespace frontend
{
  public sealed class Rule : GLib.Object
  {
#region Properties

    [GLib.Property ("name")]
    public string? Name { get; set; }

    /* Numeros.txt */

    [GLib.Property ("head-max-number")]
    public int HeadMaxNumber { get; set; }
    [GLib.Property ("heads-number")]
    public int HeadsNumber { get; set; }
    [GLib.Property ("hand-size")]
    public int HandSize { get; set; }

    /* Puntuador.txt */

    [GLib.Property ("token-rating")]
    public int TokenRating { get; set; }
    [GLib.Property ("player-rating")]
    public int PlayerRating { get; set; }
    [GLib.Property ("team-rating")]
    public int TeamRating { get; set; }

    /* Creador.txt */

    [GLib.Property ("doubles-appears")]
    public bool DoublesAppears { get; set; }
    [GLib.Property ("token-repeats")]
    public int TokenRepeats { get; set; }

    /* Finisher.txt */

    [GLib.Property ("finish-conditions")]
    public List<List<int>> FinishConditions { get; private set; }

    /* Emparejador.txt */

    [GLib.Property ("pairing-conditions")]
    public List<(int Noun, List<int> Verbs)> PairingConditions { get; private set; }

    /* Validador.txt */

    [GLib.Property ("valid-move-conditions")]
    public List<(int Noun, List<int> Verbs)> ValidMoveConditions { get; private set; }

    /* MoverTurno.txt */

    [GLib.Property ("next-player-conditions")]
    public List<(int Noun, List<int> Verbs)> NextPlayerConditions { get; private set; }

    /* Refrescador.txt */

    [GLib.Property ("valid-flushes")]
    public List<(int Condition, int Required, int Allowed, int Balance, List<int> Verbs)> ValidFlushes { get; private set; }

    /* Repartidor.txt */

    [GLib.Property ("valid-tokens")]
    public List<(int Discard, int Steal, List<int> Verbs)> ValidTakes { get; private set; }

#endregion

#region Writer

    public void Save (GLib.IFile savedir, GLib.Cancellable? cancellable = null)
    {
    }

    public void Load (GLib.IFile savedir, GLib.Cancellable? cancellable = null)
    {
      GLib.DataInputStream data;
      GLib.InputStream stream;
      GLib.IFile file;
      ulong length;
      string line;

      file = savedir.GetChild ("Numeros.txt");
      stream = file.Read (cancellable);
      data = new GLib.DataInputStream (stream);
      this.HeadMaxNumber = int.Parse (data.ReadLine (out length, cancellable));
      this.HeadsNumber = int.Parse (data.ReadLine (out length, cancellable));
      this.HandSize = int.Parse (data.ReadLine (out length, cancellable));

      file = savedir.GetChild ("Puntuador.txt");
      stream = file.Read (cancellable);
      data = new GLib.DataInputStream (stream);
      this.TokenRating = int.Parse (data.ReadLine (out length, cancellable));
      this.PlayerRating = int.Parse (data.ReadLine (out length, cancellable));
      this.TeamRating = int.Parse (data.ReadLine (out length, cancellable));

      file = savedir.GetChild ("Creador.txt");
      stream = file.Read (cancellable);
      data = new GLib.DataInputStream (stream);
      line = data.ReadLine (out length, cancellable);
      if (line == "Si")
        this.DoublesAppears = true;
      else if (line == "No")
        this.DoublesAppears = false;
      else
        throw new Exception ($"Unkown value ${line}");
      line = data.ReadLine (out length, cancellable);
      this.TokenRepeats = int.Parse (line);
    }

#endregion

#region Constructors

    public Rule () : base ()
    {
      this.FinishConditions = new List<List<int>> ();
      this.PairingConditions = new List<(int Noun, List<int> Verb)> ();
      this.ValidMoveConditions = new List<(int Noun, List<int> Verbs)> ();
      this.NextPlayerConditions = new List<(int Noun, List<int> Verbs)> ();
      this.ValidFlushes = new List<(int Condition, int Required, int Allowed, int Balance, List<int> Verbs)> ();
      this.ValidTakes = new List<(int Discard, int Steal, List<int> Verbs)> ();
    }

    public Rule (string Name) : this ()
    {
      this.Name = Name;
    }

#endregion
  }
}
