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
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace plexdata.FileWiper
{
    public partial class MainForm : Form
    {
        private const string TBB_SCALING_SMALL = "small";
        private const string TBB_SCALING_MEDIUM = "medium";
        private const string TBB_SCALING_LARGE = "large";

        private Dictionary<string, Size> toolbarScalings = new Dictionary<string, Size>();

        private volatile bool IsToolbarLocked = false;

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
            bool enabled = this.IsWiping && !this.IsToolbarLocked;
            this.tbbBack.Enabled = enabled;
            this.tbbCancel.Enabled = enabled;
            this.tbbPause.Enabled = enabled && !this.IsPausing;
            this.tbbContinue.Enabled = enabled && this.IsPausing;
            this.tbbFavorites.Enabled = !enabled;
            this.mainToolbar.Refresh();
        }

        public static string ToolbarDefaultScaling
        {
            get
            {
                return TBB_SCALING_MEDIUM;
            }
        }

        #region Toolbar context menu event handler implementation.

        private void OnMainToolbarMenuSmallImagesClick(object sender, EventArgs args)
        {
            this.ScaleToolbarButtons(TBB_SCALING_SMALL);
        }

        private void OnMainToolbarMenuMediumImagesClick(object sender, EventArgs args)
        {
            this.ScaleToolbarButtons(TBB_SCALING_MEDIUM);
        }

        private void OnMainToolbarMenuLargeImagesClick(object sender, EventArgs args)
        {
            this.ScaleToolbarButtons(TBB_SCALING_LARGE);
        }

        private void OnMainToolbarMenuShowTextClick(object sender, EventArgs args)
        {
            // Toggle current state.
            this.ShowToolbarText(!this.mainToolbarMenuShowText.Checked);
        }

        #endregion // Toolbar context menu event handler implementation.

        #region Toolbar button click event handler implementation.

        private void OnToolbarButtonExitClick(object sender, EventArgs args)
        {
            this.UserCloseRequest();
        }

        private void OnToolbarButtonOpenClick(object sender, EventArgs args)
        {
            this.tbbOpen.ShowDropDown();
        }

        private void OnToolbarButtonOpenFilesClick(object sender, EventArgs args)
        {
            this.PerformOpenAction(false);
        }

        private void OnToolbarButtonOpenFoldersClick(object sender, EventArgs args)
        {
            this.PerformOpenAction(true);
        }

        private void OnToolbarButtonBackgroundClick(object sender, EventArgs args)
        {
            this.ForceShowTray();
        }

        private void OnToolbarButtonCancelClick(object sender, EventArgs args)
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

        private void OnToolbarButtonPauseClick(object sender, EventArgs args)
        {
            this.SuspendWipings();
        }

        private void OnToolbarButtonContinueClick(object sender, EventArgs args)
        {
            this.ContinueWipings();
        }

        private void OnToolbarButtonDetailsClick(object sender, EventArgs args)
        {
            this.PerformDetailsView();
        }

        private void OnToolbarButtonFavoritesClick(object sender, EventArgs args)
        {
            this.PerformFavoritesDialog();
        }

        private void OnToolbarButtonSettingsClick(object sender, EventArgs args)
        {
            this.PerformSettingsDialog();
        }

        private void OnToolbarButtonAboutClick(object sender, EventArgs args)
        {
            this.PerformAboutBox();
        }

        private void OnToolbarButtonAboutAboutClick(object sender, EventArgs args)
        {
            this.PerformAboutBox();
        }

        private void OnToolbarButtonAboutHelpClick(object sender, EventArgs args)
        {
            this.PerformDisplayHelp();
        }

        #endregion // Toolbar button click event handler implementation.

        #region Toolbar button management handler implementation.

        private void ShowToolbarText(bool show)
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

        private void ScaleToolbarButtons(string scaling)
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
