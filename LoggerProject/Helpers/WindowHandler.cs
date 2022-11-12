using Autodesk.Revit.UI;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Helpers
	{
	/// <summary>
	/// Helper class for retaining modeless dialog on top of the Revit.
	/// </summary>
	public static class WindowHandler
		{
		/// <summary>
		/// User32 calls used to set Revit focus
		/// </summary>
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SetForegroundWindow(IntPtr hWnd);

		/// <summary>
		/// Sets the window owner.
		/// </summary>
		/// <param name="application">The application.</param>
		/// <param name="window">The window.</param>
		public static void SetWindowOwner(UIApplication application, Window window)
			{
#if Revit2019 || Revit2020 || Revit2021 || Revit2022
      HwndSource hwndSource = HwndSource.FromHwnd(application.MainWindowHandle);
      Window currentWindow = hwndSource.RootVisual as Window;
      window.Owner = currentWindow;
#else
			IWin32Window revitWindow = new JtWindowHandle(Autodesk.Windows.ComponentManager.ApplicationWindow);
			WindowInteropHelper helper = new WindowInteropHelper(window);
			helper.Owner = revitWindow.Handle;
#endif
			}

		/// <summary>
		/// Gets the window handle.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <returns></returns>
		public static int GetWindowHandle(Window window)
			{
			WindowInteropHelper helper = new WindowInteropHelper(window);
			return helper.Handle.ToInt32();
			}

		/// <summary>
		/// Activates the window.
		/// </summary>
		/// <returns></returns>
		public static bool ActivateWindow()
			{
			IntPtr ptr = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

			if (ptr != IntPtr.Zero)
				{
				return SetForegroundWindow(ptr);
				}

			return false;
			}
		}
	}