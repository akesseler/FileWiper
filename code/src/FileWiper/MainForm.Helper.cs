/*
 * MIT License
 * 
 * Copyright (c) 2022 plexdata.de
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Plexdata.FileWiper
{
    public partial class MainForm : Form
    {
        #region Win32 API helper function implementation.

#pragma warning disable IDE0051 // Remove unused private members
        private const Int32 SW_HIDE = 0;
        private const Int32 SW_SHOWNORMAL = 1;
        private const Int32 SW_SHOWMINIMIZED = 2;
        private const Int32 SW_MAXIMIZE = 3;
        private const Int32 SW_SHOWMAXIMIZED = 3;
        private const Int32 SW_SHOWNOACTIVATE = 4;
        private const Int32 SW_SHOW = 5;
        private const Int32 SW_MINIMIZE = 6;
        private const Int32 SW_SHOWMINNOACTIVE = 7;
        private const Int32 SW_SHOWNA = 8;
        private const Int32 SW_RESTORE = 9;
        private const Int32 SW_SHOWDEFAULT = 10;
        private const Int32 SW_FORCEMINIMIZE = 11;
#pragma warning restore IDE0051 // Remove unused private members

        // Windows 2000 Professional / Windows 2000 Server
        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern Boolean ShowWindow(IntPtr hWnd, Int32 command);

        // Windows 2000 Professional / Windows 2000 Server
        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        private static extern Boolean SetForegroundWindow(IntPtr hWnd);

        #endregion // Win32 API helper function implementation.
    }
}
