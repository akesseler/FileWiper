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

using Microsoft.Win32;

namespace Plexdata.FileWiper
{
    internal interface IShutdownListener
    {
        void HandleSessionSwitch(SessionSwitchReason reason);

        // Return false to refuse session ending...
        bool RequestSessionEnding(SessionEndReasons reason);

        void HandlePowerModeChanged(PowerModes mode);
    }

    internal class ShutdownTracker
    {
        private object locked = new object();
        private IShutdownListener listener = null;

        public ShutdownTracker()
            : this(null)
        {
        }

        public ShutdownTracker(IShutdownListener listener)
            : base()
        {
            this.Listener = listener;
        }

        public IShutdownListener Listener
        {
            get
            {
                return this.listener;
            }
            set
            {
                // Be aware, this lock might cause trouble!
                lock (this.locked)
                {
                    if (this.listener != null)
                    {
                        SystemEvents.PowerModeChanged -= new PowerModeChangedEventHandler(this.OnPowerModeChanged);
                        SystemEvents.SessionEnding -= new SessionEndingEventHandler(this.OnSessionEnding);
                        SystemEvents.SessionSwitch -= new SessionSwitchEventHandler(this.OnSessionSwitch);

                        this.listener = null;
                    }

                    if (value != null)
                    {
                        this.listener = value;

                        SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(this.OnPowerModeChanged);
                        SystemEvents.SessionEnding += new SessionEndingEventHandler(this.OnSessionEnding);
                        SystemEvents.SessionSwitch += new SessionSwitchEventHandler(this.OnSessionSwitch);
                    }
                }
            }
        }

        private void OnSessionSwitch(object sender, SessionSwitchEventArgs args)
        {
            if (this.Listener != null)
            {
                this.Listener.HandleSessionSwitch(args.Reason);
            }
        }

        private void OnSessionEnding(object sender, SessionEndingEventArgs args)
        {
            if (this.Listener != null)
            {
                // Return false to refuse request!
                args.Cancel = !this.Listener.RequestSessionEnding(args.Reason);
            }
        }

        private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs args)
        {
            if (this.Listener != null)
            {
                this.Listener.HandlePowerModeChanged(args.Mode);
            }
        }
    }
}
