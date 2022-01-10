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
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace plexdata.FileWiper
{
    public partial class RelaunchConfirmationDialog : Form
    {
        private const string DEFAULT_CAPTION = "Confirmation";

        private const string DEFAULT_MESSAGE = "During current wiping procedure it came up that some " +
            "of the files need administration privileges!" +
            "\r\n\r\n" +
            "Press button [Show] to display all occurred errors, or press button [Relaunch] to restart " +
            "the program with administration privileges, or press button [Cancel] to abort." +
            "\r\n\r\n" +
            "Please keep in mind that all information for current wiping procedure will be lost after " +
            "restarting the program." +
            "\r\n\r\n" +
            "What do you want to do?\r\n\r\n";

        public RelaunchConfirmationDialog()
            : this(DEFAULT_MESSAGE, DEFAULT_CAPTION)
        {
        }

        public RelaunchConfirmationDialog(string message)
            : this(message, DEFAULT_CAPTION)
        {
        }

        public RelaunchConfirmationDialog(string message, string caption)
            : base()
        {
            this.InitializeComponent();

            this.Icon = Properties.Resources.MainIcon;

            if (caption == null) { throw new ArgumentNullException("caption"); }
            this.Text = caption; // Caption can be empty...

            if (message == null) { throw new ArgumentNullException("message"); }

            this.InitializeLayout(message);
        }

        private void InitializeLayout(string message)
        {
            this.lblMessage.Text = message;
            this.Size = this.GetPreferedDialogSize(this.lblMessage, message);
        }

        private Size GetPreferedDialogSize(Control reference, string message)
        {
            Size result = this.Size;
            using (Graphics graphics = this.CreateGraphics())
            {
                TextFormatFlags format =
                    TextFormatFlags.EndEllipsis | TextFormatFlags.ExpandTabs | TextFormatFlags.ExternalLeading |
                    TextFormatFlags.NoClipping | TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak;

                Size current = reference.ClientRectangle.Size;

                Size proposed = new Size(
                    (int)(current.Width * 1.5),
                    (int)(Screen.FromControl(reference).Bounds.Height * 0.7));

                Size prefered = TextRenderer.MeasureText(graphics, message, reference.Font, proposed, format);

                result.Width += Math.Max(Math.Min(prefered.Width, proposed.Width) - current.Width, 0);
                result.Height += Math.Max(Math.Min(prefered.Height, proposed.Height) - current.Height, 0);
            }
            return result;
        }
    }
}
