
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
    /// A multi-purpose memory scanner that scans a process's memory. It can 
    /// scan a process's virtual and module memory. 
    /// </summary>
    public class MemoryScanner : IMemoryScanner
    {
        /// <summary>
        /// A list of scannners to scan the process's memory for 
        /// a pattern. 
        /// </summary>
        private List<IMemoryScanner> Scanners;

        /// <summary>
        /// The process to scan for a pattern. 
        /// </summary>
        private Process process; 

        /// <summary>
        /// Sets up the scanner to scan the process's memory. 
        /// </summary>
        /// <param name="process"></param>
        public MemoryScanner(Process process)
        {
            this.process = process;
            Scanners = new List<IMemoryScanner>();
            Scanners.Add(new ModuleMemoryScanner(process));
            Scanners.Add(new VirtualMemoryScanner(process));            
        }

        /// <summary>
        /// Allows each scanner to do a search for the signature to 
        /// retrieve a pattern's address.
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        public IntPtr Scan(Signature signature)
        {
            foreach (var scanner in Scanners)
            {
                var address = scanner.Scan(signature);
                if (address != IntPtr.Zero) return address;
            }

            return IntPtr.Zero;
        }
    }
}