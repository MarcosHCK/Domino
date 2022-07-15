/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */

namespace Frontend.Rows
{
  [GLib.TypeName ("DominoListBoxRow+finisher")]
  [Gtk.Template (ResourceName = "rows/finisher.ui")]
  public sealed class finisher : Gtk.Grid, IListBoxRow<int>
  {
    [Gtk.Builder.Object]
    private Gtk.ComboBoxText? combo1 = null;
    public event Action Changed;

    private void OnChanged (object? o, EventArgs a) => Changed ();

    public int Value
    {
      get => int.Parse (combo1!.ActiveId);
      set => combo1!.ActiveId = value.ToString ();
    }

    public finisher ()
    {
      Gtk.TemplateBuilder.InitTemplate (this);
      Changed += () => { };
    }
  }
}
