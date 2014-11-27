
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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZeroLimits.ZScanLib.MemoryManagement
{
    /// <summary>
    /// Returns the system info object for the machine. We're mostly 
    /// using it to retrieve the Lowest Memory Address: 
    /// lpMinApplicationAddress. 
    /// </summary>
    public partial class Memory
    {
        /// <summary>
        /// Returns a SYSTEM_INFO object. 
        /// Used to get min / max address space points. 
        /// </summary>
        /// <returns></returns>
        public static SYSTEM_INFO SystemInfo
        {
            get
            {
                var SystemInfo = new SYSTEM_INFO();
                GetSystemInfo(ref SystemInfo);
                return SystemInfo;
            }
        }
    }
}
