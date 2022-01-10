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
using System.Globalization;
using System.Windows.Forms;

using plexdata.Utilities;

namespace plexdata.FileWiper
{
    internal partial class DetailsView : Form
    {
        private WipingListView wipingList = null;

        public DetailsView()
            : base()
        {
            this.InitializeComponent();
            this.InitializeControls();
        }

        public DetailsView(WipingListView wipingList)
            : this()
        {
            this.wipingList = wipingList;
            if (this.wipingList != null)
            {
                this.wipingList.StateCounts.ValuesChanged += this.OnStateCountsValuesChanged;
                this.wipingList.OverallCounts.ValuesChanged += this.OnOverallCountsValuesChanged;
            }
        }

        public void SetAlgorithm(WipingAlgorithm algorithm)
        {
            if (algorithm != null)
            {
                // Adjust depending value labels.
                this.lblWipingAlgorithmValue.Text = algorithm.Display.Replace("&", "&&");
                this.lblWipingRepeatsValue.Text = this.FormatValue(algorithm.Repeats);
            }
        }

        #region Overwritten event handler section.

        protected override void OnLoad(EventArgs args)
        {
            if (Settings.IsVisibleOnAllScreens(Program.MainForm.Settings.Maintain.DetailsBounds))
            {
                // Apply last known dialog size and location if possible.
                this.DesktopBounds = Program.MainForm.Settings.Maintain.DetailsBounds;
            }
            else
            {
                // Otherwise place the dialog box onto the right upper corner.
                if (this.wipingList != null)
                {
                    Point offset = this.wipingList.PointToScreen(new Point());
                    Rectangle display = this.wipingList.ClientRectangle;

                    offset.X += display.Width - this.Width;
                    offset.Y += display.Top;

                    this.Location = offset;
                }
            }

            base.OnLoad(args);
        }

        protected override void OnFormClosing(FormClosingEventArgs args)
        {
            // Save last known size and location.
            Program.MainForm.Settings.Maintain.DetailsBounds = this.DesktopBounds;

            base.OnFormClosing(args);
        }

        #endregion // Overwritten event handler section.

        #region Private event handler section.

        private void OnStateCountsValuesChanged(object sender, EventArgs args)
        {
            WipingStateCounts counts = sender as WipingStateCounts;
            if (counts != null)
            {
                this.lblRunningLabel.Text = counts.Pausing == 0 ? "Running" : "Pausing";

                this.lblPendingValue.Text = this.FormatValue(counts.Pending);
                this.lblRunningValue.Text = this.FormatValue(counts.Pausing + counts.Processing);
                this.lblFinishedValue.Text = this.FormatValue(counts.Finished);
                this.lblCanceledValue.Text = this.FormatValue(counts.Canceled);
                this.lblFailedValue.Text = this.FormatValue(counts.Failed);
                this.lblMissingValue.Text = this.FormatValue(counts.Missing);
            }
        }

        private void OnOverallCountsValuesChanged(object sender, EventArgs args)
        {
            WipingOverallCounts counts = sender as WipingOverallCounts;
            if (counts != null)
            {
                this.lblTotalFileSizeValue.Text = CapacityConverter.Convert(counts.TotalFileSize);
                this.lblWipedFileSizeValue.Text = CapacityConverter.Convert(counts.WipedFileSize);
                this.lblTotalWipedSizeValue.Text = CapacityConverter.Convert(counts.TotalWipedSize);
            }
        }

        #endregion // Private event handler section.

        #region Private member function section.

        private void InitializeControls()
        {
            this.lblPendingValue.Text = this.FormatValue(0d);
            this.lblRunningValue.Text = this.FormatValue(0d);
            this.lblFinishedValue.Text = this.FormatValue(0d);
            this.lblCanceledValue.Text = this.FormatValue(0d);
            this.lblFailedValue.Text = this.FormatValue(0d);
            this.lblMissingValue.Text = this.FormatValue(0d);
            this.lblTotalFileSizeValue.Text = CapacityConverter.Convert(0d);
            this.lblWipedFileSizeValue.Text = CapacityConverter.Convert(0d);
            this.lblTotalWipedSizeValue.Text = CapacityConverter.Convert(0d);
        }

        private string FormatValue(double value)
        {
            return this.FormatValue(value, 0);
        }

        private string FormatValue(double value, int digits)
        {
            string format = "N" + Math.Max(Math.Min(digits, 15), 0).ToString();
            return value.ToString(format, NumberFormatInfo.CurrentInfo);
        }

        #endregion // Private member function section.
    }
}
