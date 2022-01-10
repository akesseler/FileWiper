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
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;

namespace plexdata.FileWiper
{
    public partial class HelpDialog : Form
    {
        public HelpDialog()
            : this(true)
        {
        }

        public HelpDialog(bool hideOnClose)
            : base()
        {
            this.InitializeComponent();

            this.Text = AboutBox.Title + " " + this.Text;
            this.Icon = Properties.Resources.Help;

            this.btnOK.Image = new Icon(Properties.Resources.Apply, new Size(16, 16)).ToBitmap();

            this.HideOnClose = hideOnClose;
        }

        public bool HideOnClose { get; set; }

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

        private void OnOkButtonClick(object sender, EventArgs args)
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
                const int WS_BORDER = unchecked((int)0x00800000);
                const int WS_EX_STATICEDGE = unchecked((int)0x00020000);

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
