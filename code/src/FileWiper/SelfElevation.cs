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
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace plexdata.FileWiper
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
        /// <seealso cref="plexdata.FileWiper.SelfElevation.Elevate(string, bool)">
        /// Elevate(parameters, wait)
        /// </seealso>
        public static bool Elevate(string parameters)
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
        /// <seealso cref="plexdata.FileWiper.SelfElevation.Elevate(string)">
        /// Elevate(parameters)
        /// </seealso>
        /// <seealso cref="System.Diagnostics.Process.Start(System.Diagnostics.ProcessStartInfo)">
        /// Process.Start(ProcessStartInfo)
        /// </seealso>
        public static bool Elevate(string parameters, bool wait)
        {
            try
            {
                // Be aware, starting a process with different window styles does not really 
                // work! Therefore, starting the sibling process with administrator privileges 
                // uses appropriated command line arguments instead.

                Program.TraceLogger.Write("SelfElevation", ">>> Elevate(" + parameters + ", " + wait + ")");

                ProcessStartInfo info = new ProcessStartInfo();
                info.Verb = "runas";
                info.Arguments = parameters;
                info.FileName = Application.ExecutablePath;

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
                const int ERROR_CANCELLED = 1223;

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
