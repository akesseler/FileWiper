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
using System.Text;
using System.Drawing;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Xml.Serialization;
using System.Runtime.InteropServices;

namespace plexdata.FileWiper
{
    internal class ShellExtensionHandler
    {
        public ShellExtensionHandler()
            : base()
        {
            this.Configuration = null;
            this.Filename = String.Empty;
        }

        public static bool RegisterExtension(string filename)
        {
            try
            {
                Program.TraceLogger.Write("ShellExtensionHandler", ">>> RegisterExtension(" + (String.IsNullOrEmpty(filename) ? "" : filename) + ")");
                Program.TraceLogger.Write("ShellExtensionHandler", "--- Admin Mode = " + PermissionCheck.IsRunAsAdmin);

                if (PlatformCheck.Is64BitPlatform)
                {
                    Program.TraceLogger.Write("ShellExtensionHandler", "--- Try register x64 version.");
                    ShellExtensionHandler.RegisterExtension64(IntPtr.Zero, IntPtr.Zero, filename, 0);
                }
                else
                {
                    Program.TraceLogger.Write("ShellExtensionHandler", "--- Try register x86 version.");
                    ShellExtensionHandler.RegisterExtension32(IntPtr.Zero, IntPtr.Zero, filename, 0);
                }

                Program.TraceLogger.Write("ShellExtensionHandler", "--- Registration done.");
                return true;
            }
            catch (Exception exception)
            {
                Program.FatalLogger.Write("ShellExtensionHandler", exception);
            }
            finally
            {
                Program.TraceLogger.Write("ShellExtensionHandler", "<<< RegisterExtension()");
            }
            return false;
        }

        public static bool UnregisterExtension()
        {
            try
            {
                ShellExtensionHandler handler = new ShellExtensionHandler();
                if (handler.LoadFromResources(true))
                {
                    Program.TraceLogger.Write("ShellExtensionHandler", ">>> UnregisterExtension(" + handler.ClassID + ")");
                    Program.TraceLogger.Write("ShellExtensionHandler", "--- Admin Mode = " + PermissionCheck.IsRunAsAdmin);

                    if (PlatformCheck.Is64BitPlatform)
                    {
                        Program.TraceLogger.Write("ShellExtensionHandler", "--- Try unregister x64 version.");
                        ShellExtensionHandler.UnregisterExtension64(IntPtr.Zero, IntPtr.Zero,
                            handler.ClassID, 0);
                    }
                    else
                    {
                        Program.TraceLogger.Write("ShellExtensionHandler", "--- Try unregister x86 version.");
                        ShellExtensionHandler.UnregisterExtension32(IntPtr.Zero, IntPtr.Zero,
                            handler.ClassID, 0);
                    }

                    Program.TraceLogger.Write("ShellExtensionHandler", "--- Deregistration done.");
                    return true;
                }
                else
                {
                    Program.TraceLogger.Write("ShellExtensionHandler", "--- Deregistration failed! Could not load internal resources.");
                }
            }
            catch (Exception exception)
            {
                Program.FatalLogger.Write("ShellExtensionHandler", exception);
            }
            finally
            {
                Program.TraceLogger.Write("ShellExtensionHandler", "<<< UnregisterExtension()");
            }
            return false;
        }

        public static string ConvertToIconData(Icon icon)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    // NOTE: The icons in size 12x12 using a white background.
                    // This special white background has been included because of that Windows XP 
                    // is unable to treat bitmaps (these icons) as transparent. Therefore, having 
                    // a white bitmap background looks much better. Furthermore, using menu colours 
                    // different from white should not cause any kind of trouble because it seems 
                    // that Windows XP can handle this in a proper way.
                    Size size = PlatformCheck.IsVistaOrHigher ? new Size(16, 16) : new Size(12, 12);
                    Icon helper = new Icon(icon, size);
                    Bitmap bitmap = helper.ToBitmap();
                    bitmap.Save(stream, ImageFormat.Bmp);
                    return ByteArrayToString(stream.ToArray());
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("ShellExtensionHandler", exception);
            }
            return String.Empty;
        }

        public bool LoadFromResources(bool defaults)
        {
            try
            {
                using (StringReader strReader = new StringReader(Properties.Resources.pdcmse_config_filewiper))
                {
                    using (XmlTextReader xmlReader = new XmlTextReader(strReader))
                    {
                        if (xmlReader.Read())
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(ShellExtensionConfiguration));
                            this.Configuration = serializer.Deserialize(xmlReader) as ShellExtensionConfiguration;

                            if (this.IsRegistered && !defaults)
                            {
                                return this.InitFromRegistry();
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("ShellExtensionHandler", exception);
            }
            return false;
        }

        /// <summary>
        /// Exports current configuration into a temporary file. 
        /// </summary>
        /// <returns>
        /// True if export was successful and false otherwise.
        /// </returns>
        public bool ExportToFile()
        {
            try
            {
                return this.ExportToFile(Path.Combine(Path.GetTempPath(), Path.GetTempFileName()));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("ShellExtensionHandler", exception);
            }
            return false;
        }

        /// <summary>
        /// Exports current configuration into given file. 
        /// </summary>
        /// <param name="fullpath">
        /// Fully qualified file name to export current configuration to.
        /// </param>
        /// <returns>
        /// True if export was successful and false otherwise.
        /// </returns>
        public bool ExportToFile(string fullpath)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ShellExtensionConfiguration));
                using (TextWriter writer = new StreamWriter(fullpath))
                {
                    serializer.Serialize(writer, this.Configuration);
                    this.Filename = fullpath;
                    return true;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("ShellExtensionHandler", exception);
            }
            return false;
        }

        /// <summary>
        /// Gets last used export file name.
        /// </summary>
        public string Filename { get; private set; }

        public bool IsRegistered
        {
            get
            {
                // Location: HKEY_LOCAL_MACHINE\SOFTWARE\plexdata\pdcmse\InstalledExtensions
                try
                {
                    string classid = this.ClassID;
                    string subkey = @"SOFTWARE\plexdata\pdcmse\InstalledExtensions";

                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(subkey))
                    {
                        if (key != null) // BUGFIX: Check returned key before accessing it.
                        {
                            foreach (string value in key.GetValueNames())
                            {
                                if (String.Compare(value, classid, true) == 0)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                    Program.FatalLogger.Write("ShellExtensionHandler", exception);
                }
                return false;
            }
        }

        public ShellExtensionConfiguration Configuration { get; private set; }

        /// <summary>
        /// Gets current class ID loaded from resources file.
        /// </summary>
        public string ClassID
        {
            get
            {
                const string OPEN = "{";
                const string CLOSE = "}";
                string result = String.Empty;

                if (this.Configuration != null)
                {
                    if (this.Configuration.ShellExtensionList != null &&
                        this.Configuration.ShellExtensionList.Length > 0)
                    {
                        result = this.Configuration.ShellExtensionList[0].ClassID;
                        if (!result.StartsWith(OPEN)) { result = OPEN + result; }
                        if (!result.EndsWith(CLOSE)) { result = result + CLOSE; }
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Gets or sets fully qualified path to the executable file.
        /// </summary>
        public string Executable
        {
            get
            {
                if (this.Configuration != null)
                {
                    if (this.Configuration.ShellExtensionList != null &&
                        this.Configuration.ShellExtensionList.Length > 0)
                    {
                        if (this.Configuration.ShellExtensionList[0].MenuItemList != null &&
                            this.Configuration.ShellExtensionList[0].MenuItemList.Length > 0)
                        {
                            return this.Configuration.ShellExtensionList[0].MenuItemList[0].Executable;
                        }
                    }
                }
                return String.Empty;
            }
            set
            {
                if (this.Configuration != null && !String.IsNullOrEmpty(value))
                {
                    if (this.Configuration.ShellExtensionList != null &&
                        this.Configuration.ShellExtensionList.Length > 0)
                    {
                        if (this.Configuration.ShellExtensionList[0].MenuItemList != null &&
                            this.Configuration.ShellExtensionList[0].MenuItemList.Length > 0)
                        {
                            this.Configuration.ShellExtensionList[0].MenuItemList[0].Executable = value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets Shell Extension label to be shown in the Windows Explorer.
        /// </summary>
        public string Label
        {
            get
            {
                if (this.Configuration != null)
                {
                    if (this.Configuration.ShellExtensionList != null &&
                        this.Configuration.ShellExtensionList.Length > 0)
                    {
                        if (this.Configuration.ShellExtensionList[0].MenuItemList != null &&
                            this.Configuration.ShellExtensionList[0].MenuItemList.Length > 0)
                        {
                            return this.Configuration.ShellExtensionList[0].MenuItemList[0].Label;
                        }
                    }
                }
                return String.Empty;
            }
            set
            {
                if (this.Configuration != null && !String.IsNullOrEmpty(value))
                {
                    if (this.Configuration.ShellExtensionList != null &&
                        this.Configuration.ShellExtensionList.Length > 0)
                    {
                        if (this.Configuration.ShellExtensionList[0].MenuItemList != null &&
                            this.Configuration.ShellExtensionList[0].MenuItemList.Length > 0)
                        {
                            this.Configuration.ShellExtensionList[0].MenuItemList[0].Label = value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets Shell Extension help string to be shown in the Windows Explorer.
        /// </summary>
        public string HelpString
        {
            get
            {
                if (this.Configuration != null)
                {
                    if (this.Configuration.ShellExtensionList != null &&
                        this.Configuration.ShellExtensionList.Length > 0)
                    {
                        if (this.Configuration.ShellExtensionList[0].MenuItemList != null &&
                            this.Configuration.ShellExtensionList[0].MenuItemList.Length > 0)
                        {
                            return this.Configuration.ShellExtensionList[0].MenuItemList[0].HelpString;
                        }
                    }
                }
                return String.Empty;
            }
            set
            {
                if (this.Configuration != null && !String.IsNullOrEmpty(value))
                {
                    if (this.Configuration.ShellExtensionList != null &&
                        this.Configuration.ShellExtensionList.Length > 0)
                    {
                        if (this.Configuration.ShellExtensionList[0].MenuItemList != null &&
                            this.Configuration.ShellExtensionList[0].MenuItemList.Length > 0)
                        {
                            this.Configuration.ShellExtensionList[0].MenuItemList[0].HelpString = value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets Shell Extension icon to be shown in the Windows Explorer. 
        /// Please note using null or an empty string will disable icon usage!
        /// </summary>
        public string IconData
        {
            get
            {
                if (this.Configuration != null)
                {
                    if (this.Configuration.ShellExtensionList != null &&
                        this.Configuration.ShellExtensionList.Length > 0)
                    {
                        if (this.Configuration.ShellExtensionList[0].MenuItemList != null &&
                            this.Configuration.ShellExtensionList[0].MenuItemList.Length > 0)
                        {
                            return this.Configuration.ShellExtensionList[0].MenuItemList[0].IconData;
                        }
                    }
                }
                return String.Empty;
            }
            set
            {
                if (this.Configuration != null)
                {
                    if (this.Configuration.ShellExtensionList != null &&
                        this.Configuration.ShellExtensionList.Length > 0)
                    {
                        if (this.Configuration.ShellExtensionList[0].MenuItemList != null &&
                            this.Configuration.ShellExtensionList[0].MenuItemList.Length > 0)
                        {
                            if (!String.IsNullOrEmpty(value))
                            {
                                // Enable embedded icon. 
                                this.Configuration.ShellExtensionList[0].MenuItemList[0].IconFile = "(Embedded)";
                                this.Configuration.ShellExtensionList[0].MenuItemList[0].IconData = value;
                            }
                            else
                            {
                                // Disable icon completely. 
                                this.Configuration.ShellExtensionList[0].MenuItemList[0].IconFile = String.Empty;
                                this.Configuration.ShellExtensionList[0].MenuItemList[0].IconData = String.Empty;
                            }
                        }
                    }
                }
            }
        }

        private static string ByteArrayToString(byte[] input)
        {
            if (input != null && input.Length > 0)
            {
                StringBuilder result = new StringBuilder(input.Length * 2);

                foreach (byte current in input)
                {
                    result.AppendFormat("{0:X2}", current);
                }

                return result.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        private static byte[] StringToByteArray(string input)
        {
            if (!String.IsNullOrEmpty(input))
            {
                byte[] result = new byte[input.Length / 2];

                for (int index = 0; index < input.Length; index += 2)
                {
                    result[index / 2] = Convert.ToByte(input.Substring(index, 2), 16);
                }

                return result;
            }
            else
            {
                return new byte[0];
            }
        }

        private bool InitFromRegistry()
        {
            // [HKEY_LOCAL_MACHINE\SOFTWARE\Classes\CLSID\<clsid>\Settings]
            // "Name"="<name>"
            // "Description"="<description>"
            // "Locations"="(Files)(Folders)"
            // [HKEY_LOCAL_MACHINE\SOFTWARE\Classes\CLSID\<clsid>\Settings\MenuItemEntry0]
            // "Label"="<label>"
            // "Executable"="<executable>"
            // "Parameter"=""
            // "HelpString"="<helpstring>"
            // "IconFile"="(Embedded)"
            // "IconData"=hex:42,4d,36,04,00,00,00,00,00,00,...
            // "MultiSelect"="False"

            try
            {
                string subkey = String.Format(@"SOFTWARE\Classes\CLSID\{0}\Settings\MenuItemEntry0", this.ClassID);
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(subkey))
                {
                    this.Label = key.GetValue("Label") as string;
                    this.HelpString = key.GetValue("HelpString") as string;
                    this.IconData = ByteArrayToString(key.GetValue("IconData") as byte[]);
                    return true;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Program.FatalLogger.Write("ShellExtensionHandler", exception);
            }
            return false;
        }

        #region Shell Extension import declaration section.

        [DllImport("pdcmse32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "RegisterExtensionW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern void RegisterExtension32(IntPtr hWnd, IntPtr hInstance, string szParameter, int nShow);

        [DllImport("pdcmse32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UnregisterExtensionW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern void UnregisterExtension32(IntPtr hWnd, IntPtr hInstance, string szParameter, int nShow);

        [DllImport("pdcmse64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "RegisterExtensionW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern void RegisterExtension64(IntPtr hWnd, IntPtr hInstance, string szParameter, int nShow);

        [DllImport("pdcmse64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UnregisterExtensionW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern void UnregisterExtension64(IntPtr hWnd, IntPtr hInstance, string szParameter, int nShow);

        #endregion // Shell Extension import declaration section.
    }

    [XmlRoot("ShellExtensionConfiguration")]
    public class ShellExtensionConfiguration
    {
        public ShellExtensionConfiguration()
        {
        }

        [XmlElement("Author")]
        public string Author { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlElement("LastSaved")]
        public string LastSaved { get; set; }

        [XmlArray("ShellExtensionList")]
        public ShellExtensionEntry[] ShellExtensionList { get; set; }
    }

    public class ShellExtensionEntry
    {
        public ShellExtensionEntry()
        {
        }

        [XmlElement("ClassID")]
        public string ClassID { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlElement("Locations")]
        public string Locations { get; set; }

        [XmlArray("MenuItemList")]
        public MenuItemEntry[] MenuItemList { get; set; }
    }

    public class MenuItemEntry
    {
        public MenuItemEntry()
        {
        }

        [XmlElement("Label")]
        public string Label { get; set; }

        [XmlElement("Executable")]
        public string Executable { get; set; }

        [XmlElement("Parameter")]
        public string Parameter { get; set; }

        [XmlElement("HelpString")]
        public string HelpString { get; set; }

        [XmlElement("IconFile")]
        public string IconFile { get; set; }

        [XmlElement("IconData")]
        public string IconData { get; set; }

        [XmlElement("MultiSelect")]
        public string MultiSelect { get; set; }

        [XmlArray("MenuItemList")]
        public MenuItemEntry[] MenuItemList { get; set; }
    }
}
