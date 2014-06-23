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
    public enum GameType
    {
        FFXI,
        FFXIV
    }

    public class Program
    {      
        static void Main(string[] args)
        {
            Console.WriteLine("Scanner now running....");

            // Signature for CP / MP
            // Set the pattern mask and offset.
            // var pattern = "8908C6431D018B462C8B4E288945E4894DE085C074333BC8";
            // var mask = "xxxxxxxxxxxxxxxxxxxxxxxx";
            // int offset = 0x135D4AA6;

            var mask = "xxxxxxxxx";
            var pattern = "8908C6431D018B462C";
            var offset = 0x11B14C86;

             

            //Signature and Mask for Aggroed creatures
            //Used to access top three aggroing creatures
            //8901C6431D018B4774
            //xxxxxxxxx


            // Create our signature to search for.
            var signature = Signature.Create(pattern, mask, offset);

            // Setup the scanner
            Scanner.OutputEnabled = true;
            var scanner = new Scanner(GameType.FFXIV);

            // Did the scanner set up properly?
            if (!scanner.Success)
            {
                Console.WriteLine("Scanner failed to initialize. Exiting");
                Console.ReadLine();
                Environment.Exit(0);
            }

            // Find the pointer
            var pointer = scanner.FindPointer(signature);
            Console.Write("Pointer found: {0}", (pointer).ToString("X2"));
            Console.ReadLine();

            while (true)
            {
                Console.Clear();
                // Get the memory at the address of the pointer, store it into bytesRead, convert it to an int and
                // then print it out.
                var tempBuffer = new byte[4];
                Memory.Peek(scanner.Process, pointer, tempBuffer);
                var value = BitConverter.ToInt32(tempBuffer, 0);
                Console.WriteLine("The value is {0}.", value);
                Thread.Sleep(1000);
            }

            Console.WriteLine("Press enter to quit...");
            Console.ReadLine();
        }
    }
}