using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization; // Xml serialization
using System.IO; // For stream readers/writers
using System.Diagnostics; // For Process calls.
using System.Runtime.InteropServices; // For Marshal calls
using System.ComponentModel; // For Win32 exceptions.

namespace Vivisection
{
    /*
     * Signature scanner for C#. Adapted from atom0s's version in order
     * to include some ease of use ability such as saving files and using
     * events. All other credit to him, dom1n1k and Patrick for the original
     * FindPattern implementation.
     * 
     * Usage example:
     *  SigScan _sigScan = new SigScan(someFFInstance, new IntPtr(0x123456), 1024); // 1KB at a time
     *  IntPtr pAddr = _sigScan.ScanModule("polcore", new byte[]{ 0xFF, 0x51, 0x55, 0xFC, 0x11 }, "x??xx", 12);
     *  
     * SigScan _sigScan = new SigScan();
     * _sigScan.Target = someFFInstance;
     * IntPtr pAddr = _sigScan.ScanModule("FFXiMain", new byte[] { 0xFF, 0xFF, 0x51, 0x55 }, "xx?x", 0);
     */
    #region SigScanner Delegates
    // For broadcasting events as we are scanning. Everyone likes an update.
    public delegate void ScanChanged(FFInstance Instance, DateTime When);
    public delegate void ScanException(FFInstance Instance, Exception ex);
    public delegate void ScanMemoryError(FFInstance Instance, string Message);
    public delegate void ScanResultFound(FFInstance Instance, IntPtr Location);
    #endregion

    #region SigScanner Class
    [Serializable]
    public class SigScanner
    {
        #region Imports
        /// <summary>
        /// ReadProcessMemory
        /// 
        ///     API import definition for ReadProcessMemory.
        /// </summary>
        /// <param name="hProcess">Handle to the process we want to read from.</param>
        /// <param name="lpBaseAddress">The base address to start reading from.</param>
        /// <param name="lpBuffer">The return buffer to write the read data to.</param>
        /// <param name="dwSize">The size of data we wish to read.</param>
        /// <param name="lpNumberOfBytesRead">The number of bytes successfully read.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess,
                                                     IntPtr lpBaseAddress,
                                                     [Out()] byte[] lpBuffer,
                                                     int dwSize,
                                                     out int lpNumberOfBytesRead);
        #endregion

        #region Variables
        // Target FFXI process (also helpfully the FFACE instance)
        [XmlIgnore] public FFInstance Target = null;

        // Address inside the process to begin scanning at. Use one of the
        // helper functions below to set this, too often a common error is
        // to provide a bad starting location instead of a Module base.
        public IntPtr StartPos = IntPtr.Zero;

        // How many bytes to read at once.
        public Int32 ReadSize;
        private Int32 _leftoverSize; // If the memory size is larger than we are.
        private Int32 _MAX_READSIZE = 10240; // 10KB

        // An array of bytes to store memory read in from the other process.
        [XmlIgnore] public byte[] MemoryDump { get { return _memdump; } }
        [XmlIgnore] private byte[] _memdump;
        #endregion

        #region Events
        public event ScanChanged OnScanBegin = delegate { };
        public event ScanChanged OnScanEnd = delegate { };
        public event ScanException OnScanException = delegate { };
        public event ScanMemoryError OnScanError = delegate { };
        public event ScanResultFound OnScanFound = delegate { };
        #endregion

        #region Constructors

        #region Default
        // Used for XML serialization.
        public SigScanner()
        {
            Target = null;
            StartPos = IntPtr.Zero;
            ReadSize = 0;
            _memdump = null;
        }
        #endregion

        #region Full Constructor
        public SigScanner(FFInstance TargetInstance, IntPtr Start, int maxReadSize)
        {
            Target = TargetInstance;
            StartPos = Start;
            ReadSize = maxReadSize;
        }
        #endregion

        #endregion

        #region Destructor
        ~SigScanner()
        {
            // Remove our listeners.
            OnScanBegin = null;
            OnScanEnd = null;
            OnScanException = null;
        }
        #endregion

        #region Reset
        public void Reset()
        {
            StartPos = IntPtr.Zero;
            ReadSize = _leftoverSize = 0;
            _memdump = null;
        }
        #endregion

        #region ScanModule
        /*
         * Given a module name and a process this function should find
         * the module's base address for StartPos, the size of the module
         * for ReadSize and begin dumping/checking memory chunks for the
         * pattern requested.
         * 
         * Returns zero on failure or a memory address somewhere in the
         * module on success.
         */
        public IntPtr ScanModule(string ModuleName, byte[] bPattern, string Mask, int nOffset)
        {
            // Standard checks.
            if (Target == null
                || Target.MyProcess.HasExited == true)
            {
                OnScanError(Target, "No Target or process exited already.");
                return IntPtr.Zero;
            }
            else if (String.IsNullOrEmpty(ModuleName)
                || String.IsNullOrEmpty(Mask)
                || bPattern.Length != Mask.Length)
            {
                OnScanError(Target, "Module name is wrong or mask is bad.");
                return IntPtr.Zero;
            }

            // Advertise scan began.
            OnScanBegin(Target, DateTime.Now);

            // Clean out any old scan junk.
            Reset();

            // Now look for the signature inside a given module.
            try
            {
                StartPos = GetModuleBaseAddress(ModuleName); // Moves as we scan.
                int size = GetModuleSize(ModuleName);
                IntPtr found = IntPtr.Zero;

                // Good start values?
                if (StartPos == IntPtr.Zero || size == 0)
                {
                    OnScanError(Target, "Couldn't find the module or module size was 0.");
                    return IntPtr.Zero;
                }

                // Careful with ReadSize: We want it to be a multiple of bPattern,
                // but don't miss any bytes. Keep the leftover size in case we can't
                // grab the whole memory region at once.
                if (size > _MAX_READSIZE)
                {
                    ReadSize = _MAX_READSIZE;
                    _leftoverSize = size - _MAX_READSIZE;
                }
                else { ReadSize = size; } // Scan the whole thing.

                // Scan chunks of this module's memory.
                while (ReadSize > 0)
                {
                    // Grab the current chunk of memory.
                    if (DumpMemory() == false) { return IntPtr.Zero; }

                    // Scan it for our value.
                    found = FindPattern(bPattern, Mask, nOffset);
                    if (found != IntPtr.Zero)
                    {
                        // Got it.
                        OnScanFound(Target, found);
                        return found;
                    }

                    // Update ReadSize and StartPos. FindPattern stops at
                    // _memdump's length minus the signature size to avoid
                    // an overflow on the buffer. So the new StartPos is:
                    // StartPos + (read buffer size - pattern length)
                    if (_leftoverSize == 0) { ReadSize = 0; } // Done.
                    else if (_leftoverSize > _MAX_READSIZE)
                    {
                        // Advance the starting position.
                        StartPos = new IntPtr((int)StartPos +
                                              (ReadSize - bPattern.Length));

                        // We end up grabbing the same regions of memory
                        // multiple times with this method. If we have a
                        // really large memory length to scan call this
                        // function from a separate thread.
                        ReadSize = _MAX_READSIZE;
                        _leftoverSize -= _MAX_READSIZE;
                    }
                    else
                    {
                        StartPos = new IntPtr((int)StartPos + 
                                              (ReadSize - bPattern.Length));
                        ReadSize = _leftoverSize;
                        _leftoverSize = 0;
                    }
                }

                // All done.
                OnScanEnd(Target, DateTime.Now);
            }
            catch (Exception)
            {
                // Grab the last interop error value.
                Win32Exception wExcept = new Win32Exception();
                OnScanError(Target, wExcept.Message);
                return IntPtr.Zero;
            }

            // Never found the value.
            OnScanError(Target, "No match for signature. Check signature, mask and correct module.");
            return IntPtr.Zero;
        }
        #endregion

        #region FindPattern
        /*
         * Given a signature pattern and a mask, attempt to find the
         * pattern somewhere in the previously dumped memory.
         * bPattern is the byte signature
         * Mask is the string of 'x' and '?' to compare bytes. x = check, ? = skip byte.
         * nOffset is the address added to the result if found.
         * Returns a non-zero IntPtr if the pattern was found.
         */
        private IntPtr FindPattern(byte[] bPattern, string Mask, int nOffset)
        {
            try
            {
                // Grab a chunk of memory if not previously done.
                if (_memdump == null || _memdump.Length <= 0)
                {
                    if (DumpMemory() == false) { return IntPtr.Zero; }
                }

                // Make sure mask and the pattern length matches.
                if (bPattern.Length != Mask.Length)
                {
                    OnScanError(Target, "Signature and signature mask must be same length.");
                    return IntPtr.Zero;
                }

                // Loop the byte region looking for the pattern.
                for (int x = 0; x < _memdump.Length; x++)
                {
                    if (MaskCheck(x, bPattern, Mask))
                    {
                        // FOUND. Return the location plus the offset. Why an offset?
                        // Because maybe the address we want is in the middle of the
                        // signature.
                        return new IntPtr((int)StartPos + (x + nOffset));
                    }
                }
            }
            catch (Exception)
            {
                Win32Exception wExcept = new Win32Exception();
                OnScanError(Target, wExcept.Message);
                return IntPtr.Zero;
            }

            // Nothing was found, return zero.
            return IntPtr.Zero;
        }
        #endregion

        #region GetModuleBaseAddress
        /*
         * Rarely do we want to scan the entire executable. Usually
         * offsets are in relation to a core module like FFXiMain.dll
         * or polcore.dll. 
         */
        public IntPtr GetModuleBaseAddress(string moduleTarget)
        {
            // Standard checks.
            if (Target == null
                || Target.MyProcess.HasExited == true
                || String.IsNullOrEmpty(moduleTarget))
            { return IntPtr.Zero; }

            // Looks good, loop the modules looking for requested name.
            foreach (ProcessModule PM in Target.MyProcess.Modules)
            {
                if (PM.ModuleName.Contains(moduleTarget))
                {
                    // FOUND.
                    return PM.BaseAddress;
                }
            }

            // Couldn't find the module in the process.
            OnScanError(Target, "Module was never found in the process.");
            return IntPtr.Zero;
        }
        #endregion

        #region GetModuleSize
        public int GetModuleSize(string moduleTarget)
        {
            // Standard checks.
            if (Target == null
                || Target.MyProcess.HasExited == true
                || String.IsNullOrEmpty(moduleTarget))
            { return 0; }

            // Looks good, loop the modules looking for requested name.
            foreach (ProcessModule PM in Target.MyProcess.Modules)
            {
                if (PM.ModuleName.Contains(moduleTarget))
                {
                    // FOUND.
                    return PM.ModuleMemorySize;
                }
            }

            // Couldn't find the module in the process.
            OnScanError(Target, "Module was never found in the process.");
            return 0;
        }
        #endregion

        #region DumpMemory
        /*
         * Used internally to grab chunks of memory for scanning. Starting
         * at StartPos this will grab a chunk of memory the size of ReadSize
         * and place it in _memdump for use.
         */
        private bool DumpMemory()
        {
            try
            {
                // Sanity checks: Working with correct values? Scans
                // sometimes take a while so make sure the process did
                // not die on us after we started.
                if (Target == null
                    || Target.MyProcess.HasExited == true
                    || StartPos == IntPtr.Zero
                    || ReadSize == 0)
                { return false; }

                // Create array of bytes to save into.
                _memdump = new byte[ReadSize];
                bool result = false;
                int bytesRead = 0;

                // Call ReadProcessMemory
                result = ReadProcessMemory(Target.MyProcess.Handle,
                                           StartPos,
                                           _memdump,
                                           ReadSize,
                                           out bytesRead);

                // Good read?
                if (result == false || bytesRead != ReadSize)
                {
                    Win32Exception wExcept = new Win32Exception();
                    OnScanError(Target, wExcept.Message);
                    return false;
                }
            }
            catch (Exception e)
            {
                // Report the exception to anyone listening.
                OnScanException(Target, e);
                return false;
            }

            // Completed with no problems.
            return true;
        }
        #endregion

        #region MaskCheck
        /*
         * Compares dumped memory to a byte signature using a mask with
         * '?' wildcards to skip unknown bytes in the array.
         * - nOffset is the offset into the memory dump to start at.
         * - bPattern is the byte array to check against the memory dump.
         * - Mask is a string where each character is 'x' for "valid"
         *   or a '?' for wildcard.
         */
        private bool MaskCheck(int nOffset, byte[] bPattern, string Mask)
        {
            // Loop the pattern length while comparing mask and dump.
            // If a mismatch happens return false.
            for (int i = 0; i < bPattern.Length; i++)
            {
                // Does this put us past the end of the memory dump?
                if (i + nOffset >= _memdump.Length) { return false; }

                // If the mask is a wildcard ignore this byte.
                if (Mask[i] == '?') { continue; }

                // Check the pattern against the memory dump.
                if (bPattern[i] != _memdump[nOffset + i]) { return false; }
            }

            // Matched everything, found the pattern.
            return true;
        }
        #endregion

        #region StructToBytes
        /*
         * Helpful function to turn a structure into a byte array for
         * scanning.
         * 
         * NOTE: Make sure your struct implements LayoutKind.Sequential
         *       otherwise C# will "pad" the struct to make it match the
         *       memory boundaries and your signature will never match.
         *       If you need a refresher on how to use structs in C# I
         *       highly recommend:
         *       http://www.developerfusion.com/article/84519/mastering-structs-in-c/
         */
        public byte[] StructToBytes(object aStruct)
        {
            // Find the size of the struct in memory.
            int size = Marshal.SizeOf(aStruct);

            // Allocate memory that size on the heap, get a pointer to it.
            IntPtr buf = Marshal.AllocHGlobal(size);

            // Convert the struct to a pointer, with no Dispose() on struct beforehand.
            Marshal.StructureToPtr(aStruct, buf, false);

            // Create a byte array to receive the converted struct.
            byte[] structBytes = new byte[size];

            // Then copy from the heap to our array.
            Marshal.Copy(buf, structBytes, 0, size);

            // Free the heap memory, no memory leaks please.
            Marshal.FreeHGlobal(buf);

            // Return the converted struct as bytes.
            return structBytes;
        }
        #endregion

        #region Load
        /*
         * Load a signature scanner from a file. It is the caller's
         * responsibility to make sure the returned scanner isn't null.
         */
        public static SigScanner Load(string FilePath)
        {
            SigScanner temp = null;
            try
            {
                using (TextReader TR = new StreamReader(FilePath))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(SigScanner));
                    temp = (SigScanner)xml.Deserialize(TR);
                }
            }
            catch (Exception) { return null; }
            return temp;
        }
        #endregion

        #region Save
        /*
         * Save this scanner into an XML file for later use. Caller has
         * the responsibility for making sure the path exists and is
         * writeable for this function.
         */
        public bool Save(string FilePath)
        {
            try
            {
                using (TextWriter TW = new StreamWriter(FilePath))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(SigScanner));
                    xml.Serialize(TW, this);
                }
            }
            catch (Exception) { return false; }

            // Everything seems to have completed successfully.
            return true;
        }
        #endregion
    }
    #endregion
}
