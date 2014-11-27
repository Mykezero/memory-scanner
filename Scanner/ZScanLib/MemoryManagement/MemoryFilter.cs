
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroLimits.ZScanLib.MemoryManagement
{
    public partial class Memory
    {
        /// <summary>
        /// Returns a filter to be used on pages of memory that tell use 
        /// we can read or write from them. 
        /// </summary>
        /// <example>
        /// if(PageFilter()(MemoryPage)) do_something()
        /// </example>
        /// <returns></returns>
        public static Func<MEMORY_BASIC_INFORMATION, bool> PageFilter()
        {
            return new Func<MEMORY_BASIC_INFORMATION, bool>((x) =>
            {
                // Determines whether we can read or write on the memory page. 
                var Writable = 
                    AllocationProtect.PAGE_READWRITE | 
                    AllocationProtect.PAGE_WRITECOPY | 
                    AllocationProtect.PAGE_EXECUTE_READWRITE | 
                    AllocationProtect.PAGE_EXECUTE_WRITECOPY | 
                    AllocationProtect.PAGE_GUARD;

                // Page cannot be read, return. 
                if ((x.Protect & Writable) == 0) return false;

                // Page has not been commited to memory, return. 
                if ((x.State & MEMORY_STATE.MEM_COMMIT) == 0) return false;

                // Page is guarded, return. 
                if ((x.Protect & AllocationProtect.PAGE_GUARD) != 0) return false;

                // We've passed all tests. 
                return true;
            });
        }
    }
}
