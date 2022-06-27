/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Numerics;
using Raylib_CsLo;

namespace frontend
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Runtime.InteropServices;

  public unsafe class NameSection : CreateRules.Section
  {
    public override string Title { get => "Nombre"; }
    private const int bufferSize = 1024;
    private sbyte* buffer = null;

    public string Transform (string basedir)
    {
      var name = Marshal.PtrToStringAuto ((IntPtr) buffer)!;
    return Path.Combine (basedir, name);
    }

    public override void Draw (ref Rectangle rect)
    { 
      var rec = Menu.GetNextControl (ref rect, rect.width, 30);
      RayGui.GuiTextBox (rec, buffer, bufferSize, true);
    }

    public override void Write (string basedir) { }

    public override bool IsValid ()
    {
      var name = Marshal.PtrToStringAuto ((IntPtr) buffer)!;
      if (name == "") return false;
    return true;
    }

    public NameSection() : base ()
    {
      buffer = (sbyte*) Marshal.AllocHGlobal (bufferSize);
      (new Span<sbyte>(buffer, bufferSize)).Fill (0);
    }

    public NameSection (string savedir) : this ()
    {
      var span = (new Span<sbyte>(buffer, bufferSize));
      var basename = Path.GetFileName (savedir);
      Console.WriteLine (basename);
    }

    ~NameSection ()
    {
      Marshal.FreeHGlobal ((IntPtr) buffer);
    }
  }
}
