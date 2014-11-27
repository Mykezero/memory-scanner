
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

namespace ZeroLimits.ZScanLib.MemoryManagement
{
    public partial class Memory
    {
        /// <summary>
        /// Returns a single virtual memory page. 
        /// </summary>
        /// <param name="process">The process from which to read.</param>
        /// <param name="address">The address from which to read.</param>
        /// <param name="mbi">The virtual address page. </param>
        /// <returns>True on successful reading of a page.</returns>
        public static bool PeekPage(Process process, IntPtr address, out MEMORY_BASIC_INFORMATION mbi)
        {
            // Call VirtualQueryEx with the process handle and address we 
            // want to read from and store it in MEMORY BASIC INFORMATION m. 
            int result = VirtualQueryEx(process.Handle, address, out mbi, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));

            // Will return 0 on failure to read page. 
            return result != 0;
        }

        /// <summary>
        /// Loops through all of a process's virtual memory addresses and 
        /// returns all pages. 
        /// </summary>
        /// <param name="process">The process to retrieve pages from.</param>
        /// <param name="pages">Stores the pages</param>
        /// <returns>True if any pages were returned</returns>
        public static bool PeekPages(Process process, out ICollection<MEMORY_BASIC_INFORMATION> pages)
        {
            // Collects the list of readable pages.  
            List<MEMORY_BASIC_INFORMATION> readable = new List<MEMORY_BASIC_INFORMATION>();

            // Start off at the lowest address possible. 
            long address = Memory.SystemInfo.lpMinimumApplicationAddress;

            // The max address we'll be reading. 
            long maxAddress = Memory.SystemInfo.lpMaximumApplicationAddress;

            // While there are still addresses to be read. 
            while (address < maxAddress)
            {
                // We'll store the page read in here. 
                MEMORY_BASIC_INFORMATION mbi;

                // Read the page but add it to read only if the page read is successful. 
                if (PeekPage(process, (IntPtr)address, out mbi))
                {
                    // Increment our address by the amount read (RegionSize)                
                    address = (long)mbi.BaseAddress + (long)mbi.RegionSize;

                    // Add to list of readable pages.
                    readable.Add(mbi);
                }
            }

            // outs all the pages that were read. 
            // outs the empty list on failure.
            pages = readable;

            // Return true if we've found pages. 
            return pages.Count > 0;
        }
    }
}