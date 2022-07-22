/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */
using OpenTK.Mathematics;
using System.Text;

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
    private Objects.AtrilObject? atril = null;
    private Objects.PieceBoard? board = null;
    private Game.Pieces? pieces = null;
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

          case Gdk.Key.Key_0:
            if (board != null)
            {
              var viewport = renderer!.Viewport;
              var distance = Vector3.Zero;
              distance.X = -viewport.Position.X;
              if (distance.X != 0)
              {
                SmoothMove (distance, 300);
              }
            }
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
      pieces = new Game.Pieces ();

      atril = new Objects.AtrilObject (pieces, 2);
      atril.Visible = true;
      renderer.Objects.Add (atril);

      board = new Objects.PieceBoard (pieces, 2);
      board.Visible = true;
      renderer.Objects.Add (board);

      var headerbar = headerbar1!;
      var glarea1_ = glarea1!;
      var engine_ = engine;
      var board_ = board;
      var atril_ = atril;
      var teamed = false;
      var started = false;

      engine.RollBack ();

      actionHandler = (o, arg) =>
        {
          if (arg is Backend.MoveArgs)
          {
            var a = (Backend.MoveArgs) arg;
            var atby = a.AtBy;
            var putat = a.PutAt;
            var piece = a.Piece;

            if (putat == -1)
            {
              if (started)
              {
                throw new Exception();
              }

              started = true;
            }

            if (piece != null)
            {
              var builder = new StringBuilder ();
              int heads = 0;

              builder.Append (a.Player);
              builder.Append (" jugó (");

              foreach (var head in piece)
              if (heads++ > 0)
                builder.Append (", " + head);
              else
                builder.Append (head);

              builder.Append (") usuando la cara ");
              builder.Append (atby);
              if (putat == board_.Head1Value)
                builder.Append (" por la izquierda");
              if (putat == board_.Head2Value)
                builder.Append (" por la derecha");

              var msg = builder.ToString ();

              GLib.Idle.Add (() =>
              {
                board_.Append (atby, putat, piece);
                headerbar.Subtitle = msg;
                glarea1_.QueueRender ();
                return false;
              });
            }
            else
            {
              var msg = $"{a.Player} se pasó";
              GLib.Idle.Add (() =>
              {
                headerbar.Subtitle = msg;
                return false;
              });
            }
          } else
          if (arg is Backend.ExchangeArgs)
          {
            var a = (Backend.ExchangeArgs) arg;
            var msg = $"{a.Player} descartó {a.Losses} y tomó {a.Takes}";
            GLib.Idle.Add (() =>
              {
                headerbar.Subtitle = msg;
                return false;
              });
          } else
          if (arg is Backend.EmitHandArgs)
          {
            var a = (Backend.EmitHandArgs) arg;
            GLib.Idle.Add (() =>
            {
              atril_.ShowPieces (a.Pieces);
              glarea1_.QueueRender ();
              return false;
            });
          } else
          if (arg is Backend.EmitTeamArgs)
          {
            var a = (Backend.EmitTeamArgs) arg;
            teamed = true;
          } else
          if (arg is Backend.GameOverArgs)
          {
            var a = (Backend.GameOverArgs) arg;
            var best = new List<(string Name, int Score)> ();
            var min = int.MaxValue;
            var scores = a.Scores;

            foreach (var score in scores)
            {
              if (min > score.Item2)
              {
                best.Clear ();
                min = score.Item2;
              }

              if (min == score.Item2)
                best.Add (score);
            }

            var first = best.First ();
            var kind = teamed ? "equipo" : "jugador";
            var kinds = teamed ? "equipos" : "jugadores";
            var match = best.Count;
            string how;

            if (first.Score > -1)
              how = $"con {first.Score} puntos";
            else
              how = "por pegada";

            GLib.Idle.Add (() =>
            {
              if (match > 1)
              {
                headerbar.Title = "El juego terminó!";
                headerbar.Subtitle = $"Empataron {match} {kinds}";
              }
              else
              {
                headerbar.Title = "El juego terminó!";
                if (teamed)
                  headerbar.Subtitle = $"Ganó el equipo \"{first.Name}\" {how}";
                else
                  headerbar.Subtitle = $"Ganó \"{first.Name}\" {how}";
              }

              glarea1_.QueueRender ();
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

      while (started != true)
        engine_.PollNext ();

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

      atril = null;
      board = null;
      renderer = null;
      pieces = null;
    }

#endregion

#region Constructors

    public Window (Backend engine) : base (null)
    {
      Gtk.TemplateBuilder.InitTemplate (this);
      this.engine = engine;
    }

#endregion
  }
}
