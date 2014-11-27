using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZeroLimits.ZScanLib.MemoryManagement
{
    public partial class Memory
    {
        /// <summary>
        /// kernel32.ReadProcessMemory Import
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="lpBaseAddress"></param>
        /// <param name="lpBuffer"></param>
        /// <param name="nSize"></param>
        /// <param name="lpNumberOfBytesRead"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool ReadProcessMemory(
        IntPtr hProcess,
        IntPtr lpBaseAddress,
        [In, Out] byte[] lpBuffer,
        UInt32 nSize,
        ref IntPtr lpNumberOfBytesRead
        );

        /// <summary>
        /// kernel32.WriteProcessMemory Import
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="lpBaseAddress"></param>
        /// <param name="lpBuffer"></param>
        /// <param name="nSize"></param>
        /// <param name="lpNumberOfBytesWritten"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [In, Out] byte[] lpBuffer,
            UInt32 nSize,
            ref IntPtr lpNumberOfBytesWritten
            );

        /// <summary>
        /// Used for reading virtual address pages of memory. 
        /// </summary>
        /// <param name="hProcess">The process handle</param>
        /// <param name="lpAddress">The address to read from</param>
        /// <param name="lpBuffer">The place to put the read data</param>
        /// <param name="dwLength">The amount of bytes read</param>
        /// <returns>0 - Failure</returns>
        [DllImport("kernel32.dll")]
        public static extern int VirtualQueryEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            out MEMORY_BASIC_INFORMATION lpBuffer,
            uint dwLength
            );

        /// <summary>
        /// Retrieves the system info. 
        /// </summary>
        /// <param name="lpSystemInfo"></param>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void GetSystemInfo(
            [MarshalAs(UnmanagedType.Struct)] ref SYSTEM_INFO lpSystemInfo
        );

        /// <summary>
        /// Information about a linear address space's memory page. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public AllocationProtect AllocationProtect;
            public IntPtr RegionSize;
            public MEMORY_STATE State;
            public AllocationProtect Protect;
            public MEMORY_TYPE Type;
        }

        public enum AllocationProtect : uint
        {
            PAGE_EXECUTE = 0x00000010,
            PAGE_EXECUTE_READ = 0X00000020,
            PAGE_EXECUTE_READWRITE = 0x00000040,
            PAGE_EXECUTE_WRITECOPY = 0x00000080,
            PAGE_NOACCESS = 0x00000001,
            PAGE_READONLY = 0x00000002,
            PAGE_READWRITE = 0x00000004,
            PAGE_WRITECOPY = 0x00000008,
            PAGE_GUARD = 0x00000100,
            PAGE_NOCACHE = 0x00000200,
            PAGE_WRITECOMBINE = 0x00000400,
        }

        public enum MEMORY_STATE : uint
        {
            MEM_COMMIT = 0x1000,
            MEM_FREE = 0x10000,
            MEM_RESERVE = 0x2000
        }

        public enum MEMORY_TYPE : uint
        {
            MEM_IMAGE = 0x1000000,
            MEM_MAPPED = 0x40000,
            MEM_PRIVATE = 0x20000
        }

        /// <summary>
        /// The system info structure. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            internal PROCESSOR_INFO_UNION p;
            public uint dwPageSize;
            public uint lpMinimumApplicationAddress;
            public uint lpMaximumApplicationAddress;
            public uint dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public uint wProcessorLevel;
            public uint wProcessorRevision;
        };

        /// <summary>
        /// The process info structure. 
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct PROCESSOR_INFO_UNION
        {
            [FieldOffset(0)]
            internal uint dwOemId;
            [FieldOffset(0)]
            internal ushort wProcessorArchitecture;
            [FieldOffset(2)]
            internal ushort wReserved;
        }
    }
}
