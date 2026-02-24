using System;
using System.Runtime.InteropServices;

namespace MeowTools.Utils
{
    public static class Memory
    {
        
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001FFFFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x00000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize =  0x00100000,
            
            ReadWrite = VirtualMemoryOperation | VirtualMemoryRead | VirtualMemoryWrite | QueryInformation
        }
        
        [DllImport("kernel32.dll",  SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags desiredAccess, bool inheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(
            IntPtr processHandle,
            IntPtr address,
            [Out] byte[] buffer,
            IntPtr size,
            out IntPtr numberOfBytesRead);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(
            IntPtr processHandle,
            IntPtr address,
            byte[] buffer,
            IntPtr size,
            out IntPtr numberOfBytesWritten);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        /// <summary>
        /// 根据基址和偏移计算最终地址
        /// </summary>
        public static bool GetPointerAddress(IntPtr handle, IntPtr baseAddress, int[] offsets, out IntPtr address)
        {
            address =  IntPtr.Zero;
            var currentAddress = baseAddress;
            var buffer = new byte[8];

            foreach (var offset in offsets)
            {
                if (!ReadProcessMemory(handle, currentAddress, buffer, (IntPtr)8, out _))
                {
                    return false;
                }
                
                var pointer = (IntPtr) BitConverter.ToInt64(buffer, 0);
                
                if (pointer == IntPtr.Zero)
                {
                    return false;
                }
                
                currentAddress = IntPtr.Add(pointer, offset);
            }
            
            address = currentAddress;
            return true;
        }
    }
}