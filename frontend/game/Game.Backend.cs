/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace frontend.Game
{
  public class Backend
  {
    private Process proc;
    private ConcurrentQueue<ActionArgs> pendings;
    private long QuitFlag = 0;
    private long NextFlag = 0;
    public bool HasExited { get; private set; }

#region Commands API

    public event ActionHandler Action;

    public delegate void ActionHandler (Backend engine, ActionArgs a);
    public class ActionArgs : EventArgs
    {
      public string Player { get; private set; }

      public ActionArgs (string Player)
      {
        this.Player = Player;
      }
    }

    public delegate void ExchangeHandler (Backend engine, ExchangeArgs a);
    public class ExchangeArgs : ActionArgs
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

    public delegate void MoveHandler (Backend engine, MoveArgs a);
    public class MoveArgs : ActionArgs
    {
      public bool Passes { get; private set; }
      public int PutAt { get; private set; }
      public int[]? Piece { get; private set; }

      public MoveArgs (string Player)
        : base (Player)
      {
        this.Passes = true;
      }

      public MoveArgs (string Player, int PutAt, params int[] piece)
        : base (Player)
      {
        this.Passes = false;
        this.PutAt = PutAt;
        this.Piece = piece;
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

#region Callbacks

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

    private int[]? MakePiece (string piece_)
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
          switch (action)
          {
            case "Intercambio":
              match = exchange.Match (line);
              if (!match.Success)
                {
                  var message = $"Malformed {action} action: {line}";
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
                      var message = $"Malformed {action} action: {line}";
                      throw new EngineException (message);
                    }

                  var takes = int.Parse (takes_);
                  if (takes < 0)
                    {
                      var message = $"Malformed {action} action: {line}";
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
                    var message = $"Malformed {action} action: {line}";
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
                      var message = $"Malformed {action} action: {line}";
                      throw new EngineException (message);
                    }

                  var piece_head = int.Parse (piece_head_);
                  var table_head = int.Parse (table_head_);
                  if (piece_head != table_head)
                    {
                      var message = $"Malformed {action} action: {line}";
                      throw new EngineException (message);
                    }

                  pendings.Enqueue (new MoveArgs (player, piece_head, piece));
                }
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

    public Backend (Rule rule, string binary) : this (binary)
    {
      if (rule.Name == null)
        throw new NullReferenceException ();
      else
        {
          proc.Start ();
          proc.BeginOutputReadLine ();
          proc.BeginErrorReadLine ();
          proc.StandardInput.WriteLine (rule.Name);
        }
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

    private static Regex actions;
    private static Regex exchange;
    private static Regex pass;
    private static Regex move;
    private static Regex piece;

    static Backend ()
    {
      var flags = RegexOptions.Compiled | RegexOptions.Singleline;

      actions = new Regex ("^([\\w]+)", flags);
      exchange = new Regex ("^Intercambio ([\\w]+) ([0-9\\-]+) ([0-9\\+]+)", flags);
      pass = new Regex ("^Jugada ([\\w]+) pase", flags);
      move = new Regex ("^Jugada ([\\w]+) \\(([^\\)]+)\\) ([0-9\\-]+) ([0-9\\-]+) ([0-9\\-]+)", flags);
      piece = new Regex ("([0-9]+)", flags);
    }

#endregion
  }
}
