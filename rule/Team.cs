/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/rule.
 *
 */

namespace Rule
{
  public sealed class Team : GLib.Object
  {
    [GLib.Property ("name")]
    public string? Name { get; set; }

    [GLib.Property ("players")]
    public Player[] Players { get; set; }

    public Team () => Players = new Player [0];
  }
}
