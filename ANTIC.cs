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
        private ushort _mscan_ptr = 0;
        private byte _dli = 0;
        private int _mode_line = 0;
        private int _mode_height = 0;
        private byte[] _video_memory = new byte[160 * 262]; // Simplified pixel buffer

        private ushort _pmbase_ptr = 0;

        public void Tick()
        {
            // DMA Control Check
            byte dmactl = _bus.Read(0xD400);
            bool dl_dma_enabled = (dmactl & 0x20) != 0;
            bool screen_dma_enabled = (dmactl & 0x10) != 0 || (dmactl & 0x03) != 0;
            bool pm_dma_enabled = (dmactl & 0x0C) != 0;

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
            if (dl_dma_enabled && _scanline >= 8 && _scanline < 248)
            {
                // Screen DMA simulation
                // In a real Atari, different modes steal different amounts of cycles
                // For now, we'll simulate a fixed amount of theft if screen DMA is on
                if (screen_dma_enabled && _mode_height > 0 && _cycle == 10)
                {
                    // Fetch some data (e.g. 40 bytes for a normal line)
                    // This steals many cycles.
                    if (_cpu != null) _cpu.Halt = true;

                    // Simple loop to simulate fetching 40 bytes and rendering
                    if (_cpu != null) _cpu.Halt = true;

                    for (int i = 0; i < 40; i++)
                    {
                        byte data = _bus.Read(_mscan_ptr++);
                        // Render into video memory (very simplified: 1 byte per 4 color clocks)
                        for (int p = 0; p < 4; p++)
                        {
                            int idx = _scanline * 160 + (i * 4 + p);
                            if (idx < _video_memory.Length)
                            {
                                _video_memory[idx] = data;
                            }
                        }
                    }

                    if (_cpu != null) _cpu.Halt = false;
                }

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

                if (pm_dma_enabled)
                {
                    _pmbase_ptr = (ushort)(_bus.Read(0xD407) << 8);
                }
            }

            // Simple Player DMA simulation
            if (pm_dma_enabled && _scanline >= 16 && _scanline < 240 && _cycle == 20)
            {
                if (_cpu != null) _cpu.Halt = true;

                // Fetch player data (simplified 1-line resolution)
                // PMBASE + 0x400 (P0), 0x500 (P1), etc.
                for (int i = 0; i < 4; i++)
                {
                    _bus.Read((ushort)(_pmbase_ptr + 0x400 + (i * 0x100) + _scanline));
                }

                if (_cpu != null) _cpu.Halt = false;
            }
        }

        public byte[] GetVideoBuffer() => _video_memory;

        private void ProcessDLI()
        {
            byte opcode = (byte)(_dli & 0x0F);

            // Handle Display List Interrupt (DLI) - bit 7
            if ((_dli & 0x80) != 0)
            {
                byte nmien = _bus.Read(0xD40E);
                if ((nmien & 0x80) != 0) // DLI enabled
                {
                    // Set DLI bit in NMIST
                    byte nmist = _bus.Read(0xD40F);
                    _bus.Write(0xD40F, (byte)(nmist | 0x80));

                    // Trigger NMI on CPU
                    _cpu?.Nmi();
                }
            }

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
                byte lo = _bus.Read(_dlist_ptr++);
                byte hi = _bus.Read(_dlist_ptr++);
                _mscan_ptr = (ushort)((hi << 8) | lo);
            }
        }
    }
}
