/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */
using OpenTK.Mathematics;

namespace Frontend.Game
{
  [GLib.TypeName ("DominoGameWindow")]
  [Gtk.Template (ResourceName = "gamewindow.ui")]
  public sealed class Window : Gtk.Window
  {
    [Gtk.Builder.Object]
    private Gtk.HeaderBar? headerbar1 = null;
    [Gtk.Builder.Object]
    private Gtk.GLArea? glarea1 = null;
    [Gtk.Builder.Object]
    private Gtk.ToggleButton? keep1 = null;
    [Gtk.Builder.Object]
    private Gtk.Button? forward1 = null;

    private static readonly float fov = 45;
    private static readonly uint putinterval = 400;
    private List<(int, int, int[])> boarded;
    private Objects.PieceBoard? board = null;
    private Engine.Gl? renderer = null;
    private uint clock;

    private System.EventHandler? clickedHandler;
    private Game.Backend.ActionHandler? actionHandler;
    private Game.Backend engine;

#region Callbacks

    private void OnCreateContext (object? o, Gtk.CreateContextArgs a)
    {
      var
      context = Window.CreateGlContext ();
      context.DebugEnabled = true;
      context.SetRequiredVersion (3, 3);
      context.SetUseEs (glarea1!.UseEs ? 1 : 0);
      a.RetVal = context;
    }

    private void OnRender (object? o, Gtk.RenderArgs a)
    {
      glarea1!.MakeCurrent ();
      if (glarea1!.Error != IntPtr.Zero)
        {
          a.RetVal = false;
          return;
        }

      renderer!.Render ();
      a.RetVal = true;
    }

    private void OnResize (object? o, Gtk.ResizeArgs a)
    {
      glarea1!.MakeCurrent ();
      if (glarea1!.Error != IntPtr.Zero)
        throw new Exception ("GL");

      var fovy = MathHelper.DegreesToRadians (fov);
      renderer!.Viewport.Project (a.Width, a.Height, fovy);
      renderer!.ShouldUpdate = true;
      this.QueueDraw ();
    }

    private void SmoothMove (Vector3 distance, uint interval)
    {
      const int min = 20;
      var viewport = renderer!.Viewport;
      var renderer_ = renderer!;
      var glarea1_ = glarea1!;

      var leaps = interval / min;
      var leap = distance / (float) leaps;

      GLib.Timeout.Add (min,
        () => {
          viewport.Position += leap;
          var x = viewport.Position.X;

          viewport.LookAt (x, 0, 0);
          renderer_.ShouldUpdate = true;
          glarea1_.QueueRender ();
        return --leaps > 0;
      });
    }

    private void OnKeyPressed (object? o, Gtk.KeyPressEventArgs a)
    {
      var press = a.Event;
      var mods = press.State;

      if (!mods.HasFlag (Gdk.ModifierType.ReleaseMask))
      {
        switch (a.Event.Key)
        {
          case Gdk.Key.A:
          case Gdk.Key.a:
            SmoothMove (new Vector3 (-1, 0, 0), 200);
            break;
          case Gdk.Key.D:
          case Gdk.Key.d:
            SmoothMove (new Vector3 ( 1, 0, 0), 200);
            break;

          case Gdk.Key.Key_1:
            if (board != null)
            {
              var head = board!.Head1;
              if (head != null)
              {
                var viewport = renderer!.Viewport;
                var distance = Vector3.Zero;
                distance.X = head.Position.X - viewport.Position.X;
                SmoothMove (distance, 300);
              }
            }
            break;
          case Gdk.Key.Key_2:
            if (board != null)
            {
              var head = board!.Head2;
              if (head != null)
              {
                var viewport = renderer!.Viewport;
                var distance = Vector3.Zero;
                distance.X = head.Position.X - viewport.Position.X;
                SmoothMove (distance, 300);
              }
            }
            break;
        }
      }
    }

    private void OnRealize (object? o, EventArgs a)
    {
      glarea1!.MakeCurrent ();
      if (glarea1!.Error != IntPtr.Zero)
        throw new Exception ("GL");

      renderer = new Engine.Gl ();
      renderer.Viewport.Position = new Vector3 (0, 13, 13);

      board = new Objects.PieceBoard (2);
      board.Visible = true;
      renderer.Objects.Add (board);

      foreach (var tuple in boarded)
        {
          board.Append (tuple.Item1, tuple.Item2, tuple.Item3);
        }

      var headerbar = headerbar1;
      var boarded_ = boarded;
      var glarea1_ = glarea1;
      var engine_ = engine;
      var board_ = board;
      var teamed = false;

      actionHandler += (o, arg) =>
        {
          if (arg is Backend.MoveArgs)
          {
            var a = (Backend.MoveArgs) arg;
            var atby = a.AtBy;
            var putat = a.PutAt;
            var piece = a.Piece;

            if (piece != null)
            {
              GLib.Idle.Add (() =>
              {
                boarded_.Add ((atby, putat, piece));
                if (board_ != null)
                  {
                    board_.Append (atby, putat, piece);
                    glarea1_!.QueueRender ();
                  }
                return false;
              });
            }
          } else
          if (arg is Backend.EmitTeamArgs)
          {
            var a = (Backend.EmitTeamArgs) arg;
            teamed = true;
          } else
          if (arg is Backend.GameOverArgs)
          {
            var a = (Backend.GameOverArgs) arg;
            (string Name, int Score)[] scores = a.Scores;
            var best = new List<(string Name, int Score)> ();
                best.Add (scores.First ());

            foreach (var score in scores)
            if (score.Score == best.First ().Score)
              best.Add (score);
            else
            {
              best.Clear ();
              best.Add (score);
            }

            var first = best.First ();
            var kind = teamed ? "equipo" : "jugador";
            var kinds = teamed ? "equipos" : "jugadores";
            var match = best.Count;

            GLib.Idle.Add (() =>
            {
              if (match > 1)
              {
                headerbar!.Title = "El juego terminó!";
                headerbar!.Subtitle = $"Empataron {match} {kinds}";
              }
              else
              {
                headerbar!.Title = "El juego terminó!";
                if (teamed)
                  headerbar!.Subtitle = $"Ganó el equipo \"{first.Name}\" con {first.Score} puntos";
                else
                  headerbar!.Subtitle = $"Ganó \"{first.Name}\" con {first.Score} puntos";
              }
            return false;
            });
          } else
          {
            engine_.PollNext ();
          }
        };

      clickedHandler = (o, a) =>
        {
          engine_.PollNext();
        };

      forward1!.Clicked += clickedHandler;
      engine.Action += actionHandler;
      var keep1_ = keep1;

      clock = GLib.Timeout.Add (
        putinterval, () =>
        {
          if (keep1_!.Active == true)
            engine_.PollNext ();
          return true;
        });
    }

    private void OnUnrealize (object? o, EventArgs a)
    {
      GLib.Source.Remove (clock);
      forward1!.Clicked -= clickedHandler;
      engine.Action -= actionHandler;

      board = null;
      renderer = null;
    }

#endregion

#region Constructors

    public Window (Backend engine) : base (null)
    {
      Gtk.TemplateBuilder.InitTemplate (this);
      this.boarded = new List<(int, int, int[])> ();
      this.engine = engine;
    }

#endregion
  }
}
