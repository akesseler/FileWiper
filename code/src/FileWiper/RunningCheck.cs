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
using System.Diagnostics;
using System.Collections.Generic;

using System.Runtime.InteropServices;

namespace Plexdata.FileWiper
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
