
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

/**
* Clipper (c) atom0s 2004 - 2013 [wiccaan@comcast.net]
* ---------------------------------------------------------------------------------
* This file is part of Clipper.
*
* Clipper is free software: you can redistribute it and/or modify
* it under the terms of the GNU Lesser General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* Clipper is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Lesser General Public License for more details.
*
* You should have received a copy of the GNU Lesser General Public License
* along with Clipper. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZScanLib
{
    internal class Helpers
    {
        /// <summary>
        /// Merges lists of byte arrays into one. 
        /// </summary>
        /// <param name="byteList"></param>
        /// <returns></returns>
        public static byte[] mergeBytes(List<byte[]> byteList)
        {
            // Merge all the byte arrays for searching patterns
            IEnumerable<byte> result = Enumerable.Empty<byte>();

            foreach (byte[] bytes in byteList)
            {
                result = result.Concat(bytes);
            }

            // Store the merged byte arrays.
            return result.ToArray();
        }

        /// 
        /// Credit to Author: atom0s
        /// https://github.com/atom0s/Clipper/blob/master/Clipper/Classes/Helpers.cs 
        /// 
        /// <summary>
        /// Converts a hex string to a byte array.
        /// </summary>
        /// <param name="strPattern"></param>
        /// <returns></returns>
        public static byte[] HexStringToArray(String strPattern)
        {
            return Enumerable.Range(0, strPattern.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(strPattern.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// Converts a hex string to a byte pattern. 
        /// </summary>
        /// <param name="strPattern">The string pattern unconverted</param>
        /// <param name="bytePattern">The string pattern converted to a byte pattern</param>
        /// <returns>True for success</returns>
        public static bool TryHexStringToArray(String strPattern, out byte[] bytePattern)
        {
            // Return null on failure in the bytePatten
            bytePattern = null;

            // Convert the string pattern to a byte pattern. 
            try { bytePattern = HexStringToArray(strPattern); }

            // We've failed to convert the string, return false. 
            catch (Exception) { return false; }

            // We've succeeded.
            return true;
        }

        /// <summary>
        /// HexStringToInt
        ///     
        ///     Converts a given hex string to an integer value. 
        ///     
        /// </summary>
        /// <param name="hexString">The hex string to be converted. </param>
        /// <returns>Integer - Hex string converted into integer.</returns>
        public static int HexStringToInt(String hexString)
        {
            return Int32.Parse(hexString, NumberStyles.HexNumber);
        }

        /// <summary>
        /// TryHexStringToInt
        /// 
        ///     Attempts to convert a hex string into a int value. 
        ///     
        /// </summary>
        /// <param name="hexString">
        ///     The hex string to convert to int. 
        /// </param>
        /// <returns>
        ///     Nullable Int - The string converted to nullable int.
        /// </returns>
        public static int? TryHexStringToInt(String hexString)
        {
            // Initialize our int value.
            int? value = new int?();

            // Try to perform the cast to int from hex string. 
            try { value = HexStringToInt(hexString); }
            
            // Return false on failure to convert. 
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            
            // Return true on success. 
            return value;
        }
    }
}
