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
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace Plexdata.FileWiper
{
    public partial class ExceptionView : Form
    {
        #region Constructor section.

        public ExceptionView()
            : base()
        {
            this.InitializeComponent();
            this.Icon = Properties.Resources.MainIcon;
            this.txtExceptionDetails.DetectUrls = true;
        }

        public ExceptionView(Exception exception)
            : this(new Exception[] { exception }, null, null)
        {
        }

        public ExceptionView(Exception exception, String message)
            : this(new Exception[] { exception }, message, null)
        {
        }

        public ExceptionView(Exception exception, String message, String caption)
            : this(new Exception[] { exception }, message, caption)
        {
        }

        public ExceptionView(Exception[] exceptions)
            : this(exceptions, null, null)
        {
        }

        public ExceptionView(Exception[] exceptions, String message)
            : this(exceptions, message, null)
        {
        }

        public ExceptionView(Exception[] exceptions, String message, String caption)
            : this()
        {
            this.Exceptions = exceptions;
            this.Message = message;
            this.Caption = caption;
        }

        #endregion // Constructor section.

        #region Public property section.

        private readonly List<Exception> exceptions = new List<Exception>();
        public Exception[] Exceptions
        {
            get { return this.exceptions.ToArray(); }
            set
            {
                this.exceptions.Clear();
                if (value != null)
                {
                    foreach (Exception current in value)
                    {
                        if (current != null) { this.exceptions.Add(current); }
                    }
                }
            }
        }

        public String Caption { get; set; }

        public String Message { get; set; }

        #endregion // Public property section.

        #region Overwritten event handler section.

        protected override void OnLoad(EventArgs args)
        {
            try
            {
                this.SuspendLayout();

                // Set window text if possible.
                if (!String.IsNullOrEmpty(this.Caption))
                {
                    this.Text = this.Caption;
                }

                // Set window message if possible.
                if (!String.IsNullOrEmpty(this.Message))
                {
                    this.lblMessage.Text = this.Message;
                }
                // Or hide message and icon and resize all other controls.
                else
                {
                    Int32 top = this.lblMessage.Top;
                    Int32 delta = this.lblExceptionTree.Top - top;

                    this.lblMessage.Visible = false;
                    this.picImage.Visible = false;

                    this.lblExceptionTree.Top = top;
                    this.lblExceptionDetails.Top = top;

                    this.tvcExceptionTree.Top -= delta;
                    this.tvcExceptionTree.Height += delta;
                    this.panBorderHelper.Top -= delta;
                    this.panBorderHelper.Height += delta;
                }

                this.LoadExceptions();

                base.OnLoad(args);
            }
            finally
            {
                this.ResumeLayout(false);
                this.PerformLayout();
            }
        }

        #endregion // Overwritten event handler section.

        #region Private event handler section.

        private void OnTreeBeforeExpand(Object sender, TreeViewCancelEventArgs args)
        {
            if (args.Node.FirstNode != null && args.Node.FirstNode.Text == String.Empty)
            {
                args.Node.FirstNode.Remove(); // Remove dummy node.

                if (args.Node.Tag is Exception current && current.InnerException != null)
                {
                    args.Node.Nodes.Add(this.CreateNode(current.InnerException));
                }
            }
        }

        private void OnTreeAfterSelect(Object sender, TreeViewEventArgs args)
        {
            try
            {
                const String PLACEHOLDER = "$$PLACEHOLDER$$";
                const String NEWLINE = "\\par ";
                const String NEWPARA = "\\par \\par ";

                String details = String.Empty;
                if (args.Node.Tag is Exception current)
                {
                    this.txtExceptionDetails.Rtf = String.Empty;
                    this.txtExceptionDetails.Text = PLACEHOLDER;

                    details +=
                        "{\\b Message}" + NEWLINE +
                        current.Message.Replace("\\", "\\\\") + NEWPARA;

                    if (!String.IsNullOrEmpty(current.HelpLink))
                    {
                        details +=
                            "{\\b Help Link}" + NEWLINE +
                            current.HelpLink.Replace("\\", "\\\\") + NEWPARA;
                    }

                    if (current.InnerException != null)
                    {
                        details +=
                            "{\\b Inner Exception}" + NEWLINE +
                            "{\\ul " + this.ExtractName(current.InnerException) + "}: "
                            + current.InnerException.Message.Replace("\\", "\\\\") + NEWPARA;
                    }

                    if (!String.IsNullOrEmpty(current.Source))
                    {
                        details +=
                            "{\\b Source}" + NEWLINE +
                            current.Source.Replace("\\", "\\\\") + NEWPARA;
                    }

                    if (current.Data != null && current.Data.Count > 0)
                    {
                        details += "{\\b Data}" + NEWLINE;
                        foreach (var key in current.Data.Keys)
                        {
                            details +=
                                "{\\ul " + key.ToString().Replace("\\", "\\\\") + "}: " +
                                current.Data[key].ToString().Replace("\\", "\\\\") + NEWLINE;
                        }
                        details += NEWLINE;
                    }


                    if (!String.IsNullOrEmpty(current.StackTrace))
                    {
                        details +=
                            "{\\b Stack Trace}" + NEWLINE +
                            current.StackTrace.Replace("\\", "\\\\").Replace("\r\n", NEWLINE) + NEWPARA;
                    }
                    else
                    {
                        details +=
                            "{\\b Summary}" + NEWLINE +
                            current.ToString().Replace("\\", "\\\\").Replace("\r\n", NEWLINE) + NEWPARA;

                    }

                    this.txtExceptionDetails.Rtf = this.txtExceptionDetails.Rtf.Replace(PLACEHOLDER, details);
                }
                else
                {
                    this.txtExceptionDetails.Text = String.Empty;
                }

                // Reset copy to clipboard link.
                this.lnkCopyToClipboard.LinkVisited = false;
            }
            catch (Exception exception)
            {
                this.txtExceptionDetails.Text = String.Empty;

                this.ShowErrorMessage("Setting of exception details has failed.", exception);

                Debug.WriteLine(exception);
                Program.FatalLogger.Write("ExceptionView", exception);
            }
        }

        private void OnDetailsLinkClicked(Object sender, LinkClickedEventArgs args)
        {
            try
            {
                Process.Start(args.LinkText);
            }
            catch (Exception exception)
            {
                this.ShowErrorMessage("Opening given link has failed.", exception);

                Debug.WriteLine(exception);
                Program.FatalLogger.Write("ExceptionView", exception);
            }
        }

        private void OnCopyToClipboardLinkClicked(Object sender, LinkLabelLinkClickedEventArgs args)
        {
            TreeNode selected = this.tvcExceptionTree.SelectedNode;
            if (selected != null && selected.Tag is Exception)
            {
                try
                {
                    Clipboard.SetText((selected.Tag as Exception).ToString());
                    args.Link.Visited = true;
                }
                catch (Exception exception)
                {
                    this.ShowErrorMessage("Copying data to the clipboard has failed.", exception);

                    Debug.WriteLine(exception);
                    Program.FatalLogger.Write("ExceptionView", exception);
                }
            }
        }

        #endregion // Private event handler section.

        #region Private member function section.

        private void LoadExceptions()
        {
            try
            {
                this.tvcExceptionTree.BeginUpdate();

                this.txtExceptionDetails.Text = String.Empty;
                this.tvcExceptionTree.Nodes.Clear();

                foreach (Exception current in this.exceptions)
                {
                    this.tvcExceptionTree.Nodes.Add(this.CreateNode(current));
                }

                if (this.tvcExceptionTree.Nodes.Count > 0)
                {
                    this.tvcExceptionTree.SelectedNode = this.tvcExceptionTree.Nodes[0];
                }
            }
            catch (Exception exception)
            {
                this.ShowErrorMessage("Loading of exception details has failed.", exception);

                Debug.WriteLine(exception);
                Program.FatalLogger.Write("ExceptionView", exception);
            }
            finally
            {
                this.tvcExceptionTree.EndUpdate();
            }
        }

        private TreeNode CreateNode(Exception exception)
        {
            TreeNode result = new TreeNode(this.ExtractName(exception))
            {
                Tag = exception
            };
            result.Nodes.Add(new TreeNode()); // Add dummy node.
            return result;
        }

        private String ExtractName(Exception exception)
        {
            if (exception != null)
            {
                String result = exception.GetType().ToString();
                Int32 index = result.LastIndexOf('.');
                if (index != -1) { result = result.Substring(index).Replace(".", ""); }
                return result;
            }
            else
            {
                return String.Empty;
            }
        }

        private void ShowErrorMessage(String message, Exception exception)
        {
            if (!String.IsNullOrEmpty(message) && !message.EndsWith("\n\n"))
            {
                message += "\n\n";
            }

            MessageBox.Show(this, message + exception.Message,
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion // Private member function section.
    }
}
