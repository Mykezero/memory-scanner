using Clipper.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner
{
    public class PointerResolver
    {
        /// <summary>
        /// Resolves the address for a simple pointer
        /// </summary>
        /// <param name="p"></param>
        /// <param name="lpAddress"></param>
        /// <param name="btBuffer"></param>
        /// <returns></returns>
        public static IntPtr ResolvePointer(Process p, IntPtr lpAddress, byte[] btBuffer)
        {
            Memory.Peek(p, lpAddress, btBuffer);
            var target = (IntPtr)BitConverter.ToInt32(btBuffer, 0);
            return target;
        }

        /// <summary>
        /// Resolves the pointer path and adds an offset to the end resulting pointer.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="path"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static IntPtr ResolvePointerPath(Process p, List<int> path, int offset)
        {
            IntPtr currentPtr = (IntPtr)(0x1E6BDB8  + 0xEEADB8);
            IntPtr result = IntPtr.Zero;

            // Process all of the pointer paths minus the last one
            for (int i = 0; i < path.Count; i++)
            {
                // Pointer plus offset
                currentPtr += path[i];
                
                // Buffer to store read data
                byte[] btBuffer = new byte[4];
                
                // Get the new pointer by reading it from memory
                currentPtr = ResolvePointer(p, currentPtr, btBuffer);
            }

            // Process the last pointer path
            result = currentPtr + offset;

            return result;
        }
    }
}
