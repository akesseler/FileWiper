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
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace plexdata.Utilities
{
    public class ErrorLogger : PlainLogger
    {
        #region Construction section.

        public ErrorLogger()
            : this(Application.ExecutablePath)
        {
        }

        public ErrorLogger(string filename)
            : this(filename, false)
        {
        }

        public ErrorLogger(string filename, bool ensure)
            : base()
        {
            base.SetFilename(filename, ensure ? null : ".error");
        }

        #endregion // Construction section.

        #region Public virtual method section.

        public virtual void Write(Exception exception)
        {
            this.Write(null, exception);
        }

        public virtual void Write(string context, Exception exception)
        {
            try
            {
                if (exception != null)
                {
                    base.Write(
                        context, exception.Message + Environment.NewLine +
                        "==================================================" +
                        Environment.NewLine + exception.ToString() + Environment.NewLine +
                        "==================================================");
                }
            }
            catch (Exception _exception)
            {
                Debug.WriteLine(_exception);
            }
        }

        #endregion // Public virtual method section.
    }

    public class TraceLogger : PlainLogger
    {
        #region Construction section.

        public TraceLogger()
            : this(Application.ExecutablePath)
        {
        }

        public TraceLogger(string filename)
            : this(filename, false)
        {
        }

        public TraceLogger(string filename, bool ensure)
            : base()
        {
            base.SetFilename(filename, ensure ? null : ".trace");
        }

        #endregion // Construction section.

        #region Public overwritten method section..

        public override void Write(object value)
        {
#if TRACE
            base.Write(value);
#endif // TRACE
        }

        public override void Write(string context, object value)
        {
#if TRACE
            base.Write(context, value);
#endif // TRACE
        }

        public override void Write(string message)
        {
#if TRACE
            base.Write(message);
#endif // TRACE
        }

        public override void Write(string context, string message)
        {
#if TRACE
            base.Write(context, message);
#endif // TRACE
        }

        #endregion // Public overwritten method section..
    }

    public class PlainLogger
    {
        #region Private member value section.

        private object critical = null;

        #endregion // Private member value section.

        #region Construction section.

        public PlainLogger()
            : this(Application.ExecutablePath, false)
        {
        }

        public PlainLogger(string filename)
            : this(filename, false)
        {
        }

        public PlainLogger(string filename, bool ensure)
            : base()
        {
            this.critical = new object();
            this.SetFilename(filename, ensure ? null : ".log");
        }

        #endregion // Construction section.

        #region Public property section.

        public string Filename { get; private set; }

        #endregion // Public property section.

        #region Public virtual method section.

        public virtual void Write(object value)
        {
            if (value != null) { this.WriteItem(this.BuildMessage(null, value.ToString())); }
        }

        public virtual void Write(string context, object value)
        {
            if (value != null) { this.WriteItem(this.BuildMessage(context, value.ToString())); }
        }

        public virtual void Write(string message)
        {
            this.WriteItem(this.BuildMessage(null, message));
        }

        public virtual void Write(string context, string message)
        {
            this.WriteItem(this.BuildMessage(context, message));
        }

        #endregion // Public virtual method section.

        #region Protected method section.

        protected void SetFilename(string filename, string extension)
        {
            // Validate parameter.
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }

            filename = filename.Trim();
            if (filename.Length == 0)
            {
                throw new ArgumentNullException("filename");
            }

            if (extension == null)
            {
                extension = String.Empty;
            }
            else
            {
                extension = extension.Trim();
            }

            // Save filename.
            this.Filename = extension.Length == 0 ? filename : Path.ChangeExtension(filename, extension);
        }

        protected string BuildMessage(string context, string message)
        {
            try
            {
                // Validate parameters.
                if (context == null) { context = String.Empty; } else { context = context.Trim(); }
                if (message == null) { message = String.Empty; } else { message = message.Trim(); }

                // Build result message.
                if (message.Length > 0)
                {
                    string processId = Process.GetCurrentProcess().Id.ToString().PadLeft(6, ' ');
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd, HH:mm:ss.ffff");

                    if (context.Length > 0)
                    {
                        return String.Format("({0}) {1}: [{2}] {3}", processId, timestamp, context, message);
                    }
                    else
                    {
                        return String.Format("({0}) {1}: {2}", processId, timestamp, message);
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
            return String.Empty;
        }

        #endregion // Protected method section.

        #region Private method section.

        private void WriteItem(string message)
        {
            try
            {
                // Apparently there is really no other way than 
                // to open the log file for each entry to write!
                if (!String.IsNullOrEmpty(message) && !String.IsNullOrEmpty(this.Filename))
                {
                    lock (this.critical)
                    {
                        using (StreamWriter writer = new StreamWriter(this.Filename, true, Encoding.UTF8))
                        {
                            writer.WriteLine(message.Trim());
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        #endregion // Private method section.
    }
}
