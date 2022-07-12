/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */

namespace frontend
{
  public interface IListBoxRow<T>
  {
    public T Value { get; set; }
    public event Action Changed;
  }
}
