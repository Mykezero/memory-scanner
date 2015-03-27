
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

namespace ZScanLib
{
    /// <summary>
    /// Holds the information for locating values in memory. 
    /// </summary>
    public class Signature
    {
        public Signature(string pattern, string mask, int offset)
        {
            Pattern = pattern;
            Mask = mask;
            Offset = offset;
        }

        /// <summary>
        /// A pattern that identifies a unique spot in a process's memory space. 
        /// </summary>
        public String Pattern { get; set; }

        /// <summary>
        /// A mask to control what parts of the pattern must be matched. 
        /// </summary>
        public String Mask { get; set; }

        /// <summary>
        /// A offset to be added after locating a pattern. 
        /// </summary>
        public int Offset { get; set; }
    }
}
