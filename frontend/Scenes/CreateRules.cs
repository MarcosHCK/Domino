/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Numerics;
using Raylib_CsLo;

namespace frontend
{
  public class CreateRules : Menu
  {
    const string basedir = "rules/";
    private List<Section> sections;
    private Vector2 scrollState;

    public abstract class Section
    {
      public abstract string Title { get; }
      public abstract void Draw (ref Rectangle rect);
      public abstract void Write (string basedir);
      public abstract bool IsValid ();
    }

    public bool TrySave ()
    {
      bool valid = true;
      foreach (var section in sections)
      if (!section.IsValid ())
        {
          valid = false;
          break;
        }

      if (valid)
        {
          var name = (NameSection) sections [0];
          var savedir = name.Transform (basedir);

          if (!Directory.Exists (savedir))
            Directory.CreateDirectory (savedir);

          foreach (var section in sections)
            section.Write (savedir);
          return true;
        }
    return false;
    }

    public unsafe override void Draw (Facade facade)
    {
      base.Draw (facade);

      Rectangle rec;
      rec.x = rec.y = 0;
      rec.width = (float) Raylib.GetScreenWidth ();
      rec.height = (float) Raylib.GetScreenHeight ();

      var content = new Rectangle (0, 0, rec.width, rec.height );
      var scroll = new Vector2 ( scrollState.X, scrollState.Y );
      var view = RayGui.GuiScrollPanel (rec, content, &scroll);
      var rect = new Rectangle (rec.x + scroll.X, rec.y + scroll.Y, content.width, content.height);

      Raylib.BeginScissorMode ((int) view.x, (int) view.y, (int) view.width, (int) view.height);
        scrollState.X = scroll.X;
        scrollState.Y = scroll.Y;

        rect.x += (float) padding;
        rect.y += (float) padding;
        rect.width -= (float) padding * 2;
        rect.height -= (float) padding * 2;

        foreach (var section in sections)
          section.Draw (ref rect);
      Raylib.EndScissorMode ();

      rec.height = 30;
      rec.width = 50;
      rec.x = ((float) Raylib.GetScreenWidth ()) - padding - rec.width;
      rec.y = ((float) Raylib.GetScreenHeight ()) - padding - rec.height;
      if (RayGui.GuiButton (rec, "Back"))
        Running = false;

      rec.x -= margin + rec.width;
      if (RayGui.GuiButton (rec, "Save"))
        this.TrySave ();

      rec.x -= margin + rec.width;
      if (RayGui.GuiButton (rec, "Apply"))
      if (this.TrySave ())
        Running = false;
    }

    public CreateRules () : base ()
    {
      scrollState = new Vector2 (0, 0);
      sections = new List<Section> ();
      sections.Add (new NameSection ());
      sections.Add (new NumerosSection ());
    }

    public CreateRules (string savedir) : base ()
    {
      scrollState = new Vector2 (0, 0);
      sections = new List<Section> ();
      sections.Add (new NameSection (savedir));
      sections.Add (new NumerosSection (savedir));
    }
  }
}
