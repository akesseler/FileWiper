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
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace plexdata.FileWiper
{
    internal class StringReceivedEventArgs : EventArgs
    {
        public StringReceivedEventArgs(string value)
            : base()
        {
            if (value == null)
            {
                value = String.Empty;
            }
            this.Value = value;
        }

        public string Value { get; private set; }
    }

    internal class FilenameReceivedEventArgs : StringReceivedEventArgs
    {
        public FilenameReceivedEventArgs(string value)
            : base(value)
        {
        }
    }

    internal class BufferReceivedEventArgs : EventArgs
    {
        public BufferReceivedEventArgs(byte[] buffer)
            : base()
        {
            if (buffer == null)
            {
                buffer = new byte[0];
            }
            this.Buffer = buffer;
        }

        public byte[] Buffer { get; private set; }
    }

    internal class DataReceiver : DataExchanger
    {
        public event EventHandler<StringReceivedEventArgs> StringReceived;
        public event EventHandler<FilenameReceivedEventArgs> FilenameReceived;
        public event EventHandler<BufferReceivedEventArgs> BufferReceived;
        public event EventHandler<EventArgs> ActivationReceived;

        public DataReceiver(Form parent)
            : base(parent)
        {
        }

        public bool EnableMessageFilter()
        {
            // Be aware, ChangeWindowMessageFilter() is defined since Windows Vista.
            if (PermissionCheck.IsRunAsAdmin && PlatformCheck.IsVistaOrHigher)
            {
                if (!ChangeWindowMessageFilter(WM_COPYDATA, MSGFLT_ADD))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                if (!ChangeWindowMessageFilter(WM_FILEWIPER_COMMAND, MSGFLT_ADD))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            return true;
        }

        public bool DisableMessageFilter()
        {
            // Be aware, ChangeWindowMessageFilter() is defined since Windows Vista.
            if (PermissionCheck.IsRunAsAdmin && PlatformCheck.IsVistaOrHigher)
            {
                if (!ChangeWindowMessageFilter(WM_COPYDATA, MSGFLT_REMOVE))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                if (!ChangeWindowMessageFilter(WM_FILEWIPER_COMMAND, MSGFLT_REMOVE))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            return true;
        }

        protected override void WndProc(ref Message message)
        {
            try
            {
                // If this message has been handled, don't 
                // forward it to inherit message handler!
                if (message.Msg == WM_COPYDATA)
                {
                    COPYDATASTRUCT data = (COPYDATASTRUCT)Marshal.PtrToStructure(
                        message.LParam, typeof(COPYDATASTRUCT));

                    int type = data.nType.ToInt32();

                    if (type == CONTENT_BUFFER ||
                        type == CONTENT_STRING ||
                        type == CONTENT_FILENAME)
                    {
                        byte[] buffer = new byte[data.nBuffer];
                        Marshal.Copy(data.pBuffer, buffer, 0, buffer.Length);

                        if (type == CONTENT_BUFFER)
                        {
                            if (this.BufferReceived != null)
                            {
                                this.BufferReceived(this, new BufferReceivedEventArgs(buffer));
                            }
                        }
                        else
                        {
                            string value = base.Encoding.GetString(buffer);

                            if (type == CONTENT_STRING)
                            {
                                if (this.StringReceived != null)
                                {
                                    this.StringReceived(this, new StringReceivedEventArgs(value));
                                }
                            }
                            else if (type == CONTENT_FILENAME)
                            {
                                if (this.FilenameReceived != null)
                                {
                                    this.FilenameReceived(this, new FilenameReceivedEventArgs(value));
                                }
                            }
                        }
                        message.Result = (IntPtr)1; // Return true.
                    }
                    else
                    {
                        Program.TraceLogger.Write(
                            "DataReceiver", "Received unrecognized data type: " + type);

                        message.Result = (IntPtr)0; // Return false.
                    }
                }
                // Getting WM_FILEWIPER_COMMAND might throw an exception!
                else if (message.Msg == WM_FILEWIPER_COMMAND)
                {
                    if ((message.WParam == COMMAND_ACTION) && (message.LParam == COMMAND_ACTIVATION))
                    {
                        if (this.ActivationReceived != null)
                        {
                            this.ActivationReceived(this, EventArgs.Empty);
                        }
                        message.Result = (IntPtr)1; // Return true.
                    }
                    else
                    {
                        message.Result = (IntPtr)0; // Return false.
                    }
                }
                else
                {
                    base.WndProc(ref message);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("DataReceiver", exception);
            }
        }
    }

    internal class DataDispatcher : DataExchanger
    {
        public DataDispatcher()
            : this(null)
        {
            // Message source (parent) is only needed when 
            // message receiver wants to send something back!
        }

        public DataDispatcher(Form parent)
            : base(parent)
        {
            // Very dangerous! For example the Windows Media-Player 
            // takes care about message WM_COPYDATA and crashes when 
            // it receives this message via HWND_BROADCAST.
            this.Receiver = HWND_BROADCAST;
        }

        public DataDispatcher(Form parent, IntPtr hReceiver)
            : this(parent)
        {
            this.Receiver = hReceiver;
        }

        public IntPtr Receiver { get; private set; }

        public bool SendString(string value)
        {
            return this.SendString(this.Receiver, value);
        }

        public bool SendString(IntPtr hReceiver, string value)
        {
            // A plain string is always sent as it is.

            bool success = false;
            if (!String.IsNullOrEmpty(value))
            {
                success = base.SendCopyData(hReceiver, CONTENT_STRING, value);
            }
            return success;
        }

        public bool SendFilename(string filename)
        {
            return this.SendFilename(this.Receiver, filename);
        }

        public bool SendFilename(IntPtr hReceiver, string filename)
        {
            // A filename is always trimmed and enclosed 
            // with quotes before it is sent.

            bool success = false;

            if (filename != null) { filename = filename.Trim(); }

            if (filename.Length > 0)
            {
                if (!filename.StartsWith("\"")) { filename = "\"" + filename; }
                if (!filename.EndsWith("\"")) { filename = filename + "\""; }

                success = base.SendCopyData(hReceiver, CONTENT_FILENAME, filename);
            }
            return success;
        }

        public bool SendBuffer(byte[] buffer)
        {
            return this.SendBuffer(this.Receiver, buffer);
        }

        public bool SendBuffer(IntPtr hReceiver, byte[] buffer)
        {
            return base.SendCopyData(hReceiver, CONTENT_BUFFER, buffer);
        }

        public bool SendBringToFront()
        {
            return SendBringToFront(this.Receiver);
        }

        public bool SendBringToFront(IntPtr hReceiver)
        {
            bool success = false;
            try
            {
                Program.TraceLogger.Write("DataDispatcher", ">>> SendBringToFront()");

                if (hReceiver == IntPtr.Zero)
                {
                    throw new ArgumentNullException("hReceiver");
                }

                success = SendMessage(hReceiver, WM_FILEWIPER_COMMAND, COMMAND_ACTION, COMMAND_ACTIVATION);

                // Message might be rejected by UIPI and in this 
                // case last error is set to 5 (access denied)!
                if (!success && Marshal.GetLastWin32Error() != 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                Program.TraceLogger.Write("DataDispatcher",
                    "--- SendBringToFront() Sending message " + (success ? "successful." : "failed!"));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("DataDispatcher", exception);
            }
            finally
            {
                Program.TraceLogger.Write("DataDispatcher", "<<< SendBringToFront()");
            }

            return success;
        }
    }

    internal class DataExchanger : NativeWindow
    {
        // Define additional content types for WM_COPYDATA message.
        protected const int CONTENT_BUFFER = 0x7000;
        protected const int CONTENT_STRING = 0x8000;
        protected const int CONTENT_FILENAME = 0x8001;

        // Define additional command details for WM_FILEWIPER_COMMAND message.
        protected readonly IntPtr COMMAND_ACTION = new IntPtr(0xAFFE);
        protected readonly IntPtr COMMAND_ACTIVATION = new IntPtr(0xE542);

        protected DataExchanger(Form parent)
            : base()
        {
            this.Encoding = Encoding.Unicode;

            if (parent != null)
            {
                parent.HandleCreated += new EventHandler(OnParentHandleCreated);
                parent.HandleDestroyed += new EventHandler(OnParentHandleDestroyed);
            }
        }

        public Encoding Encoding { get; set; }

        private static int msgFilewiperCommand = 0;
        protected static int WM_FILEWIPER_COMMAND
        {
            get
            {
                if (msgFilewiperCommand == 0)
                {
                    msgFilewiperCommand = RegisterWindowMessage("FileWiperInternalCommandMessage");

                    if (msgFilewiperCommand == 0)
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
                return msgFilewiperCommand;
            }
        }

        protected bool SendCopyData(IntPtr hReceiver, int type, string value)
        {
            bool success = false;
            if (!String.IsNullOrEmpty(value))
            {
                success = SendCopyData(hReceiver, type, this.Encoding.GetBytes(value));
            }
            return success;
        }

        protected bool SendCopyData(IntPtr hReceiver, int type, byte[] buffer)
        {
            bool success = false;

            if (hReceiver != IntPtr.Zero)
            {
                COPYDATASTRUCT lParam = new COPYDATASTRUCT();
                try
                {
                    lParam.nType = new IntPtr(type);
                    if (buffer != null)
                    {
                        lParam.nBuffer = buffer.Length;
                        lParam.pBuffer = Marshal.AllocHGlobal(buffer.Length);
                        Marshal.Copy(buffer, 0, lParam.pBuffer, buffer.Length);
                    }
                    else
                    {
                        lParam.nBuffer = 0;
                        lParam.pBuffer = IntPtr.Zero;
                    }

                    success = SendCopyData(hReceiver, lParam);
                }
                finally
                {
                    if (lParam.pBuffer != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(lParam.pBuffer);
                    }
                }
            }
            return success;
        }

        protected bool SendCopyData(IntPtr hReceiver, COPYDATASTRUCT lParam)
        {
            bool success = false;
            try
            {
                Program.TraceLogger.Write("DataExchanger", ">>> SendCopyData()");

                if (hReceiver == IntPtr.Zero)
                {
                    throw new ArgumentNullException("hReceiver");
                }

                success = SendCopyData(hReceiver, WM_COPYDATA, this.Handle, ref lParam);

                // Message might be rejected by receiver but this is 
                // not really an error! Therefore additionally check 
                // the system's last error code.
                if (!success && Marshal.GetLastWin32Error() != 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                Program.TraceLogger.Write("DataExchanger",
                    "--- SendCopyData() Sending message " + (success ? "successful." : "failed!"));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("DataExchanger", exception);
            }
            finally
            {
                Program.TraceLogger.Write("DataExchanger", "<<< SendCopyData()");
            }

            return success;
        }

        private void OnParentHandleCreated(object sender, EventArgs args)
        {
            try
            {
                base.AssignHandle(((Form)sender).Handle);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("DataExchanger", exception);
            }
        }

        private void OnParentHandleDestroyed(object sender, EventArgs args)
        {
            base.ReleaseHandle();
        }

        #region Win32 API related implementations.

        protected static IntPtr HWND_BROADCAST = new IntPtr(0xffff);

        // Windows 2000 Professional / Windows 2000 Server
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, PreserveSig = true, CharSet = CharSet.Auto, SetLastError = true)]
        protected static extern bool SendMessage([In] IntPtr hWnd, [In] int nMessage, [In] IntPtr wParam, [In] IntPtr lParam);

        // Windows 2000 Professional / Windows 2000 Server
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int RegisterWindowMessage(string message);

        // Windows 2000 Professional / Windows 2000 Server
        [DllImport("user32.dll", EntryPoint = "SendMessage", CallingConvention = CallingConvention.StdCall, PreserveSig = true, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SendCopyData([In] IntPtr hWnd, [In] int nMessage, [In] IntPtr wParam, [In] ref COPYDATASTRUCT lParam);

        protected const int WM_COPYDATA = 0x004A;

        // See MSDN for this struct definition!
        [StructLayout(LayoutKind.Sequential)]
        protected struct COPYDATASTRUCT
        {
            public IntPtr nType;
            public Int32 nBuffer;
            public IntPtr pBuffer;
        }

        protected const int MSGFLT_ADD = 1;
        protected const int MSGFLT_REMOVE = 2;

        // Windows Vista / Windows Server 2008
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        protected static extern bool ChangeWindowMessageFilter([In] int message, [In] int flag);

        #endregion // Win32 API related implementations.
    }
}
