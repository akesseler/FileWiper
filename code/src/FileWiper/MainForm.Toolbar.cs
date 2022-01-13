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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Plexdata.FileWiper
{
    public partial class MainForm : Form
    {
        private const String TBB_SCALING_SMALL = "small";
        private const String TBB_SCALING_MEDIUM = "medium";
        private const String TBB_SCALING_LARGE = "large";

        private readonly Dictionary<String, Size> toolbarScalings = new Dictionary<String, Size>();

        private volatile Boolean IsToolbarLocked = false;

        private void InitializeToolbar()
        {
            this.toolbarScalings.Add(TBB_SCALING_SMALL, new Size(16, 16));
            this.toolbarScalings.Add(TBB_SCALING_MEDIUM, new Size(32, 32));
            this.toolbarScalings.Add(TBB_SCALING_LARGE, new Size(48, 48));

            this.tbbOpenFiles.ImageScaling = ToolStripItemImageScaling.None;
            this.tbbOpenFiles.Image = new Icon(Properties.Resources.File, this.toolbarScalings[TBB_SCALING_SMALL]).ToBitmap();
            this.tbbOpenFolders.ImageScaling = ToolStripItemImageScaling.None;
            this.tbbOpenFolders.Image = new Icon(Properties.Resources.Folder, this.toolbarScalings[TBB_SCALING_SMALL]).ToBitmap();

            this.tbbAboutAbout.ImageScaling = ToolStripItemImageScaling.None;
            this.tbbAboutAbout.Image = new Icon(Properties.Resources.About, this.toolbarScalings[TBB_SCALING_SMALL]).ToBitmap();
            this.tbbAboutHelp.ImageScaling = ToolStripItemImageScaling.None;
            this.tbbAboutHelp.Image = new Icon(Properties.Resources.Help, this.toolbarScalings[TBB_SCALING_SMALL]).ToBitmap();

            this.ScaleToolbarButtons(this.toolbarScalings[TBB_SCALING_MEDIUM]);

            this.mainToolbarMenuMediumImages.Checked = true;
            this.mainToolbarMenuShowText.Checked =
                this.tbbExit.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText;
        }

        private void UpdateToolbarState()
        {
            Boolean enabled = this.IsWiping && !this.IsToolbarLocked;
            this.tbbBack.Enabled = enabled;
            this.tbbCancel.Enabled = enabled;
            this.tbbPause.Enabled = enabled && !this.IsPausing;
            this.tbbContinue.Enabled = enabled && this.IsPausing;
            this.tbbFavorites.Enabled = !enabled;
            this.mainToolbar.Refresh();
        }

        public static String ToolbarDefaultScaling
        {
            get
            {
                return TBB_SCALING_MEDIUM;
            }
        }

        #region Toolbar context menu event handler implementation.

        private void OnMainToolbarMenuSmallImagesClick(Object sender, EventArgs args)
        {
            this.ScaleToolbarButtons(TBB_SCALING_SMALL);
        }

        private void OnMainToolbarMenuMediumImagesClick(Object sender, EventArgs args)
        {
            this.ScaleToolbarButtons(TBB_SCALING_MEDIUM);
        }

        private void OnMainToolbarMenuLargeImagesClick(Object sender, EventArgs args)
        {
            this.ScaleToolbarButtons(TBB_SCALING_LARGE);
        }

        private void OnMainToolbarMenuShowTextClick(Object sender, EventArgs args)
        {
            // Toggle current state.
            this.ShowToolbarText(!this.mainToolbarMenuShowText.Checked);
        }

        #endregion // Toolbar context menu event handler implementation.

        #region Toolbar button click event handler implementation.

        private void OnToolbarButtonExitClick(Object sender, EventArgs args)
        {
            this.UserCloseRequest();
        }

        private void OnToolbarButtonOpenClick(Object sender, EventArgs args)
        {
            this.tbbOpen.ShowDropDown();
        }

        private void OnToolbarButtonOpenFilesClick(Object sender, EventArgs args)
        {
            this.PerformOpenAction(false);
        }

        private void OnToolbarButtonOpenFoldersClick(Object sender, EventArgs args)
        {
            this.PerformOpenAction(true);
        }

        private void OnToolbarButtonBackgroundClick(Object sender, EventArgs args)
        {
            this.ForceShowTray();
        }

        private void OnToolbarButtonCancelClick(Object sender, EventArgs args)
        {
            if (this.Settings.Behaviour.SuppressCancelQuestion)
            {
                this.CancelWipings();
            }
            else
            {
                this.RequestCancelWipings();
            }
        }

        private void OnToolbarButtonPauseClick(Object sender, EventArgs args)
        {
            this.SuspendWipings();
        }

        private void OnToolbarButtonContinueClick(Object sender, EventArgs args)
        {
            this.ContinueWipings();
        }

        private void OnToolbarButtonDetailsClick(Object sender, EventArgs args)
        {
            this.PerformDetailsView();
        }

        private void OnToolbarButtonFavoritesClick(Object sender, EventArgs args)
        {
            this.PerformFavoritesDialog();
        }

        private void OnToolbarButtonSettingsClick(Object sender, EventArgs args)
        {
            this.PerformSettingsDialog();
        }

        private void OnToolbarButtonAboutClick(Object sender, EventArgs args)
        {
            this.PerformAboutBox();
        }

        private void OnToolbarButtonAboutAboutClick(Object sender, EventArgs args)
        {
            this.PerformAboutBox();
        }

        private void OnToolbarButtonAboutHelpClick(Object sender, EventArgs args)
        {
            this.PerformDisplayHelp();
        }

        #endregion // Toolbar button click event handler implementation.

        #region Toolbar button management handler implementation.

        private void ShowToolbarText(Boolean show)
        {
            try
            {
                this.mainToolbar.SuspendLayout();

                // Show or hide text of all current toolbar items.
                foreach (ToolStripItem current in this.mainToolbar.Items)
                {
                    current.DisplayStyle = show ? ToolStripItemDisplayStyle.ImageAndText : ToolStripItemDisplayStyle.Image;
                }

                // Save new state.
                this.mainToolbarMenuShowText.Checked = show;
                this.Settings.Maintain.ToolbarText = show;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("MainForm.Toolbar", exception);
            }
            finally
            {
                this.mainToolbar.ResumeLayout();
            }
        }

        private void ScaleToolbarButtons(String scaling)
        {
            scaling = scaling.ToLower();

            if (this.toolbarScalings.ContainsKey(scaling))
            {
                this.ScaleToolbarButtons(this.toolbarScalings[scaling]);
                this.Settings.Maintain.ToolbarScaling = scaling;

                if (String.Compare(scaling, TBB_SCALING_SMALL, true) == 0)
                {
                    this.mainToolbarMenuSmallImages.Checked = true;
                }
                else if (String.Compare(scaling, TBB_SCALING_MEDIUM, true) == 0)
                {
                    this.mainToolbarMenuMediumImages.Checked = true;
                }
                else if (String.Compare(scaling, TBB_SCALING_LARGE, true) == 0)
                {
                    this.mainToolbarMenuLargeImages.Checked = true;
                }
            }
        }

        private void ScaleToolbarButtons(Size scaling)
        {
            try
            {
                this.mainToolbar.SuspendLayout();

                this.mainToolbar.ImageScalingSize = scaling;

                this.tbbExit.Image = new Icon(Properties.Resources.Exit, scaling).ToBitmap();
                this.tbbOpen.Image = new Icon(Properties.Resources.Open, scaling).ToBitmap();
                this.tbbBack.Image = new Icon(Properties.Resources.Background, scaling).ToBitmap();
                this.tbbCancel.Image = new Icon(Properties.Resources.Abort, scaling).ToBitmap();
                this.tbbPause.Image = new Icon(Properties.Resources.Pause, scaling).ToBitmap();
                this.tbbContinue.Image = new Icon(Properties.Resources.Continue, scaling).ToBitmap();
                this.tbbFavorites.Image = new Icon(Properties.Resources.Favorites, scaling).ToBitmap();
                this.tbbSettings.Image = new Icon(Properties.Resources.Settings, scaling).ToBitmap();
                this.tbbDetails.Image = new Icon(Properties.Resources.Details, scaling).ToBitmap();
                this.tbbAbout.Image = new Icon(Properties.Resources.About, scaling).ToBitmap();

                this.mainToolbarMenuSmallImages.Checked = false;
                this.mainToolbarMenuMediumImages.Checked = false;
                this.mainToolbarMenuLargeImages.Checked = false;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("MainForm.Toolbar", exception);
            }
            finally
            {
                this.mainToolbar.ResumeLayout();
            }
        }

        #endregion // Toolbar button management handler implementation.
    }
}
