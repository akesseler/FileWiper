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
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Plexdata.FileWiper
{
    internal class StringReceivedEventArgs : EventArgs
    {
        public StringReceivedEventArgs(String value)
            : base()
        {
            if (value == null)
            {
                value = String.Empty;
            }
            this.Value = value;
        }

        public String Value { get; private set; }
    }

    internal class FilenameReceivedEventArgs : StringReceivedEventArgs
    {
        public FilenameReceivedEventArgs(String value)
            : base(value)
        {
        }
    }

    internal class BufferReceivedEventArgs : EventArgs
    {
        public BufferReceivedEventArgs(Byte[] buffer)
            : base()
        {
            if (buffer == null)
            {
                buffer = new Byte[0];
            }
            this.Buffer = buffer;
        }

        public Byte[] Buffer { get; private set; }
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

        public Boolean EnableMessageFilter()
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

        public Boolean DisableMessageFilter()
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

                    Int32 type = data.nType.ToInt32();

                    if (type == CONTENT_BUFFER ||
                        type == CONTENT_STRING ||
                        type == CONTENT_FILENAME)
                    {
                        Byte[] buffer = new Byte[data.nBuffer];
                        Marshal.Copy(data.pBuffer, buffer, 0, buffer.Length);

                        if (type == CONTENT_BUFFER)
                        {
                            this.BufferReceived?.Invoke(this, new BufferReceivedEventArgs(buffer));
                        }
                        else
                        {
                            String value = base.Encoding.GetString(buffer);

                            if (type == CONTENT_STRING)
                            {
                                this.StringReceived?.Invoke(this, new StringReceivedEventArgs(value));
                            }
                            else if (type == CONTENT_FILENAME)
                            {
                                this.FilenameReceived?.Invoke(this, new FilenameReceivedEventArgs(value));
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
                    if ((message.WParam == this.COMMAND_ACTION) && (message.LParam == this.COMMAND_ACTIVATION))
                    {
                        this.ActivationReceived?.Invoke(this, EventArgs.Empty);

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

        public Boolean SendString(String value)
        {
            return this.SendString(this.Receiver, value);
        }

        public Boolean SendString(IntPtr hReceiver, String value)
        {
            // A plain string is always sent as it is.

            Boolean success = false;
            if (!String.IsNullOrEmpty(value))
            {
                success = base.SendCopyData(hReceiver, CONTENT_STRING, value);
            }
            return success;
        }

        public Boolean SendFilename(String filename)
        {
            return this.SendFilename(this.Receiver, filename);
        }

        public Boolean SendFilename(IntPtr hReceiver, String filename)
        {
            // A filename is always trimmed and enclosed 
            // with quotes before it is sent.

            Boolean success = false;

            if (filename != null) { filename = filename.Trim(); }

            if (filename.Length > 0)
            {
                if (!filename.StartsWith("\"")) { filename = "\"" + filename; }
                if (!filename.EndsWith("\"")) { filename = filename + "\""; }

                success = base.SendCopyData(hReceiver, CONTENT_FILENAME, filename);
            }
            return success;
        }

        public Boolean SendBuffer(Byte[] buffer)
        {
            return this.SendBuffer(this.Receiver, buffer);
        }

        public Boolean SendBuffer(IntPtr hReceiver, Byte[] buffer)
        {
            return base.SendCopyData(hReceiver, CONTENT_BUFFER, buffer);
        }

        public Boolean SendBringToFront()
        {
            return this.SendBringToFront(this.Receiver);
        }

        public Boolean SendBringToFront(IntPtr hReceiver)
        {
            Boolean success = false;
            try
            {
                Program.TraceLogger.Write("DataDispatcher", ">>> SendBringToFront()");

                if (hReceiver == IntPtr.Zero)
                {
                    throw new ArgumentNullException("hReceiver");
                }

                success = SendMessage(hReceiver, WM_FILEWIPER_COMMAND, this.COMMAND_ACTION, this.COMMAND_ACTIVATION);

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
        protected const Int32 CONTENT_BUFFER = 0x7000;
        protected const Int32 CONTENT_STRING = 0x8000;
        protected const Int32 CONTENT_FILENAME = 0x8001;

        // Define additional command details for WM_FILEWIPER_COMMAND message.
        protected readonly IntPtr COMMAND_ACTION = new IntPtr(0xAFFE);
        protected readonly IntPtr COMMAND_ACTIVATION = new IntPtr(0xE542);

        protected DataExchanger(Form parent)
            : base()
        {
            this.Encoding = Encoding.Unicode;

            if (parent != null)
            {
                parent.HandleCreated += new EventHandler(this.OnParentHandleCreated);
                parent.HandleDestroyed += new EventHandler(this.OnParentHandleDestroyed);
            }
        }

        public Encoding Encoding { get; set; }

        private static Int32 msgFilewiperCommand = 0;
        protected static Int32 WM_FILEWIPER_COMMAND
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

        protected Boolean SendCopyData(IntPtr hReceiver, Int32 type, String value)
        {
            Boolean success = false;
            if (!String.IsNullOrEmpty(value))
            {
                success = this.SendCopyData(hReceiver, type, this.Encoding.GetBytes(value));
            }
            return success;
        }

        protected Boolean SendCopyData(IntPtr hReceiver, Int32 type, Byte[] buffer)
        {
            Boolean success = false;

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

                    success = this.SendCopyData(hReceiver, lParam);
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

        protected Boolean SendCopyData(IntPtr hReceiver, COPYDATASTRUCT lParam)
        {
            Boolean success = false;
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

        private void OnParentHandleCreated(Object sender, EventArgs args)
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

        private void OnParentHandleDestroyed(Object sender, EventArgs args)
        {
            base.ReleaseHandle();
        }

        #region Win32 API related implementations.

        protected static IntPtr HWND_BROADCAST = new IntPtr(0xffff);

        // Windows 2000 Professional / Windows 2000 Server
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, PreserveSig = true, CharSet = CharSet.Auto, SetLastError = true)]
        protected static extern Boolean SendMessage([In] IntPtr hWnd, [In] Int32 nMessage, [In] IntPtr wParam, [In] IntPtr lParam);

        // Windows 2000 Professional / Windows 2000 Server
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Int32 RegisterWindowMessage(String message);

        // Windows 2000 Professional / Windows 2000 Server
        [DllImport("user32.dll", EntryPoint = "SendMessage", CallingConvention = CallingConvention.StdCall, PreserveSig = true, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Boolean SendCopyData([In] IntPtr hWnd, [In] Int32 nMessage, [In] IntPtr wParam, [In] ref COPYDATASTRUCT lParam);

        protected const Int32 WM_COPYDATA = 0x004A;

        // See MSDN for this struct definition!
        [StructLayout(LayoutKind.Sequential)]
        protected struct COPYDATASTRUCT
        {
            public IntPtr nType;
            public Int32 nBuffer;
            public IntPtr pBuffer;
        }

        protected const Int32 MSGFLT_ADD = 1;
        protected const Int32 MSGFLT_REMOVE = 2;

        // Windows Vista / Windows Server 2008
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        protected static extern Boolean ChangeWindowMessageFilter([In] Int32 message, [In] Int32 flag);

        #endregion // Win32 API related implementations.
    }
}
