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
using System.Diagnostics;
using System.Windows.Forms;

namespace Plexdata.FileWiper
{
    internal static class SelfElevation
    {
        /// <summary>
        /// This method tries to start the current executable with administrator privileges.
        /// </summary>
        /// <remarks>
        /// The elevated process is only started. This means the caller does not wait 
        /// for a termination of the elevated process.
        /// </remarks>
        /// <param name="parameters">
        /// List of space separated parameters to be used. This parameter can be null or empty.
        /// </param>
        /// <returns>
        /// True if the user pressed button <b>Yes</b> on the Windows elevation dialog box and 
        /// false in case of the user pressed button <b>No</b>.
        /// </returns>
        /// <seealso cref="Plexdata.FileWiper.SelfElevation.Elevate(String, Boolean)">
        /// Elevate(parameters, wait)
        /// </seealso>
        public static Boolean Elevate(String parameters)
        {
            return Elevate(parameters, false);
        }

        /// <summary>
        /// This method tries to start the current executable with administrator privileges 
        /// and waits for termination,if requested.
        /// </summary>
        /// <remarks>
        /// The elevated process is started and if successful and if it is requested this method 
        /// waits until the elevated process has terminated.
        /// <para>
        /// This method throws various exceptions. For more information about this topic please 
        /// refer to the explanations about method 
        /// <see cref="System.Diagnostics.Process.Start(System.Diagnostics.ProcessStartInfo)">
        /// Process.Start(ProcessStartInfo)</see> in the <b>MSDN</b>.
        /// </para>
        /// </remarks>
        /// <param name="parameters">
        /// List of space separated parameters to be used. This parameter can be null or empty.
        /// </param>
        /// <param name="wait">
        /// If true then the method wait until process has terminated; otherwise the method 
        /// returns immediately.
        /// </param>
        /// <returns>
        /// True if the user pressed button <b>Yes</b> on the Windows elevation dialog box 
        /// and false in case of the user pressed button <b>No</b>.
        /// </returns>
        /// <seealso cref="Plexdata.FileWiper.SelfElevation.Elevate(String)">
        /// Elevate(parameters)
        /// </seealso>
        /// <seealso cref="System.Diagnostics.Process.Start(System.Diagnostics.ProcessStartInfo)">
        /// Process.Start(ProcessStartInfo)
        /// </seealso>
        public static Boolean Elevate(String parameters, Boolean wait)
        {
            try
            {
                // Be aware, starting a process with different window styles does not really 
                // work! Therefore, starting the sibling process with administrator privileges 
                // uses appropriated command line arguments instead.

                Program.TraceLogger.Write("SelfElevation", ">>> Elevate(" + parameters + ", " + wait + ")");

                ProcessStartInfo info = new ProcessStartInfo()
                {
                    Verb = "runas",
                    Arguments = parameters,
                    FileName = Application.ExecutablePath
                };

                if (wait)
                {
                    Program.TraceLogger.Write("SelfElevation", "--- Elevate() Self elevation with waiting started.");

                    Process process = Process.Start(info);
                    process.WaitForExit();

                    Program.TraceLogger.Write("SelfElevation", "--- Elevate() Self elevation with waiting " +
                        (process.ExitCode == 0 ? "successful." : "failed with error code " + process.ExitCode.ToString() + "!"));

                    // By definition the self elevated program 
                    // returns zero if execution was successful!
                    return process.ExitCode == 0;
                }
                else
                {
                    Program.TraceLogger.Write("SelfElevation", "--- Elevate() Self elevation without waiting.");

                    Process.Start(info);
                    return true;
                }
            }
            catch (Win32Exception exception)
            {
                const Int32 ERROR_CANCELLED = 1223;

                if (exception.NativeErrorCode == ERROR_CANCELLED)
                {
                    Program.TraceLogger.Write("SelfElevation", "--- Elevate() Self elevation has been canceled!");
                    return false;
                }
                else
                {
                    Program.FatalLogger.Write("SelfElevation", exception);
                    throw exception;
                }
            }
            finally
            {
                Program.TraceLogger.Write("SelfElevation", "<<< Elevate()");
            }
        }
    }
}
