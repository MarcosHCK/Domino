/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/rule.
 *
 */

namespace Rule
{
  public sealed partial class File : GLib.Object
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
    public int[][] FinishConditions { get; set; }

    /* Emparejador.txt */

    [GLib.Property ("pairing-conditions")]
    public (int Noun, int[] Verbs)[] PairingConditions { get; set; }

    /* Validador.txt */

    [GLib.Property ("valid-move-conditions")]
    public (int Noun, int[] Verbs)[] ValidMoveConditions { get; set; }

    /* MoverTurno.txt */

    [GLib.Property ("next-player-conditions")]
    public (int Noun, int[] Verbs)[] NextPlayerConditions { get; set; }

    /* Refrescador.txt */

    [GLib.Property ("valid-flushes")]
    public (int Condition, int Required, int Allowed, int Balance, int[] Verbs)[] ValidFlushes { get; set; }

    /* Repartidor.txt */

    [GLib.Property ("valid-tokens")]
    public ((int When, int Pieces) Discard, (int When, int Pieces) Steal, int[] Verbs)[] ValidTakes { get; set; }

#endregion

#region Constructors

    public File () : base ()
    {
      FinishConditions = new int [0][];
      PairingConditions = new (int, int [])[0];
      ValidMoveConditions = new (int, int [])[0];
      NextPlayerConditions = new (int, int [])[0];
      ValidFlushes = new (int, int, int, int, int[])[0];
      ValidTakes = new ((int, int), (int, int), int[])[0];
    }

    public File (string Name) : this ()
    {
      this.Name = Name;
    }

#endregion
  }
}
