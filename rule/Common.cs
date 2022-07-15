/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/rule.
 *
 */

namespace Rule
{
  internal sealed class Common
  {
    internal static bool IsBreak (string line)
    {
      return line.StartsWith ("break");
    }
  }
}
