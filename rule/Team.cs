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

    [GLib.Property ("has-human")]
    public bool HasHuman
    {
      get
      {
        bool has = false;
        foreach (var player in Players)
        if (player.IsHuman)
        {
          has = true;
          break;
        }
      return has;
      }
    }

    [GLib.Property ("players")]
    public Player[] Players { get; set; }

    public Team () => Players = new Player [0];
  }
}
