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
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Plexdata.FileWiper
{
    internal class ShellExtensionHandler
    {
        public ShellExtensionHandler()
            : base()
        {
            this.Configuration = null;
            this.Filename = String.Empty;
        }

        public static Boolean RegisterExtension(String filename)
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

        public static Boolean UnregisterExtension()
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

        public static String ConvertToIconData(Icon icon)
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

        public Boolean LoadFromResources(Boolean defaults)
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
        public Boolean ExportToFile()
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
        public Boolean ExportToFile(String fullpath)
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
        public String Filename { get; private set; }

        public Boolean IsRegistered
        {
            get
            {
                // Location: HKEY_LOCAL_MACHINE\SOFTWARE\plexdata\pdcmse\InstalledExtensions
                try
                {
                    String classid = this.ClassID;
                    String subkey = @"SOFTWARE\plexdata\pdcmse\InstalledExtensions";

                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(subkey))
                    {
                        if (key != null) // BUGFIX: Check returned key before accessing it.
                        {
                            foreach (String value in key.GetValueNames())
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
        public String ClassID
        {
            get
            {
                const String OPEN = "{";
                const String CLOSE = "}";
                String result = String.Empty;

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
        public String Executable
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
        public String Label
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
        /// Gets or sets Shell Extension help String to be shown in the Windows Explorer.
        /// </summary>
        public String HelpString
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
        public String IconData
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

        private static String ByteArrayToString(Byte[] input)
        {
            if (input != null && input.Length > 0)
            {
                StringBuilder result = new StringBuilder(input.Length * 2);

                foreach (Byte current in input)
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

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private static Byte[] StringToByteArray(String input)
        {
            if (!String.IsNullOrEmpty(input))
            {
                Byte[] result = new Byte[input.Length / 2];

                for (Int32 index = 0; index < input.Length; index += 2)
                {
                    result[index / 2] = Convert.ToByte(input.Substring(index, 2), 16);
                }

                return result;
            }
            else
            {
                return new Byte[0];
            }
        }

        private Boolean InitFromRegistry()
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
                String subkey = String.Format(@"SOFTWARE\Classes\CLSID\{0}\Settings\MenuItemEntry0", this.ClassID);
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(subkey))
                {
                    this.Label = key.GetValue("Label") as String;
                    this.HelpString = key.GetValue("HelpString") as String;
                    this.IconData = ByteArrayToString(key.GetValue("IconData") as Byte[]);
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
        private static extern void RegisterExtension32(IntPtr hWnd, IntPtr hInstance, String szParameter, Int32 nShow);

        [DllImport("pdcmse32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UnregisterExtensionW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern void UnregisterExtension32(IntPtr hWnd, IntPtr hInstance, String szParameter, Int32 nShow);

        [DllImport("pdcmse64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "RegisterExtensionW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern void RegisterExtension64(IntPtr hWnd, IntPtr hInstance, String szParameter, Int32 nShow);

        [DllImport("pdcmse64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UnregisterExtensionW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern void UnregisterExtension64(IntPtr hWnd, IntPtr hInstance, String szParameter, Int32 nShow);

        #endregion // Shell Extension import declaration section.
    }

    [XmlRoot("ShellExtensionConfiguration")]
    public class ShellExtensionConfiguration
    {
        public ShellExtensionConfiguration()
        {
        }

        [XmlElement("Author")]
        public String Author { get; set; }

        [XmlElement("Description")]
        public String Description { get; set; }

        [XmlElement("LastSaved")]
        public String LastSaved { get; set; }

        [XmlArray("ShellExtensionList")]
        public ShellExtensionEntry[] ShellExtensionList { get; set; }
    }

    public class ShellExtensionEntry
    {
        public ShellExtensionEntry()
        {
        }

        [XmlElement("ClassID")]
        public String ClassID { get; set; }

        [XmlElement("Name")]
        public String Name { get; set; }

        [XmlElement("Description")]
        public String Description { get; set; }

        [XmlElement("Locations")]
        public String Locations { get; set; }

        [XmlArray("MenuItemList")]
        public MenuItemEntry[] MenuItemList { get; set; }
    }

    public class MenuItemEntry
    {
        public MenuItemEntry()
        {
        }

        [XmlElement("Label")]
        public String Label { get; set; }

        [XmlElement("Executable")]
        public String Executable { get; set; }

        [XmlElement("Parameter")]
        public String Parameter { get; set; }

        [XmlElement("HelpString")]
        public String HelpString { get; set; }

        [XmlElement("IconFile")]
        public String IconFile { get; set; }

        [XmlElement("IconData")]
        public String IconData { get; set; }

        [XmlElement("MultiSelect")]
        public String MultiSelect { get; set; }

        [XmlArray("MenuItemList")]
        public MenuItemEntry[] MenuItemList { get; set; }
    }
}
