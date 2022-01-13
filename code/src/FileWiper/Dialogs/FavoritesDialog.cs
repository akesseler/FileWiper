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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Plexdata.FileWiper
{
    public partial class FavoritesDialog : Form
    {
        private static readonly String DEFAULT_IMAGE_KEY = "::default::";
        private static readonly String PATH_SEPARATOR = Path.DirectorySeparatorChar.ToString();

        private String lastFolder = String.Empty;
        private Boolean canSaveData = false;

        #region Constructor section.

        public FavoritesDialog()
        {
            this.InitializeComponent();

            this.Icon = Properties.Resources.Favorites;

            this.btnAdd.Image = new Icon(Properties.Resources.ItemAdd, new Size(16, 16)).ToBitmap();
            this.btnRemove.Image = new Icon(Properties.Resources.ItemRemove, new Size(16, 16)).ToBitmap();
            this.btnExecute.Image = new Icon(Properties.Resources.Apply, new Size(16, 16)).ToBitmap();
            this.btnClose.Image = new Icon(Properties.Resources.Cancel, new Size(16, 16)).ToBitmap();

            this.cmiAdd.Image = this.btnAdd.Image;
            this.cmiRemove.Image = this.btnRemove.Image;

            try
            {
                // Get and use the folder icon.
                this.lstFavoritesList.SmallImageList = new ImageList()
                {
                    ImageSize = new Size(16, 16),
                    ColorDepth = ColorDepth.Depth32Bit
                };
                this.lstFavoritesList.SmallImageList.Images.Add(DEFAULT_IMAGE_KEY, this.GetFolderIcon(true));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("FavoritesDialog", exception);
            }

            // Damn flickering list view!
            PropertyInfo property = typeof(ListView).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            property.SetValue(this.lstFavoritesList, true, null);
        }

        #endregion // Constructor section.

        #region Overwritten event handler section.

        protected override void OnLoad(EventArgs args)
        {
            if (Settings.IsVisibleOnAllScreens(Program.MainForm.Settings.Maintain.FavoritesBounds))
            {
                // Apply last known dialog size and location if possible.
                this.DesktopBounds = Program.MainForm.Settings.Maintain.FavoritesBounds;
            }
            else
            {
                this.StartPosition = FormStartPosition.CenterParent;
            }

            foreach (String current in Program.MainForm.Settings.Favorites.Folders)
            {
                this.AddItem(current);
            }

            this.UpdateStates();

            base.OnLoad(args);
        }

        protected override void OnFormClosing(FormClosingEventArgs args)
        {
            // Save last known size and location.
            Program.MainForm.Settings.Maintain.FavoritesBounds = this.DesktopBounds;

            if (this.canSaveData)
            {
                List<String> folders = new List<String>();
                foreach (ListViewItem current in this.lstFavoritesList.Items)
                {
                    String folder = current.Tag as String;
                    Debug.Assert(folder != null);
                    folders.Add(folder);
                }

                // Save currently configured favorites.
                Program.MainForm.Settings.Favorites.Folders = folders.ToArray();
            }

            base.OnFormClosing(args);
        }

        #endregion // Overwritten event handler section.

        #region Private event handler section.

        private void OnAddButtonClick(Object sender, EventArgs args)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog()
            {
                ShowNewFolderButton = false,
                Description = "Choose a folder to be added to the list of wiping favorites.",
                SelectedPath = this.lastFolder,
                RootFolder = Environment.SpecialFolder.MyComputer
            };

            if (DialogResult.OK == dialog.ShowDialog(this))
            {
                this.AddItem(dialog.SelectedPath);
            }

            this.UpdateStates();
        }

        private void OnRemoveButtonClick(Object sender, EventArgs args)
        {
            if (this.lstFavoritesList.SelectedItems.Count > 0)
            {
                foreach (ListViewItem current in this.lstFavoritesList.SelectedItems)
                {
                    current.Remove();
                }
            }

            this.UpdateStates();
        }

        private void OnExecuteButtonClick(Object sender, EventArgs args)
        {
            this.canSaveData = true;
        }

        private void OnCloseButtonClick(Object sender, EventArgs args)
        {
            this.canSaveData = true;
        }

        private void OnFavoritesListSelectedIndexChanged(Object sender, EventArgs args)
        {
            this.UpdateStates();
        }

        private void OnFavoritesListKeyDown(Object sender, KeyEventArgs args)
        {
            if (args.Control && args.KeyCode == Keys.A)
            {
                foreach (ListViewItem current in this.lstFavoritesList.Items)
                {
                    current.Selected = true;
                }
            }

        }

        private void OnFavoritesListMenuOpening(Object sender, CancelEventArgs args)
        {
            this.cmiRemove.Enabled = this.lstFavoritesList.SelectedItems.Count > 0;

        }

        #endregion // Private event handler section.

        #region Private member function section.

        private void UpdateStates()
        {
            this.btnRemove.Enabled = this.lstFavoritesList.SelectedItems.Count > 0;
            this.btnExecute.Enabled = this.lstFavoritesList.Items.Count > 0;
        }

        private Boolean CanAddItem(String pathname)
        {
            if (!String.IsNullOrEmpty(pathname))
            {
                if (Directory.Exists(pathname) && pathname.CompareTo(Path.GetPathRoot(pathname)) != 0)
                {
                    String helper = pathname;
                    if (!helper.EndsWith(PATH_SEPARATOR)) { helper += PATH_SEPARATOR; }

                    foreach (ListViewItem current in this.lstFavoritesList.Items)
                    {
                        String existing = current.Tag as String;
                        Debug.Assert(existing != null);

                        if (!existing.EndsWith(PATH_SEPARATOR)) { existing += PATH_SEPARATOR; }

                        // Given path already exists in the list.
                        if (helper.CompareTo(existing) == 0)
                        {
                            return false;
                        }
                        else
                        {
                            String message = String.Empty;

                            if (existing.StartsWith(helper))
                            {
                                // Existing path starts with given path.
                                message =
                                    "Chosen path is a base-folder of an already existing entry!\n\n" +
                                    "Do you want to replace the existing path?";
                            }
                            else if (helper.StartsWith(existing))
                            {
                                // Given path starts with existing path.
                                message =
                                    "Chosen path is a sub-folder of an already existing entry!\n\n" +
                                    "Do you want to replace the existing path?";
                            }
                            else
                            {
                                // Existing path is not equal to given path.
                                continue;
                            }

                            DialogResult answer = MessageBox.Show(
                                this, message, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                            if (answer == DialogResult.Yes)
                            {
                                // Use original pathname!
                                this.SetItem(current, pathname);
                            }
                            return false; // Do not add!
                        }
                    }
                    return true; // Given pathname does not exist yet.
                }
            }
            return false;
        }

        private void AddItem(String pathname)
        {
            if (this.CanAddItem(pathname))
            {
                ListViewItem item = new ListViewItem();
                this.SetItem(item, pathname);
                this.lstFavoritesList.Items.Add(item);
                this.lastFolder = pathname;
            }

            this.colFolderName.Width = -1;
            this.colBaseFolder.Width = -1;
            this.colFolderState.Width = -2;
        }

        private void SetItem(ListViewItem item, String pathname)
        {
            item.SubItems.Clear();
            item.Text = Path.GetFileName(pathname);
            item.SubItems.Add(Path.GetDirectoryName(pathname));
            item.Tag = pathname;
            item.ImageKey = DEFAULT_IMAGE_KEY;
            item.ToolTipText = String.Empty;

            try
            {
                DirectoryInfo info = new DirectoryInfo(pathname);
                Int32 folders = info.GetDirectories().Length;
                Int32 files = info.GetFiles().Length;
                item.ToolTipText = String.Format(
                    "Created: {0}\nFolders: {1}\nFiles: {2}",
                    info.CreationTime.ToString(),
                    (folders == 0 ? "none" : folders.ToString()),
                    (files == 0 ? "none" : files.ToString()));

                item.SubItems.Add(
                    (files > 0 || folders > 0) ? String.Format("Items {0}", (files + folders)) : "Empty");
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("FavoritesDialog", exception);
            }
        }

        private Icon GetFolderIcon(Boolean small)
        {
            const Int32 FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
            const Int32 SHGFI_ICON = 0x100;
            const Int32 SHGFI_LARGEICON = 0x0;
            const Int32 SHGFI_SMALLICON = 0x1;
            const Int32 SHGFI_USEFILEATTRIBUTES = 0x10;

            Icon result = null;
            SHFILEINFO info = new SHFILEINFO();
            try
            {
                Int32 size = Marshal.SizeOf(info);
                Int32 attr = FILE_ATTRIBUTE_DIRECTORY;
                Int32 flags = SHGFI_USEFILEATTRIBUTES | SHGFI_ICON | (small ? SHGFI_SMALLICON : SHGFI_LARGEICON);

                // Use wildcard instead of a real folder name.
                SHGetFileInfo("*", attr, ref info, size, flags);

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

        #endregion // Private member function section.

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
    }
}
