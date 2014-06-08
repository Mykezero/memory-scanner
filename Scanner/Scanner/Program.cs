using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Diagnostics;
using Clipper.Classes;
using System.IO;
using System.Threading;

namespace Scanner
{
    class Program
    {
        static string FFXIV_GAME_NAME = "ffxiv";
        static string FFXIV_PROCESS_NAME = "ffxiv.exe";
        static string FFXI_GAME_NAME = "pol";
        static string FFXI_PROCESS_NAME = "ffximain.dll";

        static void Main(string[] args)
        {
            Console.WriteLine("Scanner now running....");

            // Get the process we are reading memory from.
            Process process = Process.GetProcessesByName(FFXIV_GAME_NAME).FirstOrDefault();

            if (process == null)
            {
                Console.WriteLine("Error: process {0} not found.", FFXIV_GAME_NAME);
                Console.ReadLine();
                return;
            }

            // Get the main module that we are reading memory from.
            var main = (from ProcessModule m in process.Modules
                        where m.ModuleName.ToLower() == FFXIV_PROCESS_NAME
                        select m).SingleOrDefault();


            if (main == null)
            {
                Console.WriteLine("Error: main module {0} not found.", FFXIV_PROCESS_NAME);
                Console.ReadLine();
                return;
            }

            Console.WriteLine(main.ModuleName);

            // Create our signature scanner for our process and module.
            SigScan sigScan = new SigScan(
                process, main.BaseAddress, main.ModuleMemorySize
                );

            // Signature for CP / MP
            // Set the pattern mask and offset.           
            var pattern = "8908C6431D018B462C8B4E288945E4894DE085C074333BC8";
            var mask = "xxxxxxxxxxxxxxxxxxxxxxxx";
            int offset = 0x135D4AA6;

            //Signature and Mask for Aggroed creatures
            //Used to access top three aggroing creatures
            //8901C6431D018B4774 
            //xxxxxxxxx

            // Get the pointer from the signature and print it out
            var pointer = sigScan.FindPattern(Helpers.HexStringToArray(pattern), mask, offset);
            Console.WriteLine("Pointer is {0}.", (pointer).ToString("X"));
            Console.ReadLine();

            while (true)
            {
                Console.Clear();
                // Get the memory at the address of the pointer, store it into bytesRead, convert it to an int and
                // then print it out.
                var tempBuffer = new byte[4];
                Memory.Peek(process, pointer, tempBuffer);
                var value = BitConverter.ToInt32(tempBuffer, 0);
                Console.WriteLine("The value is {0}.", value);
                Thread.Sleep(1000);
            }

            Console.WriteLine("Press enter to quit...");
            Console.ReadLine();
        }
    }
}