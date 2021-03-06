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

using Plexdata.Utilities;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Plexdata.FileWiper
{
    internal partial class DetailsView : Form
    {
        private readonly WipingListView wipingList = null;

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

        private void OnStateCountsValuesChanged(Object sender, EventArgs args)
        {
            if (sender is WipingStateCounts counts)
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

        private void OnOverallCountsValuesChanged(Object sender, EventArgs args)
        {
            if (sender is WipingOverallCounts counts)
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

        private String FormatValue(Double value)
        {
            return this.FormatValue(value, 0);
        }

        private String FormatValue(Double value, Int32 digits)
        {
            String format = "N" + Math.Max(Math.Min(digits, 15), 0).ToString();
            return value.ToString(format, NumberFormatInfo.CurrentInfo);
        }

        #endregion // Private member function section.
    }
}
