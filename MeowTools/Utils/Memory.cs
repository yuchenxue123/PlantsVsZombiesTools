using System;
using System.Runtime.InteropServices;

namespace MeowTools.Utils
{
    public static class Memory
    {

        #region Native Methods & Flags

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
        
        [Flags]
        public enum MemoryProtection : uint
        {
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            Guard = 0x100,
            NoCache = 0x200,
            WriteCombine = 0x400
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
        public static extern bool VirtualProtectEx(
            IntPtr processHandle,
            IntPtr address,
            IntPtr size,
            MemoryProtection newProtect,
            out MemoryProtection oldProtect);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        #endregion

        /// <summary>
        /// 根据基址和偏移计算最终地址
        /// </summary>
        public static bool GetPointerAddress(IntPtr handle, IntPtr baseAddress, int[] offsets, out IntPtr address)
        {
            address =  IntPtr.Zero;
            var currentAddress = baseAddress;
            var buffer = new byte[IntPtr.Size];

            foreach (var offset in offsets)
            {
                if (!ReadProcessMemory(handle, currentAddress, buffer, (IntPtr)buffer.Length, out _))
                {
                    return false;
                }
                
                var pointer = IntPtr.Size == 8 ? (IntPtr) BitConverter.ToInt64(buffer, 0) : (IntPtr) BitConverter.ToInt32(buffer, 0);
                
                if (pointer == IntPtr.Zero)
                {
                    return false;
                }
                
                currentAddress = IntPtr.Add(pointer, offset);
            }
            
            address = currentAddress;
            return true;
        }
        
        /// <summary>
        /// 写入汇编代码
        /// </summary>
        public static bool WriteAssembly(IntPtr handle, IntPtr address, byte[] bytes)
        {
            if (handle == IntPtr.Zero || address == IntPtr.Zero || bytes == null || bytes.Length == 0)
            {
                return false;
            }

            var size = (IntPtr)bytes.Length;
            
            // 修改内存保护属性
            if (!VirtualProtectEx(handle, address, size, MemoryProtection.ExecuteReadWrite, out var oldProtect))
            {
                return false;
            }
            
            var result = WriteProcessMemory(handle, address, bytes, size, out _);

            // 恢复原内存保护属性
            VirtualProtectEx(handle, address, size, oldProtect, out _);

            return result;
        }
        
        /// <summary>
        /// 读取汇编代码
        /// </summary>
        public static bool ReadAssembly(IntPtr handle, IntPtr address, int length, out byte[] asmBytes)
        {
            asmBytes = null;
            
            if (handle == IntPtr.Zero || address == IntPtr.Zero || length <= 0)
            {
                return false;
            }

            asmBytes = new byte[length];
            return ReadProcessMemory(handle, address, asmBytes, (IntPtr)length, out _);
        }
    }
}