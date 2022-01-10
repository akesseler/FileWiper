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
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace plexdata.FileWiper
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

        public ExceptionView(Exception exception, string message)
            : this(new Exception[] { exception }, message, null)
        {
        }

        public ExceptionView(Exception exception, string message, string caption)
            : this(new Exception[] { exception }, message, caption)
        {
        }

        public ExceptionView(Exception[] exceptions)
            : this(exceptions, null, null)
        {
        }

        public ExceptionView(Exception[] exceptions, string message)
            : this(exceptions, message, null)
        {
        }

        public ExceptionView(Exception[] exceptions, string message, string caption)
            : this()
        {
            this.Exceptions = exceptions;
            this.Message = message;
            this.Caption = caption;
        }

        #endregion // Constructor section.

        #region Public property section.

        private List<Exception> exceptions = new List<Exception>();
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

        public string Caption { get; set; }

        public string Message { get; set; }

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
                    int top = this.lblMessage.Top;
                    int delta = this.lblExceptionTree.Top - top;

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

        private void OnTreeBeforeExpand(object sender, TreeViewCancelEventArgs args)
        {
            if (args.Node.FirstNode != null && args.Node.FirstNode.Text == String.Empty)
            {
                args.Node.FirstNode.Remove(); // Remove dummy node.

                Exception current = args.Node.Tag as Exception;
                if (current != null && current.InnerException != null)
                {
                    args.Node.Nodes.Add(this.CreateNode(current.InnerException));
                }
            }
        }

        private void OnTreeAfterSelect(object sender, TreeViewEventArgs args)
        {
            try
            {
                const string PLACEHOLDER = "$$PLACEHOLDER$$";
                const string NEWLINE = "\\par ";
                const string NEWPARA = "\\par \\par ";

                string details = String.Empty;
                Exception current = args.Node.Tag as Exception;
                if (current != null)
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

        private void OnDetailsLinkClicked(object sender, LinkClickedEventArgs args)
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

        private void OnCopyToClipboardLinkClicked(object sender, LinkLabelLinkClickedEventArgs args)
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
            TreeNode result = new TreeNode(this.ExtractName(exception));
            result.Tag = exception;
            result.Nodes.Add(new TreeNode()); // Add dummy node.
            return result;
        }

        private string ExtractName(Exception exception)
        {
            if (exception != null)
            {
                string result = exception.GetType().ToString();
                int index = result.LastIndexOf('.');
                if (index != -1) { result = result.Substring(index).Replace(".", ""); }
                return result;
            }
            else
            {
                return String.Empty;
            }
        }

        private void ShowErrorMessage(string message, Exception exception)
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
