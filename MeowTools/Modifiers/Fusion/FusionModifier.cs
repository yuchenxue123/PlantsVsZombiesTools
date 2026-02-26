using System;
using System.Diagnostics;
using System.Windows;
using MeowTools.Utils;

namespace MeowTools.Modifiers.Fusion
{
    public class FusionModifier : Modifier
    {

        private const string Module = "GameAssembly.dll";
        private const int BoardOffset = 0x20895C0; // + 0xB8 (静态类) + 0x00 (静态实例字段)
        private const int GameAppOffset = 0x2098E08; // + 0xB8 (静态类) + 0x08 (静态实例字段)
        
        public FusionModifier(Process process) : base(process)
        {
        }


        public override bool ModifySunshine(long count)
        {
            return WriteSunshine(count);
        }
        
        public override bool ModifyCoin(long count)
        {
            return WriteCoin(count);
        }

        public override bool PlantNoConsume()
        {
            return false;
        }

        public override bool PlantNoCooldown()
        {
            return WritePlantNoCooldown();
        }

        private IntPtr GetBoardStaticClassAddress()
        {
            int[] offsets = { 0xB8 };
            return Memory.GetPointerAddress(
                Handle,
                BaseAddress(Module, BoardOffset),
                offsets,
                out var add) ? add : IntPtr.Zero;
        }
        
        private IntPtr GetBoardInstanceAddress()
        {
            int[] offsets = { 0xB8,  0x00 };
            return Memory.GetPointerAddress(
                Handle,
                BaseAddress(Module, BoardOffset),
                offsets,
                out var add) ? add : IntPtr.Zero;
        }

        private IntPtr GetGameAppStaticClassAddress()
        {
            int[] offsets = { 0xB8 };
            return Memory.GetPointerAddress(
                Handle,
                BaseAddress(Module, GameAppOffset),
                offsets,
                out var add) ? add : IntPtr.Zero;
        }

        private static readonly int[] SunshineOffsets = { 0xF0 };

        private bool WriteSunshine(long count)
        {
            if (Memory.GetPointerAddress(Handle, GetBoardInstanceAddress(), SunshineOffsets, out var address))
            {
                var buffer = BitConverter.GetBytes(count);
                if (Memory.WriteProcessMemory(Handle, address, buffer, (IntPtr)buffer.Length, out _))
                {
                    return true;
                }

                MessageBox.Show("写入内存失败");
                return false;
            }
            
            MessageBox.Show("无法找到阳光地址，可能是没进关卡，或者其他问题");
            return false;
        }
        
        // 这是局外的
        private static readonly int[] CoinOffsets = { 0x88 };
        
        private bool WriteCoin(long count)
        {
            if (Memory.GetPointerAddress(Handle, GetGameAppStaticClassAddress(), CoinOffsets, out var address))
            {
                var buffer = BitConverter.GetBytes(count);
                if (Memory.WriteProcessMemory(Handle, address, buffer, (IntPtr)buffer.Length, out _))
                {
                    return true;
                }

                MessageBox.Show("写入内存失败");
                return false;
            }
            
            MessageBox.Show("无法找到金币地址");
            return false;
        }
        
        
        private static readonly int[] NoCooldownOffsets = { 0x1D2 };
        
        private bool WritePlantNoCooldown()
        {
            if (Memory.GetPointerAddress(Handle, GetBoardInstanceAddress(), NoCooldownOffsets, out var address))
            {
                var buffer = BitConverter.GetBytes(true);
                if (Memory.WriteProcessMemory(Handle, address, buffer, (IntPtr)buffer.Length, out _))
                {
                    return true;
                }

                MessageBox.Show("写入内存失败");
                return false;
            }
            
            MessageBox.Show("无法找到地址");
            return false;
        }
    }
}