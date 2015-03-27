using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZScanLib
{
    /// <summary>
    /// A virtual memory scanner to find memory addresses by signature. 
    /// </summary>
    public class ModuleMemoryScanner : IMemoryScanner
    {
        /// <summary>
        /// The process to scan for a pattern. 
        /// </summary>
        private Process process;

        /// <summary>
        /// Set up the module scanning scanner for the process. 
        /// </summary>
        /// <param name="process"></param>
        public ModuleMemoryScanner(Process process)
        {
            this.process = process;
        }

        /// <summary>
        /// Scans the process's virtual memory for a specific signature. 
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        public IntPtr Scan(Signature signature)
        {
            // Read the main module's memory. 
            var buffer = new byte[process.MainModule.ModuleMemorySize];
            Memory.Peek(process, process.MainModule.BaseAddress, buffer);
            
            // Find the address for the signature. 
            var matcher = new PatternMatcher();
            var address = matcher.FindMatch(signature, buffer);

            // Return the address if one was found. 
            if (address != -1) return (IntPtr)address;
            return IntPtr.Zero;
        }
    }
}
