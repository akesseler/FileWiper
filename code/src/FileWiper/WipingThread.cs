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
using System.Threading;
using System.ComponentModel;

namespace plexdata.FileWiper
{
    internal class WipingThread : BackgroundWorker
    {
        public event EventHandler<EventArgs> WorkerSuspended;
        public event EventHandler<EventArgs> WorkerContinued;

        private GuiInvokeHelper invoker;
        private Thread thread;

        public WipingThread()
            : base()
        {
            this.invoker = new GuiInvokeHelper();
            this.thread = null;
        }

        private volatile bool paused = false;
        public bool Paused
        {
            get
            {
                return this.paused;
            }
            set
            {
                if (this.paused != value)
                {
                    this.paused = value;
                }
            }
        }

        private volatile object argument = null;
        public object Argument
        {
            get
            {
                return this.argument;
            }
            set
            {
                if (this.argument != value)
                {
                    this.argument = value;
                }
            }
        }

        public virtual void ReportSuspended()
        {
            if (this.WorkerSuspended != null)
            {
                if (this.invoker.InvokeRequired)
                {
                    this.invoker.Invoke(this.WorkerSuspended, new object[] { this, EventArgs.Empty });
                }
                else
                {
                    this.WorkerSuspended(this, EventArgs.Empty);
                }
            }
        }

        public virtual void ReportContinued()
        {
            if (this.WorkerContinued != null)
            {
                if (this.invoker.InvokeRequired)
                {
                    this.invoker.Invoke(this.WorkerContinued, new object[] { this, EventArgs.Empty });
                }
                else
                {
                    this.WorkerContinued(this, EventArgs.Empty);
                }
            }
        }

        public void Kill()
        {
            if (this.thread != null)
            {
                this.thread.Abort();
                this.thread = null;
            }
        }

        protected override void OnDoWork(DoWorkEventArgs args)
        {
            try
            {
                // A background worker thread comes from the thread pool and such 
                // a thread cannot have the STA apartment state. Therefore, do not 
                // change this thread's apartment state! 
                // See also: http://social.msdn.microsoft.com/forums/en-US/Vsexpressvb/thread/a23a47f8-efd2-4117-b107-6757254d8e27
                this.thread = Thread.CurrentThread;

                // Called by RunWorkerAsync().
                // Therefore, run means run!
                this.Paused = false;
                this.Argument = args.Argument;

                base.OnDoWork(args);
            }
            catch (ThreadAbortException)
            {
                args.Cancel = true; // Cancel property has to be set!
                Thread.ResetAbort(); // Prevent exception propagation.
            }
        }

        private class GuiInvokeHelper : ISynchronizeInvoke
        {
            // For more details about this solution see:
            // http://stackoverflow.com/questions/6708765/how-to-invoke-when-i-have-no-control-available

            private readonly SynchronizationContext context;
            private readonly Thread thread;
            private readonly object locker;

            public GuiInvokeHelper()
                : base()
            {
                this.context = SynchronizationContext.Current;
                this.thread = Thread.CurrentThread;
                this.locker = new object();
            }

            #region ISynchronizeInvoke member implementation section.

            public bool InvokeRequired
            {
                get
                {
                    return Thread.CurrentThread.ManagedThreadId != this.thread.ManagedThreadId;
                }
            }

            [Obsolete("This method is not supported!", true)]
            public IAsyncResult BeginInvoke(Delegate method, object[] args)
            {
                throw new NotSupportedException();
            }

            [Obsolete("This method is not supported!", true)]
            public object EndInvoke(IAsyncResult result)
            {
                throw new NotSupportedException();
            }

            public object Invoke(Delegate method, object[] args)
            {
                if (method == null)
                {
                    throw new ArgumentNullException("method");
                }

                lock (this.locker)
                {
                    object result = null;

                    SendOrPostCallback invoker = new SendOrPostCallback(
                        delegate(object data)
                        {
                            result = method.DynamicInvoke(args);
                        });

                    this.context.Send(new SendOrPostCallback(invoker), method.Target);

                    return result;
                }
            }

            public object Invoke(Delegate method)
            {
                return Invoke(method, null);
            }

            #endregion // ISynchronizeInvoke member implementation section.
        }
    }
}
