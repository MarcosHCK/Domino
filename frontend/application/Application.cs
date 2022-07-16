/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */

namespace Frontend
{
  public sealed class Application : Patch.Application
  {
#region Constants

    public static string ApplicationName = "Domino";
    public static string ApplicationVersion = "1.0.0.0";
    public static string ApplicationWebsite = "https://github.com/MarcosHCK/domino/";
    private static Gdk.Pixbuf? ApplicationIcon = null;
    public static string LibexecDir = "backend/bin/Debug/net6.0";
    public static string BaseDir = "backend/Partidas/";
    public static string DataDir = "data/";

#endregion

#region  GLib.Application

    protected override void OnOpen (GLib.IFile[] files, string hint)
    {
      if (files.Length != 1)
        throw new Exception ();
      else
      {
        var rulename = files [0].Basename;
        var binary = Path.Combine (LibexecDir, "Backend");
        var rule = new Rule.Playtime (rulename);
        var engine = new Game.Backend (rule.Name!, binary);
        var window = new Game.Window (engine);

        window.Icon = ApplicationIcon;
        window.Application = this;
        window.Present ();
      }
    }

    protected override void OnActivated ()
    {
      var
      window = new Window ();
      window.Icon = Application.ApplicationIcon;
      window.Application = this;
      window.Present ();
    }

#endregion

#region Constructors

    public Application ()
      : base (null, GLib.ApplicationFlags.None) { }
    public Application (string? application_id, GLib.ApplicationFlags flags)
      : base (application_id, flags) { }

    static Application ()
    {
      var uidir = Path.Combine (DataDir, "ui/");
      Gtk.TemplateBuilder.BaseDir = uidir;
      Engine.Gl.DataDir = DataDir;
    }

#endregion

    [STAThread]
    public static int Main (string[] argv)
    {
      var flags = GLib.ApplicationFlags.HandlesOpen;
      var app = new Application (null, flags);
      var result = (int) 0;

      GLib.ExceptionManager.UnhandledException +=
      (o) => {
        var e = (Exception) o.ExceptionObject;
        var dialog = new Message (e);

        dialog.Run ();
        dialog.Destroy ();

        o.ExitApplication = true;
      };

      try
      {
        result = app.Run(ApplicationName, argv);
      }
      catch (System.Exception e)
      {
        var
        dialog = new Message (e);
        dialog.Run ();
        dialog.Destroy ();
        return -1;
      }
    return result;
    }
  }
}
