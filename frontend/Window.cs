/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using GtkChild = Gtk.Builder.ObjectAttribute;

namespace frontend
{
  [GLib.TypeName ("DominoWindow")]
  [Gtk.Template (ResourceName = "window.ui")]
  public sealed class Window : Gtk.Window
  {
    private GLib.IFile basedir;
    [GtkChild]
    private Gtk.ListBox? listbox1;

    public sealed class RuleListBoxRow : Gtk.ListBoxRow
    {
      private Gtk.Label label;
      public Rule Rule;

      private void OnNameChanged (object? o, GLib.NotifyArgs args)
      {
        label.Text = Rule.Name;
      }

      public RuleListBoxRow (Rule Rule)
      {
        this.label = new Gtk.Label ();
        this.Add ((Gtk.Widget) label);
        this.Rule = Rule;

        Rule.AddNotification ("name", OnNameChanged);
        this.label.Text = this.Rule.Name;
        this.label.Visible = true;
      }
    }

#region Callbacks

    public void StartRule (object? button, EventArgs ev)
    {
      var row = listbox1!.SelectedRow;
      if (row is RuleListBoxRow)
        {
          var rule = ((RuleListBoxRow) row).Rule;
          var path = frontend.Application.LibexecDir;
          var binary = System.IO.Path.Combine (path, "backend");
          var engine = new Game.Engine (rule, binary);
          var game = new Game.Window (engine);
          var appl = Application;

          game.Application = appl;
          game.Present ();
        }
    }

    public void AddRule (object? button, EventArgs ev)
    {
      var
      dialog = new RuleDialog ();
      dialog.TransientFor = this;
      dialog.Run ();
      dialog.Destroy ();
    }

    public void EditRule (object? button, EventArgs ev)
    {
      var row = listbox1!.SelectedRow;
      if (row is RuleListBoxRow)
        {
          var rule = ((RuleListBoxRow) row).Rule;
          var dialog = new RuleDialog (rule);

          dialog.TransientFor = this;
          dialog.Run ();
          dialog.Destroy ();
        }
    }

    private static void DeleteDeep (GLib.IFile file, GLib.FileInfo info, GLib.Cancellable? cancellable = null)
    {
      switch (info.FileType)
      {
      case GLib.FileType.Directory:
        var space = "standard::name,standard::type";
        var files = file.EnumerateChildren (space, 0, cancellable);
        foreach (var info_ in files)
          {
            var file_ = file.GetChild (info_.Name);
            DeleteDeep (file_, info_, cancellable);
          }
        goto default;
      default:
        file.Delete (cancellable);
        break;
      }
    }

    public void RemoveRule (object? button, EventArgs ev)
    {
      var row_ = listbox1!.SelectedRow;
      if (row_ is RuleListBoxRow)
        {
          var row = ((RuleListBoxRow) row_);
          var name = row.Rule.Name;
          var file = basedir.GetChild (name);
          var cancel = new GLib.Cancellable ();
          var info = file.QueryInfo ("standard::type", 0, cancel);

          DeleteDeep (file, info, cancel);
          OnRemovedRule (name);
        }
    }

    public void OnAddedRule (Rule rule)
    {
      var row =
      new RuleListBoxRow (rule);
      listbox1!.Add (row);
      row.ShowAll ();
    }

    public void OnRemovedRule (string? rule)
    {
      Gtk.Widget? which = null;
      listbox1!.Foreach ((child) => {
        if (child is RuleListBoxRow)
        if (((RuleListBoxRow) child).Rule.Name == rule)
          which = child;
      });

      if (which != null)
        {
          listbox1!.Remove (which);
          which!.Destroy ();
        }
    }

#endregion

#region Constructors
    public Window () : this (false) { }
    private Window (bool re) : base (null)
    {
      Gtk.TemplateBuilder.InitTemplate (this);
      var basedir = frontend.Application.BaseDir;
      var dirs = Directory.EnumerateDirectories (basedir);
      this.basedir = GLib.FileFactory.NewForPath (basedir);
      foreach (var path in dirs)
        {
          var name = System.IO.Path.GetFileName (path);
          var file = GLib.FileFactory.NewForPath (path);
          var rule = new Rule (name);

          rule.Load (file);
          OnAddedRule (rule);
        }
    }
#endregion
  }
}
