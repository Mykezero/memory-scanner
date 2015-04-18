
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

        /// <summary>
        /// Finds the address for the given signature in the byte buffer. 
        /// Returns the address or -1 on failure. 
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private int Match(Signature signature, byte[] buffer)
        {
            // Convert our pattern to a byte array.
            var pattern = Helpers.HexStringToArray(signature.Pattern);

            // The current amount of matched bytes. 
            int count = 0; 

            // The current index in our buffer we are scanning. 
            int index = 0;

            // Loop to scan all bytes in the buffer for a pattern. 
            while (index < buffer.Length)
            {
                // If the pattern is a wildcard, skip it. 
                if (signature.Pattern[count] == '?')
                {
                    // Increase the match count. 
                    count++;

                    // Increase our current position. 
                    index++;

                    // Skip to matching the next byte. 
                    continue;
                }

                // We've failed to match this section of the pattern. 
                if (pattern[count] != buffer[index])
                {
                    // Reset the match count. 
                    count = 0;

                    // Increase our buffer position to the next byte. 
                    index++;

                    // Skip this byte. 
                    continue;
                }

                // We've matched a byte successfully so increment count by 1. 
                count++;

                // Count this byte as looked at so that we return the right address
                index++;

                // Check to see if we've matched the whole pattern. 
                if (count == pattern.Length)
                {
                    // We're subtracting the length from the index to get the starting position
                    // where we found the pattern; index at the end points to the last element. 
                    return (index - pattern.Length) + signature.Offset;
                }                
            }

            // Return a sigil value on failure. 
            return -1;
        }
    }
}
