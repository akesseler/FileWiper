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
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

using plexdata.Utilities;

namespace Plexdata.FileWiper
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">
        /// List of handed over command line parameters.
        /// </param>
        [STAThread]
        static void Main(string[] args)
        {
            // Firstly, set default exit code.
            int exitCode = 0;

            try
            {
                Program.TraceLogger.Write("Program", ">>> Application Startup");

                // Register fallback exception handlers.
                Application.ThreadException +=
                    new ThreadExceptionEventHandler(Program.OnUnhandledThreadException);
                AppDomain.CurrentDomain.UnhandledException +=
                    new UnhandledExceptionEventHandler(Program.OnUnhandledGUIException);

                // Register application exit event handler.
                Application.ApplicationExit += new EventHandler(Program.OnApplicationExit);

                // Provide visual settings.
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
#if !DEBUG
                // Show first launch confirmation warning but only if necessary.
                if (!Program.ConfirmFirstLaunchWarning())
                {
                    // Exit application in case of no confirmation.
                    exitCode = -1;
                    return;
                }
#endif // !DEBUG
                // Parse and Save current command line argument.
                Program.Parameters = new ParameterParser(args);

                // Begin to work.
                if (Program.Parameters.IsRegister)
                {
                    #region Execute registration.

                    if (PermissionCheck.IsRunAsAdmin)
                    {
                        exitCode = ShellExtensionHandler.RegisterExtension(
                            Program.Parameters.RegisterOption) ? 0 : -1;
                    }
                    else
                    {
                        MessageBox.Show("Insufficient access rights! You must start this program with " +
                            "administrator privileges if you want to register the Shell Extension.",
                            AboutBox.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    #endregion // Execute registration.
                }
                else if (Program.Parameters.IsUnregister)
                {
                    #region Execute deregistration.

                    if (PermissionCheck.IsRunAsAdmin)
                    {
                        exitCode = ShellExtensionHandler.UnregisterExtension() ? 0 : -1;
                    }
                    else
                    {
                        MessageBox.Show("Insufficient access rights! You must start this program with " +
                            "administrator privileges if you want to remove the Shell Extension.",
                            AboutBox.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    #endregion // Execute deregistration.
                }
                else if (Program.Parameters.IsRelaunch)
                {
                    #region Execute relaunch.

                    // In this case: no confirmation and no running check!

                    if (PermissionCheck.IsRunAsAdmin)
                    {
                        // Just start the MainForm. Running in tray icon or as GUI is 
                        // handled automatically because of an availability of filepaths.
                        Application.Run(Program.MainForm);
                    }
                    else
                    {
                        MessageBox.Show("Insufficient access rights! You must start this program with " +
                            "administrator privileges if you want to use the relaunch commandline argument.",
                            AboutBox.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    #endregion // Execute relaunch.
                }
                else
                {
                    if (Program.Parameters.HasFilepaths)
                    {
                        #region Handle start with files.
#if !DEBUG
                        // Ask the user to confirm current wiping!
                        if (!Program.ConfirmDestroyItems(Program.Parameters.Filepaths))
                        {
                            // Exit application in case of no confirmation.
                            exitCode = -1;
                            return;
                        }
#endif // !DEBUG
                        if (RunningCheck.IsRunning)
                        {
                            DataDispatcher dispatcher = new DataDispatcher();
                            Process sibling = RunningCheck.GetFirstSiblingProcess();

                            // Try getting sibling process to the front.
                            Program.RequestActivation(dispatcher, sibling);

                            // Send Files to running instance and exit afterwards.
                            IntPtr hReceiver = sibling.MainWindowHandle;
                            foreach (string file in Program.Parameters.Filepaths)
                            {
                                // Try to send current filename to sibling process. In case of an 
                                // error simply end with transmitting (without any error message).
                                if (!dispatcher.SendFilename(hReceiver, file)) { break; }
                            }
                        }
                        else
                        {
                            // Start in icon tray and begin wiping. Tray icon launch 
                            // is handled automatically because of having filepaths.
                            Application.Run(Program.MainForm);
                        }

                        #endregion // Handle start with files.
                    }
                    else
                    {
                        #region Handle start without files.

                        if (RunningCheck.IsRunning)
                        {
                            // Try getting sibling process to the front.
                            Program.RequestActivation(new DataDispatcher(),
                                RunningCheck.GetFirstSiblingProcess());
                        }
                        else
                        {
                            // Start GUI and wait until user exits. GUI start is 
                            // handled automatically because of empty filepaths.
                            Application.Run(Program.MainForm);
                        }

                        #endregion // Handle start without files.
                    }
                }
            }
            catch (Exception exception)
            {
                Program.HandleException(exception);
                exitCode = -1;
            }
            finally
            {
                // Set exit code...
                Environment.ExitCode = exitCode;

                // ...and exit application.
                Application.Exit();
            }
        }

        #region Internal property implementation section.

        private static MainForm mainForm = null;

        internal static MainForm MainForm
        {
            get
            {
                if (Program.mainForm == null)
                {
                    Program.mainForm = new MainForm();
                }
                return Program.mainForm;
            }
        }

        /// <summary>
        /// Returns an instance of the internal parameter parser.
        /// </summary>
        internal static ParameterParser Parameters { get; private set; }

        /// <summary>
        /// Represents the internal instance of the debug trace logger.
        /// </summary>
        private static TraceLogger traceLogger = null;

        /// <summary>
        /// Returns an instance of the internal debug trace logger.
        /// </summary>
        internal static TraceLogger TraceLogger
        {
            get
            {
                if (Program.traceLogger == null)
                {
                    Program.traceLogger = new TraceLogger();
                }
                return Program.traceLogger;
            }
        }

        /// <summary>
        /// Represents the internal instance of the error message logger.
        /// </summary>
        private static ErrorLogger errorLogger = null;

        /// <summary>
        /// Returns an instance of the internal error message logger.
        /// </summary>
        internal static ErrorLogger ErrorLogger
        {
            get
            {
                if (Program.errorLogger == null)
                {
                    Program.errorLogger = new ErrorLogger();
                }
                return Program.errorLogger;
            }
        }

        /// <summary>
        /// Represents the internal instance of the fatal message logger.
        /// </summary>
        private static ErrorLogger fatalLogger = null;

        /// <summary>
        /// Returns an instance of the internal fatal message logger.
        /// </summary>
        internal static ErrorLogger FatalLogger
        {
            get
            {
                if (Program.fatalLogger == null)
                {
                    Program.fatalLogger = new ErrorLogger(
                       Path.ChangeExtension(Application.ExecutablePath, ".fatal"), true);
                }
                return Program.fatalLogger;
            }
        }

        #endregion // Internal property implementation section.

        #region Internal event handling implementation section.

        /// <summary>
        /// Handles the event when the application is about to shut down.
        /// </summary>
        /// <param name="sender">
        /// The sender associated with this event.
        /// </param>
        /// <param name="args">
        /// An event arguments instance (not used).
        /// </param>
        private static void OnApplicationExit(object sender, EventArgs args)
        {
            Program.TraceLogger.Write("Program", "<<< Application Exit (ExitCode: " + Environment.ExitCode + ")");
        }

        /// <summary>
        /// Handles the <see cref="System.Windows.Forms.Application.ThreadException">ThreadException</see>
        /// event of an <see cref="System.Windows.Forms.Application">Application</see>.
        /// </summary>
        /// <param name="sender">
        /// The sender associated with this event.
        /// </param>
        /// <param name="args">
        /// A <see cref="System.Threading.ThreadExceptionEventArgs">ThreadExceptionEventArgs</see> that 
        /// contains the event data.
        /// </param>
        private static void OnUnhandledThreadException(object sender, ThreadExceptionEventArgs args)
        {
            Program.HandleException(args.Exception);
        }

        /// <summary>
        /// Handles the event raised by an exception that is not handled by the application domain.
        /// </summary>
        /// <param name="sender">
        /// The source of the unhandled exception event.
        /// </param>
        /// <param name="args">
        /// An <see cref="System.UnhandledExceptionEventArgs">UnhandledExceptionEventArgs</see> that 
        /// contains the event data.
        /// </param>
        private static void OnUnhandledGUIException(object sender, UnhandledExceptionEventArgs args)
        {
            Program.HandleException(args.ExceptionObject as Exception);
        }

        #endregion // Internal event handling implementation section.

        #region Internal method implementation section.

        internal static bool ConfirmDestroyItems(string[] fullpaths)
        {
            return Program.ConfirmDestroyItems(fullpaths, null);
        }

        internal static bool ConfirmDestroyItems(string[] fullpaths, Form parent)
        {
            DestroyConfirmationDialog dialog = new DestroyConfirmationDialog(fullpaths);
            dialog.ShowInTaskbar = (parent == null);
            return dialog.ShowDialog(parent) == DialogResult.Yes;
        }

        #endregion Internal method implementation section.

        #region Private method implementation section.

        private static bool ConfirmFirstLaunchWarning()
        {
            if (!File.Exists(Settings.Filename))
            {
                FirstLaunchWarning dialog = new FirstLaunchWarning();
                if (dialog.ShowDialog() == DialogResult.Yes)
                {
                    // BUGFIX: Allow relaunch after first start.
                    // Saving default settings allows to restart the program as long as 
                    // the firstly started program has not yet saved its settings. This 
                    // may happen for example when the program is started the first time, 
                    // then the user immediatly registers the Shell Extension (internal 
                    // restart with admin rights). In such a case the confirmation dialog 
                    // is shown twice and saving dummy settings avoids this.
                    return Settings.Save(Settings.Filename, new Settings());
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Shows an error dialog and exits the application afterwards.
        /// </summary>
        /// <param name="exception">
        /// An <see cref="System.Exception">Exception</see> object instance 
        /// to be handled.
        /// </param>
        private static void HandleException(Exception exception)
        {
            try
            {
                Program.FatalLogger.Write("Application", exception);

                ErrorDialog dialog = new ErrorDialog(exception);
                dialog.ShowDialog();
            }
            catch
            {
                // No way to report an exception that happens here...
                // But on the other hand, neither the error logger 
                // nor the error dialog should throw an exception.
            }
            finally
            {
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Tries to send an activation request to another running instance of this program.
        /// </summary>
        /// <param name="dispatcher">
        /// An instance of the DataDispatcher that will send the activation request message.
        /// </param>
        /// <param name="sibling">
        /// An instance of the running process.
        /// </param>
        private static void RequestActivation(DataDispatcher dispatcher, Process sibling)
        {
            try
            {
                Program.TraceLogger.Write("Program", ">>> RequestActivation()");

                if (dispatcher == null) { throw new ArgumentNullException("dispatcher"); }
                if (sibling == null) { throw new ArgumentNullException("sibling"); }

                bool success = false;
                IntPtr hReceiver = sibling.MainWindowHandle;

                if (hReceiver != IntPtr.Zero)
                {
                    success = dispatcher.SendBringToFront(hReceiver);
                }
                else
                {
                    success = dispatcher.SendBringToFront(); // Uses broadcast window handle.

                    if (success)
                    {
                        // Elegant is something different! But this 
                        // loop is required to ensure a valid handle.

                        int loops = 10;
                        while (sibling.MainWindowHandle == IntPtr.Zero)
                        {
                            Thread.Sleep(10);
                            sibling.Refresh();
                            if (0 >= --loops) { break; }
                        }
                        success = sibling.MainWindowHandle != IntPtr.Zero;
                    }
                }

                if (!success) { throw new ApplicationException("Sibling process was unreachable!"); }

                Program.TraceLogger.Write("Program", "--- RequestActivation() successful.");
            }
            finally
            {
                Program.TraceLogger.Write("Program", "<<< RequestActivation()");
            }
        }

        #endregion // Private method implementation section.
    }
}
