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

using Microsoft.Win32;

namespace plexdata.FileWiper
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
