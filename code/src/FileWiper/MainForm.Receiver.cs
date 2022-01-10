﻿/*
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
using System.Windows.Forms;

namespace Plexdata.FileWiper
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
