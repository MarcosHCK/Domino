/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Numerics;
using Raylib_CsLo;

namespace frontend
{
  public class NumerosSection : CreateRules.Section
  {
    public override string Title { get => "Numeros"; }
    public override void Draw (ref Rectangle rect)
    {
      var font = RayGui.GuiGetFont ();
      var fontProp = (int) Raylib_CsLo.GuiDefaultProperty.TEXT_SIZE;
      var default_ = (int) Raylib_CsLo.GuiControl.DEFAULT;
      var fontSize = RayGui.GuiGetStyle (default_, fontProp);
  
      {
        var lineRec = Menu.GetNextControl (ref rect, rect.width, 30);
        var lineStartX = (int) lineRec.x;
        var lineStartY = (int) lineRec.x;
        var lineStopX = (int) rect.width + (int) lineRec.x;
        var lineStopY = (int) lineRec.x;

        RayGui.GuiSetStyle (default_, fontProp, fontSize * 2);
        RayGui.GuiLine (lineRec, Title);
        RayGui.GuiSetStyle (default_, fontProp, fontSize);
      }
    }

    public override void Write (string basedir)
    {
      //throw new NotImplementedException ();
    }

    public override bool IsValid()
    {
      //throw new NotImplementedException ();
      return true;
    }

    public NumerosSection () : base () { }
    public NumerosSection (string savedir) : this () { }
  }
}
