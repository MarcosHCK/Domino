/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */

namespace frontend.Rows
{
  [GLib.TypeName ("DominoListBoxRow+refrescador")]
  [Gtk.Template (ResourceName = "rows/refrescador.ui")]
  public sealed class refrescador : Gtk.Grid, IListBoxRow<(int, int, int, int, int[])>
  {
    [Gtk.Builder.Object]
    private Gtk.ComboBoxText? combo1;
    [Gtk.Builder.Object]
    private Gtk.Adjustment? adjustment1;
    [Gtk.Builder.Object]
    private Gtk.Adjustment? adjustment2;
    [Gtk.Builder.Object]
    private Gtk.Adjustment? adjustment3;
    [Gtk.Builder.Object]
    private Gtk.Grid? placeholder1;
    private ListBox<int, Verbs> listbox1;
    public event Action Changed;

    [GLib.TypeName ("DominoListBoxRow+refrescador2")]
    [Gtk.Template (ResourceName = "rows/refrescador2.ui")]
    sealed class Verbs : Gtk.Grid, IListBoxRow<int>
    {
      [Gtk.Builder.Object]
      private Gtk.ComboBoxText? combo1;
      public event Action Changed;

      private void OnChanged (object? o, EventArgs a) => Changed ();

      public int Value
      {
        get => int.Parse (combo1!.ActiveId);
        set => combo1!.ActiveId = value.ToString ();
      }

      public Verbs ()
      {
        Gtk.TemplateBuilder.InitTemplate (this);
        Changed += () => { };
      }
    }

    private void OnChanged (object? o, EventArgs a) => Changed ();

    public (int, int, int, int, int[]) Value
    {
      get
      {
        var val1 = int.Parse (combo1!.ActiveId);
        var val2 = (int) adjustment1!.Value;
        var val3 = (int) adjustment2!.Value;
        var val4 = (int) adjustment3!.Value;
        return (val1, val2, val3, val4, listbox1.Value);
      }

      set
      {
        combo1!.ActiveId = value.Item1.ToString ();
        adjustment1!.Value = value.Item2;
        adjustment2!.Value = value.Item3;
        adjustment3!.Value = value.Item4;
        listbox1.Value = value.Item5;
      }
    }

    public refrescador ()
    {
      Gtk.TemplateBuilder.InitTemplate (this);
      Changed += () => { };

      listbox1 = new ListBox<int, Verbs> ();
      placeholder1!.Add (listbox1);
      listbox1.Hexpand = true;
      listbox1.Vexpand = true;
      listbox1.ShowAll ();
    }
  }
}