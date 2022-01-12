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

namespace Plexdata.FileWiper
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayIconMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.trayIconMenuShow = new System.Windows.Forms.ToolStripMenuItem();
            this.trayIconMenuCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.trayIconMenuPause = new System.Windows.Forms.ToolStripMenuItem();
            this.trayIconMenuContinue = new System.Windows.Forms.ToolStripMenuItem();
            this.trayIconMenuSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.trayIconMenuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.mainToolbar = new System.Windows.Forms.ToolStrip();
            this.mainToolbarMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mainToolbarMenuSmallImages = new System.Windows.Forms.ToolStripMenuItem();
            this.mainToolbarMenuMediumImages = new System.Windows.Forms.ToolStripMenuItem();
            this.mainToolbarMenuLargeImages = new System.Windows.Forms.ToolStripMenuItem();
            this.mainToolbarMenuSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.mainToolbarMenuShowText = new System.Windows.Forms.ToolStripMenuItem();
            this.tbbExit = new System.Windows.Forms.ToolStripButton();
            this.tbbOpen = new System.Windows.Forms.ToolStripSplitButton();
            this.tbbOpenFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.tbbOpenFolders = new System.Windows.Forms.ToolStripMenuItem();
            this.tbbBack = new System.Windows.Forms.ToolStripButton();
            this.tbbCancel = new System.Windows.Forms.ToolStripButton();
            this.tbbPause = new System.Windows.Forms.ToolStripButton();
            this.tbbContinue = new System.Windows.Forms.ToolStripButton();
            this.tbbDetails = new System.Windows.Forms.ToolStripButton();
            this.tbbFavorites = new System.Windows.Forms.ToolStripButton();
            this.tbbSettings = new System.Windows.Forms.ToolStripButton();
            this.tbbAbout = new System.Windows.Forms.ToolStripSplitButton();
            this.tbbAboutAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tbbAboutHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mainStatusbar = new System.Windows.Forms.StatusStrip();
            this.sblGeneral = new System.Windows.Forms.ToolStripStatusLabel();
            this.sblMode = new System.Windows.Forms.ToolStripStatusLabel();
            this.sblPlatform = new System.Windows.Forms.ToolStripStatusLabel();
            this.trayIconUpdater = new System.Windows.Forms.Timer(this.components);
            this.wipingListMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.wipingListMenuCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.wipingListMenuSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.wipingListMenuFindError = new System.Windows.Forms.ToolStripMenuItem();
            this.wipingListMenuShowError = new System.Windows.Forms.ToolStripMenuItem();
            this.wipingListMenuSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.wipingListMenuRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.wipingListMenuRemoveAll = new System.Windows.Forms.ToolStripMenuItem();
            this.wipingListMenuRemoveFinished = new System.Windows.Forms.ToolStripMenuItem();
            this.wipingListMenuRemoveCanceled = new System.Windows.Forms.ToolStripMenuItem();
            this.wipingList = new Plexdata.FileWiper.WipingListView();
            this.trayIconMenu.SuspendLayout();
            this.mainToolbar.SuspendLayout();
            this.mainToolbarMenu.SuspendLayout();
            this.mainStatusbar.SuspendLayout();
            this.wipingListMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // trayIcon
            // 
            this.trayIcon.ContextMenuStrip = this.trayIconMenu;
            this.trayIcon.Text = "File Wiper";
            this.trayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnTrayIconMouseClick);
            // 
            // trayIconMenu
            // 
            this.trayIconMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trayIconMenuShow,
            this.trayIconMenuCancel,
            this.trayIconMenuPause,
            this.trayIconMenuContinue,
            this.trayIconMenuSep1,
            this.trayIconMenuAbout});
            this.trayIconMenu.Name = "trayIconMenu";
            this.trayIconMenu.Size = new System.Drawing.Size(124, 120);
            this.trayIconMenu.Opening += new System.ComponentModel.CancelEventHandler(this.OnTrayIconMenuOpening);
            // 
            // trayIconMenuShow
            // 
            this.trayIconMenuShow.Name = "trayIconMenuShow";
            this.trayIconMenuShow.Size = new System.Drawing.Size(123, 22);
            this.trayIconMenuShow.Text = "&Show";
            this.trayIconMenuShow.Click += new System.EventHandler(this.OnTrayIconMenuShowClick);
            // 
            // trayIconMenuCancel
            // 
            this.trayIconMenuCancel.Name = "trayIconMenuCancel";
            this.trayIconMenuCancel.Size = new System.Drawing.Size(123, 22);
            this.trayIconMenuCancel.Text = "&Cancel";
            this.trayIconMenuCancel.Click += new System.EventHandler(this.OnTrayIconMenuCancelClick);
            // 
            // trayIconMenuPause
            // 
            this.trayIconMenuPause.Name = "trayIconMenuPause";
            this.trayIconMenuPause.Size = new System.Drawing.Size(123, 22);
            this.trayIconMenuPause.Text = "&Pause";
            this.trayIconMenuPause.Click += new System.EventHandler(this.OnTrayIconMenuPauseClick);
            // 
            // trayIconMenuContinue
            // 
            this.trayIconMenuContinue.Name = "trayIconMenuContinue";
            this.trayIconMenuContinue.Size = new System.Drawing.Size(123, 22);
            this.trayIconMenuContinue.Text = "C&ontinue";
            this.trayIconMenuContinue.Click += new System.EventHandler(this.OnTrayIconMenuContinueClick);
            // 
            // trayIconMenuSep1
            // 
            this.trayIconMenuSep1.Name = "trayIconMenuSep1";
            this.trayIconMenuSep1.Size = new System.Drawing.Size(120, 6);
            // 
            // trayIconMenuAbout
            // 
            this.trayIconMenuAbout.Name = "trayIconMenuAbout";
            this.trayIconMenuAbout.Size = new System.Drawing.Size(123, 22);
            this.trayIconMenuAbout.Text = "&About";
            this.trayIconMenuAbout.Click += new System.EventHandler(this.OnTrayIconMenuAboutClick);
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(150, 150);
            // 
            // mainToolbar
            // 
            this.mainToolbar.ContextMenuStrip = this.mainToolbarMenu;
            this.mainToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbbExit,
            this.tbbOpen,
            this.tbbBack,
            this.tbbCancel,
            this.tbbPause,
            this.tbbContinue,
            this.tbbDetails,
            this.tbbFavorites,
            this.tbbSettings,
            this.tbbAbout});
            this.mainToolbar.Location = new System.Drawing.Point(0, 0);
            this.mainToolbar.Name = "mainToolbar";
            this.mainToolbar.Size = new System.Drawing.Size(784, 25);
            this.mainToolbar.TabIndex = 1;
            // 
            // mainToolbarMenu
            // 
            this.mainToolbarMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainToolbarMenuSmallImages,
            this.mainToolbarMenuMediumImages,
            this.mainToolbarMenuLargeImages,
            this.mainToolbarMenuSep1,
            this.mainToolbarMenuShowText});
            this.mainToolbarMenu.Name = "mainToolbarMenu";
            this.mainToolbarMenu.Size = new System.Drawing.Size(161, 98);
            // 
            // mainToolbarMenuSmallImages
            // 
            this.mainToolbarMenuSmallImages.Name = "mainToolbarMenuSmallImages";
            this.mainToolbarMenuSmallImages.Size = new System.Drawing.Size(160, 22);
            this.mainToolbarMenuSmallImages.Text = "&Small Images";
            this.mainToolbarMenuSmallImages.Click += new System.EventHandler(this.OnMainToolbarMenuSmallImagesClick);
            // 
            // mainToolbarMenuMediumImages
            // 
            this.mainToolbarMenuMediumImages.Name = "mainToolbarMenuMediumImages";
            this.mainToolbarMenuMediumImages.Size = new System.Drawing.Size(160, 22);
            this.mainToolbarMenuMediumImages.Text = "&Medium Images";
            this.mainToolbarMenuMediumImages.Click += new System.EventHandler(this.OnMainToolbarMenuMediumImagesClick);
            // 
            // mainToolbarMenuLargeImages
            // 
            this.mainToolbarMenuLargeImages.Name = "mainToolbarMenuLargeImages";
            this.mainToolbarMenuLargeImages.Size = new System.Drawing.Size(160, 22);
            this.mainToolbarMenuLargeImages.Text = "&Large Images";
            this.mainToolbarMenuLargeImages.Click += new System.EventHandler(this.OnMainToolbarMenuLargeImagesClick);
            // 
            // mainToolbarMenuSep1
            // 
            this.mainToolbarMenuSep1.Name = "mainToolbarMenuSep1";
            this.mainToolbarMenuSep1.Size = new System.Drawing.Size(157, 6);
            // 
            // mainToolbarMenuShowText
            // 
            this.mainToolbarMenuShowText.Name = "mainToolbarMenuShowText";
            this.mainToolbarMenuShowText.Size = new System.Drawing.Size(160, 22);
            this.mainToolbarMenuShowText.Text = "Show &Text";
            this.mainToolbarMenuShowText.Click += new System.EventHandler(this.OnMainToolbarMenuShowTextClick);
            // 
            // tbbExit
            // 
            this.tbbExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbExit.Image = ((System.Drawing.Image)(resources.GetObject("tbbExit.Image")));
            this.tbbExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbExit.Name = "tbbExit";
            this.tbbExit.Size = new System.Drawing.Size(23, 22);
            this.tbbExit.Text = "Exit";
            this.tbbExit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tbbExit.ToolTipText = "Close main window and exit application.";
            this.tbbExit.Click += new System.EventHandler(this.OnToolbarButtonExitClick);
            // 
            // tbbOpen
            // 
            this.tbbOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbOpen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbbOpenFiles,
            this.tbbOpenFolders});
            this.tbbOpen.Image = ((System.Drawing.Image)(resources.GetObject("tbbOpen.Image")));
            this.tbbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbOpen.Name = "tbbOpen";
            this.tbbOpen.Size = new System.Drawing.Size(32, 22);
            this.tbbOpen.Text = "Open";
            this.tbbOpen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tbbOpen.ToolTipText = "Add additional files and folders to the wiping list.";
            this.tbbOpen.ButtonClick += new System.EventHandler(this.OnToolbarButtonOpenClick);
            // 
            // tbbOpenFiles
            // 
            this.tbbOpenFiles.Name = "tbbOpenFiles";
            this.tbbOpenFiles.Size = new System.Drawing.Size(121, 22);
            this.tbbOpenFiles.Text = "F&iles...";
            this.tbbOpenFiles.Click += new System.EventHandler(this.OnToolbarButtonOpenFilesClick);
            // 
            // tbbOpenFolders
            // 
            this.tbbOpenFolders.Name = "tbbOpenFolders";
            this.tbbOpenFolders.Size = new System.Drawing.Size(121, 22);
            this.tbbOpenFolders.Text = "F&olders...";
            this.tbbOpenFolders.Click += new System.EventHandler(this.OnToolbarButtonOpenFoldersClick);
            // 
            // tbbBack
            // 
            this.tbbBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbBack.Image = ((System.Drawing.Image)(resources.GetObject("tbbBack.Image")));
            this.tbbBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbBack.Name = "tbbBack";
            this.tbbBack.Size = new System.Drawing.Size(23, 22);
            this.tbbBack.Text = "Back";
            this.tbbBack.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tbbBack.ToolTipText = "Send main window to background.";
            this.tbbBack.Click += new System.EventHandler(this.OnToolbarButtonBackgroundClick);
            // 
            // tbbCancel
            // 
            this.tbbCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbCancel.Image = ((System.Drawing.Image)(resources.GetObject("tbbCancel.Image")));
            this.tbbCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbCancel.Name = "tbbCancel";
            this.tbbCancel.Size = new System.Drawing.Size(23, 22);
            this.tbbCancel.Text = "Cancel";
            this.tbbCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tbbCancel.ToolTipText = "Abort active wiping procedure.";
            this.tbbCancel.Click += new System.EventHandler(this.OnToolbarButtonCancelClick);
            // 
            // tbbPause
            // 
            this.tbbPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbPause.Image = ((System.Drawing.Image)(resources.GetObject("tbbPause.Image")));
            this.tbbPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbPause.Name = "tbbPause";
            this.tbbPause.Size = new System.Drawing.Size(23, 22);
            this.tbbPause.Text = "Pause";
            this.tbbPause.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tbbPause.ToolTipText = "Suspend active wiping procedure.";
            this.tbbPause.Click += new System.EventHandler(this.OnToolbarButtonPauseClick);
            // 
            // tbbContinue
            // 
            this.tbbContinue.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbContinue.Image = ((System.Drawing.Image)(resources.GetObject("tbbContinue.Image")));
            this.tbbContinue.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbContinue.Name = "tbbContinue";
            this.tbbContinue.Size = new System.Drawing.Size(23, 22);
            this.tbbContinue.Text = "Continue";
            this.tbbContinue.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tbbContinue.ToolTipText = "Continue suspended wiping procedure.";
            this.tbbContinue.Click += new System.EventHandler(this.OnToolbarButtonContinueClick);
            // 
            // tbbDetails
            // 
            this.tbbDetails.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbDetails.Image = ((System.Drawing.Image)(resources.GetObject("tbbDetails.Image")));
            this.tbbDetails.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbDetails.Name = "tbbDetails";
            this.tbbDetails.Size = new System.Drawing.Size(23, 22);
            this.tbbDetails.Text = "Details";
            this.tbbDetails.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tbbDetails.ToolTipText = "Show or hide the view of wiping details.";
            this.tbbDetails.Click += new System.EventHandler(this.OnToolbarButtonDetailsClick);
            // 
            // tbbFavorites
            // 
            this.tbbFavorites.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbFavorites.Image = ((System.Drawing.Image)(resources.GetObject("tbbFavorites.Image")));
            this.tbbFavorites.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbFavorites.Name = "tbbFavorites";
            this.tbbFavorites.Size = new System.Drawing.Size(23, 22);
            this.tbbFavorites.Text = "Favorites";
            this.tbbFavorites.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tbbFavorites.ToolTipText = "Open the favorites dialog to manage prefered wiping folders";
            this.tbbFavorites.Click += new System.EventHandler(this.OnToolbarButtonFavoritesClick);
            // 
            // tbbSettings
            // 
            this.tbbSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbSettings.Image = ((System.Drawing.Image)(resources.GetObject("tbbSettings.Image")));
            this.tbbSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbSettings.Name = "tbbSettings";
            this.tbbSettings.Size = new System.Drawing.Size(23, 22);
            this.tbbSettings.Text = "Settings";
            this.tbbSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tbbSettings.ToolTipText = "Open the settings dialog to configure the program.";
            this.tbbSettings.Click += new System.EventHandler(this.OnToolbarButtonSettingsClick);
            // 
            // tbbAbout
            // 
            this.tbbAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbAbout.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbbAboutAbout,
            this.tbbAboutHelp});
            this.tbbAbout.Image = ((System.Drawing.Image)(resources.GetObject("tbbAbout.Image")));
            this.tbbAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbAbout.Name = "tbbAbout";
            this.tbbAbout.Size = new System.Drawing.Size(32, 22);
            this.tbbAbout.Text = "About";
            this.tbbAbout.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tbbAbout.ToolTipText = "Show useful information about this program.";
            this.tbbAbout.ButtonClick += new System.EventHandler(this.OnToolbarButtonAboutClick);
            // 
            // tbbAboutAbout
            // 
            this.tbbAboutAbout.Name = "tbbAboutAbout";
            this.tbbAboutAbout.Size = new System.Drawing.Size(126, 22);
            this.tbbAboutAbout.Text = "&About";
            this.tbbAboutAbout.Click += new System.EventHandler(this.OnToolbarButtonAboutAboutClick);
            // 
            // tbbAboutHelp
            // 
            this.tbbAboutHelp.Name = "tbbAboutHelp";
            this.tbbAboutHelp.ShortcutKeyDisplayString = "(F1)";
            this.tbbAboutHelp.Size = new System.Drawing.Size(126, 22);
            this.tbbAboutHelp.Text = "&Help";
            this.tbbAboutHelp.Click += new System.EventHandler(this.OnToolbarButtonAboutHelpClick);
            // 
            // mainStatusbar
            // 
            this.mainStatusbar.BackColor = System.Drawing.Color.White;
            this.mainStatusbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sblGeneral,
            this.sblMode,
            this.sblPlatform});
            this.mainStatusbar.Location = new System.Drawing.Point(0, 538);
            this.mainStatusbar.Name = "mainStatusbar";
            this.mainStatusbar.ShowItemToolTips = true;
            this.mainStatusbar.Size = new System.Drawing.Size(784, 24);
            this.mainStatusbar.TabIndex = 2;
            // 
            // sblGeneral
            // 
            this.sblGeneral.Name = "sblGeneral";
            this.sblGeneral.Size = new System.Drawing.Size(717, 19);
            this.sblGeneral.Spring = true;
            this.sblGeneral.Text = "???";
            this.sblGeneral.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // sblMode
            // 
            this.sblMode.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.sblMode.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.sblMode.Name = "sblMode";
            this.sblMode.Size = new System.Drawing.Size(26, 19);
            this.sblMode.Text = "???";
            this.sblMode.ToolTipText = "Mode in which the program is running.";
            // 
            // sblPlatform
            // 
            this.sblPlatform.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.sblPlatform.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.sblPlatform.Name = "sblPlatform";
            this.sblPlatform.Size = new System.Drawing.Size(26, 19);
            this.sblPlatform.Text = "???";
            this.sblPlatform.ToolTipText = "Platform on which the program is running on.";
            // 
            // trayIconUpdater
            // 
            this.trayIconUpdater.Interval = 150;
            this.trayIconUpdater.Tick += new System.EventHandler(this.OnTrayIconUpdaterTick);
            // 
            // wipingListMenu
            // 
            this.wipingListMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wipingListMenuCancel,
            this.wipingListMenuSep1,
            this.wipingListMenuFindError,
            this.wipingListMenuShowError,
            this.wipingListMenuSep2,
            this.wipingListMenuRemove});
            this.wipingListMenu.Name = "wipingListMenu";
            this.wipingListMenu.Size = new System.Drawing.Size(170, 104);
            this.wipingListMenu.Opening += new System.ComponentModel.CancelEventHandler(this.OnWipingListMenuOpening);
            // 
            // wipingListMenuCancel
            // 
            this.wipingListMenuCancel.Name = "wipingListMenuCancel";
            this.wipingListMenuCancel.Size = new System.Drawing.Size(169, 22);
            this.wipingListMenuCancel.Text = "Cancel &Selection";
            this.wipingListMenuCancel.Click += new System.EventHandler(this.OnWipingListMenuItemClick);
            // 
            // wipingListMenuSep1
            // 
            this.wipingListMenuSep1.Name = "wipingListMenuSep1";
            this.wipingListMenuSep1.Size = new System.Drawing.Size(166, 6);
            // 
            // wipingListMenuFindError
            // 
            this.wipingListMenuFindError.Name = "wipingListMenuFindError";
            this.wipingListMenuFindError.Size = new System.Drawing.Size(169, 22);
            this.wipingListMenuFindError.Text = "Find &Next Error";
            this.wipingListMenuFindError.Click += new System.EventHandler(this.OnWipingListMenuItemClick);
            // 
            // wipingListMenuShowError
            // 
            this.wipingListMenuShowError.Name = "wipingListMenuShowError";
            this.wipingListMenuShowError.Size = new System.Drawing.Size(169, 22);
            this.wipingListMenuShowError.Text = "Show &Error Details";
            this.wipingListMenuShowError.Click += new System.EventHandler(this.OnWipingListMenuItemClick);
            // 
            // wipingListMenuSep2
            // 
            this.wipingListMenuSep2.Name = "wipingListMenuSep2";
            this.wipingListMenuSep2.Size = new System.Drawing.Size(166, 6);
            // 
            // wipingListMenuRemove
            // 
            this.wipingListMenuRemove.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wipingListMenuRemoveAll,
            this.wipingListMenuRemoveFinished,
            this.wipingListMenuRemoveCanceled});
            this.wipingListMenuRemove.Name = "wipingListMenuRemove";
            this.wipingListMenuRemove.Size = new System.Drawing.Size(169, 22);
            this.wipingListMenuRemove.Text = "&Remove Items";
            // 
            // wipingListMenuRemoveAll
            // 
            this.wipingListMenuRemoveAll.Name = "wipingListMenuRemoveAll";
            this.wipingListMenuRemoveAll.Size = new System.Drawing.Size(123, 22);
            this.wipingListMenuRemoveAll.Text = "&All";
            this.wipingListMenuRemoveAll.Click += new System.EventHandler(this.OnWipingListMenuItemClick);
            // 
            // wipingListMenuRemoveFinished
            // 
            this.wipingListMenuRemoveFinished.Name = "wipingListMenuRemoveFinished";
            this.wipingListMenuRemoveFinished.Size = new System.Drawing.Size(123, 22);
            this.wipingListMenuRemoveFinished.Text = "&Finished";
            this.wipingListMenuRemoveFinished.Click += new System.EventHandler(this.OnWipingListMenuItemClick);
            // 
            // wipingListMenuRemoveCanceled
            // 
            this.wipingListMenuRemoveCanceled.Name = "wipingListMenuRemoveCanceled";
            this.wipingListMenuRemoveCanceled.Size = new System.Drawing.Size(123, 22);
            this.wipingListMenuRemoveCanceled.Text = "&Canceled";
            this.wipingListMenuRemoveCanceled.Click += new System.EventHandler(this.OnWipingListMenuItemClick);
            // 
            // wipingList
            // 
            this.wipingList.AllowDrop = true;
            this.wipingList.ContextMenuStrip = this.wipingListMenu;
            this.wipingList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wipingList.FullRowSelect = true;
            this.wipingList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.wipingList.Location = new System.Drawing.Point(0, 25);
            this.wipingList.Name = "wipingList";
            this.wipingList.Size = new System.Drawing.Size(784, 513);
            this.wipingList.TabIndex = 0;
            this.wipingList.UseCompatibleStateImageBehavior = false;
            this.wipingList.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnWipingListDragDrop);
            this.wipingList.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnWipingListDragEnter);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.wipingList);
            this.Controls.Add(this.mainStatusbar);
            this.Controls.Add(this.mainToolbar);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "File Wiper";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.trayIconMenu.ResumeLayout(false);
            this.mainToolbar.ResumeLayout(false);
            this.mainToolbar.PerformLayout();
            this.mainToolbarMenu.ResumeLayout(false);
            this.mainStatusbar.ResumeLayout(false);
            this.mainStatusbar.PerformLayout();
            this.wipingListMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ContextMenuStrip trayIconMenu;
        private System.Windows.Forms.ToolStripMenuItem trayIconMenuPause;
        private System.Windows.Forms.ToolStripMenuItem trayIconMenuContinue;
        private System.Windows.Forms.ToolStripMenuItem trayIconMenuCancel;
        private System.Windows.Forms.ToolStripMenuItem trayIconMenuShow;
        private System.Windows.Forms.ToolStripSeparator trayIconMenuSep1;
        private System.Windows.Forms.ToolStripMenuItem trayIconMenuAbout;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.ToolStrip mainToolbar;
        private System.Windows.Forms.StatusStrip mainStatusbar;
        private System.Windows.Forms.ToolStripButton tbbExit;
        private System.Windows.Forms.ToolStripButton tbbBack;
        private System.Windows.Forms.ToolStripButton tbbCancel;
        private System.Windows.Forms.ToolStripButton tbbPause;
        private System.Windows.Forms.ToolStripButton tbbContinue;
        private System.Windows.Forms.ToolStripButton tbbSettings;
        private System.Windows.Forms.ContextMenuStrip mainToolbarMenu;
        private System.Windows.Forms.ToolStripMenuItem mainToolbarMenuSmallImages;
        private System.Windows.Forms.ToolStripMenuItem mainToolbarMenuMediumImages;
        private System.Windows.Forms.ToolStripMenuItem mainToolbarMenuLargeImages;
        private System.Windows.Forms.ToolStripSeparator mainToolbarMenuSep1;
        private System.Windows.Forms.ToolStripMenuItem mainToolbarMenuShowText;
        private WipingListView wipingList;
        private System.Windows.Forms.ToolStripStatusLabel sblGeneral;
        private System.Windows.Forms.ToolStripStatusLabel sblMode;
        private System.Windows.Forms.ToolStripStatusLabel sblPlatform;
        private System.Windows.Forms.Timer trayIconUpdater;
        private System.Windows.Forms.ContextMenuStrip wipingListMenu;
        private System.Windows.Forms.ToolStripMenuItem wipingListMenuCancel;
        private System.Windows.Forms.ToolStripButton tbbDetails;
        private System.Windows.Forms.ToolStripMenuItem wipingListMenuShowError;
        private System.Windows.Forms.ToolStripButton tbbFavorites;
        private System.Windows.Forms.ToolStripSplitButton tbbOpen;
        private System.Windows.Forms.ToolStripMenuItem tbbOpenFolders;
        private System.Windows.Forms.ToolStripMenuItem tbbOpenFiles;
        private System.Windows.Forms.ToolStripSeparator wipingListMenuSep1;
        private System.Windows.Forms.ToolStripMenuItem wipingListMenuRemove;
        private System.Windows.Forms.ToolStripMenuItem wipingListMenuRemoveAll;
        private System.Windows.Forms.ToolStripMenuItem wipingListMenuRemoveFinished;
        private System.Windows.Forms.ToolStripMenuItem wipingListMenuRemoveCanceled;
        private System.Windows.Forms.ToolStripSeparator wipingListMenuSep2;
        private System.Windows.Forms.ToolStripMenuItem wipingListMenuFindError;
        private System.Windows.Forms.ToolStripSplitButton tbbAbout;
        private System.Windows.Forms.ToolStripMenuItem tbbAboutAbout;
        private System.Windows.Forms.ToolStripMenuItem tbbAboutHelp;
    }
}

