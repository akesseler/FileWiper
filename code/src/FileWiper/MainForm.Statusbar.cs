﻿/*
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

using plexdata.Controls;

namespace plexdata.FileWiper
{
    public partial class MainForm : Form
    {
        private ToolStripProgressBar3D sbcProgress = null;

        private void InitializeStatusBar()
        {
#if SIMULATION
            this.sblGeneral.Text = "SIMULATION";
#else
            this.sblGeneral.Text = String.Empty;
#endif // SIMULATION
            this.sblMode.Text = PermissionCheck.IsRunAsAdmin ? "Administrator" : "Standard";
            this.sblPlatform.Text = PlatformCheck.Is64BitPlatform ? "64 Bit" : "32 Bit";

            this.sbcProgress = new ToolStripProgressBar3D();
            this.sbcProgress.Visible = false;
            this.mainStatusbar.Items.Add(this.sbcProgress);
        }

        private void UpdateStatusbar()
        {
            this.UpdateStatusbar(String.Empty);
        }

        private void UpdateStatusbar(string label)
        {
            if (this.IsCanceling)
            {
                this.sblGeneral.Text = "Canceling";
            }
            else if (this.IsCanceled)
            {
                this.sblGeneral.Text = "Canceled";
            }
            else if (this.IsPausing)
            {
                this.sblGeneral.Text = "Pausing";
                if (this.powerModePausing) { this.sblGeneral.Text += " (power mode)"; }
            }
            else if (this.IsWiping)
            {
                this.sblGeneral.Text = "Wiping";
            }
            else
            {
                if (!String.IsNullOrEmpty(label))
                {
                    this.sblGeneral.Text = label;
                }
                else
                {
#if SIMULATION
                    this.sblGeneral.Text = "SIMULATION";
#else
                    this.sblGeneral.Text = String.Empty;
#endif // SIMULATION
                }
            }
        }

        private void UpdateStatusbar(WipingOverallCounts counts)
        {
            try
            {
                this.mainStatusbar.SuspendLayout();

                this.sbcProgress.Maximum = counts.TotalFileSize;
                this.sbcProgress.Value = counts.WipedFileSize;
                this.sbcProgress.Visible = this.sbcProgress.Value != this.sbcProgress.Maximum;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("MainForm.StatusBar", exception);
            }
            finally
            {
                this.mainStatusbar.ResumeLayout(true);
            }
        }
    }

    internal class ToolStripProgressBar3D : ToolStripControlHost
    {
        public ToolStripProgressBar3D()
            : this(String.Empty)
        {
        }

        public ToolStripProgressBar3D(string name)
            : base(new ProgressBar3D(), name)
        {
            if (this.ProgressBar == null)
            {
                throw new ArgumentNullException("inner control");
            }

            ProgressBar3D control = this.ProgressBar;

            control.Padding = new Padding(2, 3, 2, 5);
            control.Size = new Size(80, control.Size.Height);
            control.Font = new Font(this.Font.FontFamily, this.Font.Size - 1.5f);
            control.ForeColorLight = Color.FromArgb(128, 128, 255);
            control.ForeColorDark = Color.FromArgb(192, 192, 255);
            control.BackColorLight = Color.FromArgb(224, 224, 224);
            control.BackColorDark = Color.FromArgb(255, 255, 255);
            control.BorderColor = Color.FromArgb(192, 192, 192);
            control.TextColorLight = Color.FromArgb(0, 0, 64);
            control.TextColorDark = Color.FromArgb(0, 0, 64);

            control.Value = 0;
            control.Maximum = 0;
            control.Digits = 0;
            control.TextEnabled = false;
            control.NullTextEnabled = false;
        }

        public ProgressBar3D ProgressBar
        {
            get { return base.Control as ProgressBar3D; }
        }

        public double Value
        {
            get { return this.ProgressBar.Value; }
            set { this.ProgressBar.Value = value; }
        }

        public double Maximum
        {
            get { return this.ProgressBar.Maximum; }
            set { this.ProgressBar.Maximum = value; }
        }
    }
}