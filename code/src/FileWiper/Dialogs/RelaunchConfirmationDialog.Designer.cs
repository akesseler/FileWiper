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
    partial class RelaunchConfirmationDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RelaunchConfirmationDialog));
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnShow = new System.Windows.Forms.Button();
            this.btnRelaunch = new System.Windows.Forms.Button();
            this.picIconBox = new System.Windows.Forms.PictureBox();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picIconBox)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMessage.AutoEllipsis = true;
            this.lblMessage.Location = new System.Drawing.Point(98, 12);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(284, 80);
            this.lblMessage.TabIndex = 3;
            this.lblMessage.Text = "???";
            // 
            // btnShow
            // 
            this.btnShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShow.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnShow.Location = new System.Drawing.Point(145, 97);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(75, 23);
            this.btnShow.TabIndex = 1;
            this.btnShow.Text = "&Show";
            this.btnShow.UseVisualStyleBackColor = true;
            // 
            // btnRelaunch
            // 
            this.btnRelaunch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRelaunch.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnRelaunch.Location = new System.Drawing.Point(226, 97);
            this.btnRelaunch.Name = "btnRelaunch";
            this.btnRelaunch.Size = new System.Drawing.Size(75, 23);
            this.btnRelaunch.TabIndex = 2;
            this.btnRelaunch.Text = "&Relaunch";
            this.btnRelaunch.UseVisualStyleBackColor = true;
            // 
            // picIconBox
            // 
            this.picIconBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picIconBox.Image = ((System.Drawing.Image)(resources.GetObject("picIconBox.Image")));
            this.picIconBox.Location = new System.Drawing.Point(12, 12);
            this.picIconBox.Name = "picIconBox";
            this.picIconBox.Size = new System.Drawing.Size(80, 80);
            this.picIconBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picIconBox.TabIndex = 0;
            this.picIconBox.TabStop = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(307, 97);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // RelaunchConfirmationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(394, 132);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.picIconBox);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnShow);
            this.Controls.Add(this.btnRelaunch);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RelaunchConfirmationDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "???";
            ((System.ComponentModel.ISupportInitialize)(this.picIconBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picIconBox;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.Button btnRelaunch;
        private System.Windows.Forms.Button btnCancel;
    }
}