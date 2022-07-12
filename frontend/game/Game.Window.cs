/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Text;

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

    private List<(int, int[])> boarded;
    private static readonly float fov = 45;
    private Engine.Gl? renderer;

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

    private void OnDeleteEvent (object? o, Gtk.DeleteEventArgs a)
    {
      a.RetVal = false;
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
            pos = renderer!.Viewport.Position;
            pos.X -= 1;
            renderer!.Viewport.Position = pos;
            renderer!.Viewport.LookAt (pos.X, 0, 0);
            renderer!.ShouldUpdate = true;
            glarea1!.QueueRender ();
            break;
          case Gdk.Key.D:
          case Gdk.Key.d:
            pos = renderer!.Viewport.Position;
            pos.X += 1;
            renderer!.Viewport.Position = pos;
            renderer!.Viewport.LookAt (pos.X, 0, 0);
            renderer!.ShouldUpdate = true;
            glarea1!.QueueRender ();
            break;
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
        }
      }
    }

    private void OnRealize (object? o, EventArgs a)
    {
      glarea1!.MakeCurrent ();
      if (glarea1!.Error != IntPtr.Zero)
        throw new Exception ("GL");

      renderer = new Engine.Gl ();

      var
      board = new Objects.PieceBoard (2);
      board.Visible = true;
      renderer.Objects.Add (board);

      var
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

      actionHandler = (o, arg) =>
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
                boarded.Add ((putat, piece));
                board.Append (putat, piece);
                glarea1!.QueueRender ();
                return false;
              });
            }
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

      forward1!.Clicked += (o, a) =>
      {
        if (renderer != null)
        {
          engine.PollNext ();
        }
      };

      var clock = (uint) 0;
      keep1!.AddNotification ("active", (o, a) =>
        {
          var keep1 = (Gtk.ToggleButton) o!;
          if (keep1.Active && clock == 0)
          {
            Console.WriteLine ("activating");
            clock = GLib.Timeout.Add (400, () =>
            {
              engine.PollNext ();
              return true;
            });
          }
          else
          if (!keep1.Active && clock != 0)
          {
            Console.WriteLine ("deactivating");
            GLib.Timeout.Remove (clock);
            clock = 0;
          }
        });
    }

#endregion
  }
}
