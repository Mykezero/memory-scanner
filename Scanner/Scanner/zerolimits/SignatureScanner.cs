using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;

namespace Scanner
{
    public class SignatureScanner
    {
        private int m_vSize;

        private byte[] m_vDumpedMemory;

        private Process m_vProcess;

        private IntPtr m_vAddress;

        private byte[] m_vDumpedRegion;


        // Used for virtual protect
        private readonly bool _closeOnDispose;

        private readonly uint _oldPermissions;

        private readonly IntPtr _processHandle;

        private readonly IntPtr _processStart;

        private IntPtr _upperMemoryBound;

        public SignatureScanner(Process proc, IntPtr addr, bool allowClose, int size = 1024 * 100)
        {
            this.m_vProcess = proc;
            this.m_vAddress = addr;
            this.m_vSize = size;

            this._closeOnDispose = allowClose;
            this._processHandle = Process.GetProcessById(proc.Id).Handle;
            this._closeOnDispose = allowClose;
            this._processStart = proc.MainModule.BaseAddress;
            this._upperMemoryBound = new IntPtr(proc.WorkingSet64);

            // Change the protection of committed pages.
            VirtualProtectEx(_processHandle, _processStart, _upperMemoryBound, ProcessAccessFlags.All,
                out _oldPermissions);
        }

        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize,
                                                    ProcessAccessFlags flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess,
                                                 [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer,
                                                     int dwSize, out int lpNumberOfBytesRead);

        ~SignatureScanner()
        {
            uint perms;
            VirtualProtectEx(_processHandle, _processStart, _upperMemoryBound, (ProcessAccessFlags)_oldPermissions,
                             out perms);
            if (_processHandle != IntPtr.Zero && _closeOnDispose)
                CloseHandle(_processHandle);
        }

        /// <summary> 
        /// DumpMemory 
        ///  
        ///     Internal memory dump function that uses the set class 
        ///     properties to dump a memory region. 
        /// </summary> 
        /// <returns>Boolean based on RPM results and valid properties.</returns> 
        private bool DumpMemory()
        {
            try
            {
                // Checks to ensure we have valid data. 
                if (this.m_vProcess == null)
                    return false;
                if (this.m_vProcess.HasExited)
                    return false;
                if (this.m_vAddress == IntPtr.Zero)
                    return false;
                if (this.m_vSize == 0)
                    return false;

                // Create the region space to dump into.
                this.m_vDumpedRegion = new byte[this.m_vSize];

                int nBytesRead;

                // Dump the memory. 
                var ret = ReadProcessMemory(
                    this.m_vProcess.Handle, this.m_vAddress, this.m_vDumpedRegion, this.m_vSize, out nBytesRead
                    );

                // Validation checks. 
                return ret && nBytesRead == this.m_vSize;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary> 
        /// MaskCheck 
        ///  
        ///     Compares the current pattern byte to the current memory dump 
        ///     byte to check for a match. Uses wildcards to skip bytes that 
        ///     are deemed unneeded in the compares. 
        /// </summary> 
        /// <param name="nOffset">Offset in the dump to start at.</param> 
        /// <param name="btPattern">Pattern to scan for.</param> 
        /// <param name="strMask">Mask to compare against.</param> 
        /// <returns>Boolean depending on if the pattern was found.</returns> 
        private bool MaskCheck(int nOffset, IEnumerable<byte> btPattern, string strMask)
        {
            // Loop the pattern and compare to the mask and dump. 
            return !btPattern.Where((t, x) => strMask[x] != '?' && ((strMask[x] == 'x') && (t != this.m_vDumpedRegion[nOffset + x]))).Any();

            // The loop was successful so we found the pattern. 
        }

        public IntPtr FindPattern(byte[] btPattern, string strMask, int nOffset)
        {
            while (true)
            {                
                // Dump the memory region if we have not dumped it yet. 
                if (this.m_vDumpedRegion == null || this.m_vDumpedRegion.Length == 0)
                {
                    if (!this.DumpMemory())
                        return IntPtr.Zero;
                }               

                // Ensure the mask and pattern lengths match. 
                if (strMask.Length != btPattern.Length)
                    return IntPtr.Zero;

                // Loop the region and look for the pattern. 
                for (int x = 0; x < this.m_vDumpedRegion.Length; x++)
                {
                    if (this.MaskCheck(x, btPattern, strMask))
                    {
                        // The pattern was found, return it. 
                        return new IntPtr((int)this.m_vAddress + (x + nOffset));
                    }
                }
                
                this.m_vAddress = new IntPtr((int)m_vAddress + m_vSize);
            }
        }

        #region Nested type: ProcessAccessFlags

        [Flags]
        private enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            // ReSharper disable UnusedMember.Local
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
            // ReSharper restore UnusedMember.Local
        }

        #endregion
    }
}