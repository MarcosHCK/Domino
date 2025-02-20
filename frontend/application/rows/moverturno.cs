/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */

namespace Frontend.Rows
{
  [GLib.TypeName ("DominoListBoxRow+moverturno")]
  [Gtk.Template (ResourceName = "rows/moverturno.ui")]
  public sealed class moverturno : Gtk.Grid, IListBoxRow<(int, int[])>
  {
    [Gtk.Builder.Object]
    private Gtk.ComboBoxText? combo1 = null;
    [Gtk.Builder.Object]
    private Gtk.Grid? placeholder1 = null;
    private ListBox<int, Verbs> listbox1;
    public event Action Changed;

    [GLib.TypeName ("DominoListBoxRow+moverturno2")]
    [Gtk.Template (ResourceName = "rows/moverturno2.ui")]
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

    public (int, int[]) Value
    {
      get => (int.Parse (combo1!.ActiveId), listbox1.Value);
      set
      {
        combo1!.ActiveId = value.Item1.ToString ();
        listbox1!.Value = value.Item2;
      }
    }

    public moverturno ()
    {
      Gtk.TemplateBuilder.InitTemplate (this);
      Changed += () => { };

      listbox1 = new ListBox<int, Verbs> ();
      placeholder1!.Add (listbox1);
      listbox1.ShowAll ();
    }
  }
}
