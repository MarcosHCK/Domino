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
    public Rule.File Rule;
 
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

    private static Gtk.Container GetPlace (Gtk.Builder builder, string name)
    {
      var object_ = builder.GetObject (name);
      if (object_ == null)
        throw new Exception ("can't find " + name);
      else
      {
        var place = object_ as Gtk.Container;
        if (place == null)
          throw new Exception ("can't find " + name);
        else
        {
          return place;
        }
      }
    }

    public RuleDialog () : this (new Rule.File ()) { }
    public RuleDialog (Rule.File Rule) : base ()
    {
      var builder = Gtk.TemplateBuilder.InitTemplate (this);
      var basedir = GLib.FileFactory.NewForPath (frontend.Application.BaseDir);
      Gtk.Container place;

      this.basedir = basedir;
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
      combobox1!.AddNotification ("active-id", (o, a) => Rule.TokenRating = int.Parse (combobox1.ActiveId));
      combobox2!.ActiveId = Rule.PlayerRating.ToString ();
      combobox2!.AddNotification ("active-id", (o, a) => Rule.PlayerRating = int.Parse (combobox2.ActiveId));
      combobox3!.ActiveId = Rule.TeamRating.ToString ();
      combobox3!.AddNotification ("active-id", (o, a) => Rule.TeamRating = int.Parse (combobox3.ActiveId));
      checkbutton1!.Active = Rule.DoublesAppears;
      checkbutton1!.AddNotification ("active", (o, a) => Rule.DoublesAppears = checkbutton1!.Active);

      place = GetPlace (builder, "placeholder1");
      var box1 = new ListBox<int[], ListBox<int, Rows.finisher>> ();
      box1.AddNotification ("values", (o, a) => Rule.FinishConditions = box1.Value);
      box1.Value = Rule.FinishConditions;
      box1.Hexpand = true;
      box1.Vexpand = true;
      box1.Visible = true;
      place.Add (box1);

      place = GetPlace (builder, "placeholder2");
      var box2 = new ListBox<(int, int[]), Rows.emparejador> ();
      box2.AddNotification ("value", (o, a) => Rule.PairingConditions = box2.Value);
      box2.Value = Rule.PairingConditions;
      box2.Visible = true;
      place.Add (box2);

      place = GetPlace (builder, "placeholder3");
      var box3 = new ListBox<(int, int[]), Rows.validador> ();
      box3.AddNotification ("value", (o, a) => Rule.ValidMoveConditions = box3.Value);
      box3.Value = Rule.ValidMoveConditions;
      box3.Visible = true;
      place.Add (box3);

      place = GetPlace (builder, "placeholder4");
      var box4 = new ListBox<(int, int[]), Rows.moverturno> ();
      box4.AddNotification ("value", (o, a) => Rule.NextPlayerConditions = box4.Value);
      box4.Value = Rule.NextPlayerConditions;
      box4.Visible = true;
      place.Add (box4);

      place = GetPlace (builder, "placeholder5");
      var box5 = new ListBox<(int, int, int, int, int[]), Rows.refrescador> ();
      box5.AddNotification ("value", (o, a) => Rule.ValidFlushes = box5.Value);
      box5.Value = Rule.ValidFlushes;
      box5.Visible = true;
      place.Add (box5);

      place = GetPlace (builder, "placeholder6");
      var box6 = new ListBox<((int, int), (int, int), int[]), Rows.repartidor> ();
      box6.AddNotification ("value", (o, a) => Rule.ValidTakes = box6.Value);
      box6.Value = Rule.ValidTakes;
      box6.Visible = true;
      place.Add (box6);
    }

#endregion
  }
}
