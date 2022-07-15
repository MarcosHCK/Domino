/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/rule.
 *
 */

namespace Rule
{
  public sealed class Team : GLib.Object, IFileBased
  {
    [GLib.Property ("name")]
    public string? Name { get; set; }

    [GLib.Property ("players")]
    public Player[]? Players { get; set; }

    public void Save (GLib.IFile savedir, GLib.Cancellable? cancellable = null) { }
    public void Load (GLib.IFile savedir, GLib.Cancellable? cancellable = null)
    {
      GLib.IFile file = savedir.GetChild ("Jugadores.txt");
      GLib.InputStream stream = file.Read (cancellable);
      GLib.DataInputStream data = new GLib.DataInputStream (stream);
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
            Name = line;
            var players = new List<Player> ();
            var humans = 0;
            var ais = 0;

            line = data.ReadLine (out length, cancellable);
            if (line == null)
              throw new Exception ("Unexpected end of line");
            else
              humans = int.Parse (line);

            if (humans > 0)
            {
              for (int i = 0; i < humans; i++)
              {
                line = data.ReadLine (out length, cancellable);
                if (line == null)
                  throw new Exception ("Unexpected end of line");
                else
                {
                  var
                  player = new Player ();
                  player.Name = line;
                  player.Type = 0;
                  players.Add (player);
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
          }
        }
      }

      data.Close (cancellable);
      stream.Close (cancellable);
    }
  }
}
