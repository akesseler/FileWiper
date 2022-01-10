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

namespace plexdata.FileWiper
{
    partial class DetailsView
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
            this.lblPendingLabel = new System.Windows.Forms.Label();
            this.lblRunningLabel = new System.Windows.Forms.Label();
            this.lblFinishedLabel = new System.Windows.Forms.Label();
            this.lblCanceledLabel = new System.Windows.Forms.Label();
            this.lblFailedLabel = new System.Windows.Forms.Label();
            this.lblMissingLabel = new System.Windows.Forms.Label();
            this.lblMissingValue = new System.Windows.Forms.Label();
            this.lblFailedValue = new System.Windows.Forms.Label();
            this.lblCanceledValue = new System.Windows.Forms.Label();
            this.lblFinishedValue = new System.Windows.Forms.Label();
            this.lblRunningValue = new System.Windows.Forms.Label();
            this.lblPendingValue = new System.Windows.Forms.Label();
            this.lblTotalFileSizeLabel = new System.Windows.Forms.Label();
            this.lblTotalFileSizeValue = new System.Windows.Forms.Label();
            this.lblWipedFileSizeLabel = new System.Windows.Forms.Label();
            this.lblWipedFileSizeValue = new System.Windows.Forms.Label();
            this.lblWipingRepeatsLabel = new System.Windows.Forms.Label();
            this.lblWipingRepeatsValue = new System.Windows.Forms.Label();
            this.lblWipingAlgorithmValue = new System.Windows.Forms.Label();
            this.lblWipingAlgorithmLabel = new System.Windows.Forms.Label();
            this.grpWipingDetails = new System.Windows.Forms.GroupBox();
            this.lblTotalWipedSizeValue = new System.Windows.Forms.Label();
            this.lblTotalWipedSizeLabel = new System.Windows.Forms.Label();
            this.grpDiskSizeDetails = new System.Windows.Forms.GroupBox();
            this.grpProcessingDetails = new System.Windows.Forms.GroupBox();
            this.grpWipingDetails.SuspendLayout();
            this.grpDiskSizeDetails.SuspendLayout();
            this.grpProcessingDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPendingLabel
            // 
            this.lblPendingLabel.AutoSize = true;
            this.lblPendingLabel.Location = new System.Drawing.Point(6, 19);
            this.lblPendingLabel.Margin = new System.Windows.Forms.Padding(3);
            this.lblPendingLabel.Name = "lblPendingLabel";
            this.lblPendingLabel.Size = new System.Drawing.Size(46, 13);
            this.lblPendingLabel.TabIndex = 0;
            this.lblPendingLabel.Text = "Pending";
            this.lblPendingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRunningLabel
            // 
            this.lblRunningLabel.AutoSize = true;
            this.lblRunningLabel.Location = new System.Drawing.Point(6, 40);
            this.lblRunningLabel.Margin = new System.Windows.Forms.Padding(3);
            this.lblRunningLabel.Name = "lblRunningLabel";
            this.lblRunningLabel.Size = new System.Drawing.Size(47, 13);
            this.lblRunningLabel.TabIndex = 1;
            this.lblRunningLabel.Text = "Running";
            this.lblRunningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFinishedLabel
            // 
            this.lblFinishedLabel.AutoSize = true;
            this.lblFinishedLabel.Location = new System.Drawing.Point(6, 61);
            this.lblFinishedLabel.Margin = new System.Windows.Forms.Padding(3);
            this.lblFinishedLabel.Name = "lblFinishedLabel";
            this.lblFinishedLabel.Size = new System.Drawing.Size(46, 13);
            this.lblFinishedLabel.TabIndex = 2;
            this.lblFinishedLabel.Text = "Finished";
            this.lblFinishedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCanceledLabel
            // 
            this.lblCanceledLabel.AutoSize = true;
            this.lblCanceledLabel.Location = new System.Drawing.Point(6, 82);
            this.lblCanceledLabel.Margin = new System.Windows.Forms.Padding(3);
            this.lblCanceledLabel.Name = "lblCanceledLabel";
            this.lblCanceledLabel.Size = new System.Drawing.Size(52, 13);
            this.lblCanceledLabel.TabIndex = 3;
            this.lblCanceledLabel.Text = "Canceled";
            this.lblCanceledLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFailedLabel
            // 
            this.lblFailedLabel.AutoSize = true;
            this.lblFailedLabel.Location = new System.Drawing.Point(6, 103);
            this.lblFailedLabel.Margin = new System.Windows.Forms.Padding(3);
            this.lblFailedLabel.Name = "lblFailedLabel";
            this.lblFailedLabel.Size = new System.Drawing.Size(35, 13);
            this.lblFailedLabel.TabIndex = 4;
            this.lblFailedLabel.Text = "Failed";
            this.lblFailedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblMissingLabel
            // 
            this.lblMissingLabel.AutoSize = true;
            this.lblMissingLabel.Location = new System.Drawing.Point(6, 124);
            this.lblMissingLabel.Margin = new System.Windows.Forms.Padding(3);
            this.lblMissingLabel.Name = "lblMissingLabel";
            this.lblMissingLabel.Size = new System.Drawing.Size(42, 13);
            this.lblMissingLabel.TabIndex = 5;
            this.lblMissingLabel.Text = "Missing";
            this.lblMissingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblMissingValue
            // 
            this.lblMissingValue.Location = new System.Drawing.Point(93, 124);
            this.lblMissingValue.Margin = new System.Windows.Forms.Padding(3);
            this.lblMissingValue.Name = "lblMissingValue";
            this.lblMissingValue.Size = new System.Drawing.Size(110, 15);
            this.lblMissingValue.TabIndex = 11;
            this.lblMissingValue.Text = "???";
            this.lblMissingValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFailedValue
            // 
            this.lblFailedValue.Location = new System.Drawing.Point(93, 103);
            this.lblFailedValue.Margin = new System.Windows.Forms.Padding(3);
            this.lblFailedValue.Name = "lblFailedValue";
            this.lblFailedValue.Size = new System.Drawing.Size(110, 15);
            this.lblFailedValue.TabIndex = 10;
            this.lblFailedValue.Text = "???";
            this.lblFailedValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCanceledValue
            // 
            this.lblCanceledValue.Location = new System.Drawing.Point(93, 82);
            this.lblCanceledValue.Margin = new System.Windows.Forms.Padding(3);
            this.lblCanceledValue.Name = "lblCanceledValue";
            this.lblCanceledValue.Size = new System.Drawing.Size(110, 15);
            this.lblCanceledValue.TabIndex = 9;
            this.lblCanceledValue.Text = "???";
            this.lblCanceledValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFinishedValue
            // 
            this.lblFinishedValue.Location = new System.Drawing.Point(93, 61);
            this.lblFinishedValue.Margin = new System.Windows.Forms.Padding(3);
            this.lblFinishedValue.Name = "lblFinishedValue";
            this.lblFinishedValue.Size = new System.Drawing.Size(110, 15);
            this.lblFinishedValue.TabIndex = 8;
            this.lblFinishedValue.Text = "???";
            this.lblFinishedValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRunningValue
            // 
            this.lblRunningValue.Location = new System.Drawing.Point(93, 40);
            this.lblRunningValue.Margin = new System.Windows.Forms.Padding(3);
            this.lblRunningValue.Name = "lblRunningValue";
            this.lblRunningValue.Size = new System.Drawing.Size(110, 15);
            this.lblRunningValue.TabIndex = 7;
            this.lblRunningValue.Text = "???";
            this.lblRunningValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPendingValue
            // 
            this.lblPendingValue.Location = new System.Drawing.Point(93, 19);
            this.lblPendingValue.Margin = new System.Windows.Forms.Padding(3);
            this.lblPendingValue.Name = "lblPendingValue";
            this.lblPendingValue.Size = new System.Drawing.Size(110, 15);
            this.lblPendingValue.TabIndex = 6;
            this.lblPendingValue.Text = "???";
            this.lblPendingValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalFileSizeLabel
            // 
            this.lblTotalFileSizeLabel.AutoSize = true;
            this.lblTotalFileSizeLabel.Location = new System.Drawing.Point(6, 19);
            this.lblTotalFileSizeLabel.Margin = new System.Windows.Forms.Padding(3);
            this.lblTotalFileSizeLabel.Name = "lblTotalFileSizeLabel";
            this.lblTotalFileSizeLabel.Size = new System.Drawing.Size(73, 13);
            this.lblTotalFileSizeLabel.TabIndex = 13;
            this.lblTotalFileSizeLabel.Text = "Total File Size";
            this.lblTotalFileSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalFileSizeValue
            // 
            this.lblTotalFileSizeValue.Location = new System.Drawing.Point(93, 19);
            this.lblTotalFileSizeValue.Margin = new System.Windows.Forms.Padding(3);
            this.lblTotalFileSizeValue.Name = "lblTotalFileSizeValue";
            this.lblTotalFileSizeValue.Size = new System.Drawing.Size(110, 15);
            this.lblTotalFileSizeValue.TabIndex = 14;
            this.lblTotalFileSizeValue.Text = "???";
            this.lblTotalFileSizeValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblWipedFileSizeLabel
            // 
            this.lblWipedFileSizeLabel.AutoSize = true;
            this.lblWipedFileSizeLabel.Location = new System.Drawing.Point(6, 40);
            this.lblWipedFileSizeLabel.Margin = new System.Windows.Forms.Padding(3);
            this.lblWipedFileSizeLabel.Name = "lblWipedFileSizeLabel";
            this.lblWipedFileSizeLabel.Size = new System.Drawing.Size(80, 13);
            this.lblWipedFileSizeLabel.TabIndex = 15;
            this.lblWipedFileSizeLabel.Text = "Processed Size";
            this.lblWipedFileSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblWipedFileSizeValue
            // 
            this.lblWipedFileSizeValue.Location = new System.Drawing.Point(93, 40);
            this.lblWipedFileSizeValue.Margin = new System.Windows.Forms.Padding(3);
            this.lblWipedFileSizeValue.Name = "lblWipedFileSizeValue";
            this.lblWipedFileSizeValue.Size = new System.Drawing.Size(110, 15);
            this.lblWipedFileSizeValue.TabIndex = 16;
            this.lblWipedFileSizeValue.Text = "???";
            this.lblWipedFileSizeValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblWipingRepeatsLabel
            // 
            this.lblWipingRepeatsLabel.AutoSize = true;
            this.lblWipingRepeatsLabel.Location = new System.Drawing.Point(6, 40);
            this.lblWipingRepeatsLabel.Margin = new System.Windows.Forms.Padding(3);
            this.lblWipingRepeatsLabel.Name = "lblWipingRepeatsLabel";
            this.lblWipingRepeatsLabel.Size = new System.Drawing.Size(47, 13);
            this.lblWipingRepeatsLabel.TabIndex = 15;
            this.lblWipingRepeatsLabel.Text = "Repeats";
            this.lblWipingRepeatsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblWipingRepeatsValue
            // 
            this.lblWipingRepeatsValue.Location = new System.Drawing.Point(94, 39);
            this.lblWipingRepeatsValue.Margin = new System.Windows.Forms.Padding(3);
            this.lblWipingRepeatsValue.Name = "lblWipingRepeatsValue";
            this.lblWipingRepeatsValue.Size = new System.Drawing.Size(110, 15);
            this.lblWipingRepeatsValue.TabIndex = 16;
            this.lblWipingRepeatsValue.Text = "???";
            this.lblWipingRepeatsValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblWipingAlgorithmValue
            // 
            this.lblWipingAlgorithmValue.Location = new System.Drawing.Point(93, 60);
            this.lblWipingAlgorithmValue.Margin = new System.Windows.Forms.Padding(3);
            this.lblWipingAlgorithmValue.Name = "lblWipingAlgorithmValue";
            this.lblWipingAlgorithmValue.Size = new System.Drawing.Size(110, 15);
            this.lblWipingAlgorithmValue.TabIndex = 18;
            this.lblWipingAlgorithmValue.Text = "???";
            this.lblWipingAlgorithmValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblWipingAlgorithmLabel
            // 
            this.lblWipingAlgorithmLabel.AutoSize = true;
            this.lblWipingAlgorithmLabel.Location = new System.Drawing.Point(6, 61);
            this.lblWipingAlgorithmLabel.Margin = new System.Windows.Forms.Padding(3);
            this.lblWipingAlgorithmLabel.Name = "lblWipingAlgorithmLabel";
            this.lblWipingAlgorithmLabel.Size = new System.Drawing.Size(50, 13);
            this.lblWipingAlgorithmLabel.TabIndex = 17;
            this.lblWipingAlgorithmLabel.Text = "Algorithm";
            this.lblWipingAlgorithmLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grpWipingDetails
            // 
            this.grpWipingDetails.Controls.Add(this.lblWipingRepeatsLabel);
            this.grpWipingDetails.Controls.Add(this.lblTotalWipedSizeValue);
            this.grpWipingDetails.Controls.Add(this.lblWipingAlgorithmValue);
            this.grpWipingDetails.Controls.Add(this.lblWipingRepeatsValue);
            this.grpWipingDetails.Controls.Add(this.lblTotalWipedSizeLabel);
            this.grpWipingDetails.Controls.Add(this.lblWipingAlgorithmLabel);
            this.grpWipingDetails.Location = new System.Drawing.Point(12, 235);
            this.grpWipingDetails.Name = "grpWipingDetails";
            this.grpWipingDetails.Size = new System.Drawing.Size(209, 84);
            this.grpWipingDetails.TabIndex = 19;
            this.grpWipingDetails.TabStop = false;
            this.grpWipingDetails.Text = "Wiping Details";
            // 
            // lblTotalWipedSizeValue
            // 
            this.lblTotalWipedSizeValue.Location = new System.Drawing.Point(93, 19);
            this.lblTotalWipedSizeValue.Margin = new System.Windows.Forms.Padding(3);
            this.lblTotalWipedSizeValue.Name = "lblTotalWipedSizeValue";
            this.lblTotalWipedSizeValue.Size = new System.Drawing.Size(110, 15);
            this.lblTotalWipedSizeValue.TabIndex = 16;
            this.lblTotalWipedSizeValue.Text = "???";
            this.lblTotalWipedSizeValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalWipedSizeLabel
            // 
            this.lblTotalWipedSizeLabel.AutoSize = true;
            this.lblTotalWipedSizeLabel.Location = new System.Drawing.Point(6, 19);
            this.lblTotalWipedSizeLabel.Margin = new System.Windows.Forms.Padding(3);
            this.lblTotalWipedSizeLabel.Name = "lblTotalWipedSizeLabel";
            this.lblTotalWipedSizeLabel.Size = new System.Drawing.Size(57, 13);
            this.lblTotalWipedSizeLabel.TabIndex = 15;
            this.lblTotalWipedSizeLabel.Text = "Processed";
            this.lblTotalWipedSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grpDiskSizeDetails
            // 
            this.grpDiskSizeDetails.Controls.Add(this.lblTotalFileSizeLabel);
            this.grpDiskSizeDetails.Controls.Add(this.lblWipedFileSizeLabel);
            this.grpDiskSizeDetails.Controls.Add(this.lblWipedFileSizeValue);
            this.grpDiskSizeDetails.Controls.Add(this.lblTotalFileSizeValue);
            this.grpDiskSizeDetails.Location = new System.Drawing.Point(12, 165);
            this.grpDiskSizeDetails.Name = "grpDiskSizeDetails";
            this.grpDiskSizeDetails.Size = new System.Drawing.Size(209, 64);
            this.grpDiskSizeDetails.TabIndex = 20;
            this.grpDiskSizeDetails.TabStop = false;
            this.grpDiskSizeDetails.Text = "Disk Size Details";
            // 
            // grpProcessingDetails
            // 
            this.grpProcessingDetails.Controls.Add(this.lblPendingLabel);
            this.grpProcessingDetails.Controls.Add(this.lblRunningLabel);
            this.grpProcessingDetails.Controls.Add(this.lblFinishedLabel);
            this.grpProcessingDetails.Controls.Add(this.lblCanceledLabel);
            this.grpProcessingDetails.Controls.Add(this.lblMissingValue);
            this.grpProcessingDetails.Controls.Add(this.lblFailedLabel);
            this.grpProcessingDetails.Controls.Add(this.lblFailedValue);
            this.grpProcessingDetails.Controls.Add(this.lblMissingLabel);
            this.grpProcessingDetails.Controls.Add(this.lblPendingValue);
            this.grpProcessingDetails.Controls.Add(this.lblCanceledValue);
            this.grpProcessingDetails.Controls.Add(this.lblRunningValue);
            this.grpProcessingDetails.Controls.Add(this.lblFinishedValue);
            this.grpProcessingDetails.Location = new System.Drawing.Point(12, 12);
            this.grpProcessingDetails.Name = "grpProcessingDetails";
            this.grpProcessingDetails.Size = new System.Drawing.Size(209, 147);
            this.grpProcessingDetails.TabIndex = 21;
            this.grpProcessingDetails.TabStop = false;
            this.grpProcessingDetails.Text = "Processing Details";
            // 
            // DetailsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(233, 331);
            this.Controls.Add(this.grpProcessingDetails);
            this.Controls.Add(this.grpDiskSizeDetails);
            this.Controls.Add(this.grpWipingDetails);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DetailsView";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Details";
            this.grpWipingDetails.ResumeLayout(false);
            this.grpWipingDetails.PerformLayout();
            this.grpDiskSizeDetails.ResumeLayout(false);
            this.grpDiskSizeDetails.PerformLayout();
            this.grpProcessingDetails.ResumeLayout(false);
            this.grpProcessingDetails.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblPendingLabel;
        private System.Windows.Forms.Label lblRunningLabel;
        private System.Windows.Forms.Label lblFinishedLabel;
        private System.Windows.Forms.Label lblCanceledLabel;
        private System.Windows.Forms.Label lblFailedLabel;
        private System.Windows.Forms.Label lblMissingLabel;
        private System.Windows.Forms.Label lblMissingValue;
        private System.Windows.Forms.Label lblFailedValue;
        private System.Windows.Forms.Label lblCanceledValue;
        private System.Windows.Forms.Label lblFinishedValue;
        private System.Windows.Forms.Label lblRunningValue;
        private System.Windows.Forms.Label lblPendingValue;
        private System.Windows.Forms.Label lblTotalFileSizeLabel;
        private System.Windows.Forms.Label lblTotalFileSizeValue;
        private System.Windows.Forms.Label lblWipedFileSizeLabel;
        private System.Windows.Forms.Label lblWipedFileSizeValue;
        private System.Windows.Forms.Label lblWipingRepeatsLabel;
        private System.Windows.Forms.Label lblWipingRepeatsValue;
        private System.Windows.Forms.Label lblWipingAlgorithmValue;
        private System.Windows.Forms.Label lblWipingAlgorithmLabel;
        private System.Windows.Forms.GroupBox grpWipingDetails;
        private System.Windows.Forms.Label lblTotalWipedSizeLabel;
        private System.Windows.Forms.Label lblTotalWipedSizeValue;
        private System.Windows.Forms.GroupBox grpDiskSizeDetails;
        private System.Windows.Forms.GroupBox grpProcessingDetails;
    }
}