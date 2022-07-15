/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */

namespace Frontend
{
  [GLib.TypeName ("DominoTeamDialog")]
  [Gtk.Template (ResourceName = "teamdialog.ui")]
  public sealed class TeamDialog : Gtk.Dialog
  {
    private GLib.IFile basedir;
    private Rule.Teams teams;

#region Types

    [GLib.TypeName ("DominoListBoxRow+player")]
    [Gtk.Template (ResourceName = "rows/player.ui")]
    sealed class PlayerRow : Gtk.Grid, IListBoxRow<(string?, int)>
    {
      [Gtk.Builder.Object]
      private Gtk.Entry? entry1 = null;
      [Gtk.Builder.Object]
      private Gtk.ComboBoxText? combo1 = null;
      [Gtk.Builder.Object]
      private Gtk.EntryBuffer? entrybuffer1 = null;
      [Gtk.Builder.Object]
      private Gtk.ToggleButton? togglebutton1 = null;
      public event Action Changed;

      private void OnChanged (object? o, EventArgs a) => Changed ();

      public (string?, int) Value
      {
        get
        {
          var types = combo1!.ActiveId;
          var type = int.Parse (types);
          if (togglebutton1!.Active)
            return (entrybuffer1!.Text, 0);
          else
            return (entrybuffer1!.Text, type);
        }

        set
        {
          entrybuffer1!.Text = value.Item1;
          combo1!.ActiveId = value.Item2.ToString ();
          togglebutton1!.Active = value.Item2 == 0;
        }
      }

      public PlayerRow ()
      {
        Gtk.TemplateBuilder.InitTemplate (this);
        Changed += () => { };

        var combo = combo1!;
        var entry = entry1!;
        var toggle = togglebutton1!;

        toggle.AddNotification ("active",
        (o, a) =>
        {
          var active = toggle.Active;
          combo.Visible = !active;
          entry.Visible = active;
        });

        toggle.Active = true;
      }
    }

    [GLib.TypeName ("DominoListBoxRow+team")]
    [Gtk.Template (ResourceName = "rows/team.ui")]
    sealed class TeamRow : Gtk.Grid, IListBoxRow<(string, (string?, int)[])>
    {
      [Gtk.Builder.Object]
      private Gtk.EntryBuffer? entrybuffer1 = null;
      private ListBox<(string?, int), PlayerRow> players;
      public event Action Changed;

      private void OnChanged (object? o, EventArgs a) => Changed ();

      public (string, (string?, int)[]) Value
      {
        get => (entrybuffer1!.Text, players.Value);
        set
        {
          entrybuffer1!.Text = value.Item1;
          players.Value = value.Item2;
        }
      }

      public TeamRow ()
      {
        var builder = Gtk.TemplateBuilder.InitTemplate (this);
        var place = GetPlace (builder, "placeholder1");
        players = new ListBox<(string?, int), PlayerRow> ();
        Changed += () => { };

        players.Changed += () =>
        {
          var me_ = place.Parent;
          var me = (TeamRow) me_;
          me.Changed ();
        };

        players.Hexpand = true;
        players.Vexpand = true;
        players.Show ();
        place.Add (players);
      }
    }

#endregion

#region Callbacks

    private void Cancel (object? o, EventArgs args)
    {
      this.Respond (Gtk.ResponseType.Close);
    }

    private void Save (object? o, EventArgs args)
    {
      teams.Save (basedir);
    }

    private void Apply (object? o, EventArgs args)
    {
      teams.Save (basedir);
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

    private static void ForwardChange (Rule.Teams teams_, (string, (string?, int)[])[] pros)
    {
      int i = 0, j = 0;
      var n_teams = pros.Length;
      teams_.Them = new Rule.Team [n_teams];

      foreach (var team in pros)
      {
        j = 0;

        var
        team_ = new Rule.Team ();
        team_.Name = team.Item1;
        team_.Players = new Rule.Player [team.Item2.Length];

        foreach (var player in team.Item2)
        {
          var
          player_ = new Rule.Player ();
          player_.Name = player.Item1;
          player_.Type = player.Item2;
          team_.Players [j++] = player_;
        }

        teams_.Them [i++] = team_;
      }
    }

    private static (string, (string?, int)[])[] BackwardChange (Rule.Teams teams_)
    {
      int i = 0, j = 0;
      int nt = 1, np = 1;
      string? name_;
      string name;
      int type;

      var teams = teams_.Them;
      var len = teams!.Length;
      var pros = new (string, (string?, int)[]) [len];
      foreach (var team in teams)
      {
        j = 0;
        var players = team.Players;
        var len1 = players!.Length;
        var pro = new (string?, int) [len1];
        foreach (var player in players)
        {
          name_ = player.Name;
          name = (name_ != null) ? name_ : $"Player{np++}";
          type = player.Type;
          pro [j++] = (name, type);
        }

        name_ = team.Name;
        name = (name_ != null) ? name_ : $"Team{nt++}";
        pros [i++] = (name, pro);
      }
    return pros;
    }

    public TeamDialog  () : this (new Rule.Teams ()) { }
    public TeamDialog (Rule.Teams teams) : base ()
    {
      var builder = Gtk.TemplateBuilder.InitTemplate (this);
      var basedir = GLib.FileFactory.NewForPath (Frontend.Application.BaseDir);
      Gtk.Container place;

      this.basedir = basedir;
      this.teams = teams;

      this.teams.Load (basedir);

      place = GetPlace (builder, "placeholder1");
      var box1 = new ListBox<(string, (string?, int)[]), TeamRow> ();
      box1.AddNotification ("value", (o, a) => ForwardChange (teams, box1.Value));
      box1.Value = BackwardChange (teams);
      box1.Hexpand = true;
      box1.Visible = true;
      place.Add (box1);
    }

#endregion
  }
}
