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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Plexdata.FileWiper
{
    public class Settings : ICloneable
    {
        #region Class construction.

        public Settings()
            : base()
        {
            this.Maintain = new Maintain();
            this.Behaviour = new Behaviour();
            this.Favorites = new Favorites();
            this.Algorithms = new WipingAlgorithms();
            this.Processing = new WipingProcessing();
        }

        public Settings(Settings other)
            : this()
        {
            this.Maintain = new Maintain(other.Maintain);
            this.Behaviour = new Behaviour(other.Behaviour);
            this.Favorites = new Favorites(other.Favorites);
            this.Algorithms = new WipingAlgorithms(other.Algorithms);
            this.Processing = new WipingProcessing(other.Processing);
        }

        #endregion // Class construction.

        #region Public property implementation.

        public Maintain Maintain { get; set; }

        public Behaviour Behaviour { get; set; }

        public Favorites Favorites { get; set; }

        public WipingAlgorithms Algorithms { get; set; }

        public WipingProcessing Processing { get; set; }

        #endregion // Public property implementation.

        #region Public static property implementation.

        public static String Filename
        {
            get { return Path.ChangeExtension(Application.ExecutablePath, ".cfg"); }
        }

        #endregion // Public static property implementation.

        #region Public static member implementation.

        public static Boolean Save(String filename, Settings root)
        {
            Boolean success = false;
            XmlSerializer serializer = null;
            TextWriter writer = null;

            try
            {
                serializer = new XmlSerializer(typeof(Settings));
                writer = new StreamWriter(filename);
                serializer.Serialize(writer, root);

                success = true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("Settings", exception);
            }
            finally
            {
                if (writer != null) { writer.Close(); }
                writer = null;
                serializer = null;
            }
            return success;
        }

        public static Boolean Load(String filename, out Settings root)
        {
            Boolean success = false;
            XmlSerializer serializer = null;
            TextReader reader = null;

            root = default;

            try
            {
                if (File.Exists(filename))
                {
                    serializer = new XmlSerializer(typeof(Settings));
                    reader = new StreamReader(filename);
                    root = (Settings)serializer.Deserialize(reader);

                    success = (root != default(Settings));
                }
                else
                {
                    root = new Settings();
                    success = true;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("Settings", exception);
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                reader = null;
                serializer = null;
            }
            return success;
        }

        public static Boolean IsVisibleOnAllScreens(Rectangle bounds)
        {
            if (bounds != null && !bounds.IsEmpty)
            {
                foreach (Screen screen in Screen.AllScreens)
                {
                    if (screen.WorkingArea.IntersectsWith(bounds))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion // Public static member implementation.

        #region ICloneable member implementation.

        public Object Clone()
        {
            return new Settings(this);
        }

        #endregion // ICloneable member implementation.
    }

    public class Maintain : ICloneable
    {
        public readonly Rectangle DefaultWindowBounds;

        #region Class construction.

        public Maintain()
            : base()
        {
            this.DefaultWindowBounds = new Rectangle(new Point(100, 50), new Size(800, 600));

            this.WindowBounds = this.DefaultWindowBounds;
            this.SettingsBounds = Rectangle.Empty;
            this.DetailsBounds = Rectangle.Empty;
            this.FavoritesBounds = Rectangle.Empty;
            this.HelpBounds = Rectangle.Empty;
            this.ActiveSettings = String.Empty;
            this.ToolbarScaling = MainForm.ToolbarDefaultScaling;
            this.ToolbarText = false;
        }

        public Maintain(Maintain other)
            : this()
        {
            this.WindowBounds = new Rectangle(other.WindowBounds.Location, other.WindowBounds.Size);
            this.SettingsBounds = new Rectangle(other.SettingsBounds.Location, other.SettingsBounds.Size);
            this.DetailsBounds = new Rectangle(other.DetailsBounds.Location, other.DetailsBounds.Size);
            this.FavoritesBounds = new Rectangle(other.FavoritesBounds.Location, other.FavoritesBounds.Size);
            this.HelpBounds = new Rectangle(other.HelpBounds.Location, other.HelpBounds.Size);
            this.ActiveSettings = other.ActiveSettings.Clone() as String;
            this.ToolbarScaling = other.ToolbarScaling;
            this.ToolbarText = other.ToolbarText;
        }

        #endregion // Class construction.

        #region Public property implementation.

        /// <summary>
        /// Gets and sets size and location of the program's main window.
        /// </summary>
        public Rectangle WindowBounds { get; set; }

        /// <summary>
        /// Gets and sets size and location of the program's settings dialog.
        /// </summary>
        public Rectangle SettingsBounds { get; set; }

        /// <summary>
        /// Gets and sets size and location of the program's details view.
        /// </summary>
        public Rectangle DetailsBounds { get; set; }

        /// <summary>
        /// Gets and sets size and location of the program's favorites dialog.
        /// </summary>
        public Rectangle FavoritesBounds { get; set; }

        /// <summary>
        /// Gets and sets size and location of the program's help dialog.
        /// </summary>
        public Rectangle HelpBounds { get; set; }

        /// <summary>
        /// Gets and sets last active settings page.
        /// </summary>
        public String ActiveSettings { get; set; }

        /// <summary>
        /// Gets and sets last known toolbar button size.
        /// </summary>
        public String ToolbarScaling { get; set; }

        /// <summary>
        /// Gets and sets last known toolbar text visibility state.
        /// </summary>
        public Boolean ToolbarText { get; set; }

        #endregion // Public property implementation.

        #region ICloneable member implementation.

        public Object Clone()
        {
            return new Maintain(this);
        }

        #endregion // ICloneable member implementation.
    }

    public class Favorites : ICloneable
    {
        #region Class construction.

        public Favorites()
            : base()
        {
            this.Folders = null;
        }

        public Favorites(Favorites other)
            : this()
        {
            this.Folders = other.Folders.ToArray();
        }

        #endregion // Class construction.

        #region Public property implementation.

        private List<String> folders = null;
        public String[] Folders
        {
            get { return this.folders.ToArray(); }
            set
            {
                this.folders = new List<String>();
                if (value != null && value.Length > 0)
                {
                    this.folders.AddRange(value);
                }
            }
        }

        #endregion // Public property implementation.

        #region ICloneable member implementation.

        public Object Clone()
        {
            return new Favorites(this);
        }

        #endregion // ICloneable member implementation.
    }

    public class Behaviour : ICloneable
    {
        #region Class construction.

        public Behaviour()
            : base()
        {
            this.AllowAutoClose = false;
            this.UseFullResources = false;
            this.IncludeFolderNames = true;
            this.AutoPauseWiping = false;
            this.AllowAutoRelaunch = false;
            this.SuppressCancelQuestion = true;
        }

        public Behaviour(Behaviour other)
            : this()
        {
            this.AllowAutoClose = other.AllowAutoClose;
            this.UseFullResources = other.UseFullResources;
            this.IncludeFolderNames = other.IncludeFolderNames;
            this.AutoPauseWiping = other.AutoPauseWiping;
            this.AllowAutoRelaunch = other.AllowAutoRelaunch;
            this.SuppressCancelQuestion = other.SuppressCancelQuestion;
        }

        #endregion // Class construction.

        #region Public property implementation.

        public Boolean AllowAutoClose { get; set; }

        public Boolean UseFullResources { get; set; }

        public Boolean IncludeFolderNames { get; set; }

        public Boolean AutoPauseWiping { get; set; }

        public Boolean AllowAutoRelaunch { get; set; }

        public Boolean SuppressCancelQuestion { get; set; }

        #endregion // Public property implementation.

        #region ICloneable member implementation.

        public Object Clone()
        {
            return new Behaviour(this);
        }

        #endregion // ICloneable member implementation.
    }

    public class WipingProcessing : ICloneable
    {
        #region Class construction.

        public WipingProcessing()
            : base()
        {
            this.AllowParallel = true;
            this.ThreadCount = 10;
        }

        public WipingProcessing(WipingProcessing other)
            : base()
        {
            this.AllowParallel = other.AllowParallel;
            this.ThreadCount = other.ThreadCount;

        }

        #endregion // Class construction.

        #region Public static property implementation.

        public static Int32 ThreadCountMinimum
        {
            get
            {
                return 1;
            }
        }

        public static Int32 ThreadCountMaximum
        {
            get
            {
                return 10;
            }
        }

        #endregion // Public static property implementation.

        #region Public property implementation.

        [XmlAttribute]
        public Boolean AllowParallel { get; set; }

        [XmlAttribute]
        public Int32 ThreadCount { get; set; }

        #endregion // Public property implementation.

        #region ICloneable member implementation.

        public Object Clone()
        {
            return new WipingProcessing(this);
        }

        #endregion // ICloneable member implementation.
    }
}
