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
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Plexdata.FileWiper
{
    public partial class SettingsDialog : Form
    {
        private readonly List<Icon> icons = null;
        private readonly ShellExtensionHandler extension = null;

        public SettingsDialog()
            : this(null)
        {
        }

        public SettingsDialog(Settings settings)
            : base()
        {
            this.InitializeComponent();

            this.Icon = Properties.Resources.Settings;

            this.btnOK.Image = new Icon(Properties.Resources.Apply, new Size(16, 16)).ToBitmap();
            this.btnCancel.Image = new Icon(Properties.Resources.Cancel, new Size(16, 16)).ToBitmap();
            this.btnDefault.Image = new Icon(Properties.Resources.Settings, new Size(16, 16)).ToBitmap();

            // Initialize internal SE icon list.
            this.icons = new List<Icon>(new Icon[] {
                Properties.Resources.Recycle1,
                Properties.Resources.Recycle2,
                Properties.Resources.Recycle3,
                Properties.Resources.Recycle4,
                Properties.Resources.Recycle5,
            });

            this.InitExtensionButtons();
            this.InitExtensionIconShow();

            // Create own shell extension handler.
            this.extension = new ShellExtensionHandler();

            // Use a settings clone!
            this.Settings = (settings != null) ? new Settings(settings) : new Settings();

            // BUGFIX: Ensure that icons are painted under Windows XP..
            // Switching on property DoubleBuffered is necessary at least for 
            // Windows XP because otherwise the listview's owner-drawn icons 
            // are not painted.
            PropertyInfo property = typeof(ListView).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            property.SetValue(this.lstIconShow, true, null);
        }

        public Settings Settings { get; private set; }

        #region Dialog event handler implementation.

        protected override void OnLoad(EventArgs args)
        {
            // Try choosing last active settings page. If impossible 
            // then the first page is chosen automatically!
            String active = this.Settings.Maintain.ActiveSettings;
            if (!String.IsNullOrEmpty(active))
            {
                foreach (TabPage page in this.tabSettings.TabPages)
                {
                    if (String.Compare(active, page.Text, true) == 0)
                    {
                        this.tabSettings.SelectedTab = page;
                        break;
                    }
                }
            }

            // Initialize controls belonging to behaviour settings.
            this.txtBehaviourDescription.Text = "";
            this.chkAllowAutoClose.GotFocus += new EventHandler(this.OnCheckboxGotFocus);
            this.chkAllowAutoClose.MouseEnter += new EventHandler(this.OnCheckboxMouseEnter);
            this.chkUseFullResources.GotFocus += new EventHandler(this.OnCheckboxGotFocus);
            this.chkUseFullResources.MouseEnter += new EventHandler(this.OnCheckboxMouseEnter);
            this.chkIncludeFolderNames.GotFocus += new EventHandler(this.OnCheckboxGotFocus);
            this.chkIncludeFolderNames.MouseEnter += new EventHandler(this.OnCheckboxMouseEnter);
            this.chkAutoPauseWiping.GotFocus += new EventHandler(this.OnCheckboxGotFocus);
            this.chkAutoPauseWiping.MouseEnter += new EventHandler(this.OnCheckboxMouseEnter);
            this.chkAllowAutoRelaunch.GotFocus += new EventHandler(this.OnCheckboxGotFocus);
            this.chkAllowAutoRelaunch.MouseEnter += new EventHandler(this.OnCheckboxMouseEnter);
            this.chkSuppressCancelQuestion.GotFocus += new EventHandler(this.OnCheckboxGotFocus);
            this.chkSuppressCancelQuestion.MouseEnter += new EventHandler(this.OnCheckboxMouseEnter);

            // Load settings for all pages.
            this.LoadBehaviourSettings();
            this.LoadWipingSettings();
            this.LoadExtensionSettings();

            // Apply last known dialog size and location.
            if (Settings.IsVisibleOnAllScreens(this.Settings.Maintain.SettingsBounds))
            {
                this.StartPosition = FormStartPosition.Manual;
                this.DesktopBounds = this.Settings.Maintain.SettingsBounds;
            }
            else
            {
                this.StartPosition = FormStartPosition.CenterParent;
            }

            base.OnLoad(args);
        }

        protected override void OnFormClosing(FormClosingEventArgs args)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                args.Cancel = !this.ValidateSettings();
            }

            // Try saving last active settings page.
            if (!args.Cancel)
            {
                // Save settings from all pages.
                this.SaveBehaviourSettings();
                this.SaveWipingSettings();

                Settings helper = null;

                if (this.DialogResult == DialogResult.OK)
                {
                    // In this case current changes are taken over by the caller.
                    helper = this.Settings;
                }
                else
                {
                    // In this case a direct manipulation of current settings is necessary.
                    helper = Program.MainForm.Settings;
                }

                if (this.tabSettings.SelectedTab != null)
                {
                    // Save last active settings page.
                    helper.Maintain.ActiveSettings = this.tabSettings.SelectedTab.Text;
                }

                // Save last known dialog size and location.
                helper.Maintain.SettingsBounds = this.DesktopBounds;
            }

            base.OnFormClosing(args);
        }

        private void OnDefaultButtonClick(Object sender, EventArgs args)
        {
            if (this.tabSettings.SelectedTab == this.tcpBehaviour)
            {
                this.DefaultBehaviourSettings();
            }
            else if (this.tabSettings.SelectedTab == this.tcpWipingSettings)
            {
                this.DefaultWipingSettings();
            }
            else if (this.tabSettings.SelectedTab == this.tcpShellExtension)
            {
                this.DefaultExtensionSettings();
            }
        }

        #endregion // Dialog event handler implementation.

        #region Dialog event handler (behaviour settings) implementation.

        private void OnCheckboxMouseEnter(Object sender, EventArgs args)
        {
            if (sender is CheckBox control && !control.Focused) { control.Focus(); }
        }

        private void OnCheckboxGotFocus(Object sender, EventArgs args)
        {
            if (sender is CheckBox control && !String.IsNullOrEmpty(control.Tag as String))
            {
                this.txtBehaviourDescription.Text = control.Tag as String;
            }
        }

        private void OnAllowAutoRelaunchClick(Object sender, EventArgs args)
        {
            if (this.chkAllowAutoRelaunch.Checked)
            {
                String message = "Enabling this option may cause some irritations while " +
                    "executing the program. This exactly means that the operating system " +
                    "will ask for extending current privileges without an appropriated " +
                    "context as soon as it becomes necessary to clean-up files which require " +
                    "administration rights." + Environment.NewLine + Environment.NewLine +
                    "Therefore, are you really sure and do you really want to enable this " +
                    "very specific option?";

                if (DialogResult.Yes != MessageBox.Show(this,
                    message, this.Text + " - Warning", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))
                {
                    this.chkAllowAutoRelaunch.Checked = false;
                }
            }
        }

        #endregion // Dialog event handler (behaviour settings) implementation.

        #region Dialog event handler (wiping settings) implementation.

        private void OnParallelProcessingCheckedChanged(Object sender, EventArgs args)
        {
            this.numParallelWipings.Enabled = this.chkParallelProcessing.Checked;
        }

        private void OnAlgorithmComboBoxSelectedIndexChanged(Object sender, EventArgs args)
        {
            this.Settings.Algorithms.Selected = this.cmbAlgorithmAlgorithm.SelectedItem as WipingAlgorithm;
            if (this.Settings.Algorithms.Selected != null)
            {
                this.SetValue(this.numAlgorithmRepeats, this.Settings.Algorithms.Selected.Repeats);
                this.txtAlgorithmDescription.Text = this.Settings.Algorithms.Selected.Description;
            }
        }

        private void OnRepeatsNumericSpinValueChanged(Object sender, EventArgs args)
        {
            if (this.cmbAlgorithmAlgorithm.SelectedItem is WipingAlgorithm selected)
            {
                selected.Repeats = this.GetValue(this.numAlgorithmRepeats);

                // Cross check because some algorithms support 
                // only a fixed number of repeats.
                this.numAlgorithmRepeats.Value = selected.Repeats;
            }
        }

        #endregion // Dialog event handler (wiping settings) implementation.

        #region Dialog event handler (shell extension) implementation.

        private void OnEnableIconUsageCheckedChanged(Object sender, EventArgs args)
        {
            this.lstIconShow.Enabled = this.chkEnableIcon.Checked;
        }

        private void OnRegisterButtonClick(Object sender, EventArgs args)
        {
            if (this.SaveExtensionSettings())
            {
                this.HandleRegistration(true);
            }
        }

        private void OnUnregisterButtonClick(Object sender, EventArgs args)
        {
            if (this.extension.IsRegistered)
            {
                this.HandleRegistration(false);
            }
        }

        private void OnIconShowDrawItem(Object sender, DrawListViewItemEventArgs args)
        {
            try
            {
                if (this.lstIconShow.View != View.Tile)
                {
                    args.DrawDefault = true;
                    return;
                }

                using (Bitmap offBitmap = new Bitmap(args.Bounds.Width, args.Bounds.Height))
                using (Graphics offScreen = Graphics.FromImage(offBitmap))
                {
                    offScreen.SmoothingMode = SmoothingMode.HighQuality;
                    offScreen.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    // Create offscreen bounding rectangle.
                    Rectangle bounds = new Rectangle(0, 0, offBitmap.Width, offBitmap.Height);

                    if (args.Item.Selected)
                    {
                        if (this.lstIconShow.Focused)
                        {
                            // Fill bounding rectangle with 
                            // "selected" background color.
                            this.DrawIconShowFocus(offScreen, bounds,
                                Color.FromArgb(89, 172, 255),
                                Color.FromArgb(229, 243, 255));
                        }
                        else
                        {
                            if (this.lstIconShow.Enabled)
                            {
                                if (this.lstIconShow.HideSelection)
                                {
                                    // Fill bounding rectangle with "normal" background color.
                                    using (Brush brush = new SolidBrush(args.Item.BackColor))
                                    {
                                        offScreen.FillRectangle(brush, bounds);
                                    }
                                }
                                else
                                {
                                    // Fill bounding rectangle with 
                                    // "un-focused" background color.
                                    this.DrawIconShowFocus(offScreen, bounds,
                                        Color.FromArgb(224, 224, 224),
                                        Color.FromArgb(249, 249, 249));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.lstIconShow.Enabled)
                        {
                            // Fill bounding rectangle with "normal" background color.
                            using (Brush brush = new SolidBrush(args.Item.BackColor))
                            {
                                offScreen.FillRectangle(brush, bounds);
                            }
                        }
                    }

                    // Prepare bounding rectangle for icon output.
                    bounds.Inflate(-2, -2);

                    // Draw the icon.
                    Image image = this.lstIconShow.LargeImageList.Images[args.ItemIndex];
                    offScreen.DrawImageUnscaled(image, bounds);

                    // Perform bit-block transfer onto destination graphics context.
                    args.Graphics.DrawImage(offBitmap, args.Bounds);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("SettingsDialog", exception);
            }
        }

        #endregion // Dialog event handler (shell extension) implementation.

        #region Private helper function implementation.

        private Boolean ValidateSettings()
        {
            // Implement settings validation, if applicable.
            return true;
        }

        private Int32 GetValue(NumericUpDown numBox)
        {
            return Convert.ToInt32(numBox.Value);
        }

        private void SetValue(NumericUpDown numBox, Int32 value)
        {
            if (value < numBox.Minimum)
            {
                value = Convert.ToInt32(numBox.Minimum);
            }
            else if (value > numBox.Maximum)
            {
                value = Convert.ToInt32(numBox.Maximum);
            }
            numBox.Value = value;
        }

        #endregion // Private helper function implementation.

        #region Private helper function (behaviour settings) implementation.

        private Boolean LoadBehaviourSettings()
        {
            try
            {
                this.chkAllowAutoClose.Checked = this.Settings.Behaviour.AllowAutoClose;
                this.chkUseFullResources.Checked = this.Settings.Behaviour.UseFullResources;
                this.chkIncludeFolderNames.Checked = this.Settings.Behaviour.IncludeFolderNames;
                this.chkAutoPauseWiping.Checked = this.Settings.Behaviour.AutoPauseWiping;
                this.chkAllowAutoRelaunch.Checked = this.Settings.Behaviour.AllowAutoRelaunch;
                this.chkSuppressCancelQuestion.Checked = this.Settings.Behaviour.SuppressCancelQuestion;

                return true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("SettingsDialog", exception);
                return false;
            }
        }

        private Boolean SaveBehaviourSettings()
        {
            try
            {
                this.Settings.Behaviour.AllowAutoClose = this.chkAllowAutoClose.Checked;
                this.Settings.Behaviour.UseFullResources = this.chkUseFullResources.Checked;
                this.Settings.Behaviour.IncludeFolderNames = this.chkIncludeFolderNames.Checked;
                this.Settings.Behaviour.AutoPauseWiping = this.chkAutoPauseWiping.Checked;
                this.Settings.Behaviour.AllowAutoRelaunch = this.chkAllowAutoRelaunch.Checked;
                this.Settings.Behaviour.SuppressCancelQuestion = this.chkSuppressCancelQuestion.Checked;

                return true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("SettingsDialog", exception);
                return false;
            }
        }

        private Boolean DefaultBehaviourSettings()
        {
            this.Settings.Behaviour = new Behaviour();
            return this.LoadBehaviourSettings();
        }

        #endregion // Private helper function (behaviour settings) implementation.

        #region Private helper function (wiping settings) implementation.

        private Boolean LoadWipingSettings()
        {
            try
            {
                this.chkParallelProcessing.Checked = this.Settings.Processing.AllowParallel;
                this.numParallelWipings.Minimum = WipingProcessing.ThreadCountMinimum;
                this.numParallelWipings.Maximum = WipingProcessing.ThreadCountMaximum;
                this.numParallelWipings.Enabled = this.chkParallelProcessing.Checked;
                this.SetValue(this.numParallelWipings, this.Settings.Processing.ThreadCount);

                this.numAlgorithmRepeats.Minimum = WipingAlgorithm.RepeatsMinimum;
                this.numAlgorithmRepeats.Maximum = WipingAlgorithm.RepeatsMaximum;

                this.cmbAlgorithmAlgorithm.Items.Clear();
                this.cmbAlgorithmAlgorithm.Items.AddRange(this.Settings.Algorithms.Algorithms);
                this.cmbAlgorithmAlgorithm.SelectedItem = this.Settings.Algorithms.Selected;

                return true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("SettingsDialog", exception);
                return false;
            }
        }

        private Boolean SaveWipingSettings()
        {
            try
            {
                this.Settings.Processing.AllowParallel = this.chkParallelProcessing.Checked;
                this.Settings.Processing.ThreadCount = this.GetValue(this.numParallelWipings);

                // Saving current algorithm settings is already done by the event handlers.

                return true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("SettingsDialog", exception);
                return false;
            }
        }

        private Boolean DefaultWipingSettings()
        {
            this.Settings.Processing = new WipingProcessing();
            this.Settings.Algorithms = new WipingAlgorithms();
            return this.LoadWipingSettings();
        }

        #endregion // Private helper function (wiping settings) implementation.

        #region Private helper function (shell extension) implementation.

        private Boolean LoadExtensionSettings()
        {
            return this.LoadExtensionSettings(false);
        }

        private Boolean LoadExtensionSettings(Boolean defaults)
        {
            try
            {
                if (this.extension.LoadFromResources(defaults))
                {
                    this.txtDisplayText.Text = this.extension.Label;
                    this.txtHelpString.Text = this.extension.HelpString;

                    String icondata = this.extension.IconData;
                    if (!String.IsNullOrEmpty(icondata))
                    {
                        this.chkEnableIcon.Checked = true;

                        // Try preselect icon from shell extension settings.
                        String current = this.extension.IconData;
                        for (Int32 index = 0; index < this.icons.Count; index++)
                        {
                            String helper = ShellExtensionHandler.ConvertToIconData(this.icons[index]);
                            if (String.Compare(helper, current, true) == 0)
                            {
                                this.lstIconShow.Items[index].Selected = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        this.chkEnableIcon.Checked = false;
                    }

                    this.UpdateExtensionButtons();

                    return true;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("SettingsDialog", exception);
            }
            return false;
        }

        private Boolean SaveExtensionSettings()
        {
            try
            {
                String display = this.txtDisplayText.Text.Trim();
                if (!String.IsNullOrEmpty(display))
                {
                    this.extension.Label = display;
                    this.extension.HelpString = this.txtHelpString.Text;
                    this.extension.Executable = Application.ExecutablePath;

                    if (this.chkEnableIcon.Checked && this.lstIconShow.SelectedIndices != null && this.lstIconShow.SelectedIndices.Count > 0)
                    {
                        this.extension.IconData = ShellExtensionHandler.ConvertToIconData(
                            this.icons[this.lstIconShow.SelectedIndices[0]]);
                    }
                    else
                    {
                        this.extension.IconData = null;
                    }

                    return this.extension.ExportToFile();
                }
                else
                {
                    MessageBox.Show(this, "Shell Extension display text is mandatory!",
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Restore previous label value.
                    this.txtDisplayText.Text = this.extension.Label;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("SettingsDialog", exception);
            }
            return false;
        }

        private Boolean DefaultExtensionSettings()
        {
            return this.LoadExtensionSettings(true);
        }

        private void InitExtensionButtons()
        {
            if (!PermissionCheck.IsRunAsAdmin)
            {
                if (PlatformCheck.IsVistaOrHigher)
                {
                    PermissionCheck.SetButtonShield(this.btnRegister, true);
                    PermissionCheck.SetButtonShield(this.btnUnregister, true);
                }
            }
        }

        private void InitExtensionIconShow()
        {
            try
            {
                // Needs to be created if it doesn't exist!
                if (this.components == null) { this.components = new Container(); }

                this.lstIconShow.View = View.Tile;
                this.lstIconShow.TileSize = new Size(55, 55);

                this.lstIconShow.LargeImageList = new ImageList(this.components)
                {
                    ColorDepth = ColorDepth.Depth32Bit,
                    ImageSize = new Size(48, 48),
                    TransparentColor = Color.Transparent
                };

                Size size = this.lstIconShow.LargeImageList.ImageSize;
                foreach (Icon icon in this.icons)
                {
                    this.lstIconShow.LargeImageList.Images.Add(new Icon(icon, size));
                    Int32 index = this.lstIconShow.LargeImageList.Images.Count - 1;
                    this.lstIconShow.Items.Add(new ListViewItem(String.Empty, index));
                }

                this.lstIconShow.Enabled = this.chkEnableIcon.Checked;

                this.lstIconShow.OwnerDraw = (this.lstIconShow.View == View.Tile);
                this.lstIconShow.DrawItem += new DrawListViewItemEventHandler(this.OnIconShowDrawItem);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("SettingsDialog", exception);
            }
        }

        private void HandleRegistration(Boolean register)
        {
            Cursor cursor = this.Cursor;
            try
            {
                this.Cursor = Cursors.WaitCursor;

                String filename = this.extension.Filename;

                if (!PermissionCheck.IsRunAsAdmin)
                {
                    if (register)
                    {
                        SelfElevation.Elevate(ParameterParser.BuildOptionRegister(filename), true);
                    }
                    else
                    {
                        SelfElevation.Elevate(ParameterParser.BuildOptionUnregister(), true);
                    }
                }
                else
                {
                    if (register)
                    {
                        ShellExtensionHandler.RegisterExtension(filename);
                    }
                    else
                    {
                        ShellExtensionHandler.UnregisterExtension();
                    }
                }

                this.UpdateExtensionButtons();

                // Cleanup!
                if (File.Exists(filename)) { File.Delete(filename); }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("SettingsDialog", exception);
            }
            finally
            {
                this.Cursor = cursor;
            }
        }

        private void UpdateExtensionButtons()
        {
            this.btnRegister.Text = (this.extension.IsRegistered ? "U&pdate" : "&Register");
        }

        private void DrawIconShowFocus(Graphics graphics, Rectangle rectangle, Color darkColor, Color lightColor)
        {
            Int32 x = rectangle.X;
            Int32 y = rectangle.Y;
            Int32 width = rectangle.Width - 1;
            Int32 height = rectangle.Height - 1;
            Int32 radius = 2;

            Int32 xw = x + width;
            Int32 yh = y + height;
            Int32 xwr = xw - radius;
            Int32 yhr = yh - radius;
            Int32 xr = x + radius;
            Int32 yr = y + radius;
            Int32 r2 = radius * 2;
            Int32 xwr2 = xw - r2;
            Int32 yhr2 = yh - r2;

            using (GraphicsPath path = new GraphicsPath())
            {
                path.StartFigure();
                path.AddArc(x, y, r2, r2, 180, 90);     // Add rounded top/left corner.
                path.AddLine(xr, y, xwr, y);            // Add top edge.
                path.AddArc(xwr2, y, r2, r2, 270, 90);  // Add rounded top/right corner.
                path.AddLine(xw, yr, xw, yhr);          // Add right edge.
                path.AddArc(xwr2, yhr2, r2, r2, 0, 90); // Add rounded bottom/right corner.
                path.AddLine(xwr, yh, xr, yh);          // Add bottom edge.
                path.AddArc(x, yhr2, r2, r2, 90, 90);   // Add rounded bottom/left corner.
                path.AddLine(x, yhr, x, yr);            // Add left edge.
                path.CloseFigure();

                // Fill rounded rectangle.
                using (LinearGradientBrush brush = new LinearGradientBrush(rectangle, darkColor, lightColor, 90.0f, true))
                {
                    // Use a bell-shaped brush with the peak 
                    // at the center of the drawing area.
                    brush.SetSigmaBellShape(0.5f, 1.0f);

                    // Draw the rounded rectangle.
                    graphics.FillPath(brush, path);

                    using (Pen pen = new Pen(darkColor, 1f))
                    {
                        graphics.DrawPath(pen, path);
                    }
                }
            }
        }

        #endregion // Private helper function (shell extension) implementation.
    }

    internal class RichTextBoxEx : RichTextBox
    {
        public RichTextBoxEx()
            : base()
        {
            // Wow, that's really easy!
            this.DetectUrls = true;
            this.LinkClicked += new LinkClickedEventHandler(this.OnLinkClicked);

            base.BorderStyle = BorderStyle.None;
        }

        private BorderStyle borderStyle = BorderStyle.Fixed3D;
        public new BorderStyle BorderStyle
        {
            get
            {
                return this.borderStyle;
            }
            set
            {
                // Using the border style in this way does not really work!  
                // But its good enough for the purpose of having a single  
                // border instead of this kind of ugly original border.
                this.borderStyle = value;
            }
        }

        /// <summary>
        /// Gets the required creation parameters when the control handle 
        /// is created.
        /// </summary>
        /// <remarks>
        /// An instance of class <i>CreateParams</i> that contains the 
        /// required creation parameters when the handle to the control 
        /// is created.
        /// </remarks>
        /// <value>
        /// The value of property <i>CreateParams</i>. The constant 
        /// <c>WS_BORDER</c> or <c>WS_EX_STATICEDGE</c> is added 
        /// depending on current border style. 
        /// </value>
        /// <seealso cref="BorderStyle"/>
        protected override CreateParams CreateParams
        {
            get
            {
                // Very smart because client rectangle is also adjusted! 
                // See also: http://support.microsoft.com/kb/316574
                const Int32 WS_BORDER = unchecked(0x00800000);
                const Int32 WS_EX_STATICEDGE = unchecked(0x00020000);

                CreateParams createParams = base.CreateParams;
                createParams.ExStyle &= (~WS_EX_STATICEDGE);
                createParams.Style &= (~WS_BORDER);

                switch (this.BorderStyle)
                {
                    case BorderStyle.Fixed3D:
                        createParams.ExStyle |= WS_EX_STATICEDGE;
                        break;
                    case BorderStyle.FixedSingle:
                        createParams.Style |= WS_BORDER;
                        break;
                }
                return createParams;
            }
        }

        private void OnLinkClicked(Object sender, LinkClickedEventArgs args)
        {
            Process.Start(args.LinkText);
        }
    }

    internal class TabControlEx : TabControl
    {
        protected override CreateParams CreateParams
        {
            get
            {
                // NOTE: The WS_EX_COMPOSITED style is disabled under Win XP.
                // Setting the COMPOSITED window style works very fine under Window 7. But 
                // under Windows XP, this style causes some sub-controls to be drawn very 
                // ugly. Additionally, the resizing behaviour is also very bad. Therefore, 
                // this feature is not supported for Windows XP, with the result that the 
                // Setting dialog flickers a bit.
                CreateParams cp = base.CreateParams;
                if (PlatformCheck.IsVistaOrHigher)
                {
                    // This makes the tab control flicker free.
                    cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                }
                return cp;
            }
        }
    }

    internal class FocusCheckBox : CheckBox
    {
        protected override Boolean ShowFocusCues
        {
            get
            {
                // Return true to ensure the focus rectangle.
                return true;
            }
        }
    }
}
