/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */

namespace Frontend.Rows
{
  [GLib.TypeName ("DominoListBoxRow+repartidor")]
  [Gtk.Template (ResourceName = "rows/repartidor.ui")]
  public sealed class repartidor : Gtk.Grid, IListBoxRow<((int, int), (int, int), int[])>
  {
    [Gtk.Builder.Object]
    private Gtk.ComboBoxText? combo1 = null;
    [Gtk.Builder.Object]
    private Gtk.ComboBoxText? combo2 = null;
    [Gtk.Builder.Object]
    private Gtk.Adjustment? adjustment1 = null;
    [Gtk.Builder.Object]
    private Gtk.Adjustment? adjustment2 = null;
    [Gtk.Builder.Object]
    private Gtk.Grid? placeholder1 = null;
    private ListBox<int, Verbs> listbox1;
    public event Action Changed;

    [GLib.TypeName ("DominoListBoxRow+repartidor2")]
    [Gtk.Template (ResourceName = "rows/repartidor2.ui")]
    sealed class Verbs : Gtk.Grid, IListBoxRow<int>
    {
      [Gtk.Builder.Object]
      private Gtk.ComboBoxText? combo1 = null;
      [Gtk.Builder.Object]
      private Gtk.CheckButton? checkbutton1 = null;
      public event Action Changed;

      private void OnChanged (object? o, EventArgs a) => Changed ();

      public int Value
      {
        get => int.Parse (combo1!.ActiveId) * ((checkbutton1!.Active) ? -1 : 1);
        set
        {
          if (value >= 0)
            combo1!.ActiveId = value.ToString ();
          else
          {
            checkbutton1!.Active = true;
            combo1!.ActiveId = (-value).ToString ();
          }
        }
      }

      public Verbs ()
      {
        Gtk.TemplateBuilder.InitTemplate (this);
        Changed += () => { };
      }
    }

    private void OnChanged (object? o, EventArgs a) => Changed ();

    public ((int, int), (int, int), int[]) Value
    {
      get
      {
        var val1 = int.Parse (combo1!.ActiveId);
        var val2 = (int) adjustment1!.Value;
        var val3 = int.Parse (combo2!.ActiveId);
        var val4 = (int) adjustment2!.Value;
        return ((val1, val2), (val3, val4), listbox1.Value);
      }

      set
      {
        combo1!.ActiveId = value.Item1.Item1.ToString ();
        adjustment1!.Value = value.Item1.Item2;
        combo2!.ActiveId = value.Item2.Item1.ToString ();
        adjustment2!.Value = value.Item2.Item2;
        listbox1.Value = value.Item3;
      }
    }

    public repartidor ()
    {
      Gtk.TemplateBuilder.InitTemplate (this);
      Changed += () => { };

      listbox1 = new ListBox<int, Verbs> ();
      placeholder1!.Add (listbox1);
      listbox1.ShowAll ();
    }
  }
}
