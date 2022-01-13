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
using System.Drawing;
using System.Windows.Forms;

namespace Plexdata.FileWiper
{
    public partial class RelaunchConfirmationDialog : Form
    {
        private const String DEFAULT_CAPTION = "Confirmation";

        private const String DEFAULT_MESSAGE = "During current wiping procedure it came up that some " +
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

        public RelaunchConfirmationDialog(String message)
            : this(message, DEFAULT_CAPTION)
        {
        }

        public RelaunchConfirmationDialog(String message, String caption)
            : base()
        {
            this.InitializeComponent();

            this.Icon = Properties.Resources.MainIcon;
            this.Text = caption ?? throw new ArgumentNullException("caption"); // Caption can be empty...

            if (message == null) { throw new ArgumentNullException("message"); }

            this.InitializeLayout(message);
        }

        private void InitializeLayout(String message)
        {
            this.lblMessage.Text = message;
            this.Size = this.GetPreferedDialogSize(this.lblMessage, message);
        }

        private Size GetPreferedDialogSize(Control reference, String message)
        {
            Size result = this.Size;
            using (Graphics graphics = this.CreateGraphics())
            {
                TextFormatFlags format =
                    TextFormatFlags.EndEllipsis | TextFormatFlags.ExpandTabs | TextFormatFlags.ExternalLeading |
                    TextFormatFlags.NoClipping | TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak;

                Size current = reference.ClientRectangle.Size;

                Size proposed = new Size(
                    (Int32)(current.Width * 1.5),
                    (Int32)(Screen.FromControl(reference).Bounds.Height * 0.7));

                Size prefered = TextRenderer.MeasureText(graphics, message, reference.Font, proposed, format);

                result.Width += Math.Max(Math.Min(prefered.Width, proposed.Width) - current.Width, 0);
                result.Height += Math.Max(Math.Min(prefered.Height, proposed.Height) - current.Height, 0);
            }
            return result;
        }
    }
}
