/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/rule.
 *
 */

namespace Rule
{
  public sealed class Player : GLib.Object
  {
    [GLib.Property ("name")]
    public string? Name { get; set; }

    [GLib.Property ("type")]
    public int Type { get; set; }
    [GLib.Property ("is-human")]
    public bool IsHuman { get => Type == 0; }
  }
}
