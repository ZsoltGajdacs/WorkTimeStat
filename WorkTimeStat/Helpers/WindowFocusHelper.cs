using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

/*
 * Taken from: https://www.codeproject.com/Answers/184472/Text-box-is-not-working-in-WPF-Popup
 */
namespace WorkTimeStat.Helpers
{
    internal static class WindowFocusHelper
    {
        /// <summary>
        /// Required to be able to type to a popup control. Brings it to the fore so it can get focus.
        /// </summary>
        public static void ActivatePopup(Popup popup)
        {
            //try to get a handle on the popup itself (via its child)
            HwndSource source = (HwndSource)PresentationSource.FromVisual(popup.Child);
            IntPtr handle = source.Handle;

            //activate the popup
            SetForegroundWindow(handle);
        }

        [DllImport("USER32.DLL")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
