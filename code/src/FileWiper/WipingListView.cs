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

using Plexdata.Controls;
using Plexdata.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Plexdata.FileWiper
{
    // This class has been inspired by the sample that can be found under link below.
    // Source: http://www.codeproject.com/Articles/9188/Embedding-Controls-in-a-ListView
    //
    // Consider usage of this undocumented feature.
    // http://www.codeproject.com/Articles/35197/Undocumented-List-View-Features
    //
    // Consider usage of autosizing last column to fit to maximum list width.
    // http://www.codeproject.com/Articles/3239/Autosize-the-last-column-in-a-ListView-control-usi
    //
    // This derived listview implementation has some bugs! When full-row-select is disabled 
    // and an associated control resides in the "first" column and the control's size is less 
    // then list item bounds then only the underlying text is selected. And not as expected 
    // the whole control.
    internal class WipingListView : ListView
    {
        private class InnerControl
        {
            public InnerControl()
                : base()
            {
                this.Control = null;
                this.Column = -1;
                this.Index = -1;
                this.Dock = DockStyle.None;
                this.Item = null;
            }

            public Control Control { get; set; }
            public Int32 Column { get; set; }
            public Int32 Index { get; set; }
            public DockStyle Dock { get; set; }
            public ListViewItem Item { get; set; }
        }

        private readonly List<InnerControl> listViewControls = new List<InnerControl>();

        private readonly IContainer components = null;

        public WipingListView()
            : base()
        {
            this.components = new Container();

            this.SmallImageList = new ImageList(this.components)
            {
                ImageSize = new Size(16, 16),
                ColorDepth = ColorDepth.Depth32Bit
            };

            this.LargeImageList = new ImageList(this.components)
            {
                ImageSize = new Size(32, 32),
                ColorDepth = ColorDepth.Depth32Bit
            };

            this.View = View.Details;
            this.FullRowSelect = true;
            this.HideSelection = true;
            this.HeaderStyle = ColumnHeaderStyle.Nonclickable;

            this.DoubleBuferring = true;

            ColumnHeader header;
            header = new ColumnHeader()
            {
                Name = WipingListKeys.FILENAME,
                Text = WipingListKeys.FILENAME,
                Width = 200,
                TextAlign = HorizontalAlignment.Left
            };
            header.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            this.Columns.Add(header);

            header = new ColumnHeader()
            {
                Name = WipingListKeys.PROGRESS,
                Text = WipingListKeys.PROGRESS,
                Width = 80,
                TextAlign = HorizontalAlignment.Center
            };
            header.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.Columns.Add(header);

            header = new ColumnHeader()
            {
                Name = WipingListKeys.FILESIZE,
                Text = WipingListKeys.FILESIZE,
                Width = 70,
                TextAlign = HorizontalAlignment.Right
            };
            header.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            this.Columns.Add(header);

            header = new ColumnHeader()
            {
                Name = WipingListKeys.FILEDATE,
                Text = WipingListKeys.FILEDATE,
                Width = 130,
                TextAlign = HorizontalAlignment.Left
            };
            header.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            this.Columns.Add(header);

            header = new ColumnHeader()
            {
                Name = WipingListKeys.FILEPATH,
                Text = WipingListKeys.FILEPATH,
                Width = 250,
                TextAlign = HorizontalAlignment.Left
            };
            header.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            this.Columns.Add(header);
        }

        #region Public property section.

        [DefaultValue(View.Details)]
        public new View View
        {
            get
            {
                return base.View;
            }
            set
            {
                // Embedded controls will be rendered only in Details mode.
                foreach (InnerControl current in this.listViewControls)
                {
                    current.Control.Visible = (value == View.Details);
                }
                base.View = value;
            }
        }

        [DefaultValue(true)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Boolean DoubleBuferring
        {
            get
            {
                PropertyInfo property = typeof(ListView).GetProperty(
                    "DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
                return (Boolean)property.GetValue(this, null);
            }
            set
            {
                PropertyInfo property = typeof(ListView).GetProperty(
                    "DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
                property.SetValue(this, value, null);
            }
        }

        #endregion // Public property section.

        #region Internal property section.

        private readonly WipingStateCounts wipingStateCounts = new WipingStateCounts();
        internal WipingStateCounts StateCounts { get { return this.wipingStateCounts; } }

        private readonly WipingOverallCounts wipingOverallCounts = new WipingOverallCounts();
        internal WipingOverallCounts OverallCounts { get { return this.wipingOverallCounts; } }

        #endregion // Internal property section.

        #region Internal method section.

        internal List<WipingListItem> Append(String[] pathlist)
        {
            try
            {
                // Don't use BeginUpdate() and EndUpdate() because all items are added at once.
                List<WipingListItem> result = new List<WipingListItem>();
                List<String> filepaths = new List<String>();
                this.ResolveFilepaths(pathlist, filepaths);

                Double totalFileSize = 0;

                // The list of resolved path names may contain duplicates! 
                // Therefore, it is necessary to remove them. Using Linq 
                // for this purpose is quite easy.
                foreach (String filepath in filepaths.Distinct().ToList())
                {
                    if (this.CanAppend(filepath))
                    {
                        WipingListItem wiping = new WipingListItem(filepath);

                        // Add new type-icon to the image list.
                        if (!this.SmallImageList.Images.ContainsKey(wiping.ImageKey))
                        {
                            this.SmallImageList.Images.Add(wiping.ImageKey, wiping.SmallIcon);
                            this.LargeImageList.Images.Add(wiping.ImageKey, wiping.LargeIcon);
                        }

                        this.StateCounts.Attach(wiping.State);

                        // Add current file size to sum of file sizes.
                        totalFileSize += wiping.FileSize;

                        result.Add(wiping);
                    }
                }

                this.Items.AddRange(result.ToArray());

                this.OverallCounts.AddTotalFileSize(totalFileSize);

                return result;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("WipingListView", exception);
                return new List<WipingListItem>();
            }
        }

        internal Boolean ShowProgress(ListViewItem item, WipingItemStates state)
        {
            // Don't use BeginUpdate()/EndUpdate() because it 
            // cause massive list flickering under Windows XP.
            try
            {
                if (item is WipingListItem current)
                {
                    Int32 column = this.Columns[WipingListKeys.PROGRESS].Index;
                    if (this.GetControl(column, item.Index) == null)
                    {
                        current.State = state;
                        current.Progress.Visible = (this.View == View.Details);
                        this.AddControl(current.Progress, column, item.Index);
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("WipingListView", exception);
                return false;
            }
            finally
            {
                Application.DoEvents();
            }
        }

        internal void HideProgress(ListViewItem item, WipingItemStates state)
        {
            // Don't use BeginUpdate()/EndUpdate() because it 
            // cause massive list flickering under Windows XP.
            try
            {
                if (item is WipingListItem current)
                {
                    current.State = state;
                    current.Progress.Visible = false;
                    this.DelControl(current.Progress);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("WipingListView", exception);
            }
            finally
            {
                Application.DoEvents();
            }
        }

        #endregion // Internal method section.

        #region Protected method section.

        protected override void Dispose(Boolean disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion // Protected method section.

        #region Protected event handler section.

        protected override void WndProc(ref Message message)
        {
            // Windows Messages
            const Int32 WM_PAINT = 0x000F;

            if (message.Msg == WM_PAINT && this.View == View.Details)
            {
                // Calculate the position of all embedded controls
                foreach (InnerControl current in this.listViewControls)
                {
                    Rectangle bounds = this.GetSubItemBounds(current.Item, current.Column);

                    if ((this.HeaderStyle != ColumnHeaderStyle.None) &&
                        (bounds.Top < this.Font.Height)) // Control overlaps ColumnHeader
                    {
                        current.Control.Visible = false;
                        continue;
                    }
                    else
                    {
                        current.Control.Visible = true;
                    }

                    switch (current.Dock)
                    {
                        case DockStyle.Fill:
                            break;
                        case DockStyle.Top:
                            bounds.Height = current.Control.Height;
                            break;
                        case DockStyle.Left:
                            bounds.Width = current.Control.Width;
                            break;
                        case DockStyle.Bottom:
                            bounds.Offset(0, bounds.Height - current.Control.Height);
                            bounds.Height = current.Control.Height;
                            break;
                        case DockStyle.Right:
                            bounds.Offset(bounds.Width - current.Control.Width, 0);
                            bounds.Width = current.Control.Width;
                            break;
                        case DockStyle.None:
                            bounds.Size = current.Control.Size;
                            break;
                    }

                    // Set embedded control's bounds and 
                    // take care about the focus rectangle.
                    current.Control.Bounds = this.GetInflatedControlBounds(bounds);
                }
            }
            base.WndProc(ref message);
        }

        protected override void OnResize(EventArgs args)
        {
            this.Invalidate(true);
            base.OnResize(args);
        }

        protected override void OnColumnReordered(ColumnReorderedEventArgs args)
        {
            this.Invalidate(true);
            base.OnColumnReordered(args);
        }

        protected override void OnMouseWheel(MouseEventArgs args)
        {
            if (!this.Focused) { this.Focus(); }

            base.OnMouseWheel(args);
        }

        #endregion // Protected event handler section.

        #region Private method section.

        private void ResolveFilepaths(String basepath, List<String> result)
        {
            try
            {
                if (!String.IsNullOrEmpty(basepath))
                {
                    // Remove quotation marks if exist.
                    basepath = basepath.Replace("\"", "");

                    // Check if given path is a directory.
                    if (Directory.Exists(basepath))
                    {
                        // Add all available files to the resulting list. 
                        // Duplicates are filtered by the caller. Therefore, 
                        // no need to do this twice.
                        result.AddRange(Directory.GetFiles(basepath));

                        // Try to add current folder list to resulting list.
                        this.ResolveFilepaths(Directory.GetDirectories(basepath), result);
                    }
                    else
                    {
                        // Assume current base path is a file, no matter if 
                        // it really exists. The file existence is checked 
                        // somewhere else!
                        result.Add(basepath);
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("WipingListView", exception);
            }
        }

        private void ResolveFilepaths(String[] basepaths, List<String> result)
        {
            if (basepaths != null && result != null)
            {
                foreach (String basepath in basepaths)
                {
                    // Resolve all children for current path.
                    this.ResolveFilepaths(basepath, result);
                }
            }
        }

        private Boolean CanAppend(String fullpath)
        {
            if (fullpath != null)
            {
                // Keep in mind accessing the list-view-items in this way might cause 
                // an exception! But only if at least one non-wiping-list-item is in 
                // the list, which should never be the case.
                foreach (WipingListItem current in this.Items)
                {
                    // Check if given file is in the list already.
                    if (String.Compare(current.FileInfo.FullName, fullpath, true) == 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private Control GetControl(Int32 column, Int32 index)
        {
            foreach (InnerControl embedded in this.listViewControls)
            {
                if (embedded.Index == index && embedded.Column == column)
                {
                    return embedded.Control;
                }
            }
            return null;
        }

        private void AddControl(Control control, Int32 column, Int32 index)
        {
            this.AddControl(control, column, index, DockStyle.Fill);
        }

        private void AddControl(Control control, Int32 column, Int32 index, DockStyle dock)
        {
            if (control != null && column < this.Columns.Count && index < this.Items.Count)
            {
                InnerControl embedded = new InnerControl()
                {
                    Control = control,
                    Column = column,
                    Index = index,
                    Dock = dock,
                    Item = this.Items[index]
                };

                embedded.Control.Bounds =
                    this.GetInflatedControlBounds(this.GetSubItemBounds(embedded.Item, embedded.Column));

                this.listViewControls.Add(embedded);

                this.Controls.Add(control);
            }
        }

        private void DelControl(Control control)
        {
            if (control != null)
            {
                for (Int32 index = 0; index < this.listViewControls.Count; index++)
                {
                    InnerControl embedded = (InnerControl)this.listViewControls[index];
                    if (embedded.Control == control)
                    {
                        this.Controls.Remove(control);
                        this.listViewControls.RemoveAt(index);
                        return;
                    }
                }
            }
        }

        private Int32[] GetColumnOrder()
        {
            // ListView messages
            const Int32 LVM_FIRST = 0x1000;
            const Int32 LVM_GETCOLUMNORDERARRAY = (LVM_FIRST + 59);

            Int32[] result = null;
            IntPtr lParam = IntPtr.Zero;

            try
            {
                lParam = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Int32)) * this.Columns.Count);

                IntPtr lResult = SendMessage(
                    this.Handle, LVM_GETCOLUMNORDERARRAY,
                    new IntPtr(this.Columns.Count), lParam);

                if (lResult.ToInt32() != 0)
                {
                    result = new Int32[this.Columns.Count];
                    Marshal.Copy(lParam, result, 0, this.Columns.Count);
                }
            }
            finally
            {
                if (lParam != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(lParam);
                }
            }
            return result;
        }

        private Rectangle GetSubItemBounds(ListViewItem item, Int32 subItem)
        {
            if (item == null) { throw new ArgumentNullException("item"); }

            Int32[] order = this.GetColumnOrder();
            if (order == null) { return Rectangle.Empty; }

            if (subItem >= order.Length)
            {
                throw new IndexOutOfRangeException("Subitem index" + subItem + " out of range");
            }

            // Retrieve the bounds of the entire ListViewItem (all subitems)
            Rectangle bounds = item.GetBounds(ItemBoundsPortion.Entire);

            // Calculate the X position of the SubItem.
            ColumnHeader columnHeader;
            for (Int32 index = 0; index < order.Length; index++)
            {
                columnHeader = this.Columns[order[index]];
                if (columnHeader.Index == subItem)
                {
                    // Use Columns[order[i]] instead of Columns[i]
                    // because the columns could be reordered.
                    bounds.Width = this.Columns[order[index]].Width;
                    break;
                }
                bounds.X += columnHeader.Width;
            }
            return bounds;
        }

        private Rectangle GetInflatedControlBounds(Rectangle bounds)
        {
            return Rectangle.Inflate(bounds, -1, -2);
        }

        #endregion // Private method section.

        #region Win32 API function declarations.

        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true, PreserveSig = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, Int32 message, IntPtr wParam, IntPtr lParam);

        #endregion // Win32 API function declarations.
    }

    internal static class WipingListKeys
    {
        public static String FILENAME = "Filename";
        public static String PROGRESS = "Progress";
        public static String FILESIZE = "Filesize";
        public static String FILEDATE = "Filedate";
        public static String FILEPATH = "Filepath";
    }

    internal class WipingStateCounts
    {
        public event EventHandler<EventArgs> ValuesChanged;

        public event EventHandler<EventArgs> InactivityIndicated;

        private readonly Object critical = new Object();

        public WipingStateCounts()
            : base()
        {
            this.Reset();
        }

        public Int32 Failed { get; private set; }

        public Int32 Missing { get; private set; }

        public Int32 Unknown { get; private set; }

        public Int32 Pending { get; private set; }

        public Int32 Pausing { get; private set; }

        public Int32 Processing { get; private set; }

        public Int32 Finished { get; private set; }

        public Int32 Canceled { get; private set; }

        public void Attach(WipingItemStates state)
        {
            lock (this.critical)
            {
                switch (state)
                {
                    case WipingItemStates.Failed:
                        this.Failed++;
                        break;
                    case WipingItemStates.Missing:
                        this.Missing++;
                        break;
                    case WipingItemStates.Unknown:
                        this.Unknown++;
                        break;
                    case WipingItemStates.Pending:
                        this.Pending++;
                        break;
                    case WipingItemStates.Pausing:
                        this.Pausing++;
                        break;
                    case WipingItemStates.Processing:
                        this.Processing++;
                        break;
                    case WipingItemStates.Finished:
                        this.Finished++;
                        break;
                    case WipingItemStates.Canceled:
                        this.Canceled++;
                        break;
                    default:
                        // Just to inform if handling of newly 
                        // added states has been forgotten.
                        Debug.Assert(false);
                        return;
                }
            }
            this.RaiseValuesChanged();
        }

        public void Change(WipingItemStates oldState, WipingItemStates newState)
        {
            if (oldState != newState)
            {
                lock (this.critical)
                {
                    // Copy of member Detach(); done by intention 
                    // to prevent event-double-sending!
                    switch (oldState)
                    {
                        case WipingItemStates.Failed:
                            this.Failed--;
                            break;
                        case WipingItemStates.Missing:
                            this.Missing--;
                            break;
                        case WipingItemStates.Unknown:
                            this.Unknown--;
                            break;
                        case WipingItemStates.Pending:
                            this.Pending--;
                            break;
                        case WipingItemStates.Pausing:
                            this.Pausing--;
                            break;
                        case WipingItemStates.Processing:
                            this.Processing--;
                            break;
                        case WipingItemStates.Finished:
                            this.Finished--;
                            break;
                        case WipingItemStates.Canceled:
                            this.Canceled--;
                            break;
                        default:
                            // Just to inform if handling of newly 
                            // added states has been forgotten.
                            Debug.Assert(false);
                            return;
                    }

                    // Copy of member Attach(); done by intention 
                    // to prevent event-double-sending!
                    switch (newState)
                    {
                        case WipingItemStates.Failed:
                            this.Failed++;
                            break;
                        case WipingItemStates.Missing:
                            this.Missing++;
                            break;
                        case WipingItemStates.Unknown:
                            this.Unknown++;
                            break;
                        case WipingItemStates.Pending:
                            this.Pending++;
                            break;
                        case WipingItemStates.Pausing:
                            this.Pausing++;
                            break;
                        case WipingItemStates.Processing:
                            this.Processing++;
                            break;
                        case WipingItemStates.Finished:
                            this.Finished++;
                            break;
                        case WipingItemStates.Canceled:
                            this.Canceled++;
                            break;
                        default:
                            // Just to inform if handling of newly 
                            // added states has been forgotten.
                            Debug.Assert(false);
                            return;
                    }
                }
                this.RaiseValuesChanged();
            }
        }

        public void Detach(WipingItemStates state)
        {
            lock (this.critical)
            {
                switch (state)
                {
                    case WipingItemStates.Failed:
                        this.Failed--;
                        break;
                    case WipingItemStates.Missing:
                        this.Missing--;
                        break;
                    case WipingItemStates.Unknown:
                        this.Unknown--;
                        break;
                    case WipingItemStates.Pending:
                        this.Pending--;
                        break;
                    case WipingItemStates.Pausing:
                        this.Pausing--;
                        break;
                    case WipingItemStates.Processing:
                        this.Processing--;
                        break;
                    case WipingItemStates.Finished:
                        this.Finished--;
                        break;
                    case WipingItemStates.Canceled:
                        this.Canceled--;
                        break;
                    default:
                        // Just to inform if handling of newly 
                        // added states has been forgotten.
                        Debug.Assert(false);
                        return;
                }
            }
            this.RaiseValuesChanged();
        }

        public void Reset()
        {
            this.Failed = 0;
            this.Missing = 0;
            this.Unknown = 0;
            this.Pending = 0;
            this.Pausing = 0;
            this.Processing = 0;
            this.Finished = 0;
            this.Canceled = 0;
        }

        public override String ToString()
        {
            return
                "Failed: " + this.Failed + " Missing: " + this.Missing +
                " Unknown: " + this.Unknown + " Pending: " + this.Pending +
                " Pausing: " + this.Pausing + " Processing: " + this.Processing +
                " Finished: " + this.Finished + " Canceled: " + this.Canceled;
        }

        private void RaiseValuesChanged()
        {
            this.ValuesChanged?.Invoke(this, EventArgs.Empty);

            // Be aware, indicating an inactivity 
            // really depends on current values!
            this.TryRaiseInactivityIndicated();
        }

        private void TryRaiseInactivityIndicated()
        {
            if (this.InactivityIndicated != null)
            {
                // Inactivity takes places as soon as no wiping is currently pending, 
                // or pausing, or processing.
                if (this.Pending == 0 && this.Pausing == 0 && this.Processing == 0)
                {
                    this.InactivityIndicated(this, EventArgs.Empty);
                }
            }
        }
    }

    internal class WipingOverallCounts
    {
        public event EventHandler<EventArgs> ValuesChanged;

        private readonly Object critical = new Object();

        public WipingOverallCounts()
            : base()
        {
            this.Reset();
        }

        /// <summary>
        /// Total sum of sizes of all files.
        /// </summary>
        public Double TotalFileSize { get; private set; }

        /// <summary>
        /// Total sum of wiped file sizes.
        /// </summary>
        public Double WipedFileSize { get; private set; }

        /// <summary>
        /// Total sum of finished wiping sizes.
        /// </summary>
        public Double TotalWipedSize { get; private set; }

        public void AddTotalFileSize(Double fileSize)
        {
            lock (this.critical)
            {
                this.TotalFileSize += Math.Max(fileSize, 0);
            }
            this.RaiseValuesChanged();
        }

        public void DelTotalFileSize(Double fileSize)
        {
            lock (this.critical)
            {
                this.TotalFileSize -= Math.Max(fileSize, 0);
            }
            this.RaiseValuesChanged();
        }

        public void AddWipingSizes(Double fileSize, Double wipedSize)
        {
            lock (this.critical)
            {
                this.WipedFileSize += Math.Max(fileSize, 0);
                this.TotalWipedSize += Math.Max(wipedSize, 0);
            }
            this.RaiseValuesChanged();
        }

        public void Reset()
        {
            this.TotalFileSize = 0;
            this.WipedFileSize = 0;
            this.TotalWipedSize = 0;
        }

        public override String ToString()
        {
            return
                "Total File Size: " + this.TotalFileSize.ToString("N0", NumberFormatInfo.CurrentInfo) +
                " Wiped File Size: " + this.WipedFileSize.ToString("N0", NumberFormatInfo.CurrentInfo) +
                " Total Wiped Size: " + this.TotalWipedSize.ToString("N0", NumberFormatInfo.CurrentInfo);
        }

        private void RaiseValuesChanged()
        {
            this.ValuesChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    internal enum WipingItemStates
    {
        Other = Int32.MaxValue,
        Failed = -2,
        Missing,
        Unknown,
        Pending,
        Pausing,
        Processing,
        Finished,
        Canceled,
    }

    internal class WipingListItem : ListViewItem
    {
        private readonly GuiInvokeHelper invoker = null;

        public WipingListItem()
            : base()
        {
            this.invoker = new GuiInvokeHelper();
            this.FileInfo = null;
            this.FileSize = 0;
            this.Progress = null;
            this.Worker = null;
            this.Exception = null;
        }

        public WipingListItem(String fullpath)
            : this()
        {
            String filename = "<empty>";
            String filepath = String.Empty;
            String filedate = String.Empty;
            String filesize = String.Empty;
            String imagekey = String.Empty;
            WipingItemStates state = WipingItemStates.Unknown;
            ListViewSubItem subitem = null;

            // Do this first because it could raise an exception.
            // In such a case no Worker, no Progress etc. is created.
            fullpath = fullpath.Replace("\"", "").Trim();
            this.FileInfo = new FileInfo(fullpath);

            this.Worker = new WipingThread()
            {
                WorkerSupportsCancellation = true
            };

            this.Progress = new ProgressBar3D()
            {
                Font = new Font(this.Font.FontFamily, this.Font.Size - 1.5f),
                Value = 0,
                Maximum = 0,
                ForeColorLight = Color.FromArgb(128, 128, 255),
                ForeColorDark = Color.FromArgb(192, 192, 255),
                BackColorLight = Color.FromArgb(224, 224, 224),
                BackColorDark = Color.FromArgb(255, 255, 255),
                BorderColor = Color.FromArgb(192, 192, 192),
                TextColorLight = Color.FromArgb(0, 0, 64),
                TextColorDark = Color.FromArgb(0, 0, 64),
                RedirectHitTest = true
            };

            filename = this.FileInfo.Name;
            filepath = this.FileInfo.DirectoryName;
            imagekey = (this.FileInfo.Extension.Length > 0 ? this.FileInfo.Extension : "::default::");
            filedate = "\u2014"; // Use em dash as default.
            filesize = "\u2014"; // Use em dash as default.

            if (this.FileInfo.Exists)
            {
                // Save current file size, beside the real file size.
                this.FileSize = this.FileInfo.Length;

                filedate = this.FileInfo.CreationTime.ToString();
                filesize = CapacityConverter.Convert(this.FileSize);
                this.Progress.Maximum = this.FileSize;

                state = WipingItemStates.Pending;
            }
            else
            {
                state = WipingItemStates.Missing;
            }

            // Ensure creation of the List view item no 
            // matter if some arguments are not valid!
            this.Text = filename;
            this.Name = WipingListKeys.FILENAME;
            this.ImageKey = imagekey;
            this.UseItemStyleForSubItems = false;

            subitem = new ListViewSubItem()
            {
                Text = state.ToString(),
                Name = WipingListKeys.PROGRESS
            };
            this.SubItems.Add(subitem);

            // Set status here; otherwise 
            // item text is not reflected.
            this.State = state;

            subitem = new ListViewSubItem()
            {
                Text = filesize,
                Name = WipingListKeys.FILESIZE
            };
            this.SubItems.Add(subitem);

            subitem = new ListViewSubItem()
            {
                Text = filedate,
                Name = WipingListKeys.FILEDATE
            };
            this.SubItems.Add(subitem);

            subitem = new ListViewSubItem()
            {
                Text = filepath,
                Name = WipingListKeys.FILEPATH
            };
            this.SubItems.Add(subitem);
        }

        public Icon SmallIcon { get { return this.GetFileIcon(this.FileInfo.FullName, true); } }

        public Icon LargeIcon { get { return this.GetFileIcon(this.FileInfo.FullName, false); } }

        public static Color GetStateColor(WipingItemStates state)
        {
            switch (state)
            {
                case WipingItemStates.Failed:
                case WipingItemStates.Unknown:
                case WipingItemStates.Missing:
                case WipingItemStates.Canceled:
                    return Color.DarkRed;
                case WipingItemStates.Pausing:
                    return Color.Navy;
                case WipingItemStates.Finished:
                    return Color.DimGray;
                case WipingItemStates.Pending:
                case WipingItemStates.Processing:
                default:
                    return Color.Black;
            }
        }

        private volatile WipingItemStates wipingState = WipingItemStates.Unknown;
        public WipingItemStates State
        {
            get { return this.wipingState; }
            set
            {
                if (this.wipingState != value)
                {
                    // Save current "old" state for later use.
                    WipingItemStates oldState = this.wipingState;

                    // Set new state.
                    this.wipingState = value;

                    if (this.SubItems.ContainsKey(WipingListKeys.PROGRESS))
                    {
                        ListViewSubItem subitem = this.SubItems[WipingListKeys.PROGRESS];

                        subitem.Text = this.wipingState.ToString();
                        subitem.ForeColor = WipingListItem.GetStateColor(value);
                    }

                    // Report current state changes, if possible.
                    if (this.ListView != null && this.ListView is WipingListView)
                    {
                        // Reporting current state is impossible for example 
                        // while creating this list view item; because in this 
                        // case the parent list view is not yet available!
                        (this.ListView as WipingListView).StateCounts.Change(oldState, this.wipingState);
                    }
                }
            }
        }

        public FileInfo FileInfo { get; private set; }

        // It is really necessary to decouple file size from current file info 
        // because after the file has been deleted getting the file's length 
        // throws an exception when accessing the file info.
        public Int64 FileSize { get; private set; }

        public ProgressBar3D Progress { get; private set; }

        public WipingThread Worker { get; private set; }

        public Exception Exception { get; set; }

        public void UpdateProgress(Int32 repeats, Int32 steps)
        {
            this.Progress.Value = 0;
            this.Progress.Maximum = this.FileSize * repeats;
            this.Progress.Steps = steps;
        }

        public void HandleProgress()
        {
            this.Progress.Increment();
        }

        public void HandleProgress(Double steps)
        {
            this.Progress.Increment(steps);
        }

        public void FinishProgress()
        {
            this.FinishProgress(this.State);
        }

        public void FinishProgress(WipingItemStates state)
        {
            if (this.ListView != null && this.ListView is WipingListView)
            {
                switch (state)
                {
                    // Remove values of "invalid" wipings from overall results.
                    case WipingItemStates.Failed:
                    case WipingItemStates.Unknown:
                    case WipingItemStates.Missing:
                    case WipingItemStates.Canceled:
                        (this.ListView as WipingListView).OverallCounts.DelTotalFileSize(this.FileSize);
                        break;
                    // Add values of finished wipings to overall results.
                    case WipingItemStates.Finished:
                        (this.ListView as WipingListView).OverallCounts.AddWipingSizes(this.FileSize, this.Progress.Maximum);
                        break;
                    case WipingItemStates.Pausing:
                    case WipingItemStates.Pending:
                    case WipingItemStates.Processing:
                    default:
                        // Do nothing in these cases!
                        break;
                }
            }
        }

        private delegate void ShowProgressDelegate(WipingItemStates state);
        public void ShowProgress(WipingItemStates state)
        {
            try
            {
                if (this.invoker.InvokeRequired)
                {
                    this.invoker.Invoke(new ShowProgressDelegate(this.ShowProgress), new Object[] { state });
                }
                else
                {
                    if (this.ListView != null)
                    {
                        (this.ListView as WipingListView).ShowProgress(this, state);
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("WipingListItem", exception);
            }
        }

        private delegate void HideProgressDelegate(WipingItemStates state);
        public void HideProgress(WipingItemStates state)
        {
            try
            {
                if (this.invoker.InvokeRequired)
                {
                    this.invoker.Invoke(new HideProgressDelegate(this.HideProgress), new Object[] { state });
                }
                else
                {
                    if (this.ListView != null)
                    {
                        (this.ListView as WipingListView).HideProgress(this, state);
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("WipingListItem", exception);
            }
        }

        private Icon GetFileIcon(String filename, Boolean small)
        {
            const Int32 FILE_ATTRIBUTE_NORMAL = 0x00000080;
            const Int32 SHGFI_ICON = 0x100;
            const Int32 SHGFI_LARGEICON = 0x0;
            const Int32 SHGFI_SMALLICON = 0x1;
            const Int32 SHGFI_USEFILEATTRIBUTES = 0x10;

            Icon result = null;
            SHFILEINFO info = new SHFILEINFO();
            try
            {
                Int32 size = Marshal.SizeOf(info);
                Int32 attr = FILE_ATTRIBUTE_NORMAL;
                Int32 flags = SHGFI_USEFILEATTRIBUTES | SHGFI_ICON | (small ? SHGFI_SMALLICON : SHGFI_LARGEICON);

                // Use wildcard in case given file does not exist.
                filename = "*" + Path.GetExtension(filename);

                SHGetFileInfo(filename, attr, ref info, size, flags);

                Int32 error = Marshal.GetLastWin32Error();

                result = (Icon)Icon.FromHandle(info.hIcon).Clone();

                if (result == null)
                {
                    throw new Win32Exception(error);
                }
                else if (result.Size == Size.Empty)
                {
                    if (error == 0) { error = Marshal.GetLastWin32Error(); }

                    throw new Win32Exception(error);
                }
            }
            finally
            {
                if (info.hIcon != IntPtr.Zero) { DestroyIcon(info.hIcon); }
            }

            return result;
        }

        #region Win32 API function declarations.

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr nIcon;
            public UInt32 attributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] // MAX_PATH
            public String displayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public String typeName;
        };

        [DllImport("shell32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true, PreserveSig = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr SHGetFileInfo(String fullpath, Int32 fileAttributes, ref SHFILEINFO fileInfo, Int32 sizeFileInfo, Int32 flags);

        [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true, PreserveSig = true)]
        private static extern Boolean DestroyIcon(IntPtr hIcon);

        #endregion // Win32 API function declarations.

        private class GuiInvokeHelper : ISynchronizeInvoke
        {
            // For more details about this solution see:
            // http://stackoverflow.com/questions/6708765/how-to-invoke-when-i-have-no-control-available

            private readonly SynchronizationContext context;
            private readonly Thread thread;
            private readonly Object locker;

            public GuiInvokeHelper()
                : base()
            {
                this.context = SynchronizationContext.Current;
                this.thread = Thread.CurrentThread;
                this.locker = new Object();
            }

            #region ISynchronizeInvoke member implementation section.

            public Boolean InvokeRequired
            {
                get
                {
                    return Thread.CurrentThread.ManagedThreadId != this.thread.ManagedThreadId;
                }
            }

            [Obsolete("This method is not supported!", true)]
            public IAsyncResult BeginInvoke(Delegate method, Object[] args)
            {
                throw new NotSupportedException();
            }

            [Obsolete("This method is not supported!", true)]
            public Object EndInvoke(IAsyncResult result)
            {
                throw new NotSupportedException();
            }

            public Object Invoke(Delegate method, Object[] args)
            {
                if (method == null)
                {
                    throw new ArgumentNullException("method");
                }

                lock (this.locker)
                {
                    Object result = null;

                    SendOrPostCallback invoker = new SendOrPostCallback(
                        delegate (Object data)
                        {
                            result = method.DynamicInvoke(args);
                        });

                    this.context.Send(new SendOrPostCallback(invoker), method.Target);

                    return result;
                }
            }

            public Object Invoke(Delegate method)
            {
                return this.Invoke(method, null);
            }

            #endregion // ISynchronizeInvoke member implementation section.
        }
    }
}
