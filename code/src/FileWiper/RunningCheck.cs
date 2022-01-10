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
using System.Diagnostics;
using System.Collections.Generic;

using System.Runtime.InteropServices;

namespace plexdata.FileWiper
{
    internal static class RunningCheck
    {
        // Exceptions, thrown by all methods:
        //
        //   System.InvalidOperationException:
        //     The process's System.Diagnostics.Process.Id property has not been set.  -or-
        //     There is no process associated with this System.Diagnostics.Process object.
        //
        //   System.PlatformNotSupportedException:
        //     The platform is Windows 98 or Windows Millennium Edition (Windows Me); set
        //     the System.Diagnostics.ProcessStartInfo.UseShellExecute property to false
        //     to access this property on Windows 98 and Windows Me.
        //
        //   System.SystemException:
        //     The process does not have an identifier, or no process is associated with
        //     the System.Diagnostics.Process.  -or- The associated process has exited.
        //
        public static bool IsRunning
        {
            get
            {
                return GetFirstSiblingProcess() != null;
            }
        }

        public static Process GetFirstSiblingProcess()
        {
            return GetFirstSiblingProcess(Process.GetCurrentProcess());
        }

        public static Process GetFirstSiblingProcess(Process process)
        {
            // This member returns first found "*.vshost" as sibling process! 
            // Only with the second and other subsequent calls this extension 
            // is removed when running inside debugger! Everything is fine as 
            // long as the program is started using e.g. the Explorer.
            if (process != null)
            {
                foreach (Process current in Process.GetProcesses())
                {
                    // Don't find myself...
                    if (process.Id != current.Id)
                    {
                        if (String.Compare(process.ProcessName, current.ProcessName, true) == 0)
                        {
                            return current;
                        }
                    }
                }
            }
            return null;
        }
    }
}
