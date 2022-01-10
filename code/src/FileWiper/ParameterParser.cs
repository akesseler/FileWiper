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
using System.Collections.Specialized;
using System.Text.RegularExpressions;

//
// This class has been inspired by a similar class written by Richard Lopes. 
// For more details see link below.
// http://www.codeproject.com/Articles/3111/C-NET-Command-Line-Arguments-Parser
//
namespace plexdata.FileWiper
{
    internal class ParameterParser
    {
        public static string REGISTER = "register";
        public static string UNREGISTER = "unregister";
        public static string FORCESHOW = "forceshow";
        public static string RELAUNCH = "relaunch";

        private const string ENABLED = "enabled";

        private const string QUOTE = "\"";
        private const string SPACE = " ";

        private StringDictionary parameters = null;
        private StringCollection filepaths = null;

        public ParameterParser()
            : base()
        {
            StringCollection helper = new StringCollection();
            helper.AddRange(Environment.GetCommandLineArgs());

            if (helper.Count > 1)
            {
                // Remove leading executable file name.
                helper.RemoveAt(0);

                string[] args = new string[helper.Count];
                helper.CopyTo(args, 0);
                this.Parse(args);
            }
            else
            {
                this.parameters = new StringDictionary();
                this.filepaths = new StringCollection();
            }
        }

        public ParameterParser(string[] args)
            : base()
        {
            this.Parse(args);
        }

        public void Parse(string[] args)
        {
            //
            // Rules:
            //  * Each parameter name starts with (-) or (--) or (/).
            //  * A parameter can have an additional value.
            //    ~ If a parameter has an additional value then this value is 
            //      separated either by (=) or by ( ).
            //    ~ If a value itself contains spaces then this value must be 
            //      surrounded by ("...")!
            //  * Colons (:) to separate parameters and values are not supported!
            //  * Single quote ('...') to enclose values with spaces are not supported!
            //  * Values must not include parameter separators as leading characters!
            //
            // These rules are necessary because otherwise trouble comes up. 
            // One reason is that the original implementation contains a bug  
            // which couldn't be solved. Another reason is the filtering of  
            // full qualified file paths; because those file paths contain a 
            // colon as drive separator.
            //
            // Samples:
            //   -param, --param, /param
            //   -param value, --param value, /param value
            //   -param=value, --param=value, /param=value
            //   -param "Hello World", --param "Hello World", /param "Hello World"
            //   -param="Hello World", --param="Hello World", /param="Hello World"
            //
            // TODO: Find a better solution for processing given parameters/values.
            //       E.g. to support -param:"value value", but that takes care about paths.
            //

            Regex spliter = new Regex(@"^-{1,2}|^/|=", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Regex cleaner = new Regex(@"^[""]?(.*?)[""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            string param = null;

            // Reset member variables.
            this.parameters = new StringDictionary();
            this.filepaths = new StringCollection();

            if (args != null)
            {
                foreach (string current in args)
                {
                    // Check if current argument is a file path 
                    // without a leading parameter name.
                    Uri filepath = null;
                    if (param == null && Uri.TryCreate(current, UriKind.Absolute, out filepath))
                    {
                        this.filepaths.Add(current);
                        continue;
                    }

                    // Try split parameters at (-), (/), (--) and values at (=), ( ).
                    string[] parts = spliter.Split(current, 3);

                    // Check and assign results.
                    if (parts.Length == 1)
                    {
                        if (param != null)
                        {
                            if (!this.parameters.ContainsKey(param))
                            {
                                parts[0] = cleaner.Replace(parts[0], "$1");
                                this.parameters.Add(param, parts[0]);
                            }
                            param = null;
                        }
                    }
                    else if (parts.Length == 2)
                    {
                        if (param != null && !this.parameters.ContainsKey(param))
                        {
                            this.parameters.Add(param, ENABLED);
                        }

                        param = parts[1];
                    }
                    else if (parts.Length == 3)
                    {
                        if (param != null && !this.parameters.ContainsKey(param))
                        {
                            this.parameters.Add(param, ENABLED);
                        }

                        param = parts[1];

                        // Remove possible enclosing characters (").
                        if (!this.parameters.ContainsKey(param))
                        {
                            parts[2] = cleaner.Replace(parts[2], "$1");
                            this.parameters.Add(param, parts[2]);
                        }
                        param = null;
                    }
                }

                if (param != null && !this.parameters.ContainsKey(param))
                {
                    this.parameters.Add(param, ENABLED);
                }
            }
        }

        public static string CombineOptions(params string[] options)
        {
            string result = String.Empty;
            for (int index = 0; index < options.Length; index++)
            {
                result += options[index].Trim() + SPACE;
            }
            return result.Trim();
        }

        public static string BuildOption(string option)
        {
            if (option == null)
            {
                throw new ArgumentNullException("option");
            }
            else
            {
                option = option.Trim();
                if (option == String.Empty)
                {
                    throw new ArgumentException(null, "option");
                }
                else
                {
                    return "--" + option.Trim();
                }
            }
        }

        public static string BuildOption(string option, string argument)
        {
            if (argument != null) { argument = argument.Trim(); }

            return ParameterParser.BuildOption(option) +
                (argument != String.Empty ? SPACE + argument : String.Empty);
        }

        public static string BuildOption(string option, string argument, bool quotes)
        {
            if (quotes && argument != null)
            {
                argument = argument.Trim();
                if (argument != String.Empty)
                {
                    if (!argument.StartsWith(QUOTE)) { argument = QUOTE + argument; }
                    if (!argument.EndsWith(QUOTE)) { argument += QUOTE; }
                }
            }
            return ParameterParser.BuildOption(option, argument);
        }

        public static string BuildOptionRegister(string fullpath)
        {
            return ParameterParser.BuildOption(ParameterParser.REGISTER, fullpath, true);
        }

        public static string BuildOptionUnregister()
        {
            return ParameterParser.BuildOption(ParameterParser.UNREGISTER);
        }

        public static string BuildOptionForceShow()
        {
            return ParameterParser.BuildOption(ParameterParser.FORCESHOW);
        }

        public static string BuildOptionRelaunch()
        {
            return ParameterParser.BuildOption(ParameterParser.RELAUNCH);
        }

        public static string BuildOptionFilepaths(string[] filepaths)
        {
            string result = String.Empty;
            if (filepaths != null)
            {
                foreach (string filepath in filepaths)
                {
                    string helper = filepath.Trim();
                    if (!helper.StartsWith(QUOTE)) { helper = QUOTE + helper; }
                    if (!helper.EndsWith(QUOTE)) { helper += QUOTE; }
                    result += helper + SPACE;
                }
            }
            return result.Trim();
        }

        public bool IsOption(string option)
        {
            if (String.IsNullOrEmpty(option))
            {
                throw new ArgumentNullException("option");
            }
            else
            {
                return this[option] != null;
            }
        }

        public bool IsRegister
        {
            get { return this.IsOption(REGISTER); }
        }

        public string RegisterOption
        {
            get
            {
                if (this.IsRegister)
                {
                    return this[REGISTER];
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public bool IsUnregister
        {
            get { return this.IsOption(UNREGISTER); }
        }

        public bool IsForceShow
        {
            get { return this.IsOption(FORCESHOW); }
        }

        public bool IsRelaunch
        {
            get { return this.IsOption(RELAUNCH); }
        }

        public bool HasFilepaths
        {
            get { return this.filepaths != null && this.filepaths.Count > 0; }
        }

        public string[] Filepaths
        {
            get
            {
                if (this.filepaths != null)
                {
                    string[] result = new string[this.filepaths.Count];
                    this.filepaths.CopyTo(result, 0);
                    return result;
                }
                else
                {
                    return new string[0];
                }
            }
        }

        // Retrieve a parameter value if it exists.
        // If key is not null and it does not exist 
        // then null is returned or this key is created.
        public string this[string parameter]
        {
            get
            {
                return (this.parameters[parameter]);
            }
        }

        public override string ToString()
        {
            string result = String.Empty;

            if (this.filepaths != null && this.filepaths.Count > 0)
            {
                result += "Filepaths:\r\n";
                foreach (string filepath in this.filepaths)
                {
                    result += "value[" + filepath + "]\r\n";
                }
            }

            if (this.parameters != null && this.parameters.Count > 0)
            {
                result += "Parameters:\r\n";
                foreach (string parameter in this.parameters.Keys)
                {
                    result += "param[" + parameter + "], value[" + this.parameters[parameter] + "]\r\n";
                }
            }

            return String.IsNullOrEmpty(result) ? base.ToString() : result;
        }
    }
}
