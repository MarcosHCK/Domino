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
    private Rule.Playtime rule;
 
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

    private void Cancel (object? o, EventArgs args)
    {
      this.Respond (Gtk.ResponseType.Close);
    }

    private void Save (object? o, EventArgs args)
    {
      rule.Save (basedir.GetChild (rule.Name));
    }

    private void Apply (object? o, EventArgs args)
    {
      rule.Save (basedir.GetChild (rule.Name));
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

    public RuleDialog () : this (new Rule.Playtime ()) { }
    public RuleDialog (Rule.Playtime rule) : base ()
    {
      var builder = Gtk.TemplateBuilder.InitTemplate (this);
      var basedir = GLib.FileFactory.NewForPath (frontend.Application.BaseDir);
      Gtk.Container place;

      this.basedir = basedir;
      this.rule = rule;

      entrybuffer1!.Text = rule.Name;
      entrybuffer1!.AddNotification ("text", (o, a) => rule.Name = entrybuffer1!.Text);
      adjustment1!.Value = rule.HeadMaxNumber;
      adjustment1!.AddNotification ("value", (o, a) => rule.HeadMaxNumber = (int) adjustment1!.Value);
      adjustment2!.Value = rule.HeadsNumber;
      adjustment2!.AddNotification ("value", (o, a) => rule.HeadsNumber = (int) adjustment2!.Value);
      adjustment3!.Value = rule.HandSize;
      adjustment3!.AddNotification ("value", (o, a) => rule.HandSize = (int) adjustment3!.Value);
      adjustment4!.Value = rule.TokenRepeats;
      adjustment4!.AddNotification ("value", (o, a) => rule.TokenRepeats = (int) adjustment4!.Value);
      combobox1!.ActiveId = rule.TokenRating.ToString ();
      combobox1!.AddNotification ("active-id", (o, a) => rule.TokenRating = int.Parse (combobox1.ActiveId));
      combobox2!.ActiveId = rule.PlayerRating.ToString ();
      combobox2!.AddNotification ("active-id", (o, a) => rule.PlayerRating = int.Parse (combobox2.ActiveId));
      combobox3!.ActiveId = rule.TeamRating.ToString ();
      combobox3!.AddNotification ("active-id", (o, a) => rule.TeamRating = int.Parse (combobox3.ActiveId));
      checkbutton1!.Active = rule.DoublesAppears;
      checkbutton1!.AddNotification ("active", (o, a) => rule.DoublesAppears = checkbutton1!.Active);

      place = GetPlace (builder, "placeholder1");
      var box1 = new ListBox<int[], ListBox<int, Rows.finisher>> ();
      box1.AddNotification ("value", (o, a) => rule.FinishConditions = box1.Value);
      box1.Value = rule.FinishConditions;
      box1.Visible = true;
      place.Add (box1);

      place = GetPlace (builder, "placeholder2");
      var box2 = new ListBox<(int, int[]), Rows.emparejador> ();
      box2.AddNotification ("value", (o, a) => rule.PairingConditions = box2.Value);
      box2.Value = rule.PairingConditions;
      box2.Visible = true;
      place.Add (box2);

      place = GetPlace (builder, "placeholder3");
      var box3 = new ListBox<(int, int[]), Rows.validador> ();
      box3.AddNotification ("value", (o, a) => rule.ValidMoveConditions = box3.Value);
      box3.Value = rule.ValidMoveConditions;
      box3.Visible = true;
      place.Add (box3);

      place = GetPlace (builder, "placeholder4");
      var box4 = new ListBox<(int, int[]), Rows.moverturno> ();
      box4.AddNotification ("value", (o, a) => rule.NextPlayerConditions = box4.Value);
      box4.Value = rule.NextPlayerConditions;
      box4.Visible = true;
      place.Add (box4);

      place = GetPlace (builder, "placeholder5");
      var box5 = new ListBox<(int, int, int, int, int[]), Rows.refrescador> ();
      box5.AddNotification ("value", (o, a) => rule.ValidFlushes = box5.Value);
      box5.Value = rule.ValidFlushes;
      box5.Visible = true;
      place.Add (box5);

      place = GetPlace (builder, "placeholder6");
      var box6 = new ListBox<((int, int), (int, int), int[]), Rows.repartidor> ();
      box6.AddNotification ("value", (o, a) => rule.ValidTakes = box6.Value);
      box6.Value = rule.ValidTakes;
      box6.Visible = true;
      place.Add (box6);
    }

#endregion
  }
}
