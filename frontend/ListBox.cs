/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */

namespace frontend
{
  public sealed class ListBox<T,R> : Gtk.Grid, IListBoxRow<T[]>
    where T: notnull
    where R: Gtk.Widget, IListBoxRow<T>, new ()
  {
    public event Action Changed;
    private T[] _Value = new T [0];
    [GLib.Property("value")]
    public T[] Value
    {
      get => (T[]) _Value.Clone ();
      set
      {
        var children = listbox.Children;
        foreach (var child in children)
          RowDelete (listbox, child);

        var i = (int) 0;
        foreach (var val in value)
        {
          var row = new R ();
          row.Value = value [i++];
          RowAdd (listbox, row);
        }
      }
    }

    private Gtk.ListBox listbox;

#region Callbacks

    private static void OnButtonPress (object? o, Gtk.ButtonPressEventArgs a)
    {
      var ev = a.Event;
      if (Gdk.EventHelper.TriggersContextMenu (ev)
        && ev.Type == Gdk.EventType.ButtonPress)
        a.RetVal = ((ListBox<T, R>) o!).DoPopupMenu (ev);
      else a.RetVal = false;
    }

    private static void OnPopupMenu (object? o, Gtk.PopupMenuArgs a)
    {
      ((ListBox<T, R>) o!).DoPopupMenu (null);
      a.RetVal = true;
    }

    private bool DoPopupMenu (Gdk.EventButton? ev)
    {
      Gtk.Menu menu;
      Gtk.MenuItem item1;
      Gtk.MenuItem item2;

      item1 = new Gtk.MenuItem ("Add");
      item1.ShowAll ();
      item2 = new Gtk.MenuItem ("Delete");
      item2.ShowAll ();

      var selected = listbox.SelectedRow;
      var box = listbox;

      item1.Activated += (o, a) => RowAdd (box, new R ());
      item2.Activated += (o, a) => RowDelete (box, selected);

      menu = new Gtk.Menu ();
      menu.Deactivated += (o, a) => menu.Destroy ();
      menu.Append (item1);
      menu.Append (item2);
      menu.AttachToWidget (this, null);
      menu.PopupAtPointer (ev);
      return true;
    }

    private static void RowAdd (Gtk.ListBox listbox, R row)
    {
      var box = (ListBox<T, R>) listbox.Parent;
      var length = box._Value.Length;

      box._Value = new T [length + 1];
      listbox.Add (row);
      row.ShowAll ();

      row.Changed += () => OnRowChanged (listbox);
    }

    private static void RowDelete (Gtk.ListBox listbox, Gtk.Widget? widget)
    {
      if (widget != null)
      {
        var box = (ListBox<T, R>) listbox.Parent;
        var length = box._Value.Length;
        box._Value = new T [length - 1];
        box.Remove (widget);
        widget.Destroy ();
        OnRowChanged (listbox);
      }
    }

    private static void OnRowChanged (Gtk.ListBox listbox)
    {
      var box = (ListBox<T, R>) listbox.Parent;
      int rowi = (int) 0;
      Gtk.Bin bin;
      R row;

      foreach (var row_ in listbox.Children)
      {
        bin = (Gtk.Bin) row_;
        row = (R) bin.Child;
        box._Value [rowi++] = row.Value;
      }
    }

#endregion

    private string Icon (string symbolic)
    {
      var theme = Gtk.IconTheme.GetForScreen (Screen);
      var info  = theme.LookupIcon (symbolic, (int) Gtk.IconSize.Button, 0);
      if (info == null)
        throw new Exception ("can't find icon " + symbolic);
      else
        return info.Filename;
    }

    public ListBox ()
    {
      var grid = this;
      var listbox = new Gtk.ListBox ();
      var box = new Gtk.ButtonBox (Gtk.Orientation.Vertical);
      var button1 = new Gtk.Button ();
      var button2 = new Gtk.Button ();
      var image1 = new Gtk.Image (Icon ("list-add"));
      var image2 = new Gtk.Image (Icon ("list-remove"));
      this.listbox = listbox;

      button1.Child = image1;
      button2.Child = image2;
      button1.Clicked += (o, a) => RowAdd (listbox, new R ());
      button2.Clicked += (o, a) => RowDelete (listbox, listbox.SelectedRow);
      box.PackStart (button1, true, true, 0);
      box.PackStart (button2, true, true, 0);
      box.LayoutStyle = Gtk.ButtonBoxStyle.Expand;
      listbox.Hexpand = true;
      listbox.Vexpand = true;
      grid.Add (listbox);
      grid.Add (box);
      grid.ShowAll ();

      this.Changed += () => { };
      this.ButtonPressEvent += OnButtonPress;
      this.PopupMenu += OnPopupMenu;
    }
  }
}
