using System;
using System.Diagnostics;
using System.Windows;
using MeowTools.Utils;

namespace MeowTools.Modifiers.Hybrid
{
    public class HybridModifier : Modifier
    {
        private const int Offset = 0x063B0FC0;

        public HybridModifier(Process process) : base(process)
        {
            
        }

        public override bool ModifySunshine(int count)
        {
            return WriteSunshine(Handle, count);
        }

        public override bool ModifyCoin(int count)
        {
            return WriteCoin(Handle, count);
        }

        private IntPtr BaseAddress()
        {
            return BaseAddress(Offset);
        }
        
        
        private static readonly int[] SunshineOffsets = {0x770, 0x60, 0x28, 0x40, 0x60, 0x28, 0x1B8};
        
        // private static readonly int[] SunshineOffsets = {0x770, 0x148, 0x28, 0x18, 0x60, 0x28, 0x1B8};

        private bool WriteSunshine(IntPtr handle, int count)
        {
            if (Memory.GetPointerAddress(handle, BaseAddress(), SunshineOffsets, out var address))
            {
                var buffer = BitConverter.GetBytes(count);
                if (Memory.WriteProcessMemory(handle, address, buffer, (IntPtr)buffer.Length, out _))
                {
                    return true;
                }

                MessageBox.Show("写入内存失败");
                return false;
            }

            MessageBox.Show("无法找到阳光地址，可能是没进关卡，或者其他问题");
            return false;
        }
        
        private static readonly int[] CoinOffsets = {0x258, 0x20, 0x60, 0x28, 0x38};
        
        // private static readonly int[] CoinOffsets = {0x258, 0x20, 0x2C8, 0x60, 0x28, 0x38};

        private bool WriteCoin(IntPtr handle, int count)
        {
            if (Memory.GetPointerAddress(handle, BaseAddress(), CoinOffsets, out var address))
            {
                var buffer = BitConverter.GetBytes(count);
                if (Memory.WriteProcessMemory(handle, address, buffer, (IntPtr)buffer.Length, out _))
                {
                    return true;
                }

                MessageBox.Show("写入内存失败");
                return false;
            }

            MessageBox.Show("无法找到金币地址");
            return false;
        }

    }
}