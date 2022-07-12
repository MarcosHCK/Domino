﻿/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using Gtk;

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
      var
      window = new Window ();
      window.Icon = Application.ApplicationIcon;
      window.Application = this;
      window.Present ();
    }

#endregion

#region Constructors

    public Application () : base (null, GLib.ApplicationFlags.None) { }
    public Application (string application_id, GLib.ApplicationFlags flags) : base (application_id, flags) { }

    static Application ()
    {
      var uidir = Path.Combine (DataDir, "ui/");
      TemplateBuilder.BaseDir = uidir;
    }

#endregion

    [STAThread]
    public static int Main (string[] argv)
    {
      var app = new Application ("org.hck.Domino", GLib.ApplicationFlags.None);
    return app.Run (ApplicationName, argv);
    }
  }
}
