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
using System.Windows.Forms;

namespace plexdata.FileWiper
{
    public partial class ErrorDialog : Form
    {
        private string caption = null;
        private string message = null;
        private Exception exception = null;

        public ErrorDialog()
            : base()
        {
            this.InitializeComponent();

            this.Icon = Properties.Resources.MainIcon;

            this.caption = "General Program Fault";
            this.message =
                "The program crashed with an unexpected internal error " +
                "and we apologize for that inconvenience. Would you be " +
                "so kind and email the error output below back to the " +
                "developer to let him know about this problem? Thanks " +
                "for your collaboration.";

            this.messageIcon.Image = Properties.Resources.Error.ToBitmap();
        }

        public ErrorDialog(Exception exception)
            : this()
        {
            this.exception = exception;
        }

        public ErrorDialog(Exception exception, string message)
            : this(exception)
        {
            this.message = message;
        }

        public ErrorDialog(Exception exception, string message, string caption)
            : this(exception, message)
        {
            this.caption = caption;
        }

        public string Caption
        {
            get { return this.caption; }
            set { this.caption = value; }
        }

        public string Message
        {
            get { return this.message; }
            set { this.message = value; }
        }

        public Exception Exception
        {
            get { return this.exception; }
            set { this.exception = value; }
        }

        private void OnLoad(object sender, EventArgs args)
        {
            if (!String.IsNullOrEmpty(this.caption))
            {
                base.Text = this.caption;
            }

            if (!String.IsNullOrEmpty(this.message))
            {
                this.messageText.Text = this.message;
            }

            if (this.exception != null)
            {
                this.errorText.Text = this.exception.ToString();
                this.clipboardLink.Visible = true;
            }
        }

        private void OnClipboardLinkClicked(object sender, LinkLabelLinkClickedEventArgs args)
        {
            Clipboard.SetText(this.errorText.Text);
            args.Link.Visited = true;
        }

        private void OnCloseButtonClick(object sender, EventArgs args)
        {
            this.Close();
        }
    }
}
