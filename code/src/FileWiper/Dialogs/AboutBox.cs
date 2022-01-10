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
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;

namespace plexdata.FileWiper
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
