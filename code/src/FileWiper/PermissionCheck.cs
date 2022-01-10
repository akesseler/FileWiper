/*
 * Copyright (C)  2013  Axel Kesseler
 * 
 * This software is free and you can use it for any purpose. Furthermore, 
 * you are free to copy, to modify and/or to redistribute this software.
 * 
 * In addition, this software is distributed in the hope that it will be 
 * useful, but WITHOUT ANY WARRANTY; without even the implied warranty of 
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 */

using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace plexdata.FileWiper
{
    internal static class PermissionCheck
    {
        internal static bool IsRunAsAdmin { get { return IsUserAnAdmin(); } }

        internal static void SetButtonShield(Button button, bool visible)
        {
            const int BCM_SETSHIELD = 0x0000160C;

            if (button != null)
            {
                // Important because otherwise shield is not shown!
                if (button.FlatStyle != FlatStyle.System)
                {
                    button.FlatStyle = FlatStyle.System;
                }

                HandleRef hWnd = new HandleRef(button, button.Handle);
                IntPtr lParam = visible ? new IntPtr(1) : new IntPtr(0);

                SendMessage(hWnd, BCM_SETSHIELD, IntPtr.Zero, lParam);
            }
        }

        #region Win32 related declaration and implementation section.

        // Windows 2000 Professional / Windows 2000 Server
        // Remarks are taken from the MSDN: "This function is a wrapper for CheckTokenMembership. 
        // It is recommended to call that function directly to determine Administrator group status 
        // rather than calling IsUserAnAdmin." 
        [DllImport("shell32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern bool IsUserAnAdmin();

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(HandleRef hWnd, int message, IntPtr wParam, IntPtr lParam);

        #endregion // Win32 related declaration and implementation section.
    }
}
