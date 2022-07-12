/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Mathematics;

namespace frontend.Game
{
  [GLib.TypeName ("DominoGameWindow")]
  [Gtk.Template (ResourceName = "gamewindow.ui")]
  public sealed class Window : Gtk.Window
  {
    [Gtk.Builder.Object]
    private Gtk.GLArea? glarea1;
    [Gtk.Builder.Object]
    private Gtk.ToggleButton? keep1;
    [Gtk.Builder.Object]
    private Gtk.Button? forward1;

    private static readonly float fov = 45;
    private static readonly uint putinterval = 400;
    private List<(int, int[])> boarded;
    private Objects.PieceBoard? board;
    private Engine.Gl? renderer;
    private uint clock;

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
      const int min = 50;
      var renderer_ = renderer;
      var viewport = renderer!.Viewport;
      var glarea1_ = glarea1;

      var leaps = interval / min;
      var leap = distance / (float) leaps;

      GLib.Timeout.Add (min,
        () => {
          viewport.Position += leap;
          var x = viewport.Position.X;

          viewport.LookAt (x, 0, 0);
          renderer!.ShouldUpdate = true;
          glarea1_!.QueueRender ();
          return leaps-- > 0;
      });
    }

    private void OnKeyPressed (object? o, Gtk.KeyPressEventArgs a)
    {
      var press = a.Event;
      var mods = press.State;
      Vector3 pos;

      if (!mods.HasFlag (Gdk.ModifierType.ReleaseMask))
      {
        switch (a.Event.Key)
        {
          case Gdk.Key.A:
          case Gdk.Key.a:
            SmoothMove (new Vector3 ( 1, 0, 0), 200);
            break;
          case Gdk.Key.D:
          case Gdk.Key.d:
            SmoothMove (new Vector3 (-1, 0, 0), 200);
            break;
/*
          case Gdk.Key.W:
          case Gdk.Key.w:
            pos = renderer!.Viewport.Position;
            pos.Y += 1;
            renderer!.Viewport.Position = pos;
            renderer!.ShouldUpdate = true;
            glarea1!.QueueRender ();
            break;
          case Gdk.Key.S:
          case Gdk.Key.s:
            pos = renderer!.Viewport.Position;
            pos.Y -= 1;
            renderer!.Viewport.Position = pos;
            renderer!.ShouldUpdate = true;
            glarea1!.QueueRender ();
            break;
*/
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
        }
      }
    }

    private void OnRealize (object? o, EventArgs a)
    {
      glarea1!.MakeCurrent ();
      if (glarea1!.Error != IntPtr.Zero)
        throw new Exception ("GL");

      Engine.Skybox skybox;
      renderer = new Engine.Gl ();

      board = new Objects.PieceBoard (2);
      board.Visible = true;
      renderer.Objects.Add (board);

      skybox = new Engine.Skybox ();
      skybox.Direction = Vector3.UnitY;
      skybox.Angle = MathHelper.DegreesToRadians (180);
      skybox.Position = Vector3.Zero;
      skybox.Scale = Vector3.One * 60;
      skybox.Visible = true;
      renderer.Objects.Add (skybox);

      foreach (var tuple in boarded)
        {
          board.Append (tuple.Item1, tuple.Item2);
        }

      var boarded_ = boarded;
      var glarea1_ = glarea1;
      var engine_ = engine;
      var board_ = board;

      actionHandler += (o, arg) =>
        {
          if (arg is Backend.MoveArgs)
          {
            var a = (Backend.MoveArgs) arg;
            var putat = a.PutAt;
            var piece = a.Piece;

            if (piece != null)
            {
              GLib.Idle.Add (() =>
              {
                boarded_.Add ((putat, piece));
                if (board_ != null)
                  {
                    board_.Append (putat, piece);
                    glarea1_!.QueueRender ();
                  }
                return false;
              });
            }
          } else
          {
            engine_.PollNext ();
          }
        };

      engine.Action += actionHandler;
    }

    private void OnUnrealize (object? o, EventArgs a)
    {
      engine.Action -= actionHandler;
      renderer = null;
    }

#endregion

#region Constructors

    public Window (Backend engine) : base (null)
    {
      Gtk.TemplateBuilder.InitTemplate (this);

      this.boarded = new List<(int, int[])> ();
      this.engine = engine;

      var renderer_ = renderer;
      var engine_ = engine;
      var keep1_ = keep1;

      forward1!.Clicked += (o, a) =>
        {
          if (renderer_ != null)
            engine_.PollNext();
        };

      clock = GLib.Timeout.Add (
        putinterval, () =>
        {
          if (keep1_!.Active == true)
            engine_.PollNext ();
          return true;
        });
    }

    ~ Window ()
    {
      GLib.Source.Remove (clock);
    }

#endregion
  }
}
