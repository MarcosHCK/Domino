/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using System.Runtime.InteropServices;

namespace Patch
{
  public abstract class Application : Gtk.Application
  {
    static OpenNativeDelegate Open_cb_delegate;
		static OpenNativeDelegate OpenVMCallback
    {
			get
      {
				if (Open_cb_delegate == null)
					Open_cb_delegate = new OpenNativeDelegate (Open_cb);
				return Open_cb_delegate;
			}
		}

    static void OverrideOpen (GLib.GType gtype)
		{
			OverrideOpen (gtype, OpenVMCallback);
		}

		static void OverrideOpen (GLib.GType gtype, OpenNativeDelegate callback)
		{
			unsafe
      {
				var class_abi = GLib.Application.class_abi;
				IntPtr* raw_ptr = (IntPtr*) (((long) gtype.GetClassPtr ()) + (long) class_abi.GetFieldOffset ("open"));
				*raw_ptr = Marshal.GetFunctionPointerForDelegate (callback);
			}
		}

		[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
		delegate void OpenNativeDelegate (IntPtr inst, IntPtr files, int n_files, IntPtr hint);

		static void Open_cb (IntPtr inst, IntPtr files, int n_files, IntPtr hint)
		{
			try
      {
				var __obj = (Application) GLib.Object.GetObject (inst, false);
        var __files = new GLib.IFile [n_files];
        var __hint = GLib.Marshaller.Utf8PtrToString (hint);

        unsafe
        {
          var span = new Span<IntPtr> ((void*) files, n_files);
          for (int i = 0; i < n_files; i++)
          {
						var __file = GLib.FileAdapter.GetObject (span [i], false);
            __files [i] = (GLib.IFile) __file;
          }
        }

				__obj.OnOpen (__files, __hint);
			} catch (Exception e)
      {
				GLib.ExceptionManager.RaiseUnhandledException (e, false);
			}
		}

		[GLib.DefaultSignalHandler (Type = typeof(Application), ConnectionMethod = "OverrideOpen")]
		protected virtual void OnOpen (GLib.IFile[] files, string hint)
		{
			throw new NotImplementedException ();
		}

    public Application ()
      : base (null, GLib.ApplicationFlags.None) { }
    public Application (string? application_id, GLib.ApplicationFlags flags)
      : base (application_id, flags) { }
  }
}
