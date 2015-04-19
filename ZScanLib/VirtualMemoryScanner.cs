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
    public class VirtualMemoryScanner : IMemoryScanner
    {
        /// <summary>
        /// The process to scan for a pattern.
        /// </summary>
        private Process process;

        /// <summary>
        /// Set up the virtual memory scanning scanner for the process. 
        /// </summary>
        /// <param name="process"></param>
        public VirtualMemoryScanner(Process process)
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
            var virtualMemory = new VirtualMemory(process);

            var matcher = new PatternMatcher();

            foreach (var page in virtualMemory.Pages.AsParallel().Where(x => x.IsReadable))
            {
                // Read the page data and parse it to find the address. 
                var address = matcher.FindMatch(signature, page.Read());

                // If the address was found return that address. 
                if (address != -1) return (IntPtr)address;
            }

            return IntPtr.Zero;
        }
    }
}
