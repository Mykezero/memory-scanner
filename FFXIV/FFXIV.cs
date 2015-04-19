using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZScanLib;

namespace FFXIV
{
    public class FFXIV
    {
        /// <summary>
        /// The id of the process
        /// </summary>
        private int pid;

        /// <summary>
        /// The process to read memory from. 
        /// </summary>
        private Process process;

        private IMemoryScanner scanner;

        /// <summary>
        /// A collection of signatures by name. 
        /// </summary>
        public Dictionary<String, Signature> SignatureMap = 
            new Dictionary<string, Signature>();

        public FFXIV(int pid)
        {
            process = Process.GetProcessById(pid);
            scanner = new MemoryScanner(process);
        }

        public class Player
        {
            public int HPP 
            { 
                get; 
                set; 
            }
        }
    }
}
