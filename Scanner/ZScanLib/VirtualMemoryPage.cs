
/*///////////////////////////////////////////////////////////////////
<ZScanLib, a small library for reading and working with memory.>
Copyright (C) <2014>  <Zerolimits>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
*/
///////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZScanLib
{
    /// <summary>
    /// A page contained the in contained process's virtual memory layout.
    /// </summary>
    internal class VirtualMemoryPage
    {
        /// <summary>
        /// Base addresss of the page. 
        /// </summary>
        private IntPtr address;

        /// <summary>
        /// The process this page belongs to. 
        /// </summary>
        private Process process;

        /// <summary>
        /// Information about the page. 
        /// </summary>
        private WinAPI.MEMORY_BASIC_INFORMATION info;

        /// <summary>
        /// Create the Virtual Memory Page object by passing its parent process
        /// and base address into this object. 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="address"></param>
        public VirtualMemoryPage(Process process, IntPtr address)
        {
            this.address = address;
            this.process = process;
        }

        /// <summary>
        /// The information about this page. 
        /// </summary>
        public WinAPI.MEMORY_BASIC_INFORMATION Info
        {
            get 
            {
                // Update the page info
                WinAPI.VirtualQueryEx(
                    process.Handle,
                    address,
                    out info,
                    (uint)Marshal.SizeOf(typeof(WinAPI.MEMORY_BASIC_INFORMATION)));

                return info;
            }
        }        

        /// <summary>
        /// Update this page's data. 
        /// </summary>
        public byte[] Read()
        {
            if (!IsReadable) return null;
            var data = new byte[(long)Info.RegionSize];            
            Memory.Peek(process, Info.BaseAddress, data);
            return data;
        }

        /// <summary>
        /// Returns a filter to be used on pages of memory that tell use 
        /// we can read or write from them. 
        /// </summary>
        /// <returns></returns>
        public bool IsReadable
        {
            get
            {
                // Determines whether we can read or write on the memory page. 
                var ReadWriteAccess =
                    WinAPI.AllocationProtect.PAGE_READWRITE |
                    WinAPI.AllocationProtect.PAGE_WRITECOPY |
                    WinAPI.AllocationProtect.PAGE_EXECUTE_READWRITE |
                    WinAPI.AllocationProtect.PAGE_EXECUTE_WRITECOPY |
                    WinAPI.AllocationProtect.PAGE_GUARD;

                // Page cannot be read, return. 
                if ((info.Protect & ReadWriteAccess) == 0) return false;

                // Page has not been commited to memory, return. 
                if ((info.State & WinAPI.MEMORY_STATE.MEM_COMMIT) == 0) return false;

                // Page is guarded, return. 
                if ((info.Protect & WinAPI.AllocationProtect.PAGE_GUARD) != 0) return false;

                // We've passed all tests. 
                return true;
            }
        }
    }
}