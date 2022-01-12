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
using System.Windows.Forms;

namespace Plexdata.FileWiper
{
    public partial class ErrorDialog : Form
    {
        private String caption = null;
        private String message = null;
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

        public ErrorDialog(Exception exception, String message)
            : this(exception)
        {
            this.message = message;
        }

        public ErrorDialog(Exception exception, String message, String caption)
            : this(exception, message)
        {
            this.caption = caption;
        }

        public String Caption
        {
            get { return this.caption; }
            set { this.caption = value; }
        }

        public String Message
        {
            get { return this.message; }
            set { this.message = value; }
        }

        public Exception Exception
        {
            get { return this.exception; }
            set { this.exception = value; }
        }

        private void OnLoad(Object sender, EventArgs args)
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

        private void OnClipboardLinkClicked(Object sender, LinkLabelLinkClickedEventArgs args)
        {
            Clipboard.SetText(this.errorText.Text);
            args.Link.Visited = true;
        }

        private void OnCloseButtonClick(Object sender, EventArgs args)
        {
            this.Close();
        }
    }
}
