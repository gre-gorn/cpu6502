using System;
using System.Collections.Generic;
using System.Text;

namespace Emulators
{
    public class Bus : IBus
    {
        private byte _size = 0;
        public byte Size
        {
            get => _size;
            set
            {
                if (value <= 64)
                {
                    _size = value;
                    ram = new byte[_size * 1024];
                    ClearRam();
                }
            }
        }

        private void ClearRam()
        {
            Array.Clear(ram, 0, _size * 1024);
        }

        //ram
        byte[] ram;

        public Bus(byte size)
        {
            Size = size;
        }

        //read/write methods
        public void Write(ushort addr, byte data)
        {
            if (addr >= 0x0000 && addr < _size * 1024)
                ram[addr] = data;
        }

        public byte Read(ushort addr)
        {
            if (addr >= 0x0000 && addr < _size * 1024)
                return ram[addr];
            else
                return 0x00;
        }
    }
}
