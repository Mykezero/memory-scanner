
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
using System.Text;
using System.Threading.Tasks;

namespace ZScanLib
{
    /// <summary>
    /// A container class for searching the virtual memory of a process. 
    /// </summary>
    internal class VirtualMemory
    {
        /// <summary>
        /// The process for we are reading virtual memory from. 
        /// </summary>
        Process process;

        /// <summary>
        /// Pass in the process we want to retrieve virtual memory from. 
        /// </summary>
        /// <param name="process"></param>
        public VirtualMemory(Process process)
        {
            this.process = process;
        }

        /// <summary>
        /// Contains the list of all virtual memory pages for the 
        /// contained process. 
        /// </summary>
        public IEnumerable<VirtualMemoryPage> Pages 
        {
            get 
            {
                return RetrievePages();
            }
        }        

        /// <summary>
        /// Loops through all of a process's virtual memory addresses and 
        /// returns all pages. 
        /// </summary>
        /// <param name="process">The process to retrieve pages from.</param>
        /// <param name="pages">Stores the pages</param>
        /// <returns>True if any pages were returned</returns>
        private IEnumerable<VirtualMemoryPage> RetrievePages()
        {
            // Collects the list of readable pages.  
            var pages = new List<VirtualMemoryPage>();

            // Start off at the lowest address possible. 
            long address = AddressInfo.MinimumAddress;

            // The max address we'll be reading. 
            long maxAddress = AddressInfo.MaximumAddress;

            // While there are still addresses to be read. 
            while (address < maxAddress)
            {
                // We'll store the page read in here. 
                VirtualMemoryPage page = new VirtualMemoryPage(process, (IntPtr)address);

                // Increment our address by the amount read (RegionSize)                
                address = (long)page.Info.BaseAddress + (long)page.Info.RegionSize;

                // Add to list of readable pages.
                pages.Add(page);
            }

            // Return true if we've found pages. 
            return pages;
        }
    }
}
