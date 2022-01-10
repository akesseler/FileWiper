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
using System.Runtime.InteropServices;

namespace plexdata.FileWiper
{
    public partial class MainForm : Form
    {
        #region Win32 API helper function implementation.

        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_MAXIMIZE = 3;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SW_SHOWMINNOACTIVE = 7;
        private const int SW_SHOWNA = 8;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;
        private const int SW_FORCEMINIMIZE = 11;

        // Windows 2000 Professional / Windows 2000 Server
        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int command);

        // Windows 2000 Professional / Windows 2000 Server
        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        #endregion // Win32 API helper function implementation.
    }
}
