
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
using ZeroLimits.ZScanLib.MemoryManagement;

namespace ZeroLimits.ZScanLib
{
    public enum ScanType
    {
        Signature,
        Pointer
    }

    public enum AllocationStrategy
    {
        Dynamic,
        Static
    }

    public class Program
    {
        public class ScannerArgs
        {
            [ArgRequired(PromptIfMissing = true)]
            [DefaultValue("ffxiv")]
            public String ProcessName { get; set; }

            [ArgRequired(PromptIfMissing = true)]
            [DefaultValue("8908C64719018B462C8B4E288945E8894D0885C0")]
            public String Signature { get; set; }

            [ArgRequired(PromptIfMissing = true)]
            [DefaultValue("xx????xxxxxxxxxxxxxx")]
            public String Mask { get; set; }
            
            [ArgRequired(PromptIfMissing = true)]
            [DefaultValue(0)]
            public int Offset { get; set; }
        }

        public static void Main(string[] args)
        {
            // Obtain the Scanner Args data from the user. 
            var parsed = Args.Parse<ScannerArgs>(args);

            // Set up the scanner so that it does scan pages of memory. 
            Memory.Scanner.PageScanEnabled = true;

            // Set module scan to false, we are looking for dynamic memory
            // that resided in application pages of memory. 
            Memory.Scanner.ModuleScanEnabled = false;

            // Set the target process. 
            var process = Process
                .GetProcessesByName(parsed.ProcessName)
                .FirstOrDefault();

            // Set the scanner's process to search in. 
            Memory.Scanner.Process = process;

            // Scan for the byte pattern. 
            IntPtr address = Memory.Scanner.Scan(parsed.Mask,
                Helpers.HexStringToArray(parsed.Signature),
                parsed.Offset);

            // Save to save the read data. 
            // Will be reading an int so four bytes. 
            byte[] btBuffer = new byte[4];

            // Read the memory. 
            Memory.Peek(process, address, btBuffer);

            // Convert the value read into an int. 
            int value = BitConverter.ToInt32(btBuffer, 0);

            Console.WriteLine("The value is : " + value);

            Console.WriteLine("Press enter to quit...");
            Console.ReadLine();
        }
    }
}