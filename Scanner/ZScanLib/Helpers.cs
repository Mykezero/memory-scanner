
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
        /// 
        /// Credit to Author: atom0s
        /// https://github.com/atom0s/Clipper/blob/master/Clipper/Classes/Helpers.cs 
        /// Note: The clipper project has moved to 
        /// <summary>
        /// Converts a hex string to a byte array.
        /// </summary>
        /// <param name="strPattern"></param>
        /// <returns></returns>
        public static byte[] HexStringToArray(String strPattern)
        {
            // Pattern cannot be of odd length. 
            if (strPattern.Length % 2 != 0)
            {
                throw new ArgumentException("Pattern length must be in multiples of twos. ");
            }

            return Enumerable.Range(0, strPattern.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(strPattern.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
