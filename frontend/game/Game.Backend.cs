/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Frontend.Game
{
  public class Backend
  {
    private Process proc;
    private ConcurrentQueue<ActionArgs> pendings;
    private long QuitFlag = 0;
    public bool HasExited { get; private set; }

#region Commands API

    public event ActionHandler Action;

    public delegate void ActionHandler (Backend engine, ActionArgs a);
    public abstract class ActionArgs : EventArgs
    {
      public string Player { get; private set; }

      public ActionArgs (string Player)
      {
        this.Player = Player;
      }
    }

    public sealed class ExchangeArgs : ActionArgs
    {
      public int Losses { get; private set; }
      public int Takes { get; private set; }

      public ExchangeArgs (string Player, int Losses, int Takes)
        : base (Player)
      {
        this.Losses = Losses;
        this.Takes = Takes;
      }
    }

    public sealed class MoveArgs : ActionArgs
    {
      public bool Passes { get; private set; }
      public int AtBy { get; private set; }
      public int PutAt { get; private set; }
      public int[]? Piece { get; private set; }

      public MoveArgs (string Player)
        : base (Player)
      {
        this.Passes = true;
      }

      public MoveArgs (string Player, int AtBy, int PutAt, params int[] piece)
        : base (Player)
      {
        this.Passes = false;
        this.AtBy = AtBy;
        this.PutAt = PutAt;
        this.Piece = piece;
      }
    }

    public sealed class EmitHandArgs : ActionArgs
    {
      public int[][] Pieces { get; private set; }
      public EmitHandArgs (string Player, int[][] Pieces)
        : base (Player)
      {
        this.Pieces = Pieces;
      } 
    }

    public sealed class EmitTeamArgs : ActionArgs
    {
      public (string, string[])[] Teams { get; private set; }
      public EmitTeamArgs ((string, string[])[] Teams)
        : base ("")
      {
        this.Teams = Teams;
      }
    }

    public sealed class GameOverArgs : ActionArgs
    {
      public (string, int)[] Scores { get; private set; }
      public GameOverArgs ((string, int)[] Scores)
        : base ("")
      {
        this.Scores = Scores;
      }
    }

#endregion

#region API

    public void PollNext ()
    {
      ActionArgs? action;
      if (pendings.TryDequeue (out action))
        Action (this, action);
    }

#endregion

#region internal API

    [System.Serializable]
    public class EngineException : System.Exception
    {
      public EngineException () { }
      public EngineException (string message) : base (message) { }
      public EngineException (string message, System.Exception inner) : base (message, inner) { }
      protected EngineException (
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base (info, context) { }
    }

    private static int[]? MakePiece (string piece_)
    {
      var match = piece.Match (piece_);
      var list = (List<int>?) null;

      if (match.Success)
      {
        list = new List<int> ();

        do
        {
          list.Add (int.Parse (match.Value));
          match = match.NextMatch ();
        } while (match.Success);
      }
    return list == null ? null : list.ToArray ();
    }

    private static string[]? MakeTeam (string entry)
    {
      var match = teamplayer.Match (entry);
      var list = (List<string>?) null;

      if (match.Success)
      {
        list = new List<string> ();

        do
        {
          list.Add (match.Value);
          match = match.NextMatch ();
        } while (match.Success);
      }
    return list == null ? null : list.ToArray ();
    }

#endregion

#region Callbacks

    private static bool IsBreak (string line)
    {
      return line == "break";
    }

    private void OnOutput (object? o, DataReceivedEventArgs a)
    {
      if (Interlocked.Read (ref QuitFlag) > 0)
        return;

      var line = a.Data;
      if (line != null)
      {
        var match = actions.Match (line);
        if (match.Success)
        {
          var action = match.Value;

          DataReceivedEventHandler? handler = null;
          var pendings_ = pendings;
          var proc_ = proc;

          switch (action)
          {

            case "Intercambio":
              match = exchange.Match (line);
              if (!match.Success)
                {
                  var message = $"Malformed action {action}: {line}";
                  throw new EngineException (message);
                }
              else
                {
                  var player = match.Groups [1].Value;
                  var losses_ = match.Groups [2].Value;
                  var takes_ = match.Groups [3].Value;

                  var losses = int.Parse (losses_);
                  if (losses > 0)
                    {
                      var message = $"Malformed action {action}: {line}";
                      throw new EngineException (message);
                    }

                  var takes = int.Parse (takes_);
                  if (takes < 0)
                    {
                      var message = $"Malformed action {action}: {line}";
                      throw new EngineException (message);
                    }

                  pendings.Enqueue (new ExchangeArgs (player, losses, takes));
                }
              break;

            case "Jugada":
              match = move.Match (line);
              if (!match.Success)
                {
                  match = pass.Match (line);
                  if (!match.Success)
                  {
                    var message = $"Malformed action {action}: {line}";
                    throw new EngineException (message);
                  }
                  else
                  {
                    var player = match.Groups [1].Value;

                    pendings.Enqueue (new MoveArgs (player));
                  }
                }
              else
                {
                  var player = match.Groups [1].Value;
                  var piece_ = match.Groups [2].Value;
                  var piece_head_ = match.Groups [3].Value;
                  var table_head_ = match.Groups [4].Value;

                  var piece = MakePiece (piece_);
                  if (piece == null)
                    {
                      var message = $"Malformed action {action}: {line}";
                      throw new EngineException (message);
                    }

                  var piece_head = int.Parse (piece_head_);
                  var table_head = int.Parse (table_head_);

                  pendings.Enqueue (new MoveArgs (player, piece_head, table_head, piece));
                }
              break;

            case "Mano":
              match = hand.Match (line);
              if (!match.Success)
                {
                  var message = $"Malformed action {action}: {line}";
                  throw new EngineException (message);
                }
              else
                {
                  var player = match.Groups [1].Value;
                  var pieces = new List<int[]> ();
                  handler = (o, a) => {
                    var line = a.Data;
                    if (line != null)
                    {
                      if (IsBreak (line))
                      {
                        var array = pieces.ToArray ();
                        var emit = new EmitHandArgs(player, array);
                        pendings_.Enqueue (emit);
                        proc_.OutputDataReceived -= handler;
                      }
                      else
                      {
                        var piece = MakePiece (line);
                        if (piece == null)
                          throw new EngineException ("Malformed piece entry");
                        else
                          pieces.Add (piece);
                      }
                    }
                  };

                  proc.OutputDataReceived += handler;
                }
              break;

            case "Equipos":
              var teams = new List<(string, string[])> ();
              handler = (o, a) => {
                var line = a.Data;
                if (line != null)
                {
                  var match = actions.Match (line);
                  if (match.Success)
                  {
                    var name = match.Value;
                    if (name != "break")
                    {
                      match = team.Match (line);
                      if (!match.Success)
                        throw new EngineException ("Malformed team entry");
                      else
                      {
                        var name_ = match.Groups [1].Value;
                        var entry = match.Groups [2].Value;
                        if (name != name_)
                          throw new Exception ("Malformed team entry");
                        else
                        {
                          var players = MakeTeam (entry);
                          if (players == null)
                            throw new Exception ("Malformed team entry");
                          else
                          {
                            teams.Add ((name, players));
                          }
                        }
                      }
                    }
                    else
                    {
                      var array = teams.ToArray ();
                      pendings_.Enqueue (new EmitTeamArgs (array));
                      proc_.OutputDataReceived -= handler;
                    }
                  }
                }
              };

              proc.OutputDataReceived += handler;
              break;

            case "GameOver":
              var scores = new List<(string,int)> ();
              handler = (o, a) => {
                var line = a.Data;
                if (line != null)
                {
                  var match = actions.Match (line);
                  if (match.Success)
                  {
                    var contender = match.Value;
                    if (contender != "Introduzca"
                      && contender != "break")
                    {
                      match = score.Match (line);
                      if (!match.Success)
                        throw new EngineException ("Malformed score entry");
                      else
                      {
                        var name = match.Groups[1].Value;
                        var value = match.Groups [2].Value;
                        if (name != contender)
                          throw new Exception ("Malformed score entry");
                        else
                        {
                          var score = int.Parse (value);
                          scores.Add ((contender, score));
                        }
                      }
                    }
                    else
                    {
                      var array = scores.ToArray ();
                      pendings_.Enqueue (new GameOverArgs (array));
                      proc_.OutputDataReceived -= handler;
                    }
                  }
                }
              };

              proc.OutputDataReceived += handler;
              break;
          }
        }
      }
    }

    private void OnError (object? o, DataReceivedEventArgs a)
    {
      if (HasExited)
        {
          Console.Error.WriteLine (proc.MainModule);
          Console.Error.WriteLine (proc.ExitCode);
          throw new EngineException ("Game engine crashed");
        }
      else
        {
          Console.Error.WriteLine (a.Data);
          throw new EngineException ("Game engine give us error output");
        }
    }

    private void OnExited (object? o, EventArgs a)
    {
      HasExited = true;
    }

#endregion

#region Constructors

    private Backend (string binary)
    {
      ProcessStartInfo info;
      pendings = new ConcurrentQueue<ActionArgs> ();

      info = new ProcessStartInfo ();
      info.FileName = binary;
      info.CreateNoWindow = true;
      info.RedirectStandardInput = true;
      info.RedirectStandardOutput = true;
      info.RedirectStandardError = true;

      proc = new Process ();
      proc.StartInfo = info;
      proc.EnableRaisingEvents = true;
      proc.OutputDataReceived += OnOutput;
      proc.ErrorDataReceived += OnError;
      proc.Exited += OnExited;

      Action += (o, a) => { };
    }

    public Backend (string ruleset, string binary) : this (binary)
    {
      proc.Start ();
      proc.BeginOutputReadLine ();
      proc.BeginErrorReadLine ();
      proc.StandardInput.WriteLine (ruleset);
    }

    ~Backend ()
    {
      Console.WriteLine ("termination");
      Interlocked.Increment (ref QuitFlag);
      proc.CancelErrorRead ();
      proc.CancelOutputRead ();
      proc.Kill ();
      proc.WaitForExit ();
    }

    private readonly static Regex actions;
    private readonly static Regex exchange;
    private readonly static Regex pass;
    private readonly static Regex move;
    private readonly static Regex piece;
    private readonly static Regex hand;
    private readonly static Regex team;
    private readonly static Regex teamplayer;
    private readonly static Regex score;

    static Backend ()
    {
      var flags = RegexOptions.Compiled | RegexOptions.Singleline;

      actions = new Regex ("^([\\w]+)", flags);
      exchange = new Regex ("^Intercambio ([\\w]+) ([0-9\\-]+) ([0-9\\+]+)", flags);
      pass = new Regex ("^Jugada ([\\w]+) pase", flags);
      move = new Regex ("^Jugada ([\\w]+) \\(([^\\)]+)\\) ([0-9\\-]+) ([0-9\\-]+) ([0-9\\-]+)", flags);
      piece = new Regex ("([0-9]+)", flags);
      hand = new Regex ("^Mano ([\\w]+)", flags);
      team = new Regex ("^([\\w]+)\\: (.+)", flags);
      teamplayer = new Regex ("([\\w]+)", flags);
      score = new Regex ("^([\\w]+) ([0-9\\-]+)", flags);
    }

#endregion
  }
}
