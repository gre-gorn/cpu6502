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

        //ANTIC registers
        //direct memory acces control, write, 0xd400
        byte DMACTL
        {
            set
            {
                _bus.Write(0xd400, value);
            }
            get
            {
                return _bus.Read(0xd400);
            }
        }

        //character control, write, 0xd401
        byte CHACTL
        {
            set
            {
                _bus.Write(0xd401, value);
            }
            get
            {
                return _bus.Read(0xd401);
            }
        }

        //display list pointer (low byte), write, 0xd402
        byte DLISTL
        {
            set
            {
                _bus.Write(0xd402, value);
            }
            get
            {
                return _bus.Read(0xd402);
            }
        }

        //display list pointer (high byte), write, 0xd403
        byte DLISTH
        {
            set
            {
                _bus.Write(0xd403, value);
            }
            get
            {
                return _bus.Read(0xd403);
            }
        }

        //horizontal fine scroll, write, 0xd404
        byte HSCROL
        {
            set
            {
                _bus.Write(0xd404, value);
            }
            get
            {
                return _bus.Read(0xd404);
            }
        }

        //vertical fine scroll, write, 0xd405
        byte VSCROL
        {
            set
            {
                _bus.Write(0xd405, value);
            }
            get
            {
                return _bus.Read(0xd405);
            }
        }

        //player/missile base address, write, 0xd407
        byte PMBASE
        {
            set
            {
                _bus.Write(0xd407, value);
            }
            get
            {
                return _bus.Read(0xd407);
            }
        }

        //character set base address, write, 0xd409
        byte CHBASE
        {
            set
            {
                _bus.Write(0xd409, value);
            }
            get
            {
                return _bus.Read(0xd409);
            }
        }

        //wait for sync, write, 0xd40a
        byte WSYNC
        {
            set
            {
                _bus.Write(0xd40a, value);
            }
            get
            {
                return _bus.Read(0xd40a);
            }
        }

        //verctical line counter, read, 0xd40b
        byte VCOUNT
        {
            get
            {
                return _bus.Read(0xd40b);
            }
        }

        //light pen horizontal position, read, 0xd40c
        //write to ram to be able to read by CPU
        byte PENH
        {
            get
            {
                return _bus.Read(0xd40c);
            }
        }

        //light pen vertical position, read, 0xd40d
        //write to ram to be able to read by CPU
        byte PENV
        {
            get
            {
                return _bus.Read(0xd40d);
            }
        }

        //nmi enable, write, 0xd40e
        //read from ram after CPU writes the value
        byte NMIEN
        {
            set
            {
                _bus.Write(0xd40e, value);
            }
            get
            {
                return _bus.Read(0xd40e);
            }
        }

        //nmi reset, write, 0xd40f
        //read from ram after CPU writes the value
        byte NMIRES
        {
            set
            {
                _bus.Write(0xd40f, value);
            }
            get
            {
                return _bus.Read(0xd40f);
            }
        }
        
        //nmi status, read, 0xd40f
        //write to ram to be able to read by CPU
        byte NMIST
        {
            get
            {
                return _bus.Read(0xd40f);
            }
        }

        private IBus _bus;
        
        //Link ANTIC with system Bus
        public void ConnectBus(ref IBus bus)
        {
            _bus = bus;
        }

        public void Tick()
        {
            ushort dlist_addr = (ushort)((DLISTH << 8) | DLISTL);
            Console.WriteLine("dlist: ${0:X4}", dlist_addr);
        }

        /// <summary>
        /// Example method demonstrating ANTIC register usage
        /// Shows how to read from and write to ANTIC registers
        /// </summary>
        public void ExampleRegisterUsage()
        {
            // Example: Writing to ANTIC registers
            
            // Set display list pointer to address $8000
            DLISTL = 0x00;  // Low byte
            DLISTH = 0x80;  // High byte
            
            // Enable DMA for display list and playfield
            DMACTL = 0x22;  // Enable display list DMA and single-line resolution
            
            // Set character set base address
            CHBASE = 0xE0;  // Point to character ROM
            
            // Set horizontal and vertical scroll
            HSCROL = 0x08;  // Fine scroll 8 pixels right
            VSCROL = 0x04;  // Fine scroll 4 pixels down
            
            // Example: Reading from ANTIC registers
            
            // Read current vertical line counter
            byte currentLine = VCOUNT;
            Console.WriteLine($"Current scanline: {currentLine}");
            
            // Read light pen positions (if light pen is connected)
            byte penX = PENH;
            byte penY = PENV;
            Console.WriteLine($"Light pen position: X={penX}, Y={penY}");
            
            // Read NMI status
            byte nmiStatus = NMIST;
            Console.WriteLine($"NMI Status: 0x{nmiStatus:X2}");
            
            // Read back the display list pointer we set earlier
            ushort displayListAddr = (ushort)((DLISTH << 8) | DLISTL);
            Console.WriteLine($"Display List Address: 0x{displayListAddr:X4}");
        }
    }
}
