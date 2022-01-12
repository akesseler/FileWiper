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
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Plexdata.FileWiper
{
    // See also: http://msdn.microsoft.com/en-us/library/ms704147(v=vs.85).aspx
    public static class SystemPowerStatus
    {
        public static Boolean IsBattery
        {
            get
            {
                SYSTEM_POWER_STATUS powerStatus = new SYSTEM_POWER_STATUS();
                if (SystemPowerStatus.GetSystemPowerStatus(ref powerStatus))
                {
                    return (Byte)ACLineStatus.Battery == powerStatus.ACLineStatus;
                }
                else
                {
                    throw new InvalidOperationException(null, new Win32Exception(Marshal.GetLastWin32Error()));
                }
            }
        }

        #region Win32 API related implementations.

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_POWER_STATUS
        {
            public Byte ACLineStatus;
            public Byte BatteryFlag;
            public Byte BatteryLifePercent;
            public Byte Reserved1;
            public Int32 BatteryLifetime;
            public Int32 BatteryFullLifetime;
        }

        private enum ACLineStatus : Byte
        {
            Battery = 0,
            ACLine = 1,
            Unknown = 255
        }

        [FlagsAttribute]
        private enum BatteryFlag : Byte
        {
            High = 1,
            Low = 2,
            Critical = 4,
            Charging = 8,
            NoSystemBattery = 128,
            Unknown = 255
        }

        // Windows 2000 Professional / Windows 2000 Server
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
        private extern static Boolean GetSystemPowerStatus(ref SYSTEM_POWER_STATUS powerStatus);

        #endregion // Win32 API related implementations.
    }
}
