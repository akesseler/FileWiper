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
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;

namespace Plexdata.FileWiper
{
    partial class AboutBox : Form
    {
        public AboutBox()
            : base()
        {
            this.InitializeComponent();

            this.Text += AboutBox.Title;
            this.Icon = Properties.Resources.About;

            this.btnOK.Image = new Icon(Properties.Resources.Apply, new Size(16, 16)).ToBitmap();

            this.lblProduct.Text = AboutBox.Product;
            this.lblVersion.Text = String.Format("{0} {1}", this.lblVersion.Text, AboutBox.Version);
            this.lblCopyright.Text = AboutBox.Copyright;
            this.txtDescription.Text = AboutBox.Description;

            this.lnkIconAuthor.Text = "Icons by Open Icon Library";
            this.lnkIconAuthor.Links.Add(9, 17, "http://openiconlibrary.sourceforge.net/");
        }

        private void OnLogoImageClick(object sender, EventArgs args)
        {
            Process.Start("http://www.plexdata.de/");
        }

        private void OnIconAuthorClick(object sender, LinkLabelLinkClickedEventArgs args)
        {
            args.Link.Visited = true;
            Process.Start(args.Link.LinkData.ToString());
        }

        #region Assembly Attribute Accessors

        public static string Title
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != String.Empty)
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public static string Version
        {
            get
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return String.Format("{0}.{1} ({2})", version.Major, version.Minor, version.Build << 8 | (byte)version.Revision);
            }
        }

        public static string Description
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return String.Empty;
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public static string Product
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return String.Empty;
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public static string Copyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return String.Empty;
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public static string Company
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return String.Empty;
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        #endregion // Assembly Attribute Accessors
    }
}
