/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */

namespace frontend
{
  [GLib.TypeName ("DominoGameWindow")]
  [Gtk.Template (ResourceName = "gamewindow.ui")]
  public sealed class GameWindow : Gtk.Window
  {
    public GameWindow () : base (null) => Gtk.TemplateBuilder.InitTemplate (this);
    public GameWindow (Rule rule) : this () { }
  }
}
