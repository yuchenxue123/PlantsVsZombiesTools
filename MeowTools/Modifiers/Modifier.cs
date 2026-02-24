using System;
using System.Diagnostics;
using MeowTools.Utils;

namespace MeowTools.Modifiers
{
    public abstract class Modifier
    {
        private readonly Process _process;
        
        protected IntPtr Handle;

        protected Modifier(Process process)
        {
            _process = process;
            CreateHandle();
        }

        private void CreateHandle()
        {
            Handle = Memory.OpenProcess(Memory.ProcessAccessFlags.ReadWrite, false, _process.Id);
        }

        public void Close()
        {
            if (Handle != IntPtr.Zero)
            {
                Memory.CloseHandle(Handle);
            }
        }


        protected IntPtr BaseAddress(int offset)
        {
            var module =  _process.MainModule;
            
            if (module == null)
            {
                return IntPtr.Zero;
            }
            
            return module.BaseAddress + offset;
        }
        
        protected IntPtr BaseAddress(string module, int offset)
        {
            var address = IntPtr.Zero;
            
            foreach (ProcessModule processModule in _process.Modules)
            {
                if (processModule.ModuleName != module) continue;
                
                address = processModule.BaseAddress;
                break;
            }
            
            if (address == IntPtr.Zero)
            {
                return address;
            }
            
            return address + offset;
        }


        public abstract bool ModifySunshine(int count);
        
        public abstract bool ModifyCoin(int count);
    }
}