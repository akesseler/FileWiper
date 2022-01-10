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
using System.Runtime.InteropServices;

namespace Plexdata.FileWiper
{
    internal static class PlatformCheck
    {
        /// <summary>
        /// Determines whether current system is Vista or higher.
        /// </summary>
        /// <returns>
        /// True if current system is Vista or higher and false 
        /// if not.
        /// </returns>
        /// <exception cref="Win32Exception">
        /// This exception occurs when getting current version 
        /// information has failed.
        /// </exception>
        internal static bool IsVistaOrHigher
        {
            get
            {
                OSVERSIONINFO version = new OSVERSIONINFO();
                version.size = Marshal.SizeOf(typeof(OSVERSIONINFO));
                if (!GetVersionEx(ref version))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // A system is running under Vista or higher as soon 
                // as the major version is greater or equal 6.
                return version.major >= 6;
            }
        }

        /// <summary>
        /// Determines whether current platform is a 64-bit platform.
        /// </summary>
        /// <returns>
        /// True if current platform is a 64-bit platform and false 
        /// if current platform is either a x86-platform or platform 
        /// type is unknown.
        /// </returns>
        internal static bool Is64BitPlatform
        {
            get
            {
                /// Indicates an Intel or AMD based x64 platform.
                const int PROCESSOR_ARCHITECTURE_AMD64 = 0x0009;

                /// Indicates an Intel Itanium based Processor (IPF).
                const int PROCESSOR_ARCHITECTURE_IA64 = 0x0006;

                SYSTEM_INFO info = new SYSTEM_INFO();
                GetNativeSystemInfo(out info);
                return info.architecture == PROCESSOR_ARCHITECTURE_AMD64 ||
                       info.architecture == PROCESSOR_ARCHITECTURE_IA64;
            }
        }

        #region Win32 related declaration and implementation section.

        /// <summary>
        /// Contains operating system version information.
        /// </summary>
        /// <remarks>
        /// The information includes major and minor version 
        /// numbers, a build number, a platform identifier, 
        /// and descriptive text about the operating system.
        /// </remarks>
        /// <seealso cref="GetVersionEx">
        /// GetVersionEx
        /// </seealso>
        [StructLayout(LayoutKind.Sequential)]
        private struct OSVERSIONINFO
        {
            /// <summary>
            /// The size of this data structure, in bytes. 
            /// Set this member to sizeof(OSVERSIONINFO). 
            /// </summary>
            public Int32 size;

            /// <summary>
            /// The major version number of the operating system.
            /// </summary>
            public Int32 major;

            /// <summary>
            /// The minor version number of the operating system.
            /// </summary>
            public Int32 minor;

            /// <summary>
            /// The build number of the operating system.
            /// </summary>
            public Int32 build;

            /// <summary>
            /// The operating system platform.
            /// </summary>
            public Int32 platform;

            /// <summary>
            /// A null-terminated string that indicates the latest Service 
            /// Pack installed on the system. If no Service Pack has been 
            /// installed, the string is empty.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string servicepack;
        }

        /// <summary>
        /// Contains information about the current computer system.
        /// </summary>
        /// <remarks>
        /// This information includes the architecture and type of 
        /// the processor, the number of processors in the system, 
        /// the page size, and other such information.
        /// </remarks>
        /// <seealso cref="GetNativeSystemInfo">GetNativeSystemInfo</seealso>
        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_INFO
        {
            /// <summary>
            /// The processor architecture of the installed operating 
            /// system. This member can be one of the following values.
            /// <list type="table">
            /// <listheader>
            /// <term>Value</term>
            /// <description>Meaning</description>
            /// </listheader>
            /// <item>
            /// <term>PROCESSOR_ARCHITECTURE_INTEL<br/>(0x000)</term>
            /// <description>Indicates an Intel based x86 platform.</description>
            /// </item>
            /// <item>
            /// <term>PROCESSOR_ARCHITECTURE_AMD64<br/>(0x0009)</term>
            /// <description>Indicates an Intel or AMD based x64 platform.</description>
            /// </item>
            /// <item>
            /// <term>PROCESSOR_ARCHITECTURE_IA64<br/>(0x0006)</term>
            /// <description>Indicates an Intel Itanium based Processor (IPF).</description>
            /// </item>
            /// <item>
            /// <term>PROCESSOR_ARCHITECTURE_UNKNOWN<br/>(0xFFFF)</term>
            /// <description>Indicates an unknown processor architecture.</description>
            /// </item>
            /// </list>
            /// </summary>
            public Int16 architecture;

            /// <summary>
            /// This member is reserved for future use.
            /// </summary>
            public Int16 reserved;

            /// <summary>
            /// The page size and the granularity of page protection 
            /// and commitment. 
            /// </summary>
            public Int32 pageSize;

            /// <summary>
            /// A pointer to the lowest memory address accessible to 
            /// applications and dynamic-link libraries (DLLs).
            /// </summary>
            public IntPtr pMinimumApplicationAddress;

            /// <summary>
            /// A pointer to the highest memory address accessible to 
            /// applications and DLLs.
            /// </summary>
            public IntPtr pMaximumApplicationAddress;

            /// <summary>
            /// A mask representing the set of processors configured into 
            /// the system. Bit 0 is processor 0; bit 31 is processor 31.
            /// </summary>
            public Int32 activeProcessorMask;

            /// <summary>
            /// The number of logical processors in the current group.
            /// </summary>
            public Int32 numberOfProcessors;

            /// <summary>
            /// An obsolete member that is retained for compatibility. 
            /// </summary>
            public Int32 processorType;

            /// <summary>
            /// The granularity for the starting address at which virtual 
            /// memory can be allocated.
            /// </summary>
            public Int32 allocationGranularity;

            /// <summary>
            /// The architecture-dependent processor level.
            /// </summary>
            public Int16 processorLevel;

            /// <summary>
            /// The architecture-dependent processor revision. 
            /// </summary>
            public Int16 processorRevision;
        }

        // Windows 2000 Professional / Windows 2000 Server
        /// <summary>
        /// Retrieves information about the current operating system.
        /// </summary>
        /// <param name="version">
        /// An <see cref="OSVERSIONINFO">OSVERSIONINFO</see> structure 
        /// that receives the operating system information. 
        /// </param>
        /// <returns>
        /// True if function call was successful and false otherwise.
        /// </returns>
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern bool GetVersionEx(
            [In, Out] ref OSVERSIONINFO version
        );

        // Windows XP / Windows Server 2003
        /// <summary>
        /// Retrieves information about the current system to 
        /// an application running under WOW64.
        /// </summary>
        /// <remarks>
        /// This method requires Windows XP as minimum operating 
        /// system.
        /// </remarks>
        /// <param name="info">
        /// A pointer to a <see cref="SYSTEM_INFO">SYSTEM_INFO</see> 
        /// structure that receives the information. 
        /// </param>
        /// <seealso cref="SYSTEM_INFO">SYSTEM_INFO</seealso>
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern void GetNativeSystemInfo(
            [Out] out SYSTEM_INFO info
        );

        #endregion // Win32 related declaration and implementation section.
    }
}
