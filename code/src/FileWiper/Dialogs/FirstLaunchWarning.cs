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
using System.Windows.Forms;

namespace plexdata.FileWiper
{
    public partial class FirstLaunchWarning : Form
    {
        public FirstLaunchWarning()
            : base()
        {
            this.InitializeComponent();
            this.Icon = Properties.Resources.MainIcon;
        }
    }
}
