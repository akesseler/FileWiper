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
using System.Security;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using System.Collections.Generic;

using Microsoft.Win32;

using plexdata.Controls;
using plexdata.Utilities;

namespace Plexdata.FileWiper
{
    public partial class MainForm : Form, IShutdownListener
    {
        private object critical = new object();
        private bool suppressAtStartup = false;
        private bool powerModePausing = false;
        private AboutBox aboutBox = null;
        private DetailsView detailsView = null;
        private HelpDialog helpDialog = null;
        private string lastOpenFolder = String.Empty;

        private ShutdownTracker shutdownTracker = null;

        public MainForm()
            : base()
        {
            // Ensure an instance of the program's settings!
            this.Settings = new Settings();

            // Suppress Tray Icon and GUI launch handling at startup.
            this.suppressAtStartup = true;
            this.InitializeComponent();
            this.suppressAtStartup = false;

            // Setup main form icon.
            this.Icon = Properties.Resources.MainIcon;
            this.Text += String.Format(" - {0}", AboutBox.Version);

#if SIMULATION
#if DEBUG
            this.Text += " [DEBUG, SIMULATION]";
#else
            this.Text += " [RELEASE, SIMULATION]";
#endif // DEBUG
#else
#if DEBUG
            this.Text += " [DEBUG, EMBATTLED]";
#endif // DEBUG
#endif // SIMULATION

            // Initialize sub components.
            this.InitializeDataReceiver(PermissionCheck.IsRunAsAdmin);
            this.InitializeStatusBar();
            this.InitializeToolbar();
            this.InitializeTrayIcon();
            this.InitializeWipingListMenu();

            // Add state and overall count listeners.
            this.wipingList.StateCounts.InactivityIndicated += this.OnStateCountsInactivityIndicated;
            this.wipingList.OverallCounts.ValuesChanged += this.OnOverallCountsValuesChanged;

            // Initialize details view.
            this.detailsView = new DetailsView(this.wipingList);
            this.detailsView.FormClosing += this.OnDetailsViewFormClosing;

            this.shutdownTracker = new ShutdownTracker(this);
        }

        internal Settings Settings { get; private set; }

        #region Overwritten protected event handler implementation.

        protected override void OnLoad(EventArgs args)
        {
            this.LoadSettings();

            if (Program.Parameters.HasFilepaths)
            {
                this.AppendWipings(Program.Parameters.Filepaths);
            }

            this.UpdateState();

            base.OnLoad(args);
        }

        protected override void OnShown(EventArgs args)
        {
            Program.TraceLogger.Write("MainForm", ">>> OnShown()");

            if (Program.Parameters.IsForceShow)
            {
                Program.TraceLogger.Write("MainForm", "--- OnShown() Force showing main window.");

                this.ForceShowForm();
            }
            else
            {
                // Now it's time to decide in which 
                // state the main form is shown. 
                if (this.IsWiping)
                {
                    Program.TraceLogger.Write("MainForm", "--- OnShown() Wiping is active, so start into icon tray.");

                    this.ForceShowTray();
                }
                else
                {
                    Program.TraceLogger.Write("MainForm", "--- OnShown() Wiping is not active, so show main window.");

                    this.ForceShowForm();
                }
            }

            base.OnShown(args);

            Program.TraceLogger.Write("MainForm", "<<< OnShown()");
        }

        protected override void OnResizeEnd(EventArgs args)
        {
            // Update current window size and location settings 
            // but only if neither minimized nor maximized.
            if (this.WindowState == FormWindowState.Normal)
            {
                this.Settings.Maintain.WindowBounds = this.DesktopBounds;
            }
            base.OnResizeEnd(args);
        }

        protected override void OnSizeChanged(EventArgs args)
        {
            // Do not handle this event as long as startup is 
            // active! See also remarks in the constructor.
            if (!this.suppressAtStartup && this.IsWiping)
            {
                // Minimize to tray only if wiping
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.ForceShowTray();
                }
                else
                {
                    this.ForceShowForm();
                }
            }
            base.OnSizeChanged(args);
        }

        protected override void OnFormClosing(FormClosingEventArgs args)
        {
            Program.TraceLogger.Write("MainForm", ">>> OnFormClosing() " + args.CloseReason.ToString());

            if (args.CloseReason == CloseReason.UserClosing)
            {
                if (this.IsWiping)
                {
                    this.ForceShowTray();
                    args.Cancel = true;
                }
                else
                {
                    this.SaveSettings();
                }
            }
            else
            {
                if (this.IsWiping)
                {
                    this.CancelWipings();
                }
                this.SaveSettings();
            }

            base.OnFormClosing(args);

            Program.TraceLogger.Write("MainForm", "<<< OnFormClosing()");
        }

        protected override void OnHelpRequested(HelpEventArgs args)
        {
            this.PerformDisplayHelp();
            base.OnHelpRequested(args);
        }

        #endregion // Overwritten protected event handler implementation.

        #region Private member function implementation.

        private void UpdateState()
        {
            this.UpdateToolbarState();
            this.UpdateStatusbar();
            Application.DoEvents();
        }

        /// <summary>
        /// This member function is mainly responsible to lock the toolbar 
        /// while new wipings are added. Further, it sets the pausing flag 
        /// to suspend all currently running background worker threads which 
        /// in turn guarantees a responding user interface.
        /// </summary>
        private void DisableWhileConfigureWipings()
        {
            this.IsPausing = true;
            this.IsToolbarLocked = true;
            this.tbbSettings.Enabled = false;
            this.UpdateState();
        }

        /// <summary>
        /// This member function is mainly responsible to unlock the toolbar 
        /// after new wipings have been added. Further, it sets the pausing 
        /// flag to its original state. In case of running background worker 
        /// threads have been suspended they will be continued.
        /// </summary>
        /// <param name="pausing">
        /// Value of previous pausing state.
        /// </param>
        private void EnableAfterConfigureWipings(bool pausing)
        {
            this.IsPausing = pausing;
            this.IsToolbarLocked = false;
            this.tbbSettings.Enabled = true;
            this.UpdateState();
        }

        private void LoadSettings()
        {
            Cursor cursor = this.Cursor;
            try
            {
                this.Cursor = Cursors.WaitCursor;

                Settings helper;
                if (Settings.Load(Settings.Filename, out helper))
                {
                    this.Settings = new Settings(helper);
                    this.ApplySettings(true);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("MainForm", exception);
            }
            finally
            {
                this.Cursor = cursor;
            }
        }

        private void ApplySettings(bool initial)
        {
            Cursor cursor = this.Cursor;
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (initial)
                {
                    // Check and apply window size and location.
                    if (Settings.IsVisibleOnAllScreens(this.Settings.Maintain.WindowBounds))
                    {
                        this.StartPosition = FormStartPosition.Manual;
                        this.DesktopBounds = this.Settings.Maintain.WindowBounds;
                    }
                    else
                    {
                        this.StartPosition = FormStartPosition.WindowsDefaultLocation;
                        this.Size = this.Settings.Maintain.WindowBounds.Size;
                    }

                    // Apply toolbar settings.
                    this.ScaleToolbarButtons(this.Settings.Maintain.ToolbarScaling);
                    this.ShowToolbarText(this.Settings.Maintain.ToolbarText);

                    // Set currently known wiping algorithm.
                    this.detailsView.SetAlgorithm(this.Settings.Algorithms.Selected);
                }
                else
                {
                    // Set new wiping algorithm.
                    this.detailsView.SetAlgorithm(this.Settings.Algorithms.Selected);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("MainForm", exception);
            }
            finally
            {
                this.Cursor = cursor;
            }
        }

        private void SaveSettings()
        {
            Cursor cursor = this.Cursor;
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Save current window size and location but 
                // only if neither minimized nor maximized.
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.Settings.Maintain.WindowBounds = this.DesktopBounds;
                }

                // Toolbar settings are up-to-date already!

                Settings.Save(Settings.Filename, this.Settings);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("MainForm", exception);
            }
            finally
            {
                this.Cursor = cursor;
            }
        }

        private void PerformOpenAction(bool folder)
        {
            try
            {
                List<string> actionItems = new List<string>();

                if (folder)
                {
                    FolderBrowserDialog dialog = new FolderBrowserDialog();
                    dialog.ShowNewFolderButton = false;
                    dialog.Description = "Choose a folder to be added to the list of wipings.";
                    dialog.SelectedPath = this.lastOpenFolder;
                    dialog.RootFolder = Environment.SpecialFolder.MyComputer;

                    if (DialogResult.OK == dialog.ShowDialog(this))
                    {
                        actionItems.Add(dialog.SelectedPath);
                        this.lastOpenFolder = dialog.SelectedPath;
                    }
                }
                else
                {
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Filter = "Any file (*.*)|*.*";
                    dialog.InitialDirectory = this.lastOpenFolder;
                    dialog.CheckPathExists = true;
                    dialog.ShowReadOnly = false;
                    dialog.ReadOnlyChecked = false;
                    dialog.CheckFileExists = true;
                    dialog.ValidateNames = true;
                    dialog.Multiselect = true;

                    if (DialogResult.OK == dialog.ShowDialog(this))
                    {
                        actionItems.AddRange(dialog.FileNames);
                        this.lastOpenFolder = Path.GetDirectoryName(dialog.FileName);
                    }
                }

                if (actionItems.Count > 0)
                {
                    string[] helper = actionItems.ToArray();
                    if (Program.ConfirmDestroyItems(helper, this))
                    {
                        this.AppendWipings(helper);
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("MainForm", exception);
            }
        }

        private void PerformDetailsView()
        {
            if (this.detailsView.Visible)
            {
                this.detailsView.Hide();
            }
            else
            {
                this.detailsView.Show(this);
            }
            if (!this.Focused) { this.Focus(); }
        }

        private void PerformFavoritesDialog()
        {
            try
            {
                FavoritesDialog dialog = new FavoritesDialog();
                // "Yes" means: Execute favorites...
                if (DialogResult.Yes == dialog.ShowDialog(this))
                {
                    //
                    // Do not destroy a favorite base folder!
                    //
                    List<string> fullpaths = new List<string>();
                    string[] favorites = Program.MainForm.Settings.Favorites.Folders;

                    foreach (string favorite in favorites)
                    {
                        fullpaths.AddRange(Directory.GetFileSystemEntries(favorite));
                    }

                    if (fullpaths.Count > 0)
                    {
                        if (Program.ConfirmDestroyItems(favorites, this))
                        {
                            this.AppendWipings(fullpaths.ToArray());
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("MainForm", exception);
            }
        }

        private void PerformSettingsDialog()
        {
            Cursor cursor = this.Cursor;
            try
            {
                this.Cursor = Cursors.WaitCursor;
                SettingsDialog dialog = new SettingsDialog(this.Settings);
                if (DialogResult.OK == dialog.ShowDialog(this))
                {
                    lock (critical) { this.Settings = new Settings(dialog.Settings); }
                    this.ApplySettings(false);
                }
            }
            finally
            {
                this.Cursor = cursor;
            }
        }

        private void PerformAboutBox()
        {
            if (this.aboutBox != null)
            {
                this.aboutBox.Close();
                this.aboutBox.Dispose();
                this.aboutBox = null;
            }

            this.aboutBox = new AboutBox();

            if (!this.Visible)
            {
                this.aboutBox.StartPosition = FormStartPosition.CenterScreen;
                this.aboutBox.ShowInTaskbar = true;
                this.aboutBox.Icon = this.Icon;
            }

            this.aboutBox.ShowDialog();
        }

        private void PerformDisplayHelp()
        {

            if (this.helpDialog == null)
            {
                this.helpDialog = new HelpDialog(true);
            }

            if (this.helpDialog.Visible)
            {
                this.helpDialog.BringToFront();
            }
            else
            {
                Debug.Assert(!this.helpDialog.IsDisposed);
                this.helpDialog.Show();
            }
        }

        private void PerformProgramRelaunch()
        {
            // Firstly, save current settings.
            this.SaveSettings();

            // Right here it is necessary to distinguish between remaining base folders 
            // and top-level files. In case of having some base folders left, the only 
            // content are files that caused trouble while wiping them. In such a case 
            // only the paths with those file are still available. In case of having some 
            // top-level files left (files that are parallel to the list of base folders) 
            // it becomes necessary to handover the full qualified file path.
            //
            // But this applies only if option "IncludeFolderNames" is enabled. In this 
            // case the pending base folder list may contain paths. In case of option 
            // "IncludeFolderNames" is disabled the pending base folder list must be ignored!

            // Determine all remaining base paths as well 
            // as all file names that caused an exception.
            List<string> fullpaths = new List<string>();
#if SIMULATION
            foreach (WipingListItem current in this.failedWipings)
            {
                fullpaths.Add(current.FileInfo.FullName);
            }
#else
            if (this.Settings.Behaviour.IncludeFolderNames)
            {
                foreach (string current in this.pendingBaseFolders)
                {
                    if (Directory.Exists(current))
                    {
                        fullpaths.Add(current);
                    }
                }
            }

            foreach (WipingListItem current in this.failedWipings)
            {
                fullpaths.Add(current.FileInfo.FullName);
            }
#endif // SIMULATION

            // Build relaunch parameter list.
            string parameters = ParameterParser.BuildOptionFilepaths(fullpaths.ToArray());

            if (this.Visible)
            {
                // Be aware, additional parameters must be appended!
                parameters = ParameterParser.CombineOptions(parameters,
                    ParameterParser.BuildOptionForceShow());
            }

            // Be aware, additional parameters must be appended!
            parameters = ParameterParser.CombineOptions(parameters,
                ParameterParser.BuildOptionRelaunch());

            if (SelfElevation.Elevate(ParameterParser.CombineOptions(parameters)))
            {
                // Don't use member function Close() because otherwise the 
                // settings are saved twice. In other words skip handling 
                // in member function OnFormClosing() completely!
                Application.Exit();
            }
            else
            {
                // User abort (probably)! Therefore, show all errors instead.
                if (this.Settings.Behaviour.AllowAutoRelaunch)
                {
                    this.PerformShowExceptions();
                }
            }
        }

        private void PerformShowExceptions()
        {
            List<Exception> exceptions = new List<Exception>();
            foreach (WipingListItem current in this.failedWipings)
            {
                // It is not necessary to check if current exception is null or 
                // not for two reasons: As first wipings are only added to the 
                // list when they have failed and secondly the exception dialog 
                // below can handle invalid exceptions.
                exceptions.Add(current.Exception);
            }

            ExceptionView dialog = new ExceptionView();
            dialog.Caption = "File Wiping Errors";
            dialog.Exceptions = exceptions.ToArray();
            dialog.ShowDialog(this);
        }

        private bool RequestCancelWipings()
        {
            bool success = false;
            if (this.IsWiping)
            {
                string message = "File wiping is still pending and cancelling " +
                    "it may cause partly unreadable files!\n\nDo you really want " +
                    "to cancel currently active wiping?";

                if (DialogResult.Yes == MessageBox.Show(this, message,
                    this.Text + " - Warning", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1))
                {
                    this.CancelWipings();
                    success = true;
                }
            }
            else
            {
                success = true;
            }

            return success;
        }

        private void UserCloseRequest()
        {
            if (this.RequestCancelWipings())
            {
                this.Close();
                Application.Exit();
            }
        }

        private void ForceShowForm()
        {
            // Be aware, the main window is not pushed to front in any case.

            this.TrayIconHide();

            if (!this.IsDisposed)
            {
                // If the about box is currently shown 
                // then simply close and destroy it.
                if (this.aboutBox != null && this.aboutBox.Visible)
                {
                    this.aboutBox.Close();
                    this.aboutBox.Dispose();
                    this.aboutBox = null;
                }

                // Show window if it is currently hidden.
                if (!this.Visible) { this.Show(); }

                if (this.WindowState == FormWindowState.Minimized)
                {
                    // Restore previous window state if it is currently minimized.
                    ShowWindow(this.Handle, SW_RESTORE);
                }

                // Bring main window to front.
                SetForegroundWindow(this.Handle);

                // Activate the window.
                this.Activate();
                this.Focus();

                // Sometimes repainting the form while showing it is 
                // handicapped by worker-thread execution. Therefore, 
                // force repainting the form.
                Application.DoEvents();
            }
        }

        private void ForceShowTray()
        {
            this.WindowState = FormWindowState.Minimized;
            this.Hide();

            this.TrayIconShow();
        }

        private void HandleAutoClose()
        {
            if (this.Settings.Behaviour.AllowAutoClose)
            {
                this.UserCloseRequest();
            }
            else
            {
                this.ForceShowForm();
            }
        }

        private void HandleWipingFault()
        {
            // Log all wiping exceptions in any case.
            foreach (WipingListItem current in this.failedWipings)
            {
                Program.ErrorLogger.Write("Wiping Error", current.Exception);
            }

            if (this.Settings.Behaviour.AllowAutoRelaunch)
            {
                // Check current permission to prevent ongoing relaunches!
                // Also prevent relaunching in case of cancellation!
                if (!PermissionCheck.IsRunAsAdmin && !this.IsCanceled)
                {
                    this.PerformProgramRelaunch();
                }
            }
            else
            {
                // Auto relaunch is disabled, therefore bring 
                // the main window to the front as first.
                this.ForceShowForm();

                // Do not ask for relaunch and do not show any 
                // kind of error message in case of cancellation.
                if (!this.IsCanceled)
                {
                    // Check current permission to suppress dialog in admin mode.
                    if (!PermissionCheck.IsRunAsAdmin)
                    {
                        RelaunchConfirmationDialog dialog = new RelaunchConfirmationDialog();
                        switch (dialog.ShowDialog(this))
                        {
                            case DialogResult.Yes: // Relaunch...
                                this.PerformProgramRelaunch();
                                break;
                            case DialogResult.No:  // Show...
                                this.PerformShowExceptions();
                                break;
                            case DialogResult.Cancel:
                            default:
                                break;
                        }
                    }
                    else
                    {
                        // In admin mode instead simply show all remaining errors.
                        this.PerformShowExceptions();
                    }
                }
            }

            // Clear this list because all errors are handled.
            this.failedWipings.Clear();
        }

        private void WaitFinishCallback(object state)
        {
            // Ensure that wiping has really completed!
            while (this.IsWiping)
            {
                Thread.Sleep(10);
                Application.DoEvents();
            }

            // Reset pausing flag too.
            this.IsPausing = false;

            //Go back to the user interface thread.
            this.WaitFinishCompleted(state);
        }

        private delegate void WaitFinishCompletedDelegate(object state);
        private void WaitFinishCompleted(object state)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new WaitFinishCompletedDelegate(this.WaitFinishCompleted), state);
            }
            else
            {
                // Try to remove all pending folders. If something fails member 
                // HandleWipingFault() will take care about remaining folders.
                this.CleanPendingFolders();

                // Check whether wipings have failed.
                if (this.IsWipingFault)
                {
                    this.HandleWipingFault();
                }
                // Check for auto close conditions.
                else if (!this.Visible)
                {
                    this.HandleAutoClose();
                }

                // All pending base folders are handled 
                // until here, so it is a good idea to 
                // clean this list.
                this.pendingBaseFolders.Clear();
            }
        }

        #endregion // Private member function implementation.

        #region Private event handler implementation.

        private void OnStateCountsInactivityIndicated(object sender, EventArgs args)
        {
            // Keep in mind, this event handler is called from inside the 
            // background worker's completion routine. Therefore, schedule 
            // the processing of this event which in turn ensures that the 
            // background worker's completion routine can end up.
            ThreadPool.QueueUserWorkItem(this.WaitFinishCallback, sender);
        }

        private void OnOverallCountsValuesChanged(object sender, EventArgs args)
        {
            WipingOverallCounts counts = sender as WipingOverallCounts;
            if (counts != null)
            {
                // Update tooltip in any case!
                this.TrayIconChangeTooltip(counts.WipedFileSize, counts.TotalFileSize);

                // Update statusbar progress.
                this.UpdateStatusbar(counts);
            }
        }

        private void OnDetailsViewFormClosing(object sender, FormClosingEventArgs args)
        {
            if (args.CloseReason == CloseReason.UserClosing)
            {
                args.Cancel = true;
                this.PerformDetailsView();
            }
        }

        #endregion // Private event handler implementation.

        #region Shutdown Tracker event handling section.

        public void HandleSessionSwitch(SessionSwitchReason reason)
        {
            // No other action needed!
        }

        public bool RequestSessionEnding(SessionEndReasons reason)
        {
            // No other action needed! Therefore, 
            // allow ending of this session.
            return true;
        }

        public void HandlePowerModeChanged(PowerModes mode)
        {
            Program.TraceLogger.Write("MainForm", ">>> HandlePowerModeChanged() " + mode.ToString());

            try
            {
                if (mode == PowerModes.StatusChange)
                {
                    bool isBattery = SystemPowerStatus.IsBattery;

                    Program.TraceLogger.Write("MainForm", "--- HandlePowerModeChanged() IsBattery=" + isBattery);

                    if (isBattery)
                    {
                        if (!this.IsPausing)
                        {
                            Program.TraceLogger.Write("MainForm", "--- HandlePowerModeChanged() Suspend active wipings.");

                            // Suspend currently active wipings only if power mode 
                            // is battery and if it is not already pausing.
                            this.powerModePausing = true;
                            this.SuspendWipings();
                        }
                    }
                    else
                    {
                        if (this.IsPausing && this.powerModePausing)
                        {
                            Program.TraceLogger.Write("MainForm", "--- HandlePowerModeChanged() Resume suspended wipings.");

                            // Resume pausing wipings only if power mode is not battery 
                            // and if it was suspended by a previous power change event.
                            this.ContinueWipings();
                        }

                        this.powerModePausing = false;
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("MainForm", exception);
            }
            finally
            {
                Program.TraceLogger.Write("MainForm", "<<< HandlePowerModeChanged()");
            }
        }

        #endregion // Shutdown Tracker event handling section.
    }
}
