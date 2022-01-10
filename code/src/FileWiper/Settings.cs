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
using System.IO;
using System.Xml;
using System.Linq;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace plexdata.FileWiper
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

        public static string Filename
        {
            get { return Path.ChangeExtension(Application.ExecutablePath, ".cfg"); }
        }

        #endregion // Public static property implementation.

        #region Public static member implementation.

        public static bool Save(string filename, Settings root)
        {
            bool success = false;
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

        public static bool Load(string filename, out Settings root)
        {
            bool success = false;
            XmlSerializer serializer = null;
            TextReader reader = null;

            root = default(Settings);

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

        public static bool IsVisibleOnAllScreens(Rectangle bounds)
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

        public object Clone()
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
            this.ActiveSettings = other.ActiveSettings.Clone() as string;
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
        public string ActiveSettings { get; set; }

        /// <summary>
        /// Gets and sets last known toolbar button size.
        /// </summary>
        public string ToolbarScaling { get; set; }

        /// <summary>
        /// Gets and sets last known toolbar text visibility state.
        /// </summary>
        public bool ToolbarText { get; set; }

        #endregion // Public property implementation.

        #region ICloneable member implementation.

        public object Clone()
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

        private List<string> folders = null;
        public string[] Folders
        {
            get { return this.folders.ToArray(); }
            set
            {
                this.folders = new List<string>();
                if (value != null && value.Length > 0)
                {
                    this.folders.AddRange(value);
                }
            }
        }

        #endregion // Public property implementation.

        #region ICloneable member implementation.

        public object Clone()
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

        public bool AllowAutoClose { get; set; }

        public bool UseFullResources { get; set; }

        public bool IncludeFolderNames { get; set; }

        public bool AutoPauseWiping { get; set; }

        public bool AllowAutoRelaunch { get; set; }

        public bool SuppressCancelQuestion { get; set; }

        #endregion // Public property implementation.

        #region ICloneable member implementation.

        public object Clone()
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

        public static int ThreadCountMinimum
        {
            get
            {
                return 1;
            }
        }

        public static int ThreadCountMaximum
        {
            get
            {
                return 10;
            }
        }

        #endregion // Public static property implementation.

        #region Public property implementation.

        [XmlAttribute]
        public bool AllowParallel { get; set; }

        [XmlAttribute]
        public int ThreadCount { get; set; }

        #endregion // Public property implementation.

        #region ICloneable member implementation.

        public object Clone()
        {
            return new WipingProcessing(this);
        }

        #endregion // ICloneable member implementation.
    }
}
