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
                //TODO: _bus.Write(0xd400, Value);
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
                //TODO: _bus.Write(0xd401, Value);
            }
            get
            {
                //TODO: _bus.Read(0xd400);
                return 0x00;
            }
        }

        //display list pointer (low byte), write, 0xd402
        byte DLISTL
        {
            get
            {
                return _bus.Read(0xd402);
            }
        }

        //display list pointer (high byte), write, 0xd403
        byte DLISTH
        {
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
                //TODO: _bus.Write(0xd404, Value);
            }
            get
            {
                //TODO: _bus.Read(0xd404);
                return 0x00;
            }
        }

        //vertical fine scroll, write, 0xd405
        byte VSCROL
        {
            set
            {
                //TODO: _bus.Write(0xd405, Value);
            }
            get
            {
                //TODO: _bus.Read(0xd405);
                return 0x00;
            }
        }

        //player/missile base address, write, 0xd407
        byte PMBASE
        {
            set
            {
                //TODO: _bus.Write(0xd407, Value);
            }
            get
            {
                //TODO: _bus.Read(0xd407);
                return 0x00;
            }
        }

        //character set base address, write, 0xd409
        byte CHBASE
        {
            set
            {
                //TODO: _bus.Write(0xd409, Value);
            }
            get
            {
                //TODO: _bus.Read(0xd409);
                return 0x00;
            }
        }

        //wait for sync, write, 0xd40a
        byte WSYNC
        {
            set
            {
                //TODO: _bus.Write(0xd40a, Value);
            }
            get
            {
                //TODO: _bus.Read(0xd40a);
                return 0x00;
            }
        }

        //verctical line counter, read, 0xd40b
        byte VCOUNT
        {
            get
            {
                //TODO: _bus.Read(0xd40b);
                return 0x00;
            }
        }

        //light pen horizontal position, read, 0xd40c
        //write to ram to be able to read by CPU
        byte PENH
        {
            get
            {
                //TODO: _bus.Read(0xd40c);
                return 0x00;
            }
        }

        //light pen vertical position, read, 0xd40d
        //write to ram to be able to read by CPU
        byte PENV
        {
            set
            {
                _bus.Write(0xd40d, value);
            }
        }

        //nmi enable, write, 0xd40e
        //read from ram after CPU writes the value
        byte NMIEN
        {
            get
            {
                return _bus.Read(0xd40e);
            }
        }

        //nmi reset, write, 0xd40f
        //read from ram after CPU writes the value
        byte NMIRES
        {
            get
            {
                return _bus.Read(0xd40f);
            }
        }
        //nmi status, read
        //write to ram to be able to read by CPU
        
        byte NMIST
        {
            set
            {
                _bus.Write(0xd40f, value);
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
    }
}
