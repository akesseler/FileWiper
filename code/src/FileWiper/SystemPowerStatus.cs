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
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace plexdata.FileWiper
{
    // See also: http://msdn.microsoft.com/en-us/library/ms704147(v=vs.85).aspx
    public static class SystemPowerStatus
    {
        public static bool IsBattery
        {
            get
            {
                SYSTEM_POWER_STATUS powerStatus = new SYSTEM_POWER_STATUS();
                if (SystemPowerStatus.GetSystemPowerStatus(ref powerStatus))
                {
                    return (byte)ACLineStatus.Battery == powerStatus.ACLineStatus;
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
            public byte ACLineStatus;
            public byte BatteryFlag;
            public byte BatteryLifePercent;
            public byte Reserved1;
            public Int32 BatteryLifetime;
            public Int32 BatteryFullLifetime;
        }

        private enum ACLineStatus : byte
        {
            Battery = 0,
            ACLine = 1,
            Unknown = 255
        }

        [FlagsAttribute]
        private enum BatteryFlag : byte
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
        private extern static bool GetSystemPowerStatus(ref SYSTEM_POWER_STATUS powerStatus);

        #endregion // Win32 API related implementations.
    }
}
