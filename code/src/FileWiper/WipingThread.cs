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
using System.Threading;

namespace Plexdata.FileWiper
{
    internal class WipingThread : BackgroundWorker
    {
        public event EventHandler<EventArgs> WorkerSuspended;
        public event EventHandler<EventArgs> WorkerContinued;

        private readonly GuiInvokeHelper invoker;
        private Thread thread;

        public WipingThread()
            : base()
        {
            this.invoker = new GuiInvokeHelper();
            this.thread = null;
        }

        private volatile Boolean paused = false;
        public Boolean Paused
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

        private volatile Object argument = null;
        public Object Argument
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
                    this.invoker.Invoke(this.WorkerSuspended, new Object[] { this, EventArgs.Empty });
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
                    this.invoker.Invoke(this.WorkerContinued, new Object[] { this, EventArgs.Empty });
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
            private readonly Object locker;

            public GuiInvokeHelper()
                : base()
            {
                this.context = SynchronizationContext.Current;
                this.thread = Thread.CurrentThread;
                this.locker = new Object();
            }

            #region ISynchronizeInvoke member implementation section.

            public Boolean InvokeRequired
            {
                get
                {
                    return Thread.CurrentThread.ManagedThreadId != this.thread.ManagedThreadId;
                }
            }

            [Obsolete("This method is not supported!", true)]
            public IAsyncResult BeginInvoke(Delegate method, Object[] args)
            {
                throw new NotSupportedException();
            }

            [Obsolete("This method is not supported!", true)]
            public Object EndInvoke(IAsyncResult result)
            {
                throw new NotSupportedException();
            }

            public Object Invoke(Delegate method, Object[] args)
            {
                if (method == null)
                {
                    throw new ArgumentNullException("method");
                }

                lock (this.locker)
                {
                    Object result = null;

                    SendOrPostCallback invoker = new SendOrPostCallback(
                        delegate (Object data)
                        {
                            result = method.DynamicInvoke(args);
                        });

                    this.context.Send(new SendOrPostCallback(invoker), method.Target);

                    return result;
                }
            }

            public Object Invoke(Delegate method)
            {
                return this.Invoke(method, null);
            }

            #endregion // ISynchronizeInvoke member implementation section.
        }
    }
}
