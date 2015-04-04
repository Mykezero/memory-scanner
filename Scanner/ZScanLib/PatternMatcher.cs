
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZScanLib
{
    /// <summary>
    /// A pattern matcher that can located a byte sequence in a chunk of bytes. 
    /// </summary>
    public class PatternMatcher
    {
        /// <summary>
        /// Finds the starting index + offset of where the pattern was found. 
        /// 
        /// Returns -1 on failure to match the pattern. 
        /// </summary>
        /// <returns></returns>
        public int FindMatch(Signature signature, byte[] buffer)
        {
            return Match(signature, buffer);
        }

        private int Match(Signature signature, byte[] buffer)
        {
            var pattern = Helpers.HexStringToArray(signature.Pattern);

            int count = 0, index = 0;

            while (index < buffer.Length)
            {
                // We did not match the pattern
                if (pattern[count] != buffer[index])
                {
                    // Its is not a wildcard character. 
                    if (signature.Mask[count] != '?') count = 0;
                }
                else
                {
                    // Yes, we've matched another part of the pattern so increment
                    // count by one and check to see if we've matched the whole pattern. 
                    if (++count == pattern.Length)
                    {
                        // We're subtracting the length from the index to get the starting position
                        // where we found the pattern; index at the end points to the last element. 
                        return (index - pattern.Length) + signature.Offset;
                    }
                }

                // Increment index so we can look at the next byte in the buffer. 
                index++;
            }

            return -1;
        }
    }
}
