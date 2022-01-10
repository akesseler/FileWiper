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
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Plexdata.FileWiper
{
    public partial class DestroyConfirmationDialog : Form
    {
        private const string DEFAULT_CAPTION = "Confirmation";

        private readonly string DEFAULT_MESSAGE =
            "Do you really want to continue?";

        private readonly string MULTI_ITEM_MESSAGE =
            "Do you really want to permanently destroy {0} elements?";

        private readonly string SINGLE_FILE_MESSAGE =
            "Do you really want to permanently destroy file \"{0}\"?";

        private readonly string SINGLE_FOLDER_MESSAGE =
            "Do you really want to permanently destroy folder \"{0}\" and its content?";

        private readonly string MESSAGE_APPENDIX =
            "\n\nBe aware, this procedure is definitely irrevocable!\n\nPress "
            + "button No to abort or button Yes if you want to continue.";

        public DestroyConfirmationDialog()
            : this(new string[0], String.Empty, DEFAULT_CAPTION)
        {
        }

        public DestroyConfirmationDialog(string[] fullpaths)
            : this(fullpaths, String.Empty, DEFAULT_CAPTION)
        {
        }

        public DestroyConfirmationDialog(string message)
            : this(new string[0], message, DEFAULT_CAPTION)
        {
        }

        public DestroyConfirmationDialog(string[] fullpaths, string message)
            : this(fullpaths, message, DEFAULT_CAPTION)
        {
        }

        public DestroyConfirmationDialog(string[] fullpaths, string message, string caption)
            : base()
        {
            this.InitializeComponent();

            this.Icon = Properties.Resources.MainIcon;

            if (caption == null) { throw new ArgumentNullException("caption"); }
            this.Text = caption; // Caption can be empty...

            if (message == null) { throw new ArgumentNullException("message"); }
            if (fullpaths == null) { throw new ArgumentNullException("fullpaths"); }

            this.InitializeLayout(fullpaths, message);
        }

        public DestroyConfirmationDialog(string message, string caption)
            : this(new string[0], message, caption)
        {
        }

        private void InitializeLayout(string[] fullpaths, string message)
        {
            if (message == String.Empty)
            {
                if (fullpaths.Length > 0)
                {
                    if (fullpaths.Length > 1)
                    {
                        message = String.Format(MULTI_ITEM_MESSAGE, fullpaths.Length) + MESSAGE_APPENDIX;
                    }
                    else
                    {
                        if (Directory.Exists(fullpaths[0]))
                        {
                            message = String.Format(SINGLE_FOLDER_MESSAGE, fullpaths[0]) + MESSAGE_APPENDIX;
                        }
                        else if (File.Exists(fullpaths[0]))
                        {
                            message = String.Format(SINGLE_FILE_MESSAGE, Path.GetFileName(fullpaths[0])) + MESSAGE_APPENDIX;
                        }
                        else
                        {
                            message = DEFAULT_MESSAGE;
                        }
                    }
                }
                else
                {
                    message = DEFAULT_MESSAGE;
                }
            }

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

                // Add one more line to ensure a distance 
                // between text and buttons.
                result.Height += reference.Font.Height;
            }
            return result;
        }
    }
}
