/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/rule.
 *
 */

namespace Rule
{
  public interface IFileBased
  {
    public abstract void Save (GLib.IFile savedir, GLib.Cancellable? cancellable = null);
    public abstract void Load (GLib.IFile savedir, GLib.Cancellable? cancellable = null);
  }
}
