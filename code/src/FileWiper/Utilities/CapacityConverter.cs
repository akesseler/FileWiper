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

namespace plexdata.Utilities
{
    public static class CapacityConverter
    {
        private class CapacityItem
        {
            public CapacityItem(double value, string format1, string format2)
            {
                this.Value = value;
                this.Format1 = format1;
                this.Format2 = format2;
            }
            public double Value { get; private set; }
            public string Format1 { get; private set; }
            public string Format2 { get; private set; }
        }

        private const double BiB = 1d;                         // Bibibyte = 2^1  Byte
        private const double KiB = 1024d;                      // Kibibyte = 2^10 Byte
        private const double MiB = 1048576d;                   // Mebibyte = 2^20 Byte
        private const double GiB = 1073741824d;                // Gibibyte = 2^30 Byte
        private const double TiB = 1099511627776d;             // Tebibyte = 2^40 Byte
        private const double PiB = 1125899906842624d;          // Pebibyte = 2^50 Byte
        private const double EiB = 1152921504606846976d;       // Exbibyte = 2^60 Byte
        private const double ZiB = 1180591620717411303424d;    // Zebibyte = 2^70 Byte
        private const double YiB = 1208925819614629174706176d; // Yobibyte = 2^80 Byte

        // Inverted list item order!
        private static CapacityItem[] capacityList = new CapacityItem[]{
            new CapacityItem(YiB, "{0:N0} YB",   "{0:N1} YB"),
            new CapacityItem(ZiB, "{0:N0} ZB",   "{0:N1} ZB"),
            new CapacityItem(EiB, "{0:N0} EB",   "{0:N1} EB"),
            new CapacityItem(PiB, "{0:N0} PB",   "{0:N1} PB"),
            new CapacityItem(TiB, "{0:N0} TB",   "{0:N1} TB"),
            new CapacityItem(GiB, "{0:N0} GB",   "{0:N1} GB"),
            new CapacityItem(MiB, "{0:N0} MB",   "{0:N1} MB"),
            new CapacityItem(KiB, "{0:N0} KB",   "{0:N1} KB"),
            new CapacityItem(BiB, "{0:N0} Byte", "{0:N1} Byte"),
        };

        public static string Convert(double value)
        {
            string result = String.Empty;
            string format = String.Empty;

            if (value == 0)
            {
                // The capacity converter should return 
                // a "0 Byte" string if value is zero!
                format = "{0:N0} Byte";
            }
            else
            {
                foreach (CapacityItem current in CapacityConverter.capacityList)
                {
                    if (value / current.Value >= 1)
                    {
                        value = value / current.Value;
                        value = (value > 100) ? Math.Round(value, 0) : Math.Round(value, 1);
                        format = (value > 100) ? current.Format1 : current.Format2;
                        break;
                    }
                }
            }
            return String.Format(
                format,
                value.ToString(System.Globalization.NumberFormatInfo.CurrentInfo));
        }
    }
}
