/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Runtime.InteropServices;
namespace Engine;

public partial class Gl
{
  internal sealed class Loader : OpenTK.IBindingsContext
  {
    private static Func loader;
    private const int RTLD_LAZY = 0x0001;
    private const int RTLD_GLOBAL = 0x0100;
    private static bool IsWindows, IsOSX;

#region Types

    private delegate IntPtr Func (string name);

    private class Windows
    {
        [DllImport ("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress (IntPtr hModule, string procName);
        [DllImport ("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibraryW (string lpszLib);
    }

    private class Linux
    {
        [DllImport ("libdl.so")]
        public static extern IntPtr dlopen (string path, int flags);
        [DllImport ("libdl.so")]
        public static extern IntPtr dlsym (IntPtr handle, string symbol);
    }

    private class OSX
    {
        [DllImport ("/usr/lib/libSystem.dylib")]
        public static extern IntPtr dlopen (string path, int flags);
        [DllImport ("/usr/lib/libSystem.dylib")]
        public static extern IntPtr dlsym (IntPtr handle, string symbol);
    }

    [DllImport ("libc")]
    private static extern int uname (IntPtr buf);

#endregion

#region private API

    public static IntPtr LoadLibrary (string libname)
    {
      if (IsWindows)
        return Windows.LoadLibraryW (libname);
      if (IsOSX)
        return OSX.dlopen (libname, RTLD_GLOBAL | RTLD_LAZY);
      return Linux.dlopen (libname, RTLD_GLOBAL | RTLD_LAZY);
    }

    public static IntPtr GetProcAddress (IntPtr library, string function)
    {
      if (IsWindows)
        return Windows.GetProcAddress (library, function);
      if (IsOSX)
        return OSX.dlsym (library, function);
      return Linux.dlsym (library, function);
    }

    public static T LoadFunction<T> (IntPtr procaddress)
    {
      if (procaddress == IntPtr.Zero)
        return default (T)!;
      return Marshal.GetDelegateForFunctionPointer<T> (procaddress);
    }

#endregion

#region Constructors

    static Loader ()
    {
      switch (Environment.OSVersion.Platform)
      {
      case PlatformID.Win32NT:
      case PlatformID.Win32S:
      case PlatformID.Win32Windows:
      case PlatformID.WinCE:
        IsWindows = true;
        break;
      case PlatformID.MacOSX:
        IsOSX = true;
        break;
      case PlatformID.Unix:
        try {
          var buf = Marshal.AllocHGlobal (8192);
          if (uname (buf) == 0
            && Marshal.PtrToStringAnsi (buf) == "Darwin")
              IsOSX = true;
          Marshal.FreeHGlobal (buf);
        } catch { }
        break;
      }

      if (IsWindows)
        {
          var lib = LoadLibrary ("opengl32.dll");
          if (lib == IntPtr.Zero)
            throw new Exception ("can't find OpenGL library");
          else
            {
              loader = (name) => GetProcAddress (lib, name);
            }
        }
      else if (IsOSX)
        {
          throw new NotImplementedException ();
        }
      else
        {
          var lib = LoadLibrary ("libGL.so");
          if (lib == IntPtr.Zero)
            throw new Exception ("can't find OpenGL library");
          else
            {
              IntPtr addr;
              lib = IntPtr.Zero;

              addr = GetProcAddress (lib, "glXGetProcAddress");
              if (addr != IntPtr.Zero)
                loader = LoadFunction<Func> (addr);
              else
                {
                  addr = GetProcAddress (lib, "glXGetProcAddressARB");
                  if (addr != IntPtr.Zero)
                    loader = LoadFunction<Func> (addr);
                  else
                    throw new Exception ("can't find a loader");
                }
            }
        }
    }

#endregion

    public IntPtr GetProcAddress (string name) => loader (name);
  }
}
