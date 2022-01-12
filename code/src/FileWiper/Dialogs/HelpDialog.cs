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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Plexdata.FileWiper
{
    public partial class HelpDialog : Form
    {
        public HelpDialog()
            : this(true)
        {
        }

        public HelpDialog(Boolean hideOnClose)
            : base()
        {
            this.InitializeComponent();

            this.Text = AboutBox.Title + " " + this.Text;
            this.Icon = Properties.Resources.Help;

            this.btnClose.Image = new Icon(Properties.Resources.Apply, new Size(16, 16)).ToBitmap();

            this.HideOnClose = hideOnClose;
        }

        public Boolean HideOnClose { get; set; }

        protected override void OnLoad(EventArgs args)
        {
            try
            {
                this.webHelpDisplay.DocumentText = Properties.Resources.MainHelpPage;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }

            // Apply last known dialog size and location.
            if (Settings.IsVisibleOnAllScreens(Program.MainForm.Settings.Maintain.HelpBounds))
            {
                this.StartPosition = FormStartPosition.Manual;
                this.DesktopBounds = Program.MainForm.Settings.Maintain.HelpBounds;
            }
            else
            {
                this.StartPosition = FormStartPosition.CenterParent;
            }

            base.OnLoad(args);
        }

        protected override void OnFormClosing(FormClosingEventArgs args)
        {
            // Save last known dialog size and location.
            Program.MainForm.Settings.Maintain.HelpBounds = this.DesktopBounds;

            if (this.HideOnClose && args.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                args.Cancel = true;
            }

            base.OnFormClosing(args);
        }

        private void OnCloseButtonClick(Object sender, EventArgs args)
        {
            this.Close();
        }
    }

    internal class HelpDisplayPanel : Panel
    {
        public HelpDisplayPanel()
            : base()
        {
            this.BorderStyle = BorderStyle.Fixed3D;
        }

        private BorderStyle borderStyle = BorderStyle.Fixed3D;
        [DefaultValue(BorderStyle.Fixed3D)]
        public new BorderStyle BorderStyle
        {
            get
            {
                return this.borderStyle;
            }
            set
            {
                // Using the border style in this way does not really work!  
                // But its good enough for the purpose of having a single  
                // border instead of this kind of ugly original border.
                this.borderStyle = value;
            }
        }

        /// <summary>
        /// Gets the required creation parameters when the control handle 
        /// is created.
        /// </summary>
        /// <remarks>
        /// An instance of class <i>CreateParams</i> that contains the 
        /// required creation parameters when the handle to the control 
        /// is created.
        /// </remarks>
        /// <value>
        /// The value of property <i>CreateParams</i>. The constant 
        /// <c>WS_BORDER</c> or <c>WS_EX_STATICEDGE</c> is added 
        /// depending on current border style. 
        /// </value>
        /// <seealso cref="BorderStyle"/>
        protected override CreateParams CreateParams
        {
            get
            {
                // Very smart because client rectangle is also adjusted! 
                // See also: http://support.microsoft.com/kb/316574
                const Int32 WS_BORDER = unchecked(0x00800000);
                const Int32 WS_EX_STATICEDGE = unchecked(0x00020000);

                CreateParams createParams = base.CreateParams;
                createParams.ExStyle &= (~WS_EX_STATICEDGE);
                createParams.Style &= (~WS_BORDER);

                switch (this.BorderStyle)
                {
                    case BorderStyle.Fixed3D:
                        createParams.ExStyle |= WS_EX_STATICEDGE;
                        break;
                    case BorderStyle.FixedSingle:
                        createParams.Style |= WS_BORDER;
                        break;
                }
                return createParams;
            }
        }
    }
}
