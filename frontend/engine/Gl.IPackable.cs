/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/Frontend.
 *
 */
using OpenTK.Mathematics;
namespace Frontend.Engine;

public partial class Gl
{
  public interface IPackable
  {
    public abstract void Pack (Stream stream);

    public static void Pack (Stream stream, float value)
    {
      unsafe
      {
        var ar = stackalloc float [] { value };
        var span = new ReadOnlySpan<byte> (ar, 1 * sizeof (float));
        stream.Write (span);
      }
    }

    public static void Pack (Stream stream, Vector2 vec2)
    {
      unsafe
      {
        var ar = stackalloc float [] { vec2.X, vec2.Y };
        var span = new ReadOnlySpan<byte> (ar, 2 * sizeof (float));
        stream.Write (span);
      }
    }

    public static void Pack (Stream stream, Vector3 vec3)
    {
      unsafe
      {
        var ar = stackalloc float [] { vec3.X, vec3.Y, vec3.Z, 0 };
        var span = new ReadOnlySpan<byte> (ar, 4 * sizeof (float));
        stream.Write (span);
      }
    }

    public static void Pack (Stream stream, Vector4 vec4)
    {
      unsafe
      {
        var ar = stackalloc float [] { vec4.X, vec4.Y, vec4.Z, vec4.W };
        var span = new ReadOnlySpan<byte> (ar, 4 * sizeof (float));
        stream.Write (span);
      }
    }
  }
}
