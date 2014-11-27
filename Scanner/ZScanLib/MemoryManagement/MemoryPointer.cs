
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

namespace ZeroLimits.ZScanLib.MemoryManagement
{
    public partial class Memory
    {
        // Solution inspired by stefsot's write_pointer_2 function. 
        // http://www.hackforums.net/showthread.php?tid=957392
        public static IntPtr PeekPointer(Process process, IntPtr address, params int[] offsets)
        {
            // Stores the current memory address being processed. 
            byte[] bytes = null;

            // Read the base address. 
            Memory.Peek(process, address, bytes);
            address = (IntPtr)BitConverter.ToInt32(bytes, 0);

            // Read the offsets until the final location is found. 
            foreach (var offset in offsets)
            {
                Memory.Peek(process, address, bytes);
                address = (IntPtr)BitConverter.ToInt32(bytes, 0);
            }

            // Return the final address. 
            return address;
        }

        /// <summary>
        /// Resolves the address for a simple pointer. 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="lpAddress"></param>
        /// <param name="btBuffer"></param>
        /// <returns></returns>
        public static IntPtr PeekPointer(Process process)
        {
            return PeekPointer(process);
        }
    }
}
