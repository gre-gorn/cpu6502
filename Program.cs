using System;
using Emulators;

namespace Emulator
{
    class Program
    {
        static void PrintCPUState(ref CPU6502 cpu)
        {
            if (cpu is null) return;

            Console.WriteLine();
            Console.WriteLine("CPU STATUS");
            Console.WriteLine("NV-BDIZC   PC  AR XR YR SP");
            Console.Write(cpu.GetFlag(CPU6502.FLAGS65C02.N));
            Console.Write(cpu.GetFlag(CPU6502.FLAGS65C02.V));
            Console.Write(cpu.GetFlag(CPU6502.FLAGS65C02.U));
            Console.Write(cpu.GetFlag(CPU6502.FLAGS65C02.B));
            Console.Write(cpu.GetFlag(CPU6502.FLAGS65C02.D));
            Console.Write(cpu.GetFlag(CPU6502.FLAGS65C02.I));
            Console.Write(cpu.GetFlag(CPU6502.FLAGS65C02.Z));
            Console.Write(cpu.GetFlag(CPU6502.FLAGS65C02.C));
            //Console.WriteLine(" PC   AR XR YR SP");
            Console.Write(string.Format("  {0:X4}", cpu.PC));
            Console.Write(string.Format(" {0:X2}", cpu.A));
            Console.Write(string.Format(" {0:X2}", cpu.X));
            Console.Write(string.Format(" {0:X2}", cpu.Y));
            Console.WriteLine(string.Format(" {0:X2}", cpu.SP));
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Emulator 65C02");
            IBus bus = new Bus(64);//create bus with 64kb of RAM
            ushort base_addr = 0x8000;
            /*
              *=$8000
              LDX #10
              STX $0000
              LDX #3
              STX $0001
              LDY $0000
              LDA #0
              CLC
              loop
              ADC $0001
              DEY
              BNE loop
              STA $0002
              NOP
              NOP
              NOP
           */
            //feed ram with program
            byte[] prg;
            //prg = new byte[]
            //    {
            //        0xA2, 0x0A, 0x8E, 0x00, 0x00, 0xA2, 0x03, 0x8E,
            //        0x01, 0x00, 0xAC, 0x00, 0x00, 0xA9, 0x00, 0x18,
            //        0x6D, 0x01, 0x00, 0x88, 0xD0, 0xFA, 0x8D, 0x02,
            //        0x00, 0xEA, 0xEA, 0xEA
            //    };

            /*
                *=$8000
                LDA #80
                STA $d402
                LDA #00
                STA $d403
                NOP
                BRK
             */
            prg = new byte[]
            {
                0xA9, 0x00, 0x8D, 0x02, 0xD4, 0xA9,
                0x80, 0x8D, 0x03, 0xD4, 0xEA, 0x00
            };

            for (int i = 0; i < prg.Length; i++)
            {
                bus.Write((ushort)(base_addr + i), prg[i]);
            }

            //set reset handler
            bus.Write(0xFFFC, 0x00);
            bus.Write(0xFFFD, 0x80);

            ANTIC antic = new ANTIC();
            antic.ConnectBus(ref bus);
            
            CPU6502 cpu = new CPU6502();
            cpu.ConnectBus(ref bus);
            cpu.Reset();

            Console.WriteLine("Press key [Space - step, R - Reset, I - Irq, N - NMI Irq]");
            while (true)
            {
                ConsoleKey key = Console.ReadKey().Key;
                if (key == ConsoleKey.Escape) break;

                switch (key)
                {
                    case ConsoleKey.R: cpu.Reset(); break;
                    case ConsoleKey.I: cpu.Irq(); break;
                    case ConsoleKey.N: cpu.Nmi(); break;
                    case ConsoleKey.Spacebar:
                        ExecuteStep(ref cpu);
                        PrintFullState(ref cpu);
                        break;
                }
            }

            antic.Tick();
        }

        static void ExecuteStep(ref CPU6502 cpu)
        {
            do
            {
                cpu.Clock();
            }
            while (!cpu.Complete());
        }
        static void PrintFullState(ref CPU6502 cpu)
        {
            PrintCPUState(ref cpu);
            foreach (string str in cpu.Disassemble(cpu.PC, cpu.PC).Values)
            {
                Console.WriteLine(str);
            }
        }
    }
}
