using Clipper.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner
{
    public class Scanner
    {
        static string FFXIV_PROCESS_NAME = "ffxiv";
        static string FFXIV_MODULE_NAME = "ffxiv.exe";
        static string FFXI_PROCESS_NAME = "pol";
        static string FFXI_MODULE_NAME = "ffximain.dll";

        /// <summary>
        /// The name of the game's process we'll be searching for a particular value.
        /// </summary>
        String ProcessName = String.Empty;

        /// <summary>
        /// The name of the game's main module we'll be searching for a particular value.
        /// </summary>
        String ModuleName = String.Empty;

        /// <summary>
        /// The process the data is stored in.
        /// </summary>
        public Process Process;

        /// <summary>
        /// The module the data is stored in.
        /// </summary>
        ProcessModule Module;

        /// <summary>
        /// The scanner we will use to find our signaturw
        /// </summary>
        SigScan SigScan;

        /// <summary>
        /// Should we print to the console?
        /// </summary>
        public static bool OutputEnabled = false;

        /// <summary>
        /// Returns whether the scanner was initialized properly and is 
        /// ready to read memory
        /// </summary>
        private bool _success = false;

        /// <summary>
        /// Allows outside to see the status of success
        /// </summary>
        public bool Success
        {
            get { return _success; }
            private set { _success = value; }
        }
        
        public Scanner(GameType gameType)
        {
            switch (gameType)
            {
                case GameType.FFXI:
                    this.ProcessName = FFXI_PROCESS_NAME;
                    this.ModuleName = FFXI_MODULE_NAME;                        
                    break;
                case GameType.FFXIV:
                    this.ProcessName = FFXIV_PROCESS_NAME;
                    this.ModuleName = FFXIV_MODULE_NAME;
                    break;
                default:
                    OutputMessage("Invalid Game Selection. Shutting down.");
                    Environment.Exit(0);
                    break;
            }

            // Get the process we are reading memory from.
            this.Process = Process.GetProcessesByName(this.ProcessName).FirstOrDefault();

            if (this.Process == null)
            {
                OutputMessage("Error: process {0} not found.", ProcessName);
                return;
            }

            OutputMessage("Process Located!");

            // Get the main module that we are reading memory from.
            this.Module = (from ProcessModule m in this.Process.Modules
                           where m.ModuleName.ToLower() == this.ModuleName
                           select m).SingleOrDefault();

            if (this.Module == null)
            {
                OutputMessage("Error: main module {0} not found.", ProcessName);
                return;
            }

            OutputMessage("Module Located!");

            // Create our signature scanner for our process and module.
            this.SigScan = new SigScan(
                this.Process, this.Module.BaseAddress, this.Module.ModuleMemorySize
                );

            OutputMessage("Scanner Created!");
            Success = true;
        }

        private void OutputMessage(String message, params Object[] values)
        {
            if (OutputEnabled) Console.WriteLine(message, values);
        }

        public IntPtr FindPointer(Signature signature)
        {
            return this.SigScan.FindPattern(Helpers.HexStringToArray(signature.Pattern), signature.Mask, signature.Offset);
        }
    }
}
