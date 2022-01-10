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
    public partial class MainForm : Form
    {
        DataReceiver dataReceiver = null;

        private void InitializeDataReceiver(bool adminMode)
        {
            this.dataReceiver = new DataReceiver(this);
            this.dataReceiver.FilenameReceived += this.OnDataReceiverFilenameReceived;
            this.dataReceiver.ActivationReceived += this.OnDataReceiverActivationReceived;

            if (adminMode)
            {
                // Allow receiving of message WM_COPYDATA 
                // if program runs with admin privileges.
                this.dataReceiver.EnableMessageFilter();
            }
        }

        private void OnDataReceiverActivationReceived(object sender, EventArgs args)
        {
            Program.TraceLogger.Write("MainForm.Receiver", ">>> OnDataReceiverActivationReceived()");

            this.ForceShowForm();

            Program.TraceLogger.Write("MainForm.Receiver", "<<< OnDataReceiverActivationReceived()");
        }

        private void OnDataReceiverFilenameReceived(object sender, FilenameReceivedEventArgs args)
        {
            Program.TraceLogger.Write("MainForm.Receiver", ">>> OnDataReceiverFilenameReceived() value=" + (args.Value != null ? args.Value : "null"));

            this.AppendWiping(args.Value);

            Program.TraceLogger.Write("MainForm.Receiver", "<<< OnDataReceiverFilenameReceived()");
        }
    }
}
