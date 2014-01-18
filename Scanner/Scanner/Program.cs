using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Diagnostics;
using Vivisection;

namespace Scanner
{
    class Program
    {
        public static List<Process> Processes 
        { 
            get 
            { 
                return Process.GetProcessesByName("pol").ToList(); 
            } 
        }

        public static List<FFInstance> Instances 
        {
            get { return Processes.Select(x => new FFInstance(x)).Where(x => x.Valid).ToList(); }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Scanner now running....");
            SigScanner Scanner = new SigScanner();
            Scanner.Target = Instances[0];

            if (Instances[0] == null) 
            { 
                Console.WriteLine("There are no instances currently loaded, shutting down.");
                System.Environment.Exit(0);
            }

            var pdata = Instances[0].Instance.Target;

            TARGETINFO findme = new TARGETINFO 
             {
                 CurrentID = pdata.ID,
                 CurrentMask = pdata.Mask,
                 CurrentSvrID = pdata.SubServerID,
                 HPP = (byte)pdata.HPPCurrent,
                 // IsLocked = pdata.IsLocked,
                 // IsSub = pdata.IsSub,
                 Name = pdata.Name,
                 SubID = pdata.SubID,
                 SubMask = pdata.SubMask,
                 SubSrvID = pdata.SubServerID,
             };

            byte[] findsig = Scanner.StructToBytes(findme);

            string findmeMask =
                  "xxxx" // curid
                + "xx" //curmask
                + "xxxx" //cursvrid
                + "xxxx"//hpp
                + "????"//islocked
                + "????" //issub
                + "xxxx" //name
                + "xxxx" //subid
                + "xx" //submask
                + "xxxx"; //subsrvid

            IntPtr InventoryStart = Scanner.ScanModule("FFXiMain", findsig, findmeMask, 0);

            Console.WriteLine("Signature : " + InventoryStart.ToString("X2"));

            Console.ReadLine();
        }

        public static String CreateSig(int Number)
        {
            String mask = "";
            for (int i = 0; i < Number; i++)
            {
                mask += "x";
            }
            return mask;
        }
    }
}
