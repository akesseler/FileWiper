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
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Security.Principal;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace Plexdata.FileWiper
{
    public partial class MainForm : Form
    {
        List<WipingListItem> activeWipings = new List<WipingListItem>();
        List<WipingListItem> pendingWipings = new List<WipingListItem>();
        List<WipingListItem> failedWipings = new List<WipingListItem>();
        List<string> pendingBaseFolders = new List<string>();

        #region Private property implementation.

        /// <summary>
        /// This flag is used in many cases and indicated for example whether 
        /// a running background worker thread should be suspended or not.
        /// </summary>
        /// <remarks>
        /// Additionally, this flag is needed to adjust the toolbar as well 
        /// as the state of the tray-icon. Further, this flag also prevents 
        /// startups of another pending wipings.
        /// </remarks>
        private volatile bool IsPausing = false;

        /// <summary>
        /// This flag is basically necessary to suppress the listview's 
        /// context menu while an ongoing cancellation.
        /// </summary>
        /// <remarks>
        /// Further, this flag also prevents startups of another pending 
        /// wipings. But this happens just rarely and a forecast is not 
        /// possible because of using a multi-threading environment!
        /// </remarks>
        private volatile bool IsCanceling = false;

        /// <summary>
        /// This flag is basically necessary to prevent an application 
        /// restart in case of some files need administration rights.
        /// </summary>
        private volatile bool IsCanceled = false;

        /// <summary>
        /// This flag is needed to make appropriated decisions about how 
        /// the user interface should behave.
        /// </summary>
        /// <remarks>
        /// Determine the sizes of the active and pending wiping lists 
        /// is really good enough to make a reliable decision.
        /// </remarks>
        private bool IsWiping
        {
            get
            {
                return this.activeWipings.Count > 0 || this.pendingWipings.Count > 0;
            }
        }

        private bool IsWipingFault
        {
            get
            {
                return this.failedWipings.Count > 0;
            }
        }

        #endregion // Private property implementation.

        #region Private context menu function implementation.

        private void InitializeWipingListMenu()
        {
            // Setup list view context menu items.
            Size scaling = SystemInformation.SmallIconSize;
            this.wipingListMenu.ImageScalingSize = scaling;
            this.wipingListMenuCancel.Image = new Icon(Properties.Resources.Cancel, scaling).ToBitmap();
            this.wipingListMenuFindError.Image = new Icon(Properties.Resources.Next, scaling).ToBitmap();
            this.wipingListMenuShowError.Image = new Icon(Properties.Resources.Error, scaling).ToBitmap();
            this.wipingListMenuRemove.Image = new Icon(Properties.Resources.ItemRemove, scaling).ToBitmap();
        }

        private void OnWipingListMenuOpening(object sender, CancelEventArgs args)
        {
            // Assume the context menu cannot be shown.
            args.Cancel = true;

            try
            {
                // First of all set default states.
                this.wipingListMenuCancel.Enabled = false;
                this.wipingListMenuShowError.Enabled = false;
                this.wipingListMenuFindError.Enabled = false;

                if (!this.IsCanceling)
                {
                    // Show the context menu.
                    args.Cancel = false;

                    if (this.wipingList.SelectedItems.Count > 0)
                    {
                        // Keep in mind accessing the list-view-items in this way might cause 
                        // an exception! But only if at least one non-wiping-list-item is in 
                        // the list, which should never be the case.
                        foreach (WipingListItem current in this.wipingList.SelectedItems)
                        {
                            // Cancellation is possible if current item state 
                            // is either pausing, or pending, or processing.
                            this.wipingListMenuCancel.Enabled |=
                                current.State == WipingItemStates.Pausing ||
                                current.State == WipingItemStates.Pending ||
                                current.State == WipingItemStates.Processing;

                            // At least one list item can be canceled. 
                            // Therefore, stop looping.
                            if (this.wipingListMenuCancel.Enabled) { break; }
                        }
                    }

                    if (this.wipingList.SelectedItems.Count == 1)
                    {
                        // Keep in mind accessing the list-view-items in this way might cause 
                        // an exception! But only if at least one non-wiping-list-item is in 
                        // the list, which should never be the case.
                        this.wipingListMenuShowError.Enabled =
                            (this.wipingList.SelectedItems[0] as WipingListItem).State == WipingItemStates.Failed;
                    }

                    // See remarks below! But in this case the 
                    // known bug is intentionally ignored.
                    this.wipingListMenuFindError.Enabled =
                        this.wipingList.StateCounts.Failed > 0 ||
                        this.wipingList.StateCounts.Missing > 0 ||
                        this.wipingList.StateCounts.Unknown > 0;

                    this.wipingListMenuRemove.Enabled = this.wipingList.Items.Count > 0;
#if DEBUG
                    this.wipingListMenuRemoveAll.Enabled = !this.IsWiping && !this.IsPausing;
#else
                    this.wipingListMenuRemove.DropDownItems.Remove(this.wipingListMenuRemoveAll);
#endif
                    // BUG: Using Finished and Canceled state count does not really work as menu item state!
                    //
                    // Of course, it would be a very good idea to enable or disable the Finished and Canceled 
                    // menu items according to current values of the state counts. But this is unfortunately 
                    // not really possible because those counts are not (and should not) reset! To fix this 
                    // problem two solutions are possible: 1) Loop through all existing List-View items and 
                    // check their actual state and 2) Use additional variables that track all removed Finished 
                    // and Canceled items. But both solutions will raise another problem. The Finished menu 
                    // item is never up-to-date as long as the context menu is open and another wiping has 
                    // finished in the meantime. Therefore, leave both menu items enabled.
                    //
                    // this.wipingListMenuRemoveFinished.Enabled = this.wipingList.StateCounts.Finished > 0;
                    // this.wipingListMenuRemoveCanceled.Enabled = this.wipingList.StateCounts.Canceled > 0;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("MainForm.Wiping", exception);
            }
        }

        private void OnWipingListMenuItemClick(object sender, EventArgs args)
        {
            try
            {
                if (this.wipingList.SelectedItems.Count > 0)
                {
                    if (sender == this.wipingListMenuCancel)
                    {
                        // Keep in mind accessing the list-view-items in this way might cause 
                        // an exception! But only if at least one non-wiping-list-item is in 
                        // the list, which should never be the case.
                        foreach (WipingListItem current in this.wipingList.SelectedItems)
                        {
                            if (this.pendingWipings.Contains(current))
                            {
                                this.pendingWipings.Remove(current);
                                this.CancelWiping(current);
                            }
                            else if (this.activeWipings.Contains(current))
                            {
                                this.activeWipings.Remove(current);
                                this.CancelWiping(current);
                            }
                        }
                    }
                    else if (sender == this.wipingListMenuShowError)
                    {
                        // Keep in mind accessing the list-view-items in this way might cause 
                        // an exception! But only if at least one non-wiping-list-item is in 
                        // the list, which should never be the case.
                        ExceptionView dialog = new ExceptionView();
                        dialog.Caption = "File Wiping Error";
                        dialog.Exceptions = new Exception[] { 
                            (this.wipingList.SelectedItems[0] as WipingListItem).Exception };
                        dialog.ShowDialog(this);
                    }
                }

                if (sender == this.wipingListMenuFindError)
                {
                    int offset = 0;
                    int repeats = this.wipingList.Items.Count;

                    if (this.wipingList.SelectedIndices.Count > 0)
                    {
                        offset = this.wipingList.SelectedIndices[0];
                    }
                    else if (this.wipingList.FocusedItem != null)
                    {
                        offset = this.wipingList.FocusedItem.Index;
                        this.wipingList.FocusedItem.Selected = true;
                    }

                    for (int index = offset; repeats > 0; index++, repeats--)
                    {
                        // Keep in mind accessing the list-view-items in this way might cause 
                        // an exception! But only if at least one non-wiping-list-item is in 
                        // the list, which should never be the case.
                        WipingListItem current = this.wipingList.Items[index] as WipingListItem;

                        if ((index != offset) &&
                            (current.State == WipingItemStates.Failed ||
                            current.State == WipingItemStates.Missing ||
                            current.State == WipingItemStates.Unknown))
                        {
                            this.wipingList.SelectedItems.Clear();
                            current.Selected = true;
                            current.Focused = true;
                            current.EnsureVisible();
                            break;
                        }

                        if (index + 1 >= this.wipingList.Items.Count)
                        {
                            index = -1;
                        }
                    }
                }
                else if (sender == this.wipingListMenuRemoveAll)
                {
                    this.RemoveListViewItems(WipingItemStates.Other);
                }
                else if (sender == this.wipingListMenuRemoveFinished)
                {
                    this.RemoveListViewItems(WipingItemStates.Finished);
                }
                else if (sender == this.wipingListMenuRemoveCanceled)
                {
                    this.RemoveListViewItems(WipingItemStates.Canceled);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("MainForm.Wiping", exception);
            }
        }

        private void RemoveListViewItems(WipingItemStates state)
        {
            Cursor cursor = this.Cursor;
            bool pausing = this.IsPausing;
            try
            {
                this.Cursor = Cursors.WaitCursor;
                this.DisableWhileConfigureWipings();

                for (int index = 0; index < this.wipingList.Items.Count; index++)
                {
                    // Keep in mind accessing the list-view-items in this way might cause 
                    // an exception! But only if at least one non-wiping-list-item is in 
                    // the list, which should never be the case.
                    WipingListItem current = this.wipingList.Items[index] as WipingListItem;

                    if (state == WipingItemStates.Other)
                    {
                        // Safety first, try to kill all possibly running wipings.
                        if (current.State == WipingItemStates.Pausing ||
                            current.State == WipingItemStates.Pending ||
                            current.State == WipingItemStates.Processing)
                        {
                            this.CancelWiping(current);
                        }
                        current.Remove();
                        index--;
                    }
                    else if (state == WipingItemStates.Finished && current.State == WipingItemStates.Finished)
                    {
                        current.Remove();
                        // It could be possible that one currently running 
                        // wiping has finished in the meantime. Therefore, 
                        // start from the beginning.
                        index = -1;
                    }
                    else if (state == WipingItemStates.Canceled && current.State == WipingItemStates.Canceled)
                    {
                        current.Remove();
                        index--;
                    }
                }
            }
            finally
            {
                this.EnableAfterConfigureWipings(pausing);
                this.Cursor = cursor;
            }
        }

        #endregion // Private context menu function implementation.

        #region Private member function implementation.

        private void AppendWiping(string fullpath)
        {
            this.AppendWipings(new string[] { fullpath });
        }

        private void AppendWipings(string[] fullpaths)
        {
            Program.TraceLogger.Write("MainForm.Wiping", ">>> AppendWipings()");

            Cursor cursor = this.Cursor;
            bool pausing = this.IsPausing;
            try
            {
                this.Cursor = Cursors.WaitCursor;
                this.DisableWhileConfigureWipings();

                bool added = false;
                foreach (WipingListItem wiping in this.wipingList.Append(fullpaths))
                {
                    wiping.Worker.DoWork += new DoWorkEventHandler(this.OnWipingDoWork);
                    wiping.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.OnWipingCompleted);
                    wiping.Worker.WorkerContinued += new EventHandler<EventArgs>(this.OnWipingContinued);
                    wiping.Worker.WorkerSuspended += new EventHandler<EventArgs>(this.OnWipingSuspended);
                    this.pendingWipings.Add(wiping);

                    added = true; // Could add at least one wiping item.
                }

                if (added)
                {
                    // Save all given base folders as pending for wiping.
                    foreach (string current in fullpaths)
                    {
                        string fullpath = current.Replace("\"", "");

                        Program.TraceLogger.Write("MainForm.Wiping", "--- AppendWipings() Try add pending base folder [" + fullpath + "]");

                        if (Directory.Exists(fullpath) && (File.GetAttributes(fullpath) & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            Program.TraceLogger.Write("MainForm.Wiping", "--- AppendWipings() Pending base folder added [" + fullpath + "]");

                            if (!this.pendingBaseFolders.Contains(fullpath))
                            {
                                this.pendingBaseFolders.Add(fullpath);
                            }
                        }
                    }
                }
            }
            finally
            {
                this.IsCanceled = false;

                this.EnableAfterConfigureWipings(pausing);

                // Try to start new or other pending wipings. 
                this.TryStartWipings();

                this.Cursor = cursor;

                Program.TraceLogger.Write("MainForm.Wiping", "<<< AppendWipings()");
            }
        }

        private void TryStartWipings()
        {
            try
            {
                if (!this.IsPausing && !this.IsCanceling)
                {
                    int maximum = this.Settings.Processing.AllowParallel ? this.Settings.Processing.ThreadCount : 1;

                    while (this.activeWipings.Count < maximum && this.pendingWipings.Count > 0)
                    {
                        WipingListItem wiping = this.pendingWipings[0];

                        wiping.Worker.RunWorkerAsync(wiping);

                        this.activeWipings.Add(wiping);
                        this.pendingWipings.RemoveAt(0);
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("MainForm.Wiping", exception);
            }
        }

        private void SuspendWipings()
        {
            this.IsPausing = true;
            this.UpdateState();
        }

        private void ContinueWipings()
        {
            this.IsPausing = false;
            this.TryStartWipings();
            this.UpdateState();
        }

        private void CancelWiping(WipingListItem current)
        {
            if (current != null)
            {
                if (current.Worker.IsBusy)
                {
                    // See comment in the else part!
                    Debug.Assert(current.Worker.WorkerSupportsCancellation);

                    if (current.Worker.WorkerSupportsCancellation)
                    {
                        // Cancel asynchronous operation if possible.
                        // State is set in the cancellation handler.
                        current.Worker.CancelAsync();
                    }
                    else
                    {
                        // Be aware, killing a background worker thread using method Abort() 
                        // is a very bad idea because background worker threads using the 
                        // thread-pool and such threads will be recycled by the system. But 
                        // this shouldn't be a problem as long as no one sets Worker Supports 
                        // Cancelation property to false! See section "Background Worker" in
                        // http://www.slideshare.net/gohsiauken/threading-in-c
                        current.Worker.Kill();
                        current.State = WipingItemStates.Canceled;
                    }
                }
                else if (current.State == WipingItemStates.Pending)
                {
                    current.State = WipingItemStates.Canceled;
                }
                current.FinishProgress();
            }
        }

        private void CancelWipings()
        {
            Cursor cursor = this.Cursor;
            try
            {
                this.Cursor = Cursors.WaitCursor;
                this.IsCanceling = true;
                this.UpdateState();

                // Firstly, get a copy of both lists.
                List<WipingListItem> affected = new List<WipingListItem>();
                affected.AddRange(this.activeWipings);
                affected.AddRange(this.pendingWipings);

                // Clear all active and pending items.
                this.activeWipings.Clear();
                this.pendingWipings.Clear();
                this.pendingBaseFolders.Clear();

                // Perform cancellation of all affected wiping items.
                foreach (WipingListItem current in affected)
                {
                    this.CancelWiping(current);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("MainForm.Wiping", exception);
            }
            finally
            {
                this.IsCanceled = true;
                this.IsCanceling = false;
                this.UpdateState();
                this.Cursor = cursor;
            }
        }

        #endregion // Private member function implementation.

        #region Private wiping handler implementation.

        private void OnWipingDoWork(object sender, DoWorkEventArgs args)
        {
            Program.TraceLogger.Write("MainForm.Wiping", ">>> OnWipingDoWork()");

            Dictionary<object, object> fileData = new Dictionary<object, object>();
            WipingThread worker = sender as WipingThread;
            WipingListItem wiping = worker.Argument as WipingListItem;
            WipingItemStates result = wiping.State;

            try
            {
                // Execute pending wipings only.
                if (wiping.FileInfo.Exists && wiping.State == WipingItemStates.Pending)
                {
                    Program.TraceLogger.Write("MainForm.Wiping", "--- OnWipingDoWork(): Wipe content of file [" + wiping.FileInfo.FullName + "]");

                    // Clone chosen wiping method to avoid changes made via settings dialog.
                    WipingAlgorithm algorithm = this.Settings.Algorithms.Selected.Clone() as WipingAlgorithm;

                    // Update progress bar values.
                    wiping.UpdateProgress(algorithm.Repeats, WipingAlgorithm.DefaultBufferSize);

                    // Ensure visibility of the progress bar. 
                    wiping.ShowProgress(WipingItemStates.Processing);
#if SIMULATION
                    Program.TraceLogger.Write("SIMULATION", "OnWipingDoWork(): Writing file content is just simulated!");
#endif // SIMULATION
                    // Try to remove read only flag if it is set.
                    if (wiping.FileInfo.IsReadOnly) { wiping.FileInfo.Attributes &= (~FileAttributes.ReadOnly); }

                    try
                    {
                        // Now collect some important information for use inside 
                        // the exception handling. Doing this beforehand is really 
                        // important because of current file info becomes invalid 
                        // during the execution below!

                        // Provide current file attributes as additional data.
                        fileData.Add("Attributes", wiping.FileInfo.Attributes);

                        // Provide current file access rights as additional 
                        // data (might be more important for analysis).
                        AuthorizationRuleCollection collection =
                            wiping.FileInfo.GetAccessControl().GetAccessRules(true, true, typeof(NTAccount));

                        foreach (FileSystemAccessRule current in collection)
                        {
                            string key = current.IdentityReference.Value;
                            string val = (current.IsInherited ? "Inherited: " : "Explicit: ") + current.FileSystemRights;

                            object tmp;
                            if (fileData.TryGetValue(key, out tmp))
                            {
                                fileData[key] = (tmp as string) + "; " + val;
                            }
                            else
                            {
                                fileData.Add(key, val);
                            }
                        }
                    }
                    catch (Exception fatal)
                    {
                        Debug.WriteLine(fatal);
                        Program.FatalLogger.Write("MainForm.Wiping", fatal);
                    }

                    // Now, ready to start the actual doing.
                    using (FileStream stream = File.Open(wiping.FileInfo.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        // Execute every single repeat.
                        for (int index = 0; index < algorithm.Repeats && !args.Cancel; index++)
                        {
                            // Wipe file content package-wise.
                            while (stream.Position < stream.Length && !args.Cancel)
                            {
                                // Stop execution on cancel request.
                                if (worker.CancellationPending)
                                {
                                    args.Cancel = true;
                                    continue;
                                }
                                else if (this.IsPausing)
                                {
                                    // Be aware, clicking pausing or continuing button will not be  
                                    // reflected in any case. This means e.g. a thread is starting  
                                    // anyway. In such a case a thread switch took place. Therefore, 
                                    // catch just the global states to ensure pausing or continuing.

                                    worker.ReportSuspended();

                                    // Wait until continuing.
                                    while (this.IsPausing && !worker.CancellationPending)
                                    {
                                        Thread.Sleep(100);
                                    }

                                    // Stop execution on cancel request.
                                    if (worker.CancellationPending)
                                    {
                                        args.Cancel = true;
                                        continue;
                                    }

                                    worker.ReportContinued();
                                }

                                // Be aware, slowing down current thread is very important! In case 
                                // of the main window is invisible slowing down the current thread a 
                                // little bit helps to reduce the overall CPU usage. In case of the 
                                // main window is visible it is necessary to slow down the current 
                                // thread a bit more than just a little because otherwise resizing 
                                // (which means redrawing) is really slow.
                                if (this.Settings.Behaviour.UseFullResources)
                                {
                                    // Just allow thread switching.
                                    Thread.Sleep(0);
                                }
                                else if (!this.Visible)
                                {
                                    // Slow down a bit when running in background.
                                    Thread.Sleep(1);
                                }
                                else
                                {
                                    // Slow down a bit more when running in foreground.
                                    Thread.Sleep(25);
                                }

                                // Do the job...
                                algorithm.WipeContent(stream);

                                // Handle current progress.
                                wiping.HandleProgress();
                            }

                            // File end has been reached, start 
                            // from beginning for next repeat.
                            stream.Position = 0;
                        }
                    }

                    if (!args.Cancel)
                    {
                        Program.TraceLogger.Write("MainForm.Wiping", "--- OnWipingDoWork(): Wipe name of file [" + wiping.FileInfo.FullName + "]");

                        // Force finalization of deleting this file and 
                        // don't pause and don't cancel this operation! 

                        // Wipe current file name and repeat it as often as necessary.
                        string filename = wiping.FileInfo.FullName;
                        for (int index = 0; index < algorithm.Repeats; index++)
                        {
                            filename = WipingAlgorithm.WipeEntry(filename);
                        }
#if SIMULATION
                        Program.TraceLogger.Write("SIMULATION", "OnWipingDoWork(): Deleting file is just simulated!");
#else
                        File.Delete(filename);
#endif // SIMULATION
                        // Now the job is done. Set result state.
                        result = WipingItemStates.Finished;
                    }
                }
            }
            catch (Exception exception)
            {
                // Add all previously collected additional information to this exception.
                foreach (object key in fileData.Keys)
                {
                    try
                    {
                        exception.Data.Add(key, fileData[key]);
                    }
                    catch (Exception fatal)
                    {
                        // What else can be done?
                        Debug.WriteLine(fatal);
                        Program.FatalLogger.Write("MainForm.Wiping", fatal);
                    }
                }

                // Keep in mind the handling of exceptions is dispensed by intention! This is  
                // because of unhandled exceptions are processed within the completed handler!
                throw exception;
            }
            finally
            {
                Program.TraceLogger.Write("MainForm.Wiping", "<<< OnWipingDoWork() State: " + result);
            }

            // Set thread result and exit.
            args.Result = result;
        }

        private void OnWipingCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            Program.TraceLogger.Write("MainForm.Wiping", ">>> OnWipingCompleted()");

            // The whole handling is really a little bit fragile but 
            // apparently is there no other and more robust way.

            try
            {
                // Safety check to ensure nothing is wrong.
                Debug.Assert(sender is WipingThread);

                WipingListItem wiping = (sender as WipingThread).Argument as WipingListItem;
                if (wiping != null)
                {
                    // Be aware and don't change the current item's state directly 
                    // because it raises an event!
                    WipingItemStates resultState = WipingItemStates.Unknown;
                    if (args.Cancelled == true)
                    {
                        resultState = WipingItemStates.Canceled;
                    }
                    else if (args.Error != null)
                    {
                        resultState = WipingItemStates.Failed;
                        wiping.Exception = args.Error;
                        this.failedWipings.Add(wiping);
                    }
                    else
                    {
                        // Do never access args.Result before all other cases have been 
                        // evaluated, because in case of args.Error is not null (exception 
                        // inside DoWork() callback) args.Result is really invalid!

                        // Safety check to ensure nothing is wrong.
                        Debug.Assert(args.Result is WipingItemStates);
                        resultState = (WipingItemStates)args.Result;
                    }

                    Program.TraceLogger.Write("MainForm.Wiping", "--- OnWipingCompleted() State: " + resultState);

                    // Remove current wiping item from the list of 
                    // active wipings before doing something else!
                    this.activeWipings.Remove(wiping);

                    // Finish belonging progress as first...
                    wiping.FinishProgress(resultState);
                    // ...and hide its progress bar afterwards.
                    wiping.HideProgress(resultState);

                    // Now try to start other pending wipings. 
                    this.TryStartWipings();
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("MainForm.Wiping", exception);
            }
            finally
            {
                this.UpdateState();

                Program.TraceLogger.Write("MainForm.Wiping", "<<< OnWipingCompleted()");
            }
        }

        private void OnWipingSuspended(object sender, EventArgs args)
        {
            WipingThread worker = sender as WipingThread;
            if (worker != null)
            {
                WipingListItem wiping = worker.Argument as WipingListItem;
                if (wiping != null)
                {
                    wiping.HideProgress(WipingItemStates.Pausing);
                }
            }
        }

        private void OnWipingContinued(object sender, EventArgs args)
        {
            WipingThread worker = sender as WipingThread;
            if (worker != null)
            {
                WipingListItem wiping = worker.Argument as WipingListItem;
                if (wiping != null)
                {
                    wiping.ShowProgress(WipingItemStates.Processing);
                }
            }
        }

        private void CleanPendingFolders()
        {
            Cursor cursor = this.Cursor;
            try
            {
                if (this.Settings.Behaviour.IncludeFolderNames && this.pendingBaseFolders.Count > 0)
                {
                    this.Cursor = Cursors.WaitCursor;

                    this.UpdateStatusbar("Cleanup");

                    foreach (string current in this.pendingBaseFolders)
                    {
                        // TODO: Use a variable number of wiping of folder names.
                        this.CleanPendingFolder(current, 1);
                    }
                }
            }
            catch (Exception exception)
            {
                // What should be done with this exception?
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("MainForm.Wiping", exception);
            }
            finally
            {
                this.Cursor = cursor;
                this.UpdateStatusbar(String.Empty);
            }
        }

        private void CleanPendingFolder(string folder, int repeats)
        {
            Program.TraceLogger.Write("MainForm.Wiping", ">>> CleanPendingFolder()");

            // How this method works. Basically, this method recursively tries 
            // to wipe and remove every sub-folder that exists under the given 
            // base-folder. But if a sub-folder is not empty (this may happen if 
            // a file couldn't be deleted) then this particular folder is kept! 
            // This makes indeed sense because files that couldn't be wiped could 
            // be possibly wiped in admin-mode. If admin-mode wiping fails too 
            // then this folder should always be kept.

            if (Directory.Exists(folder))
            {
                foreach (string children in Directory.GetDirectories(folder))
                {
                    this.CleanPendingFolder(children, repeats);
                }

                if (Directory.GetFileSystemEntries(folder).Length == 0)
                {
                    Program.TraceLogger.Write("MainForm.Wiping", "--- CleanPendingFolder(): Wipe and delete pending folder [" + folder + "]");

                    // Wipe current folder name and repeat it as often as necessary.
                    for (int index = 0; index < repeats; index++)
                    {
                        folder = WipingAlgorithm.WipeEntry(folder);
                    }
#if SIMULATION
                    Program.TraceLogger.Write("SIMULATION", "CleanPendingFolder(): Removing folder is just simulated!");
#else
                    Directory.Delete(folder);
#endif // SIMULATION
                }
            }
            Program.TraceLogger.Write("MainForm.Wiping", "<<< CleanPendingFolder()");
        }

        #endregion // Private wiping handler implementation.

        #region Private Drag & Drop handler implementation.

        private void OnWipingListDragEnter(object sender, DragEventArgs args)
        {
            try
            {
                if (args.Data.GetDataPresent(DataFormats.FileDrop, false))
                {
                    string[] candidates = args.Data.GetData(DataFormats.FileDrop) as string[];
                    if (candidates != null)
                    {
                        bool candrop = true;
                        foreach (string current in candidates)
                        {
                            // Check if current candidate is a folder. Files don't need 
                            // to be checked because they are allowed.
                            FileInfo info = new FileInfo(current);
                            if ((info.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                            {
                                // Getting the parent's directory name returns null if 
                                // current path is a root path (such as a drive) and 
                                // dropping of complete drives is not supported.
                                candrop &= Path.IsPathRooted(current) &&
                                    !String.IsNullOrEmpty(Path.GetDirectoryName(current));
                            }
                        }

                        // Adjust given drop effects accordingly.
                        args.Effect = candrop ? DragDropEffects.All : DragDropEffects.None;

                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("MainForm.Wiping", exception);
            }
        }

        private void OnWipingListDragDrop(object sender, DragEventArgs args)
        {
            try
            {
                // Ensure the main window at the top.
                if (!this.Focused) { this.ForceShowForm(); }

                if (args.Data.GetDataPresent(DataFormats.FileDrop, false))
                {
                    string[] filepaths = args.Data.GetData(DataFormats.FileDrop) as string[];
                    if (filepaths != null)
                    {
#if !DEBUG
                        if (Program.ConfirmDestroyItems(filepaths, this))
#endif // !DEBUG
                        {
                            this.AppendWipings(filepaths);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("MainForm.Wiping", exception);
            }
            finally
            {
                args.Effect = DragDropEffects.None;
            }
        }

        #endregion // Private Drag & Drop handler implementation.
    }
}
