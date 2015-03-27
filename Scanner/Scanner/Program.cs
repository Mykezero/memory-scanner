
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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Threading;
using PowerArgs;

namespace ZScanLib
{
    public class Program
    {
        public class ScannerArgs
        {
            [ArgRequired(PromptIfMissing = true)]
            [DefaultValue("pol")]
            public String ProcessName { get; set; }

            [ArgRequired(PromptIfMissing = true)]
            [DefaultValue("8D048D00000000B9000000008D780400000000C70000000000F3A5")]
            public String Signature { get; set; }

            [ArgRequired(PromptIfMissing = true)]
            [DefaultValue("XXX????X????XX????XX?????XX")]
            public String Mask { get; set; }

            [ArgRequired(PromptIfMissing = true)]
            [DefaultValue(0)]
            public int Offset { get; set; }
        }

        public static void Main(string[] args)
        {
            // Obtain the Scanner Args data from the user. 
            var parsed = Args.Parse<ScannerArgs>(args);
            var signature = new Signature(parsed.Signature, parsed.Mask, parsed.Offset);

            // Set the target process. 
            var process = Process.GetProcessesByName(parsed.ProcessName).FirstOrDefault();
            if (process == null)
            {
                Console.WriteLine("No process found. ");
                Console.WriteLine("Press enter to quit...");
                Console.ReadLine();
                return;
            }
            
            // Scan and retrieve the address. 
            MemoryScanner scanner = new MemoryScanner(process);
            var address = scanner.Scan(signature);

            // Read the value from memory. 
            byte[] btBuffer = new byte[sizeof(int)];
            Memory.Peek(process, address, btBuffer);
            
            // Convert and display it to the user. 
            int value = BitConverter.ToInt32(btBuffer, 0);
            Console.WriteLine(value);

            Console.WriteLine("Press enter to quit...");
            Console.ReadLine();
        }
    }
}