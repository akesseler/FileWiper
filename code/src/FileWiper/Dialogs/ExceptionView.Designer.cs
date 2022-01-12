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
    partial class ExceptionView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionView));
            this.tvcExceptionTree = new System.Windows.Forms.TreeView();
            this.btnClose = new System.Windows.Forms.Button();
            this.txtExceptionDetails = new System.Windows.Forms.RichTextBox();
            this.lblExceptionTree = new System.Windows.Forms.Label();
            this.panBorderHelper = new System.Windows.Forms.Panel();
            this.lblExceptionDetails = new System.Windows.Forms.Label();
            this.lnkCopyToClipboard = new System.Windows.Forms.LinkLabel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.picImage = new System.Windows.Forms.PictureBox();
            this.panBorderHelper.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).BeginInit();
            this.SuspendLayout();
            // 
            // tvcExceptionTree
            // 
            this.tvcExceptionTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tvcExceptionTree.BackColor = System.Drawing.Color.White;
            this.tvcExceptionTree.Location = new System.Drawing.Point(12, 88);
            this.tvcExceptionTree.Name = "tvcExceptionTree";
            this.tvcExceptionTree.Size = new System.Drawing.Size(200, 233);
            this.tvcExceptionTree.TabIndex = 1;
            this.tvcExceptionTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.OnTreeBeforeExpand);
            this.tvcExceptionTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnTreeAfterSelect);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(497, 327);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // txtExceptionDetails
            // 
            this.txtExceptionDetails.BackColor = System.Drawing.Color.White;
            this.txtExceptionDetails.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtExceptionDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtExceptionDetails.Location = new System.Drawing.Point(0, 0);
            this.txtExceptionDetails.Name = "txtExceptionDetails";
            this.txtExceptionDetails.ReadOnly = true;
            this.txtExceptionDetails.Size = new System.Drawing.Size(352, 231);
            this.txtExceptionDetails.TabIndex = 0;
            this.txtExceptionDetails.Text = "";
            this.txtExceptionDetails.WordWrap = false;
            this.txtExceptionDetails.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.OnDetailsLinkClicked);
            // 
            // lblExceptionTree
            // 
            this.lblExceptionTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExceptionTree.AutoSize = true;
            this.lblExceptionTree.Location = new System.Drawing.Point(12, 72);
            this.lblExceptionTree.Name = "lblExceptionTree";
            this.lblExceptionTree.Size = new System.Drawing.Size(59, 13);
            this.lblExceptionTree.TabIndex = 0;
            this.lblExceptionTree.Text = "&Exceptions";
            // 
            // panBorderHelper
            // 
            this.panBorderHelper.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panBorderHelper.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panBorderHelper.Controls.Add(this.txtExceptionDetails);
            this.panBorderHelper.Location = new System.Drawing.Point(218, 88);
            this.panBorderHelper.Name = "panBorderHelper";
            this.panBorderHelper.Size = new System.Drawing.Size(354, 233);
            this.panBorderHelper.TabIndex = 2;
            // 
            // lblExceptionDetails
            // 
            this.lblExceptionDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExceptionDetails.AutoSize = true;
            this.lblExceptionDetails.Location = new System.Drawing.Point(215, 72);
            this.lblExceptionDetails.Name = "lblExceptionDetails";
            this.lblExceptionDetails.Size = new System.Drawing.Size(39, 13);
            this.lblExceptionDetails.TabIndex = 2;
            this.lblExceptionDetails.Text = "&Details";
            // 
            // lnkCopyToClipboard
            // 
            this.lnkCopyToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkCopyToClipboard.AutoSize = true;
            this.lnkCopyToClipboard.Location = new System.Drawing.Point(12, 332);
            this.lnkCopyToClipboard.Name = "lnkCopyToClipboard";
            this.lnkCopyToClipboard.Size = new System.Drawing.Size(90, 13);
            this.lnkCopyToClipboard.TabIndex = 3;
            this.lnkCopyToClipboard.TabStop = true;
            this.lnkCopyToClipboard.Text = "Copy to Clipboard";
            this.lnkCopyToClipboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lnkCopyToClipboard.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnCopyToClipboardLinkClicked);
            // 
            // lblMessage
            // 
            this.lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMessage.Location = new System.Drawing.Point(68, 13);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(504, 56);
            this.lblMessage.TabIndex = 5;
            this.lblMessage.Text = "???";
            // 
            // picImage
            // 
            this.picImage.Image = ((System.Drawing.Image)(resources.GetObject("picImage.Image")));
            this.picImage.Location = new System.Drawing.Point(12, 12);
            this.picImage.Name = "picImage";
            this.picImage.Size = new System.Drawing.Size(50, 56);
            this.picImage.TabIndex = 6;
            this.picImage.TabStop = false;
            // 
            // ExceptionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(584, 362);
            this.Controls.Add(this.picImage);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.panBorderHelper);
            this.Controls.Add(this.lblExceptionTree);
            this.Controls.Add(this.lblExceptionDetails);
            this.Controls.Add(this.lnkCopyToClipboard);
            this.Controls.Add(this.tvcExceptionTree);
            this.Controls.Add(this.btnClose);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(900, 600);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "ExceptionView";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Exception View";
            this.panBorderHelper.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvcExceptionTree;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.RichTextBox txtExceptionDetails;
        private System.Windows.Forms.Label lblExceptionTree;
        private System.Windows.Forms.Label lblExceptionDetails;
        private System.Windows.Forms.Panel panBorderHelper;
        private System.Windows.Forms.LinkLabel lnkCopyToClipboard;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.PictureBox picImage;
    }
}