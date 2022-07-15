/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */

namespace frontend
{
  [GLib.TypeName ("DominoWindow")]
  [Gtk.Template (ResourceName = "window.ui")]
  public sealed class Window : Gtk.Window
  {
    private GLib.IFile basedir;
    [Gtk.Builder.Object]
    private Gtk.ListBox? listbox1;

    sealed class RuleListBoxRow : Gtk.ListBoxRow
    {
      private Gtk.Label label;
      public Rule.Playtime Rule { get; private set; }

      private void OnNameChanged (object? o, GLib.NotifyArgs args)
      {
        label.Text = Rule.Name;
      }

      public RuleListBoxRow (Rule.Playtime Rule)
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

    private void StartRule (object? button, EventArgs a)
    {
      var row = listbox1!.SelectedRow;
      if (row is RuleListBoxRow)
        {
          var rule = ((RuleListBoxRow) row).Rule;
          if (rule.Name != null)
          {
            var args = new string [] { rule.Name, };
            var domain = Thread.GetDomain ();
            var basedir = domain.BaseDirectory;
            var path = System.IO.Path.Combine (basedir, "Frontend");
            System.Diagnostics.Process.Start (path, args);
          }
        }
    }

    private void AddRule (object? button, EventArgs a)
    {
      var
      dialog = new RuleDialog ();
      dialog.TransientFor = this;
      dialog.Run ();
      dialog.Destroy ();
      UpdateRules ();
    }

    private void EditRule (object? button, EventArgs a)
    {
      var row = listbox1!.SelectedRow;
      if (row is RuleListBoxRow)
        {
          var rule = ((RuleListBoxRow) row).Rule;
          var dialog = new RuleDialog (rule);

          dialog.TransientFor = this;
          dialog.Run ();
          dialog.Destroy ();
          UpdateRules ();
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

    private void RemoveRule (object? button, EventArgs ev)
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

    private void EditPlayers (object? button, EventArgs a)
    {
      var
      dialog = new TeamDialog ();
      dialog.TransientFor = this;
      dialog.Run ();
      dialog.Destroy ();
    }

    private void OnAddedRule (Rule.Playtime rule)
    {
      var row =
      new RuleListBoxRow (rule);
      listbox1!.Add (row);
      row.ShowAll ();
    }

    private void OnRemovedRule (string? rule)
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

    private void UpdateRules ()
    {
      var basedir = frontend.Application.BaseDir;
      var dirs = Directory.EnumerateDirectories (basedir);

      foreach (RuleListBoxRow row in listbox1!.Children)
        OnRemovedRule (row.Rule.Name);

      foreach (var path in dirs)
        {
          var name = System.IO.Path.GetFileName (path);
          var file = GLib.FileFactory.NewForPath (path);
          var rule = new Rule.Playtime (name);

          rule.Load (file);
          OnAddedRule (rule);
        }
    }

    public Window () : this (false) { }
    private Window (bool re) : base (null)
    {
      Gtk.TemplateBuilder.InitTemplate (this);
      var basedir = frontend.Application.BaseDir;
      this.basedir = GLib.FileFactory.NewForPath (basedir);
      UpdateRules ();
    }
#endregion
  }
}
