/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */

namespace frontend
{
  public sealed class TreeViewManager : GLib.Object
  {
    private Gtk.PopupMenuHandler handler1;
    private Gtk.ButtonPressEventHandler handler2;
    private Gtk.EditedHandler handler3;
    private Gtk.TreeView treeview;
    public List<int> Values { get; private set; }

    public TreeViewManager (Gtk.TreeView view)
    {
      var menu = new Menu ();
      var t1 = GLib.GType.Boolean;
      var t2 = GLib.GType.String;
      var store = new Gtk.TreeStore (t1, t2);
      var list = new Gtk.ListStore (t2);

      this.Values = new List<int> ();
      this.treeview = view;

      handler1 = (o, a) =>
        { System.Console.WriteLine ("ase1");
          OnPopupMenu (menu, (Gtk.TreeView) o, null);
          a.RetVal = true;
        };

      handler2 = (o, a) =>
        { System.Console.WriteLine ("ase2");
          var ev = a.Event;

          if (Gdk.EventHelper.TriggersContextMenu (ev)
            && ev.Type == Gdk.EventType.ButtonPress)
            {
              OnPopupMenu (menu, (Gtk.TreeView) o, ev);
              a.RetVal = true;
            }
          else
            {
              a.RetVal = false;
            }
        };

      view.Model = store;

      var
      text = new Gtk.CellRendererText ();
      text.Editable = false;

      var
      combo = new Gtk.CellRendererCombo ();
      combo.Editable = true;
      combo.HasEntry = false;
      combo.TextColumn = 0;
      combo.Model = list;

      handler3 = (o, a) =>
        {
          Gtk.TreeIter iter;
          var cell = (Gtk.CellRenderer) o!;
          var path = new Gtk.TreePath (a.Path);
          store.GetIter (out iter, path);

          if (cell == combo)
            {
              var idx = path.Indices [1];
              var val = int.Parse (a.NewText);
              Values [idx] = val;
            }
        };

      view.InsertColumn (0, "column0", text, "text", 1);
      view.InsertColumn (1, "column1", combo, "visible", 0);
      var iter = store.AppendValues (false, "1");
      store.AppendValues (iter, true, "2");

      combo.Edited += handler3;

      treeview.PopupMenu += handler1;
      treeview.ButtonPressEvent += handler2;
      menu.AttachToWidget (treeview, null);
    }

    ~TreeViewManager ()
    {
      treeview.RemoveColumn (treeview.GetColumn (0));
      treeview.RemoveColumn (treeview.GetColumn (1));
      treeview.ButtonPressEvent -= handler2;
      treeview.PopupMenu -= handler1;
    }

    static void OnPopupMenu (Menu menu, Gtk.TreeView view, Gdk.EventButton? ev)
    {
      System.Console.WriteLine ("as");
      if (ev == null)
        {
          menu.RemoveContext ();
          menu.PopupAtPointer (ev);
        }
      else
        {
          Gtk.TreePath path;
          Gtk.TreeIter iter;
          Gtk.ITreeModel model;
          int wx = (int) ev.X;
          int wy = (int) ev.Y;
          int x, y;

          view.ConvertWidgetToBinWindowCoords (wx, wy, out x, out y);
          view.GetPathAtPos (x, y, out path);
          model = view.Model;

          if (path != null)
            {
              model.GetIter (out iter, path);
              menu.AddContext (model, iter);
              menu.PopupAtPointer (ev);
            }
          else
            {
              menu.RemoveContext ();
              menu.PopupAtPointer (ev);
            }
        }
    }

    [GLib.TypeName ("DominoTreeViewManagermenu")]
    [Gtk.Template (ResourceName = "managermenu.ui")]
    public class Menu : Gtk.Menu
    {
      [Gtk.Builder.Object]
      private Gtk.MenuItem? item1;
      [Gtk.Builder.Object]
      private Gtk.MenuItem? item2;

      public void AddContext (Gtk.ITreeModel model, Gtk.TreeIter iter) { }
      public void RemoveContext () { }
    }
  }
}
