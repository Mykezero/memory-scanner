
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

/// 
/// Credit to Author: atom0s
/// https://github.com/atom0s/Clipper/blob/master/Clipper/Classes/SigScan.cs
///
/// Changes: 
/// The find pattern method is altered to search application pages of memory to find
/// byte patterns now. 
/// 

/**
 * Clipper (c) atom0s 2004 - 2013 [wiccaan@comcast.net]
 * ---------------------------------------------------------------------------------
 * This file is part of Clipper.
 *
 *      Clipper is free software: you can redistribute it and/or modify
 *      it under the terms of the GNU Lesser General Public License as published by
 *      the Free Software Foundation, either version 3 of the License, or
 *      (at your option) any later version.
 *
 *      Clipper is distributed in the hope that it will be useful,
 *      but WITHOUT ANY WARRANTY; without even the implied warranty of
 *      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *      GNU Lesser General Public License for more details.
 *
 *      You should have received a copy of the GNU Lesser General Public License
 *      along with Clipper.  If not, see <http://www.gnu.org/licenses/>.
 */

/**
 * sigScan C# Implementation - Written by atom0s [aka Wiccaan]
 * Class Version: 2.0.0
 * 
 * [ CHANGE LOG ] ------------------------------------------------------------------------- 
 *
 *      2.0.0 
 *          - Updated to no longer require unsafe or fixed code. 
 *          - Removed unneeded methods and code. 
 *          
 *      1.0.0
 *          - First version written and release. 
 *          
 * [ CREDITS ] ---------------------------------------------------------------------------- 
 * 
 *      sigScan is based on the FindPattern code written by 
 *      dom1n1k and Patrick at GameDeception.net 
 *      
 *      Full credit to them for the purpose of this code. I, atom0s, simply 
 *      take credit for converting it to C#. 
 * 
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZeroLimits.ZScanLib;

namespace ZeroLimits.ZScanLib.MemoryManagement
{

    // Add SigScan to our memory library. 
    public partial class Memory
    {
        static Memory() { Scanner = new SigScan(); }

        public static SigScan Scanner { get; set; }
    }

    public class SigScan
    {
        /// <summary>
        /// ReadProcessMemory
        ///
        /// API import definition for ReadProcessMemory.
        /// </summary>
        /// <param name="hProcess">Handle to the process we want to read from.</param>
        /// <param name="lpBaseAddress">The base address to start reading from.</param>
        /// <param name="lpBuffer">The return buffer to write the read data to.</param>
        /// <param name="dwSize">The size of data we wish to read.</param>
        /// <param name="lpNumberOfBytesRead">The number of bytes successfully read.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(
        IntPtr hProcess,
        IntPtr lpBaseAddress,
        [Out] byte[] lpBuffer,
        int dwSize,
        out int lpNumberOfBytesRead
        );


        /// <summary> 
        /// m_vDumpedRegion 
        ///  
        ///     The memory dumped from the external process. 
        /// </summary> 
        private byte[] m_vDumpedRegion;

        /// <summary> 
        /// m_vProcess 
        ///  
        ///     The process we want to read the memory of. 
        /// </summary> 
        private Process m_vProcess;

        /// <summary> 
        /// m_vAddress 
        ///  
        ///     The starting address we want to begin reading at. 
        /// </summary> 
        private IntPtr m_vAddress;

        /// <summary> 
        /// m_vSize 
        ///  
        ///     The number of bytes we wish to read from the process. 
        /// </summary> 
        private Int32 m_vSize;

        /// <summary>
        /// Process
        /// 
        ///     The process to read from. 
        /// </summary>
        public Process Process
        {
            get { return this.m_vProcess; }
            set { this.m_vProcess = value; }
        }

        /// <summary>
        /// The address we want to read. 
        /// </summary>
        public IntPtr Address
        {
            get { return this.m_vAddress; }
            set { this.m_vAddress = value; }
        }

        /// <summary>
        /// The number of bytes to read from the process. 
        /// </summary>
        public Int32 Size
        {
            get { return this.m_vSize; }
            set { this.m_vSize = value; }
        }


        /// <summary> 
        /// SigScan 
        ///  
        ///     Main class constructor that uses no params.  
        ///     Simply initializes the class properties and  
        ///     expects the user to set them later. 
        /// </summary> 
        public SigScan()
        {
            this.m_vProcess = null;
            this.m_vAddress = IntPtr.Zero;
            this.m_vSize = 0;
            this.m_vDumpedRegion = null;
            this.ModuleScanEnabled = true;
        }

        /// <summary> 
        /// SigScan 
        ///  
        ///     Overloaded class constructor that sets the class 
        ///     properties during construction. 
        /// </summary> 
        /// <param name="proc">The process to dump the memory from.</param> 
        /// <param name="addr">The started address to begin the dump.</param> 
        /// <param name="size">The size of the dump.</param> 
        public SigScan(Process proc, IntPtr addr, int size)
        {
            this.m_vProcess = proc;
            this.m_vAddress = addr;
            this.m_vSize = size;
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

                int nBytesRead = 0;

                // Dump the memory. 
                var ret = ReadProcessMemory(
                    this.m_vProcess.Handle, this.m_vAddress, this.m_vDumpedRegion, this.m_vSize, out nBytesRead);

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

        /// <summary>
        /// Scan a module's memory for the signature. 
        /// </summary>
        /// <param name="strMask"></param>
        /// <param name="btPattern"></param>
        /// <param name="nOffset"></param>
        /// <returns></returns>
        private IntPtr ScanModule(String strMask, byte[] btPattern, int nOffset)
        {
            try
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
                // Pattern was not found.
                return IntPtr.Zero;
            }
            catch (Exception)
            {
                return IntPtr.Zero;
            }
        }

        private IntPtr ScanPages(String strMask, byte[] btPattern, int nOffset)
        {
            List<IntPtr> addresses = new List<IntPtr>();

            // A place to hold of virtual memory pages. 
            ICollection<Memory.MEMORY_BASIC_INFORMATION> pages;

            var pointers = new ConcurrentBag<IntPtr>();
            
            var match = new ConcurrentBag<double>();
            
            match.Add(0);          
            
            Memory.MEMORY_BASIC_INFORMATION? highestMatchPage = null;
            
            object pageLock = new object();

            // Returns if no pages were read. 
            if (!Memory.PeekPages(this.m_vProcess, out pages)) return IntPtr.Zero;

            Console.WriteLine("Page Count: " + pages.Where(x => Memory.PageFilter()(x)).Count());

            Parallel.ForEach(pages, page =>
            {
                IntPtr address = IntPtr.Zero;

                // Filter out pages that aren't read/writable. 
                if (!Memory.PageFilter()(page)) return;

                // Set up the storage region for data to be read. 
                var btBuffer = new byte[(int)page.RegionSize];

                // The amount read. 
                IntPtr bytesRead = IntPtr.Zero;

                // Read the memory
                bool success =
                    Memory.ReadProcessMemory(
                    this.Process.Handle,
                    page.BaseAddress,
                    btBuffer,
                    (uint)page.RegionSize,
                    ref bytesRead);

                // Skips if we could read the memory. 
                if (!success) return;

                var percent = CalculateMatchPercentage(strMask, btPattern, btBuffer);

                if (!highestMatchPage.HasValue)
                {
                    lock (pageLock)
                    {
                        if (!highestMatchPage.HasValue)
                            highestMatchPage = page;
                    }
                }

                if (match.Max() < percent)
                {
                    lock (pageLock)
                    {
                        if (match.Max() < percent)
                            highestMatchPage = page;
                    }
                }

                match.Add(percent);

                // Loop the region and look for the pattern. 
                address = FindSignature(btPattern, strMask, btBuffer, nOffset);

                if (address != IntPtr.Zero)
                {
                    pointers.Add(address);
                }
            });

            Console.WriteLine("Match Percent: " + match.Max() * 100 + "%");

            // Pattern was not found. 
            return IntPtr.Zero;
        }

        /// <summary>
        /// Returns the address for a given pattern with an offset. 
        /// </summary>
        /// <param name="strMask"></param>
        /// <param name="btPattern"></param>
        /// <param name="nOffset"></param>
        /// <returns></returns>
        public IntPtr Scan(String strMask, byte[] btPattern, int nOffset)
        {
            IntPtr address = IntPtr.Zero;

            // Return first match from process module. 
            if (this.ModuleScanEnabled)
            {
                address = ScanModule(strMask, btPattern, nOffset);
                if (address != IntPtr.Zero) return address;
            }

            // Return first match from memory pages.
            if (this.PageScanEnabled)
            {
                address = ScanPages(strMask, btPattern, nOffset);
                if (address != IntPtr.Zero) return address;
            }

            // Pattern match failed. 
            return address;
        }

        /// <summary>
        /// Checks the memory for a match. 
        /// </summary>
        /// <param name="btPattern"></param>
        /// <param name="strMask"></param>
        /// <param name="nOffset"></param>
        /// <returns></returns>
        public IntPtr PatternCheck(byte[] btPattern, String strMask, int nOffset)
        {
            // Ensure the mask and pattern lengths match.
            if (strMask.Length != btPattern.Length)
                return IntPtr.Zero;

            for (int x = 0; x < this.m_vDumpedRegion.Length - btPattern.Length; x++)
            {
                if (this.MaskCheck(x, btPattern, strMask))
                {
                    // The pattern was found, return it. 
                    return new IntPtr((int)this.m_vAddress + (x + nOffset));
                }
            }

            return IntPtr.Zero;
        }

        public IntPtr FindSignature(byte[] btPattern, String strMask, byte[] btBuffer, int nOffset)
        {
            // Ensure the mask and pattern lengths match.
            if (strMask.Length != btPattern.Length)
                return IntPtr.Zero;

            int count = 0;

            for (int x = 0; x < btPattern.Length; x++)
            {
                if (strMask[x] == '?')
                {
                    count++;
                    continue;
                }
                else if (btPattern[x] == btBuffer[x])
                {
                    count++;
                }
                else { count = 0; }

                // We've matched the pattern. 
                if (count == btPattern.Length)
                {
                    return new IntPtr((int)this.m_vAddress + (x + nOffset));
                }
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strMask"></param>
        /// <param name="btPattern"></param>
        /// <param name="btBuffer"></param>
        /// <returns></returns>
        public double CalculateMatchPercentage(String strMask, byte[] btPattern, byte[] btBuffer)
        {
            int count = 0;
            double matchPercent = 0;

            for (int x = 0; x < btPattern.Length; x++)
            {
                if (strMask[x] == '?')
                {
                    count++;
                    continue;
                }
                else if (btPattern[x] == btBuffer[x])
                {
                    count++;
                    matchPercent = Math.Max(matchPercent, (double)count / (double)btPattern.Length);
                }
                else
                {
                    count = 0;
                }

                // We've matched the pattern. 
                if (count == btPattern.Length)
                    return 1.0;
            }

            btPattern.Where((x, t) => x == btBuffer[t]);

            return matchPercent;
        }

        /// <summary> 
        /// ResetRegion 
        ///  
        ///     Resets the memory dump array to nothing to allow 
        ///     the class to redump the memory. 
        /// </summary> 
        public void ResetRegion()
        {
            this.m_vDumpedRegion = null;
        }

        /// <summary>
        /// Is module scanning enabled?
        /// </summary>
        public bool ModuleScanEnabled { get; set; }

        /// <summary>
        /// Is memory page scanning enabled?
        /// </summary>
        public bool PageScanEnabled { get; set; }
    }
}