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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace Plexdata.FileWiper
{
    [XmlInclude(typeof(ZeroAlgorithm))]
    [XmlInclude(typeof(OneAlgorithm))]
    [XmlInclude(typeof(ZeroOneAlgorithm))]
    [XmlInclude(typeof(SimpleRandomAlgorithm))]
    [XmlInclude(typeof(SecureRandomAlgorithm))]
    [XmlInclude(typeof(RandomStringAlgorithm))]
    [XmlInclude(typeof(GutmannAlgorithm))]
    public class WipingAlgorithms : ICloneable
    {
        private readonly List<WipingAlgorithm> algorithms;
        private WipingAlgorithm selected;

        public WipingAlgorithms()
            : base()
        {
            this.algorithms = new List<WipingAlgorithm>(
                new WipingAlgorithm[] {
                    new ZeroAlgorithm(),
                    new OneAlgorithm(),
                    new ZeroOneAlgorithm(),
                    new SimpleRandomAlgorithm(),
                    new SecureRandomAlgorithm(),
                    new RandomStringAlgorithm(),
                    new GutmannAlgorithm(),
                }
            );
            this.selected = this.algorithms[4];
        }

        public WipingAlgorithms(WipingAlgorithms other)
            : base()
        {
            // Ensure a clone!
            this.algorithms = new List<WipingAlgorithm>();
            foreach (WipingAlgorithm current in other.algorithms)
            {
                WipingAlgorithm clone = current.Clone() as WipingAlgorithm;
                this.algorithms.Add(clone);

                if (clone.GetType() == other.Selected.GetType())
                {
                    this.selected = clone;
                }
            }
        }

        [XmlElement]
        public WipingAlgorithm Selected
        {
            get { return this.selected; }
            set { this.selected = value; }
        }

        [XmlArray]
        public WipingAlgorithm[] Algorithms
        {
            get { return this.algorithms.ToArray(); }
            set
            {
                this.algorithms.Clear();
                if (value != null && value.Length > 0)
                {
                    this.algorithms.AddRange(value);
                }
            }
        }

        #region ICloneable member implementation.

        public Object Clone()
        {
            return new WipingAlgorithms(this);
        }

        #endregion // ICloneable member implementation.
    }

    public abstract class WipingAlgorithm : ICloneable
    {
        private const Int32 DEFAULT_COUNT = 4 * 1024; // Use 4 KB as default buffer size.

        protected WipingAlgorithm()
            : this(null, 0)
        {
        }

        protected WipingAlgorithm(String display)
            : this(display, 1)
        {
        }

        protected WipingAlgorithm(String display, Int32 repeats)
            : base()
        {
            this.Display = display;
            this.Description = String.Empty;
            this.Repeats = repeats;
        }

        public static Int32 DefaultBufferSize
        {
            get { return DEFAULT_COUNT; }
        }

        public static Int32 RepeatsMinimum
        {
            get
            {
                return 1;
            }
        }

        public static Int32 RepeatsMaximum
        {
            get
            {
                return 10;
            }
        }

        [XmlIgnore]
        public virtual String Display { get; protected set; }

        [XmlIgnore]
        public virtual String Description { get; protected set; }

        [XmlAttribute]
        public virtual Int32 Repeats { get; set; }

        public static String WipeEntry(String fullpath)
        {
            // Precondition checks!
            if (fullpath == null) { throw new ArgumentNullException("fullpath"); }
            fullpath = fullpath.Trim();
            if (fullpath == String.Empty) { throw new ArgumentException(null, "fullpath"); }

            // Determine type of given full path.
            Boolean isDirectory =
                ((File.GetAttributes(fullpath) & FileAttributes.Directory) == FileAttributes.Directory);

            // Extract base path of given full path.
            String basepath = Path.GetDirectoryName(fullpath);
            if (String.IsNullOrEmpty(basepath))
            {
                Exception exception = new Win32Exception(161); // ERROR_BAD_PATHNAME
                exception.Data.Add("Base Path", fullpath);
                throw exception;
            }

            // Generate the new file/folder name using a random file name.
            Int32 length = Path.GetFileName(fullpath).Length;

            String newname = String.Empty;

            while (newname.Length < length)
            {
                newname += Path.GetRandomFileName().Replace(".", "");
            }

            newname = newname.Substring(0, length);

            // Generate the new full path consisting of given base path and a random file/folder name.
            String newpath = Path.Combine(basepath, newname);

            if (isDirectory)
            {
                // Try renaming given directory.
#if SIMULATION
                Program.TraceLogger.Write("SIMULATION", String.Format(
                    "WipeEntry(): Renaming folder from [{0}] to [{1}] is just simulated!", fullpath, newpath));
#else
                Directory.Move(fullpath, newpath);
#endif // SIMULATION
            }
            else
            {
                // Try renaming given file.
#if SIMULATION
                Program.TraceLogger.Write("SIMULATION", String.Format(
                    "WipeEntry(): Renaming file from [{0}] to [{1}] is just simulated!", fullpath, newpath));
#else
                File.Move(fullpath, newpath);
#endif // SIMULATION
            }

#if SIMULATION
            return fullpath;
#else
            return newpath;
#endif // SIMULATION
        }

        public virtual void WipeContent(FileStream stream)
        {
            this.WipeContent(stream, DEFAULT_COUNT);
        }

        public virtual void WipeContent(FileStream stream, Int32 count)
        {
            if (stream == null) { throw new ArgumentNullException("stream"); }
            if (count <= 0) { throw new ArgumentOutOfRangeException("count"); }

            // Save current number of remaining bytes.
            Int64 remaining = stream.Length - stream.Position;

            // Adjust given block size to possible data length. This is 
            // important if remaining file content is smaller than given 
            // block size.
            count = (Int32)(remaining < count ? remaining : count);

            Byte[] buffer = this.GetBytes(count);

            // Adjust resulting buffer size to possible data length. This 
            // is important if "get bytes" changes given block size and 
            // remaining file content is smaller than that. The Gutmann 
            // method does this for example.
            count = (Int32)(remaining < buffer.Length ? remaining : buffer.Length);

#if SIMULATION
            stream.Read(buffer, 0, count);
#else
            stream.Write(buffer, 0, count);
#endif // SIMULATION

            stream.Flush();
        }

        public override String ToString()
        {
            if (!String.IsNullOrEmpty(this.Display))
            {
                return this.Display;
            }
            else
            {
                return base.ToString();
            }
        }

        #region ICloneable member implementation.

        public Object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion // ICloneable member implementation.

        protected abstract Byte[] GetBytes(Int32 count);
    }

    public sealed class ZeroAlgorithm : WipingAlgorithm
    {
        public ZeroAlgorithm()
            : base("Zero")
        {
            this.Description =
                "This method wipes the file content with a series of zeros. Further, " +
                "this algorithm is very fast and it provides a good security.";
        }

        protected override Byte[] GetBytes(Int32 count)
        {
            Byte[] result = new Byte[count];
            for (Int32 index = 0; index < result.Length; index++)
            {
                result[index] = 0x00;
            }
            return result;
        }
    }

    public sealed class OneAlgorithm : WipingAlgorithm
    {
        public OneAlgorithm()
            : base("One")
        {
            this.Description =
                "This method wipes the file content with a series of ones. Further, " +
                "this algorithm is very fast and it provides a good security.";
        }

        protected override Byte[] GetBytes(Int32 count)
        {
            Byte[] result = new Byte[count];
            for (Int32 index = 0; index < result.Length; index++)
            {
                result[index] = 0xFF;
            }
            return result;
        }
    }

    public sealed class ZeroOneAlgorithm : WipingAlgorithm
    {
        Boolean toggle = false;

        public ZeroOneAlgorithm()
            : base("Zero & One")
        {
            this.Description =
                "This method wipes the file content with a series of zeros followed " +
                "by a series of ones. Further, this algorithm is fast but it provides " +
                "a better security.";
        }

        public override void WipeContent(FileStream stream, Int32 count)
        {
            // Save current position.
            Int64 position = stream.Position;
            base.WipeContent(stream, count);

            this.toggle = !this.toggle;
            if (this.toggle)
            {
                // Restore previous position 
                // before writing second part.
                stream.Position = position;
            }
        }

        protected override Byte[] GetBytes(Int32 count)
        {
            Byte[] result = new Byte[count];
            for (Int32 index = 0; index < result.Length; index++)
            {
                result[index] = (Byte)(this.toggle ? 0xFF : 0x00);
            }
            return result;
        }
    }

    public sealed class SimpleRandomAlgorithm : WipingAlgorithm
    {
        private readonly Random generator = null;

        public SimpleRandomAlgorithm()
            : base("Simple Random")
        {
            this.generator = new Random((Int32)(DateTime.Now.Ticks & 0x00000000FFFFFFFF));
            this.Description =
                "This method wipes the file content with a series of random bytes using " +
                "standard random number generator. Further, this algorithm is pretty fast " +
                "and it provides a high security.";
        }

        protected override Byte[] GetBytes(Int32 count)
        {
            Byte[] result = new Byte[count];
            this.generator.NextBytes(result);
            return result;
        }
    }

    public sealed class SecureRandomAlgorithm : WipingAlgorithm
    {
        private readonly RandomNumberGenerator generator = null;

        public SecureRandomAlgorithm()
            : base("Secure Random")
        {
            this.generator = RandomNumberGenerator.Create();
            this.Description =
                "This method wipes the file content with a series of random bytes using " +
                "cryptographic random number generator. Further, this algorithm is pretty " +
                "fast and it provides a high security.";
        }

        protected override Byte[] GetBytes(Int32 count)
        {
            Byte[] result = new Byte[count];
            this.generator.GetNonZeroBytes(result);
            return result;
        }
    }

    public sealed class RandomStringAlgorithm : WipingAlgorithm
    {
        public RandomStringAlgorithm()
            : base("Random String")
        {
            this.Description =
                "This method wipes the file content with a randomly created string. " +
                "Further, this algorithm is slower but it provides a higher security.";
        }

        protected override Byte[] GetBytes(Int32 count)
        {
            Byte[] result = new Byte[count];
            StringBuilder builder = new StringBuilder(result.Length);
            while (builder.Length < result.Length)
            {
                // See link below why GetRandomFileName() is used.
                // http://www.dotnetperls.com/random-string
                builder.Append(Path.GetRandomFileName().Replace(".", ""));
            }
            Array.Copy(Encoding.ASCII.GetBytes(builder.ToString()), result, result.Length);
            return result;
        }
    }

    public sealed class GutmannAlgorithm : WipingAlgorithm
    {
        #region Gutmann values declaration section.
        // For more information why the Gutmann method is 
        // only interesting for paranoids see here: 
        // http://www.nber.org/sys-admin/overwritten-data-guttman.html
        // For more details about the Gutmann values see: 
        // http://en.wikipedia.org/wiki/gutmann_method
        private static readonly Byte[,] GUTMANN_VALUES =
        {
            { 0x55, 0x55, 0x55 }, { 0xAA, 0xAA, 0xAA }, { 0x92, 0x49, 0x24 },
            { 0x49, 0x24, 0x92 }, { 0x24, 0x92, 0x49 }, { 0x00, 0x00, 0x00 },
            { 0x11, 0x11, 0x11 }, { 0x22, 0x22, 0x22 }, { 0x33, 0x33, 0x33 },
            { 0x44, 0x44, 0x44 }, { 0x55, 0x55, 0x55 }, { 0x66, 0x66, 0x66 },
            { 0x77, 0x77, 0x77 }, { 0x88, 0x88, 0x88 }, { 0x99, 0x99, 0x99 },
            { 0xAA, 0xAA, 0xAA }, { 0xBB, 0xBB, 0xBB }, { 0xCC, 0xCC, 0xCC },
            { 0xDD, 0xDD, 0xDD }, { 0xEE, 0xEE, 0xEE }, { 0xFF, 0xFF, 0xFF },
            { 0x92, 0x49, 0x24 }, { 0x49, 0x24, 0x92 }, { 0x24, 0x92, 0x49 },
            { 0x6D, 0xB6, 0xDB }, { 0xB6, 0xDB, 0x6D }, { 0xDB, 0x6D, 0xB6 },
        };

        private static readonly Int32 RANDOM_REPEATS = 4;

        private static readonly Int32 GUTMANN_REPEATS = 27;

        #endregion // Gutmann values declaration section.

        /// <summary>
        /// Represents the number of block executions!
        /// </summary>
        private Int32 execution = 0;

        private readonly RandomNumberGenerator generator = null;

        public GutmannAlgorithm()
            : base("Gutmann Method")
        {
            this.generator = RandomNumberGenerator.Create();
            this.Description =
                "This method wipes the file content using the method that has been invented " +
                "by Peter Gutmann. Further, this algorithm is the slowest but it provides the " +
                "highest possible security.\n\nFor more information about this algorithm " +
                "please see:\nhttp://en.wikipedia.org/wiki/gutmann_method.\n\nFor additional " +
                "information about why this algorithm should only be used by paranoids please " +
                "see:\nhttp://www.nber.org/sys-admin/overwritten-data-guttman.html.";
        }

        [XmlAttribute]
        public override Int32 Repeats
        {
            get
            {
                // Keep in mind the Gutmann algorithm only supports one repeat! 
                // This is because of the Gutmann algorithm overwrites the file 
                // content at least 27 times. Therefore, more than one repeat 
                // of the whole algorithm is not allowed!
                return 1;
            }
            set
            {
                // Intentionally empty!
            }
        }

        public override void WipeContent(FileStream stream, Int32 count)
        {
            // Save current position.
            Int64 position = stream.Position;
            base.WipeContent(stream, count);

            if (this.execution < GUTMANN_REPEATS + 2 * RANDOM_REPEATS)
            {
                // Reset current stream position because the 
                // Gutmann algorithm hasn't been executed 
                // completely.
                stream.Position = position;
            }
            else
            {
                // Job done! Prepare execution count to be able 
                // to handle next block or next file stream.
                this.execution = 0;
            }
        }

        protected override Byte[] GetBytes(Int32 count)
        {
            Byte[] result = new Byte[this.FixBufferCount(count)];

            // Firstly, execute all leading random writings.
            if (this.execution < RANDOM_REPEATS)
            {
                this.FillRandomBytes(result);
                this.execution++;
            }
            // Secondly, execute all Gutmann writings.
            else if (this.execution - RANDOM_REPEATS < GUTMANN_REPEATS)
            {
                this.FillGutmannBytes(this.execution - RANDOM_REPEATS, result);
                this.execution++;
            }
            // Thirdly, execute all following random writings.
            else if (this.execution < GUTMANN_REPEATS + 2 * RANDOM_REPEATS)
            {
                this.FillRandomBytes(result);
                this.execution++;
            }

            return result;
        }

        private void FillRandomBytes(Byte[] buffer)
        {
            this.generator.GetNonZeroBytes(buffer);
        }

        private void FillGutmannBytes(Int32 line, Byte[] buffer)
        {
            if (line > GUTMANN_VALUES.GetUpperBound(0))
            {
                throw new ArgumentOutOfRangeException("line");
            }

            // Just a safety check...
            System.Diagnostics.Debug.Assert(buffer.Length % 3 == 0);

            for (Int32 index = 0; index < buffer.Length; index += 3)
            {
                buffer[index + 0] = GUTMANN_VALUES[line, 0];
                buffer[index + 1] = GUTMANN_VALUES[line, 1];
                buffer[index + 2] = GUTMANN_VALUES[line, 2];
            }

        }

        private Int32 FixBufferCount(Int32 count)
        {
            // If Modulo operation produces a rest then 
            // the buffer size needs to be adjusted.
            if (count % 3 != 0)
            {
                // Calculate number of elements (3 byte block), 
                // add one more element, and from that calculate 
                // new block size. This new block size is dividable 
                // by 3 without any rest.
                return (count / 3 + 1) * 3;
            }
            else
            {
                // Current count fits already.
                return count;
            }
        }
    }
}

