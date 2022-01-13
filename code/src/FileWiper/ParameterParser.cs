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
using System.Collections.Specialized;
using System.Text.RegularExpressions;

//
// This class has been inspired by a similar class written by Richard Lopes. 
// For more details see link below.
// http://www.codeproject.com/Articles/3111/C-NET-Command-Line-Arguments-Parser
//
namespace Plexdata.FileWiper
{
    internal class ParameterParser
    {
        public static String REGISTER = "register";
        public static String UNREGISTER = "unregister";
        public static String FORCESHOW = "forceshow";
        public static String RELAUNCH = "relaunch";

        private const String ENABLED = "enabled";

        private const String QUOTE = "\"";
        private const String SPACE = " ";

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

                String[] args = new String[helper.Count];
                helper.CopyTo(args, 0);
                this.Parse(args);
            }
            else
            {
                this.parameters = new StringDictionary();
                this.filepaths = new StringCollection();
            }
        }

        public ParameterParser(String[] args)
            : base()
        {
            this.Parse(args);
        }

        public void Parse(String[] args)
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
            String param = null;

            // Reset member variables.
            this.parameters = new StringDictionary();
            this.filepaths = new StringCollection();

            if (args != null)
            {
                foreach (String current in args)
                {
                    // Check if current argument is a file path 
                    // without a leading parameter name.
                    if (param == null && Uri.TryCreate(current, UriKind.Absolute, out Uri filepath))
                    {
                        this.filepaths.Add(current);
                        continue;
                    }

                    // Try split parameters at (-), (/), (--) and values at (=), ( ).
                    String[] parts = spliter.Split(current, 3);

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

        public static String CombineOptions(params String[] options)
        {
            String result = String.Empty;
            for (Int32 index = 0; index < options.Length; index++)
            {
                result += options[index].Trim() + SPACE;
            }
            return result.Trim();
        }

        public static String BuildOption(String option)
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

        public static String BuildOption(String option, String argument)
        {
            if (argument != null) { argument = argument.Trim(); }

            return ParameterParser.BuildOption(option) +
                (argument != String.Empty ? SPACE + argument : String.Empty);
        }

        public static String BuildOption(String option, String argument, Boolean quotes)
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

        public static String BuildOptionRegister(String fullpath)
        {
            return ParameterParser.BuildOption(ParameterParser.REGISTER, fullpath, true);
        }

        public static String BuildOptionUnregister()
        {
            return ParameterParser.BuildOption(ParameterParser.UNREGISTER);
        }

        public static String BuildOptionForceShow()
        {
            return ParameterParser.BuildOption(ParameterParser.FORCESHOW);
        }

        public static String BuildOptionRelaunch()
        {
            return ParameterParser.BuildOption(ParameterParser.RELAUNCH);
        }

        public static String BuildOptionFilepaths(String[] filepaths)
        {
            String result = String.Empty;
            if (filepaths != null)
            {
                foreach (String filepath in filepaths)
                {
                    String helper = filepath.Trim();
                    if (!helper.StartsWith(QUOTE)) { helper = QUOTE + helper; }
                    if (!helper.EndsWith(QUOTE)) { helper += QUOTE; }
                    result += helper + SPACE;
                }
            }
            return result.Trim();
        }

        public Boolean IsOption(String option)
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

        public Boolean IsRegister
        {
            get { return this.IsOption(REGISTER); }
        }

        public String RegisterOption
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

        public Boolean IsUnregister
        {
            get { return this.IsOption(UNREGISTER); }
        }

        public Boolean IsForceShow
        {
            get { return this.IsOption(FORCESHOW); }
        }

        public Boolean IsRelaunch
        {
            get { return this.IsOption(RELAUNCH); }
        }

        public Boolean HasFilepaths
        {
            get { return this.filepaths != null && this.filepaths.Count > 0; }
        }

        public String[] Filepaths
        {
            get
            {
                if (this.filepaths != null)
                {
                    String[] result = new String[this.filepaths.Count];
                    this.filepaths.CopyTo(result, 0);
                    return result;
                }
                else
                {
                    return new String[0];
                }
            }
        }

        // Retrieve a parameter value if it exists.
        // If key is not null and it does not exist 
        // then null is returned or this key is created.
        public String this[String parameter]
        {
            get
            {
                return (this.parameters[parameter]);
            }
        }

        public override String ToString()
        {
            String result = String.Empty;

            if (this.filepaths != null && this.filepaths.Count > 0)
            {
                result += "Filepaths:\r\n";
                foreach (String filepath in this.filepaths)
                {
                    result += "value[" + filepath + "]\r\n";
                }
            }

            if (this.parameters != null && this.parameters.Count > 0)
            {
                result += "Parameters:\r\n";
                foreach (String parameter in this.parameters.Keys)
                {
                    result += "param[" + parameter + "], value[" + this.parameters[parameter] + "]\r\n";
                }
            }

            return String.IsNullOrEmpty(result) ? base.ToString() : result;
        }
    }
}
