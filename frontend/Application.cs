/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using Gtk;
using System.Reflection;

namespace frontend
{
  public sealed class Application : Gtk.Application
  {
#region Constants
    public static string ApplicationName = "Domino";
    public static string ApplicationVersion = "1.0.0.0";
    public static string ApplicationWebsite = "https://github.com/MarcosHCK/domino/";
    private static Gdk.Pixbuf? ApplicationIcon = null;
    public static string LibexecDir = "backend/bin/Debug/net6.0/";
    public static string DataDir = "data/";
    public static string BaseDir = "rules/";
#endregion

#region  GLib.Application

    protected override void OnActivated ()
    {
      var rule = new Rule ("Usual");
      var ruleName = Path.Combine (BaseDir, rule.Name!);
      var saveDir = GLib.FileFactory.NewForPath (ruleName);
          rule.Load (saveDir);

      var execBin = Path.Combine (LibexecDir, "backend");
      var engine = new Game.Engine (rule, execBin);

      var
      window = new Game.Window (engine);
      window.Icon = Application.ApplicationIcon;
      window.Application = this;
      window.Present ();
    }

#endregion

#region Constructors

    public Application () : base (null, GLib.ApplicationFlags.None) { }
    public Application (string application_id, GLib.ApplicationFlags flags) : base (application_id, flags) { }

#endregion

    [STAThread]
    public static int Main (string[] argv)
    {
      var app = new Application ("org.hck.Domino", GLib.ApplicationFlags.None);
    return app.Run (ApplicationName, argv);
    }
  }
}
