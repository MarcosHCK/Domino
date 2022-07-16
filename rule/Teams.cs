/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/rule.
 *
 */

namespace Rule
{
  public sealed class Teams : GLib.Object, IFileBased
  {
    [GLib.Property ("teams")]
    public Team[] Them { get; set; }

    [GLib.Property ("has-human")]
    public bool HasHuman
    {
      get
      {
        bool has = false;
        foreach (var team in Them)
        if (team.HasHuman)
        {
          has = true;
          break;
        }
      return has;
      }
    }

    public void Save (GLib.IFile savedir, GLib.Cancellable? cancellable = null)
    {
      GLib.IFile file = savedir.GetChild ("Jugadores.txt");
      GLib.OutputStream stream = file.Replace (null, false, 0, cancellable);
      GLib.DataOutputStream data = new GLib.DataOutputStream (stream);

      var teams = Them!;
      foreach (var team in teams)
      {
        data.PutString (team.Name + "\n", cancellable);
        var humans = 0;
        var i = 0;

        foreach (var player in team.Players)
        if (player.IsHuman)
          ++humans;

        data.PutString (humans + "\n", cancellable);

        foreach (var player in team.Players)
        if (player.IsHuman)
          data.PutString (player.Name + "\n", cancellable);

        foreach (var player in team.Players)
        if (!player.IsHuman)
        if (i > 0)
          data.PutString (" " + player.Type, cancellable);
        else
        {
          data.PutString (player.Type.ToString (), cancellable);
          ++i;
        }

        data.PutString ("\n", cancellable);
      }

      data.PutString ("break\n", cancellable);
      data.Close (cancellable);
      stream.Close (cancellable);
    }

    public void Load (GLib.IFile savedir, GLib.Cancellable? cancellable = null)
    {
      GLib.IFile file = savedir.GetChild ("Jugadores.txt");
      GLib.InputStream stream = file.Read (cancellable);
      GLib.DataInputStream data = new GLib.DataInputStream (stream);
      var teams = new List<Team> ();
      ulong length;

      while (true)
      {
        var line = data.ReadLine (out length, cancellable);
        if (line == null)
          throw new Exception ("Unexpected end of line");
        else
        {
          if (Common.IsBreak (line))
            break;
          else
          {
            var players = new List<Player> ();
            var team = new Team ();
            var humans = 0;
            var ais = 0;

            team.Name = line;

            line = data.ReadLine (out length, cancellable);
            if (line == null)
              throw new Exception ("Unexpected end of line");
            else
            {
              var inputs = line.Split (' ');
              if (inputs.Length == 1)
              {
                humans = int.Parse (inputs [0]);
                if (humans != 0)
                {
                  throw new Exception ("Invalid human players count");
                }
              }
              else
              {
                humans = int.Parse (inputs [0]);
                if (humans <= 0 || (inputs.Length - 1) != humans)
                  throw new Exception ("Invalid human players count");
                else
                {
                  for (int i = 0; i < humans; i++)
                  {
                    var
                    player = new Player ();
                    player.Name = inputs [i + 1];
                    player.Type = 0;
                    players.Add (player);
                  }
                }
              }
            }

            line = data.ReadLine (out length, cancellable);
            if (line == null)
              throw new Exception ("Unexpected end of line");
            else
            {
              var players_ = line.Split ('\x20');
              foreach (var player_ in players_)
              {
                var
                player = new Player ();
                player.Name = $"AI{ais++}";
                player.Type = int.Parse (player_);
                players.Add (player);
              }
            }

            if (players.Count == 0)
              throw new Exception ("Empty team " + team.Name);
            else
            {
              team.Players = players.ToArray ();
              teams.Add (team);
            }
          }
        }
      }

      Them = teams.ToArray ();
      data.Close (cancellable);
      stream.Close (cancellable);
    }

    public Teams () => Them = new Team [0];
  }
}
