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
    partial class ErrorDialog
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
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.clipboardLink = new System.Windows.Forms.LinkLabel();
            this.closeButton = new System.Windows.Forms.Button();
            this.messageIcon = new System.Windows.Forms.PictureBox();
            this.messageText = new System.Windows.Forms.Label();
            this.messagePanel = new System.Windows.Forms.Panel();
            this.errorText = new System.Windows.Forms.TextBox();
            this.errorPanel = new System.Windows.Forms.Panel();
            this.buttonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.messageIcon)).BeginInit();
            this.messagePanel.SuspendLayout();
            this.errorPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonPanel
            // 
            this.buttonPanel.BackColor = System.Drawing.Color.White;
            this.buttonPanel.Controls.Add(this.clipboardLink);
            this.buttonPanel.Controls.Add(this.closeButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 253);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Padding = new System.Windows.Forms.Padding(8);
            this.buttonPanel.Size = new System.Drawing.Size(444, 39);
            this.buttonPanel.TabIndex = 0;
            // 
            // clipboardLink
            // 
            this.clipboardLink.Dock = System.Windows.Forms.DockStyle.Left;
            this.clipboardLink.Location = new System.Drawing.Point(8, 8);
            this.clipboardLink.Name = "clipboardLink";
            this.clipboardLink.Size = new System.Drawing.Size(337, 23);
            this.clipboardLink.TabIndex = 1;
            this.clipboardLink.TabStop = true;
            this.clipboardLink.Text = "Copy to Clipboard";
            this.clipboardLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.clipboardLink.Visible = false;
            this.clipboardLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnClipboardLinkClicked);
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.closeButton.Location = new System.Drawing.Point(361, 8);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.OnCloseButtonClick);
            // 
            // messageIcon
            // 
            this.messageIcon.Dock = System.Windows.Forms.DockStyle.Left;
            this.messageIcon.Location = new System.Drawing.Point(8, 8);
            this.messageIcon.Name = "messageIcon";
            this.messageIcon.Size = new System.Drawing.Size(32, 39);
            this.messageIcon.TabIndex = 1;
            this.messageIcon.TabStop = false;
            // 
            // messageText
            // 
            this.messageText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messageText.Location = new System.Drawing.Point(40, 8);
            this.messageText.Name = "messageText";
            this.messageText.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.messageText.Size = new System.Drawing.Size(396, 39);
            this.messageText.TabIndex = 0;
            this.messageText.Text = "???";
            // 
            // messagePanel
            // 
            this.messagePanel.Controls.Add(this.messageText);
            this.messagePanel.Controls.Add(this.messageIcon);
            this.messagePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.messagePanel.Location = new System.Drawing.Point(0, 0);
            this.messagePanel.Name = "messagePanel";
            this.messagePanel.Padding = new System.Windows.Forms.Padding(8);
            this.messagePanel.Size = new System.Drawing.Size(444, 55);
            this.messagePanel.TabIndex = 1;
            // 
            // errorText
            // 
            this.errorText.BackColor = System.Drawing.Color.White;
            this.errorText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorText.Location = new System.Drawing.Point(8, 0);
            this.errorText.Multiline = true;
            this.errorText.Name = "errorText";
            this.errorText.ReadOnly = true;
            this.errorText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.errorText.Size = new System.Drawing.Size(428, 198);
            this.errorText.TabIndex = 0;
            this.errorText.TabStop = false;
            this.errorText.WordWrap = false;
            // 
            // errorPanel
            // 
            this.errorPanel.BackColor = System.Drawing.Color.White;
            this.errorPanel.Controls.Add(this.errorText);
            this.errorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorPanel.Location = new System.Drawing.Point(0, 55);
            this.errorPanel.Name = "errorPanel";
            this.errorPanel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.errorPanel.Size = new System.Drawing.Size(444, 198);
            this.errorPanel.TabIndex = 2;
            // 
            // ErrorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(444, 292);
            this.Controls.Add(this.errorPanel);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.messagePanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(460, 330);
            this.Name = "ErrorDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " ";
            this.Load += new System.EventHandler(this.OnLoad);
            this.buttonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.messageIcon)).EndInit();
            this.messagePanel.ResumeLayout(false);
            this.errorPanel.ResumeLayout(false);
            this.errorPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.PictureBox messageIcon;
        private System.Windows.Forms.Label messageText;
        private System.Windows.Forms.Panel messagePanel;
        private System.Windows.Forms.TextBox errorText;
        private System.Windows.Forms.Panel errorPanel;
        private System.Windows.Forms.LinkLabel clipboardLink;
    }
}