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
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;

using plexdata.Utilities;

namespace Plexdata.FileWiper
{
    public partial class MainForm : Form
    {
        private class TrayIconOverlay
        {
            public TrayIconOverlay()
                : this(SystemInformation.SmallIconSize)
            {
            }

            public TrayIconOverlay(Size iconSize)
                : base()
            {
                this.MoveRight = false;
                this.Delta = 1;
                this.Width = 6;
                this.Height = 6;
                this.Left = 0;
                this.Top = iconSize.Height - this.Height;
                this.MaxOffset = iconSize.Width - this.Width;
            }

            public bool MoveRight { get; set; }

            public int MaxOffset { get; private set; }

            public int Delta { get; private set; }

            public int Left { get; set; }

            public int Top { get; private set; }

            public int Width { get; private set; }

            public int Height { get; private set; }

            public Rectangle BoundsWiping
            {
                get
                {
                    return new Rectangle(this.Left, this.Top, this.Width, this.Height);
                }
            }

            public Rectangle[] BoundsPausing
            {
                get
                {
                    return new Rectangle[]{
                        new Rectangle(0, this.Top, this.Width / 3, this.Height),
                        new Rectangle(this.Width / 3 * 2, this.Top, this.Width / 3, this.Height),
                    };
                }
            }

            public void Move()
            {
                if (!this.MoveRight)
                {
                    //if (this.Left < this.MaxOffset - 1)
                    if (this.Left < this.MaxOffset)
                    {
                        this.Left += this.Delta;
                    }
                    else
                    {
                        this.Left -= this.Delta;
                        this.MoveRight = true;
                    }
                }
                else
                {
                    if (this.Left > 0)
                    {
                        this.Left -= this.Delta;
                    }
                    else
                    {
                        this.Left += this.Delta;
                        this.MoveRight = false;
                    }
                }
            }
        }

        private TrayIconOverlay trayIconOverlay = null;

        private Icon trayIconIcon = null;

        private void InitializeTrayIcon()
        {
            Size scaling = SystemInformation.SmallIconSize;

            this.trayIconIcon = new Icon(Properties.Resources.Recycle1, scaling);
            this.trayIconOverlay = new TrayIconOverlay(scaling);

            this.trayIcon.Icon = this.trayIconIcon;
            this.trayIconMenuShow.Image = new Icon(Properties.Resources.Show, scaling).ToBitmap();
            this.trayIconMenuCancel.Image = new Icon(Properties.Resources.Cancel, scaling).ToBitmap();
            this.trayIconMenuPause.Image = new Icon(Properties.Resources.Pause, scaling).ToBitmap();
            this.trayIconMenuContinue.Image = new Icon(Properties.Resources.Continue, scaling).ToBitmap();
            this.trayIconMenuAbout.Image = new Icon(Properties.Resources.About, scaling).ToBitmap();
        }

        private void UpdateTrayIconMenuState()
        {
            bool wiping = this.IsWiping;
            this.trayIconMenuCancel.Enabled = wiping;
            this.trayIconMenuPause.Enabled = wiping && !this.IsPausing;
            this.trayIconMenuContinue.Enabled = wiping && this.IsPausing;
        }

        private void TrayIconShow()
        {
            if (!this.trayIcon.Visible)
            {
                this.trayIcon.Visible = true;
                this.trayIcon.Icon = this.trayIconIcon;
                this.trayIconUpdater.Start();
            }
        }

        private void TrayIconHide()
        {
            if (this.trayIcon.Visible)
            {
                this.trayIcon.Visible = false;
                this.trayIcon.Icon = this.trayIconIcon;
                this.trayIconUpdater.Stop();
            }
        }

        private void TrayIconChangeTooltip(double wipedFileSize, double totalFileSize)
        {
            this.TrayIconChangeTooltip(wipedFileSize, totalFileSize, 0);
        }

        private void TrayIconChangeTooltip(double wipedFileSize, double totalFileSize, int digits)
        {
            try
            {
                double progress = Math.Round(
                    // Avoid division by zero for maximum value.
                    (wipedFileSize * 100 / Math.Max(totalFileSize, 1)),
                    // Math.Round() causes an ArgumentOutOfRangeException 
                    // if digits is less than 0 or greater than 15. 
                    Math.Max(Math.Min(digits, 15), 0),
                    MidpointRounding.AwayFromZero);

                string tooltip = String.Format(
                    "{0}: {1}%, Wiped {2}, Total {3}",
                    Application.ProductName,
                    progress.ToString("N0"),
                    CapacityConverter.Convert(wipedFileSize),
                    CapacityConverter.Convert(totalFileSize));

                // See MSDN, property NotifyIcon.Text throws an ArgumentException 
                // when the ToolTip text longer thant 63 characters.
                if (tooltip.Length < 64) { this.trayIcon.Text = tooltip; }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("MainForm.TrayIcon", exception);
            }
        }

        #region Tray icon context menu event handler implementation.

        private void OnTrayIconMenuOpening(object sender, CancelEventArgs args)
        {
            this.UpdateTrayIconMenuState();
        }

        private void OnTrayIconMouseClick(object sender, MouseEventArgs args)
        {
            if (args.Button == MouseButtons.Left)
            {
                this.trayIconMenuShow.PerformClick();
            }
        }

        private void OnTrayIconMenuContinueClick(object sender, EventArgs args)
        {
            this.ContinueWipings();
        }

        private void OnTrayIconMenuPauseClick(object sender, EventArgs args)
        {
            this.SuspendWipings();
        }

        private void OnTrayIconMenuCancelClick(object sender, EventArgs args)
        {
            this.ForceShowForm();
            if (this.Settings.Behaviour.SuppressCancelQuestion)
            {
                this.CancelWipings();
            }
            else
            {
                this.RequestCancelWipings();
            }
        }

        private void OnTrayIconMenuShowClick(object sender, EventArgs args)
        {
            this.ForceShowForm();
            if (this.Settings.Behaviour.AutoPauseWiping)
            {
                this.SuspendWipings();
            }
        }

        private void OnTrayIconMenuAboutClick(object sender, EventArgs args)
        {
            this.PerformAboutBox();
        }

        #endregion // Tray icon context menu event handler implementation.

        #region Tray icon update timer event handler implementation.

        private void OnTrayIconUpdaterTick(object sender, EventArgs args)
        {
            // Once again spending a lot of hours to find out how to draw an icon on a bitmap 
            // without having an ugly result? But unfortunately, without success! Damn bitmap. 
            // Therefore, the solution in here is not really satisfying but it should be good 
            // enough for all people, that don't look that close on the tray icon.
            try
            {
                if (this.IsWiping)
                {
                    Rectangle bounds = new Rectangle(Point.Empty, SystemInformation.SmallIconSize);

                    using (Bitmap bitmap = new Bitmap(this.trayIconIcon.Width, this.trayIconIcon.Height))
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.DrawIconUnstretched(this.trayIconIcon, bounds);
                        if (this.IsPausing)
                        {
                            graphics.FillRectangles(Brushes.Navy, this.trayIconOverlay.BoundsPausing);
                        }
                        else
                        {
                            Rectangle rect = this.trayIconOverlay.BoundsWiping;
                            graphics.FillRectangle(Brushes.OrangeRed, rect);
                            rect.Height -= 1; rect.Width -= 1;
                            graphics.DrawRectangle(Pens.Orange, rect);

                            this.trayIconOverlay.Move();
                        }
                        this.trayIcon.Icon = Icon.FromHandle(bitmap.GetHicon());
                    }
                }
                else
                {
                    this.trayIcon.Icon = this.trayIconIcon;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("MainForm.TrayIcon", exception);
            }
        }

        #endregion // Tray icon update timer event handler implementation.
    }
}
