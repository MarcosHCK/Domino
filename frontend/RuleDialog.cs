/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */

namespace frontend
{
  [GLib.TypeName ("DominoRuleDialog")]
  [Gtk.Template (ResourceName = "ruledialog.ui")]
  public sealed class RuleDialog : Gtk.Dialog
  {
    private GLib.IFile basedir;
    public Rule Rule;

    private TreeViewManager manager1;

    [Gtk.Builder.Object]
    private Gtk.EntryBuffer? entrybuffer1;
    [Gtk.Builder.Object]
    private Gtk.Adjustment? adjustment1;
    [Gtk.Builder.Object]
    private Gtk.Adjustment? adjustment2;
    [Gtk.Builder.Object]
    private Gtk.Adjustment? adjustment3;
    [Gtk.Builder.Object]
    private Gtk.Adjustment? adjustment4;
    [Gtk.Builder.Object]
    private Gtk.ComboBoxText? combobox1;
    [Gtk.Builder.Object]
    private Gtk.ComboBoxText? combobox2;
    [Gtk.Builder.Object]
    private Gtk.ComboBoxText? combobox3;
    [Gtk.Builder.Object]
    private Gtk.CheckButton? checkbutton1;
    [Gtk.Builder.Object]
    private Gtk.TreeView? treeview1;

#region Callbacks

    public void Cancel (object? o, EventArgs args)
    {
      this.Respond (Gtk.ResponseType.Close);
    }

    public void Save (object? o, EventArgs args)
    {
      Rule.Save (basedir.GetChild (Rule.Name));
    }

    public void Apply (object? o, EventArgs args)
    {
      Rule.Save (basedir.GetChild (Rule.Name));
      this.Respond (Gtk.ResponseType.Apply);
    }

#endregion

#region Constructors
    public RuleDialog () : this (new Rule ()) => Gtk.TemplateBuilder.InitTemplate (this);

    public RuleDialog (Rule Rule) : base ()
    {
      basedir = GLib.FileFactory.NewForPath (frontend.Application.BaseDir);
      this.Rule = Rule;

      entrybuffer1!.Text = Rule.Name;
      entrybuffer1!.AddNotification ("text", (o, a) => Rule.Name = entrybuffer1!.Text);
      adjustment1!.Value = Rule.HeadMaxNumber;
      adjustment1!.AddNotification ("value", (o, a) => Rule.HeadMaxNumber = (int) adjustment1!.Value);
      adjustment2!.Value = Rule.HeadsNumber;
      adjustment2!.AddNotification ("value", (o, a) => Rule.HeadsNumber = (int) adjustment2!.Value);
      adjustment3!.Value = Rule.HandSize;
      adjustment3!.AddNotification ("value", (o, a) => Rule.HandSize = (int) adjustment3!.Value);
      adjustment4!.Value = Rule.TokenRepeats;
      adjustment4!.AddNotification ("value", (o, a) => Rule.TokenRepeats = (int) adjustment4!.Value);
      combobox1!.ActiveId = Rule.TokenRating.ToString ();
      combobox1!.AddNotification ("token-rating", (o, a) => Rule.TokenRating = int.Parse (combobox1.ActiveId));
      combobox2!.ActiveId = Rule.PlayerRating.ToString ();
      combobox2!.AddNotification ("token-rating", (o, a) => Rule.PlayerRating = int.Parse (combobox2.ActiveId));
      combobox3!.ActiveId = Rule.TeamRating.ToString ();
      combobox3!.AddNotification ("token-rating", (o, a) => Rule.TeamRating = int.Parse (combobox3.ActiveId));
      checkbutton1!.Active = Rule.DoublesAppears;
      checkbutton1!.AddNotification ("active", (o, a) => Rule.DoublesAppears = checkbutton1!.Active);

      manager1 = new TreeViewManager (treeview1!);
    }
#endregion
  }
}
