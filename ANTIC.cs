using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulators
{
    /*
     * https://en.wikipedia.org/wiki/ANTIC
     */
    public class ANTIC
    {
        public enum GraphicMode
        {
            Null,
        }

        //When CPU writes value to RAM ANTIC reads that value from RAM
        //For below table we need to:
        //- read value for 'Write' registers
        //- write value for 'Read' registers
        /*
            DMACTL | Direct Memory Access Control        | Write | $D400 | 54272 | SDMCTL | $022F | 559
            CHACTL | Character Control                   | Write | $D401 | 54273 | CHART  | $02F3 | 755
            DLISTL | Display List Pointer (low byte)     | Write | $D402 | 54274 | SDLSTL | $0230 | 560
            DLISTH | Display List Pointer (high byte)    | Write | $D403 | 54275 | SDLSTH | $0231 | 561
            HSCROL | Horizontal Fine Scroll              | Write | $D404 | 54276
            VSCROL | Vertical Fine Scroll                | Write | $D405 | 54277
            PMBASE | Player/Missile Base Address         | Write | $D407 | 54279
            CHBASE | Character Set Base Address          | Write | $D409 | 54281 | CHBAS  | $02F4 | 756
            WSYNC  | Wait for Horizontal Sync            | Write | $D40A | 54282 
            VCOUNT | Vertical Line Counter               | Read  | $D40B | 54283
            PENH   | Light Pen Horizontal Position       | Read  | $D40C | 54284 | LPENH  | $0234 | 564
            PENV   | Light Pen Vertical Position         | Read  | $D40D | 54285 | LPENV  | $0235 | 565
            NMIEN  | Non-Maskable Interrupt (NMI) Enable | Write | $D40E | 54286
            NMIRES | Non-Maskable Interrupt (NMI) Reset  | Write | $D40F | 54287
            NMIST  | Non-Maskable Interrupt (NMI) Status | Read  | $D40F | 54287
         */


        private IBus _bus;

        //Link ANTIC with system Bus
        public void ConnectBus(ref IBus bus)
        {
            _bus = bus;
        }

        private CPU6502 _cpu;
        public void ConnectCPU(CPU6502 cpu)
        {
            _cpu = cpu;
        }

        private int _scanline = 0;
        private int _cycle = 0; // 0-113 cycles per scanline
        private ushort _dlist_ptr = 0;
        private byte _dli = 0;
        private int _mode_line = 0;
        private int _mode_height = 0;

        public void Tick()
        {
            // DMA Control Check
            byte dmactl = _bus.Read(0xD400);
            bool dl_dma_enabled = (dmactl & 0x20) != 0;

            _cycle++;
            if (_cycle >= 114)
            {
                _cycle = 0;
                _scanline++;

                if (_scanline >= 262)
                {
                    _scanline = 0;
                }

                // Update VCOUNT in RAM
                _bus.Write(0xD40B, (byte)(_scanline >> 1));

                // Clear WSYNC halt at the start of horizontal blanking or start of line
                // According to docs, RDY is reset by the beginning of horizontal blank.
                // In our simple model, we'll clear it at cycle 0.
                if (_cpu != null)
                {
                    _cpu.Halt = false;
                }
            }

            // Monitor WSYNC write
            // In a real system, the write itself triggers the latch.
            // Here we check if the value at $D40A is non-zero (assuming the CPU just wrote to it)
            // or we could check every cycle.
            if (_bus.Read(0xD40A) != 0)
            {
                if (_cpu != null) _cpu.Halt = true;
                _bus.Write(0xD40A, 0); // Clear the register after halting
            }

            // Simple Display List Fetching Logic
            // In real hardware, this happens at specific cycles.
            // Here we'll simulate cycle-stealing when fetching a new instruction.
            if (dl_dma_enabled && _scanline >= 8 && _scanline < 248)
            {
                if (_mode_line >= _mode_height)
                {
                    // Fetch new instruction
                    // Cycle stealing: ANTIC halts CPU to fetch instruction
                    if (_cpu != null) _cpu.Halt = true;

                    _dli = _bus.Read(_dlist_ptr++);

                    // Reset CPU Halt after "stealing" a cycle (simplified)
                    // In reality, it might stay halted for more cycles if it's a JMP or LSM
                    if (_cpu != null) _cpu.Halt = false;

                    ProcessDLI();
                    _mode_line = 0;
                }
                else
                {
                    _mode_line++;
                }
            }

            if (_scanline == 0 && _cycle == 0)
            {
                if (dl_dma_enabled)
                {
                    byte lo = _bus.Read(0xD402);
                    byte hi = _bus.Read(0xD403);
                    _dlist_ptr = (ushort)((hi << 8) | lo);
                    _mode_line = 0;
                    _mode_height = 0;
                }
            }
        }

        private void ProcessDLI()
        {
            byte opcode = (byte)(_dli & 0x0F);

            // Very simplified mode height mapping
            if (opcode == 0x00) // Blank lines
            {
                _mode_height = ((_dli & 0x70) >> 4) + 1;
            }
            else if (opcode == 0x02 || opcode == 0x03 || opcode == 0x04 || opcode == 0x05)
            {
                _mode_height = 8;
            }
            else if (opcode == 0x06 || opcode == 0x07)
            {
                _mode_height = 10;
            }
            else if (opcode == 0x0F) // Jump
            {
                if ((_dli & 0x40) != 0) // JVB
                {
                    // Fetch address
                    byte lo = _bus.Read(_dlist_ptr++);
                    byte hi = _bus.Read(_dlist_ptr++);
                    _dlist_ptr = (ushort)((hi << 8) | lo);
                    _mode_height = 0; // Trigger immediate refetch next line
                }
            }
            else
            {
                _mode_height = 1;
            }

            // Handle Load Memory Scan (LMS) - bit 6
            if (opcode >= 0x02 && (_dli & 0x40) != 0)
            {
                // Fetch 2 bytes address (simplified, we don't store it yet)
                _bus.Read(_dlist_ptr++);
                _bus.Read(_dlist_ptr++);
            }
        }
    }
}
