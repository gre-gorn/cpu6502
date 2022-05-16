using Emulators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emulators
{
	public class CPU6502
	{
		// CPU Core register
		// This is all the 6502 has.
		public byte A = 0x00;// Accumulator Register
		public byte X = 0x00;// X Register
		public byte Y = 0x00;// Y Register
		public byte SP = 0x00;// Stack Pointer (points to location on bus)
		public ushort PC = 0x0000;// Program Counter
		public byte SR = 0x00;// Status Register

		//An emulation of the 65C02
		public CPU6502()
		{			
			lookup = new Instruction[]
			{
			new("BRK", BRK, IMM, 7), new("ORA", ORA, IZX, 6), new("???", XXX, IMP, 2), new("???", XXX, IMP, 8), new("???", NOP, IMP, 3), new("ORA", ORA, ZP0, 3), new("ASL", ASL, ZP0, 5), new("???", XXX, IMP, 5), new("PHP", PHP, IMP, 3), new("ORA", ORA, IMM, 2), new("ASL", ASL, IMP, 2), new("???", XXX, IMP, 2), new("???", NOP, IMP, 4), new("ORA", ORA, ABS, 4), new("ASL", ASL, ABS, 6), new("???", XXX, IMP, 6),
			new("BPL", BPL, REL, 2), new("ORA", ORA, IZY, 5), new("???", XXX, IMP, 2), new("???", XXX, IMP, 8), new("???", NOP, IMP, 4), new("ORA", ORA, ZPX, 4), new("ASL", ASL, ZPX, 6), new("???", XXX, IMP, 6), new("CLC", CLC, IMP, 2), new("ORA", ORA, ABY, 4), new("???", NOP, IMP, 2), new("???", XXX, IMP, 7), new("???", NOP, IMP, 4), new("ORA", ORA, ABX, 4), new("ASL", ASL, ABX, 7), new("???", XXX, IMP, 7),
			new("JSR", JSR, ABS, 6), new("AND", AND, IZX, 6), new("???", XXX, IMP, 2), new("???", XXX, IMP, 8), new("BIT", BIT, ZP0, 3), new("AND", AND, ZP0, 3), new("ROL", ROL, ZP0, 5), new("???", XXX, IMP, 5), new("PLP", PLP, IMP, 4), new("AND", AND, IMM, 2), new("ROL", ROL, IMP, 2), new("???", XXX, IMP, 2), new("BIT", BIT, ABS, 4), new("AND", AND, ABS, 4), new("ROL", ROL, ABS, 6), new("???", XXX, IMP, 6),
			new("BMI", BMI, REL, 2), new("AND", AND, IZY, 5), new("???", XXX, IMP, 2), new("???", XXX, IMP, 8), new("???", NOP, IMP, 4), new("AND", AND, ZPX, 4), new("ROL", ROL, ZPX, 6), new("???", XXX, IMP, 6), new("SEC", SEC, IMP, 2), new("AND", AND, ABY, 4), new("???", NOP, IMP, 2), new("???", XXX, IMP, 7), new("???", NOP, IMP, 4), new("AND", AND, ABX, 4), new("ROL", ROL, ABX, 7), new("???", XXX, IMP, 7),
			new("RTI", RTI, IMP, 6), new("EOR", EOR, IZX, 6), new("???", XXX, IMP, 2), new("???", XXX, IMP, 8), new("???", NOP, IMP, 3), new("EOR", EOR, ZP0, 3), new("LSR", LSR, ZP0, 5), new("???", XXX, IMP, 5), new("PHA", PHA, IMP, 3), new("EOR", EOR, IMM, 2), new("LSR", LSR, IMP, 2), new("???", XXX, IMP, 2), new("JMP", JMP, ABS, 3), new("EOR", EOR, ABS, 4), new("LSR", LSR, ABS, 6), new("???", XXX, IMP, 6),
			new("BVC", BVC, REL, 2), new("EOR", EOR, IZY, 5), new("???", XXX, IMP, 2), new("???", XXX, IMP, 8), new("???", NOP, IMP, 4), new("EOR", EOR, ZPX, 4), new("LSR", LSR, ZPX, 6), new("???", XXX, IMP, 6), new("CLI", CLI, IMP, 2), new("EOR", EOR, ABY, 4), new("???", NOP, IMP, 2), new("???", XXX, IMP, 7), new("???", NOP, IMP, 4), new("EOR", EOR, ABX, 4), new("LSR", LSR, ABX, 7), new("???", XXX, IMP, 7),
			new("RTS", RTS, IMP, 6), new("ADC", ADC, IZX, 6), new("???", XXX, IMP, 2), new("???", XXX, IMP, 8), new("???", NOP, IMP, 3), new("ADC", ADC, ZP0, 3), new("ROR", ROR, ZP0, 5), new("???", XXX, IMP, 5), new("PLA", PLA, IMP, 4), new("ADC", ADC, IMM, 2), new("ROR", ROR, IMP, 2), new("???", XXX, IMP, 2), new("JMP", JMP, IND, 5), new("ADC", ADC, ABS, 4), new("ROR", ROR, ABS, 6), new("???", XXX, IMP, 6),
			new("BVS", BVS, REL, 2), new("ADC", ADC, IZY, 5), new("???", XXX, IMP, 2), new("???", XXX, IMP, 8), new("???", NOP, IMP, 4), new("ADC", ADC, ZPX, 4), new("ROR", ROR, ZPX, 6), new("???", XXX, IMP, 6), new("SEI", SEI, IMP, 2), new("ADC", ADC, ABY, 4), new("???", NOP, IMP, 2), new("???", XXX, IMP, 7), new("???", NOP, IMP, 4), new("ADC", ADC, ABX, 4), new("ROR", ROR, ABX, 7), new("???", XXX, IMP, 7),
			new("???", NOP, IMP, 2), new("STA", STA, IZX, 6), new("???", NOP, IMP, 2), new("???", XXX, IMP, 6), new("STY", STY, ZP0, 3), new("STA", STA, ZP0, 3), new("STX", STX, ZP0, 3), new("???", XXX, IMP, 3), new("DEY", DEY, IMP, 2), new("???", NOP, IMP, 2), new("TXA", TXA, IMP, 2), new("???", XXX, IMP, 2), new("STY", STY, ABS, 4), new("STA", STA, ABS, 4), new("STX", STX, ABS, 4), new("???", XXX, IMP, 4),
			new("BCC", BCC, REL, 2), new("STA", STA, IZY, 6), new("???", XXX, IMP, 2), new("???", XXX, IMP, 6), new("STY", STY, ZPX, 4), new("STA", STA, ZPX, 4), new("STX", STX, ZPY, 4), new("???", XXX, IMP, 4), new("TYA", TYA, IMP, 2), new("STA", STA, ABY, 5), new("TXS", TXS, IMP, 2), new("???", XXX, IMP, 5), new("???", NOP, IMP, 5), new("STA", STA, ABX, 5), new("???", XXX, IMP, 5), new("???", XXX, IMP, 5),
			new("LDY", LDY, IMM, 2), new("LDA", LDA, IZX, 6), new("LDX", LDX, IMM, 2), new("???", XXX, IMP, 6), new("LDY", LDY, ZP0, 3), new("LDA", LDA, ZP0, 3), new("LDX", LDX, ZP0, 3), new("???", XXX, IMP, 3), new("TAY", TAY, IMP, 2), new("LDA", LDA, IMM, 2), new("TAX", TAX, IMP, 2), new("???", XXX, IMP, 2), new("LDY", LDY, ABS, 4), new("LDA", LDA, ABS, 4), new("LDX", LDX, ABS, 4), new("???", XXX, IMP, 4),
			new("BCS", BCS, REL, 2), new("LDA", LDA, IZY, 5), new("???", XXX, IMP, 2), new("???", XXX, IMP, 5), new("LDY", LDY, ZPX, 4), new("LDA", LDA, ZPX, 4), new("LDX", LDX, ZPY, 4), new("???", XXX, IMP, 4), new("CLV", CLV, IMP, 2), new("LDA", LDA, ABY, 4), new("TSX", TSX, IMP, 2), new("???", XXX, IMP, 4), new("LDY", LDY, ABX, 4), new("LDA", LDA, ABX, 4), new("LDX", LDX, ABY, 4), new("???", XXX, IMP, 4),
			new("CPY", CPY, IMM, 2), new("CMP", CMP, IZX, 6), new("???", NOP, IMP, 2), new("???", XXX, IMP, 8), new("CPY", CPY, ZP0, 3), new("CMP", CMP, ZP0, 3), new("DEC", DEC, ZP0, 5), new("???", XXX, IMP, 5), new("INY", INY, IMP, 2), new("CMP", CMP, IMM, 2), new("DEX", DEX, IMP, 2), new("???", XXX, IMP, 2), new("CPY", CPY, ABS, 4), new("CMP", CMP, ABS, 4), new("DEC", DEC, ABS, 6), new("???", XXX, IMP, 6),
			new("BNE", BNE, REL, 2), new("CMP", CMP, IZY, 5), new("???", XXX, IMP, 2), new("???", XXX, IMP, 8), new("???", NOP, IMP, 4), new("CMP", CMP, ZPX, 4), new("DEC", DEC, ZPX, 6), new("???", XXX, IMP, 6), new("CLD", CLD, IMP, 2), new("CMP", CMP, ABY, 4), new("NOP", NOP, IMP, 2), new("???", XXX, IMP, 7), new("???", NOP, IMP, 4), new("CMP", CMP, ABX, 4), new("DEC", DEC, ABX, 7), new("???", XXX, IMP, 7),
			new("CPX", CPX, IMM, 2), new("SBC", SBC, IZX, 6), new("???", NOP, IMP, 2), new("???", XXX, IMP, 8), new("CPX", CPX, ZP0, 3), new("SBC", SBC, ZP0, 3), new("INC", INC, ZP0, 5), new("???", XXX, IMP, 5), new("INX", INX, IMP, 2), new("SBC", SBC, IMM, 2), new("NOP", NOP, IMP, 2), new("???", SBC, IMP, 2), new("CPX", CPX, ABS, 4), new("SBC", SBC, ABS, 4), new("INC", INC, ABS, 6), new("???", XXX, IMP, 6),
			new("BEQ", BEQ, REL, 2), new("SBC", SBC, IZY, 5), new("???", XXX, IMP, 2), new("???", XXX, IMP, 8), new("???", NOP, IMP, 4), new("SBC", SBC, ZPX, 4), new("INC", INC, ZPX, 6), new("???", XXX, IMP, 6), new("SED", SED, IMP, 2), new("SBC", SBC, ABY, 4), new("NOP", NOP, IMP, 2), new("???", XXX, IMP, 7), new("???", NOP, IMP, 4), new("SBC", SBC, ABX, 4), new("INC", INC, ABX, 7), new("???", XXX, IMP, 7),
			};
		}



		//Below methods are represantion of PINs in hardware

		//Reset Interrupt
		// Forces the 6502 into a known state. This is hard-wired inside the CPU. The
		// registers are set to 0x00, the status register is cleared except for unused
		// bit which remains at 1. An absolute address is read from location 0xFFFC
		// which contains a second address that the program counter is set to. This 
		// allows the programmer to jump to a known and programmable location in the
		// memory to start executing from. Typically the programmer would set the value
		// at location 0xFFFC at compile time.
		public void Reset()
		{
			// Get address to set program counter to
			_addr_abs = 0xFFFC;
			ushort lo = _bus.Read((ushort)(_addr_abs + 0x00));
			ushort hi = _bus.Read((ushort)(_addr_abs + 0x01));

			//set program counter
			PC = (ushort)((hi << 8) | lo);

			//reset CPU registers
			A = 0x00;
			X = 0x00;
			Y = 0x00;
			SP = 0xFD;
			SR = (byte)(0x00 | FLAGS65C02.U);

			//clear help
			_addr_rel = 0x0000;
			_addr_abs = 0x0000;
			_fetched = 0x00;

			//reset takes some cycles
			_cycles = 8;//TODO: check how do I know?
		}

		//Interrupt Request - standard interrupt can be disabled setting I flag
		public void Irq()
		{
			// If interrupts are allowed
			if (GetFlag(FLAGS65C02.I) == 0)
			{
				// Push the program counter to the stack. It's 16-bits dont
				// forget so that takes two pushes
				_bus.Write((ushort)(0x0100 + SP), (byte)((PC >> 8) & 0x00FF));
				SP--;
				_bus.Write((ushort)(0x0100 + SP), (byte)(PC & 0x00FF));
				SP--;

				// Then Push the status register to the stack
				SetFlag(FLAGS65C02.B, false);
				SetFlag(FLAGS65C02.U, true);
				SetFlag(FLAGS65C02.I, true);
				_bus.Write((ushort)(0x0100 + SP), SR);
				SP--;

				// Read new program counter location from fixed address
				_addr_abs = 0xFFFE;
				ushort lo = _bus.Read((ushort)(_addr_abs + 0x00));
				ushort hi = _bus.Read((ushort)(_addr_abs + 0x01));
				PC = (ushort)((hi << 8) | lo);

				// IRQs take time
				_cycles = 7;
			}
		}

		//Non-Maskable Interrupt Request - cannot be disabled
		public void Nmi()
		{
			_bus.Write((ushort)(0x0100 + SP), (byte)((PC >> 8) & 0x00FF));
			SP--;
			_bus.Write((ushort)(0x0100 + SP), (byte)(PC & 0x00FF));
			SP--;

			SetFlag(FLAGS65C02.B, false);
			SetFlag(FLAGS65C02.U, true);
			SetFlag(FLAGS65C02.I, true);
			_bus.Write((ushort)(0x0100 + SP), SR);
			SP--;

			_addr_abs = 0xFFFA;
			ushort lo = _bus.Read((ushort)(_addr_abs + 0x00));
			ushort hi = _bus.Read((ushort)(_addr_abs + 0x01));
			PC = (ushort)((hi << 8) | lo);

			_cycles = 8;
		}

		//one clock cycle
		public void Clock()
		{
			if (_cycles == 0)
			{
				_opcode = _bus.Read(PC);

				// Increment program counter, we read the opcode byte
				PC++;

				// Get Starting number of cycles
				_cycles = lookup[_opcode].GetCycles();

				// Perform fetch of intermmediate data using the
				// required addressing mode
				byte additional_cycle1 = lookup[_opcode].GetAddrMode().Invoke();

				// Perform operation
				byte additional_cycle2 = lookup[_opcode].GetOperate().Invoke();

				// The addressmode and opcode may have altered the number
				// of cycles this instruction requires before its completed
				_cycles += (byte)(additional_cycle1 & additional_cycle2);

				//TODO: duplicated code?
				// Always set the unused status flag bit to 1
				SetFlag(FLAGS65C02.U, true);
			}

			_clock_count++;
			_cycles--;
		}

		//check if current CPU insruction has completed execution
		//used for debugging step-by-step
		public bool Complete()
		{
			return _cycles == 0x00;
		}

		private IBus _bus;

		//Link CPU with Bus
		public void ConnectBus(ref IBus n)
		{
			_bus = n;
		}

		//disassemble instruction
		public Dictionary<ushort, string> Disassemble(ushort begin, ushort end)
		{
			ushort addr = begin;
			byte value, lo, hi;
			Dictionary<ushort, string> mapLines = new();
			ushort line_addr;

			while (addr <= end)
			{
				line_addr = addr;

				string sInst = string.Format("${0:X4}: ", addr);
				byte opcode = _bus.Read(addr);
				Instruction ins = lookup[opcode];
				addr++;
				sInst += ins.GetName() + " ";

				AddrMode addrMode = ins.GetAddrMode();
				if (addrMode == IMP)
				{
					sInst += " {IMP}";
				}
				else if (addrMode == IMM)
				{
					value = _bus.Read(addr);
					addr++;
					sInst += string.Format("#${0:X2}", value) + " {IMM}";
				}
				else if (addrMode == ZP0)
				{
					lo = _bus.Read(addr);
					addr++;
					sInst += string.Format("${0:X2}", lo) + " {ZP0}";
				}
				else if (addrMode == ZPX)
				{
					lo = _bus.Read(addr);
					addr++;
					sInst += string.Format("${0:X2}", lo) + ", X {ZPX}";
				}
				else if (addrMode == ZPY)
				{
					lo = _bus.Read(addr);
					addr++;
					sInst += string.Format("${0:X2}", lo) + ", Y {ZPY}";
				}
				else if (addrMode == IZX)
				{
					lo = _bus.Read(addr);
					addr++;
					sInst += string.Format("(${0:X2}", lo) + ", X) {IZX}";
				}
				else if (addrMode == IZY)
				{
					lo = _bus.Read(addr);
					addr++;
					sInst += string.Format("(${0:X2}", lo) + "), Y {IZY}";
				}
				else if (addrMode == ABS)
				{
					lo = _bus.Read(addr);
					addr++;
					hi = _bus.Read(addr);
					addr++;
					sInst += string.Format("${0:X4}", (ushort)(hi << 8) | lo) + " {ABS}";
				}
				else if (addrMode == ABX)
				{
					lo = _bus.Read(addr);
					addr++;
					hi = _bus.Read(addr);
					addr++;
					sInst += string.Format("${0:X4}", (ushort)(hi << 8) | lo) + ", X {ABX}";
				}
				else if (addrMode == ABY)
				{
					lo = _bus.Read(addr);
					addr++;
					hi = _bus.Read(addr);
					addr++;
					sInst += string.Format("${0:X4}", (ushort)(hi << 8) | lo) + ", Y {ABY}";
				}
				else if (addrMode == IND)
				{
					lo = _bus.Read(addr);
					addr++;
					hi = _bus.Read(addr);
					addr++;
					sInst += string.Format("(${0:X4}", (ushort)(hi << 8) | lo) + ") {IND}";
				}
				else if (addrMode == REL)
				{
					ushort v = _bus.Read(addr);
					if ((v & 0x80) == 0x80)
					{
						v |= 0xFF00;
					}
					addr++;
					sInst += string.Format("${0:X2}", (byte)v) + string.Format(" [${0:X4}", (ushort)(addr + v)) + "] {REL}";
				}
				mapLines[line_addr] = sInst;
			}

			return mapLines;
		}

		[Flags]
		//FLAGS
		public enum FLAGS65C02
		{
			N = 1 << 0,   // Negative
			V = 1 << 1,   // Overflow
			U = 1 << 2,   // Unused
			B = 1 << 3,   // Break
			D = 1 << 4,   // Decimal Mode
			I = 1 << 5,   // Disable Interrupts
			Z = 1 << 6,   // Zero
			C = 1 << 7,   // Carry Bit
		};

		//Get/Set status register
		public byte GetFlag(FLAGS65C02 f)
		{
			return (byte)(((SR & (byte)f) > 0) ? 1 : 0);
		}

		// Sets or clears a specific bit of the status register
		void SetFlag(FLAGS65C02 f, bool v)
		{
			if (v)
				SR |= (byte)f;
			else
				SR &= (byte)~f;
		}

		//fetched byte
		private byte _fetched = 0x00;
		private ushort _temp = 0x0000;

		//calculated final memory address
		private ushort _addr_abs = 0x0000;

		//branch address
		private ushort _addr_rel = 0x0000;

		//opcode
		private byte _opcode = 0x00;

		//remaining cycles for the instruction
		private byte _cycles = 0;

		private int _clock_count = 0;

		public delegate byte Operate();
		public delegate byte AddrMode();

		struct Instruction
		{
			private readonly string _name;
			private readonly Operate _operate;
			private readonly AddrMode _addrmode;
			private readonly byte _cycles;

			public Instruction(string name, Operate operate, AddrMode addrmode, byte cycles) : this()
			{
				_name = name;
				_operate = operate;
				_addrmode = addrmode;
				_cycles = cycles;
			}

			public string GetName()
			{
				return _name;
			}

			public Operate GetOperate()
			{
				return _operate;
			}

			public AddrMode GetAddrMode()
			{
				return _addrmode;
			}

			public byte GetCycles()
			{
				return _cycles;
			}
		}

		private readonly Instruction[] lookup;

		// ADDRESSING MODES

		// Simple intruction without data or address fetch
		public byte IMP()
		{
			_fetched = A;

			return 0x00;
		}

		// Immediate
		public byte IMM()
		{
			_addr_abs = PC++;

			return 0x00;
		}

		// Zero Page
		byte ZP0()
		{
			_addr_abs = _bus.Read(PC);
			PC++;
			_addr_abs &= 0x00FF;

			return 0x00;
		}

		// Zero Page with index in X
		byte ZPX()
		{
			_addr_abs = (ushort)(_bus.Read(PC) + X);
			PC++;
			_addr_abs &= 0x00FF;

			return 0x00;
		}

		// Zero Page with index in Y
		byte ZPY()
		{
			_addr_abs = (ushort)(_bus.Read(PC) + Y);
			PC++;
			_addr_abs &= 0x00FF;

			return 0x00;
		}

		// Relative, with 8-bit offset, signed
		byte REL()
		{
			_addr_rel = _bus.Read(PC);
			PC++;
			if ((_addr_rel & 0x80) == 0x80)
			{
				_addr_rel |= 0xFF00;
			}

			return 0x00;
		}

		// Absolute with 16-bit address to fetch
		byte ABS()
		{
			ushort lo = _bus.Read(PC);
			PC++;
			ushort hi = _bus.Read(PC);
			PC++;

			_addr_abs = (ushort)((hi << 8) | lo);

			return 0x00;
		}

		// Absolute with X index
		byte ABX()
		{
			ushort lo = _bus.Read(PC);
			PC++;
			ushort hi = _bus.Read(PC);
			PC++;

			_addr_abs = (ushort)((hi << 8) | lo);
			_addr_abs += X;

			if ((_addr_abs & 0xFF00) != (hi << 8))
				return 0x01;
			else
				return 0x00;
		}

		// Absolute with Y index
		byte ABY()
		{
			ushort lo = _bus.Read(PC);
			PC++;
			ushort hi = _bus.Read(PC);
			PC++;

			_addr_abs = (ushort)((hi << 8) | lo);
			_addr_abs += Y;

			if ((_addr_abs & 0xFF00) != (hi << 8))
				return 0x01;
			else
				return 0x00;
		}

		// Indirect
		byte IND()
		{
			ushort ptr_lo = _bus.Read(PC);
			PC++;
			ushort ptr_hi = _bus.Read(PC);
			PC++;

			ushort ptr = (ushort)((ptr_hi << 8) | ptr_lo);

			if (ptr_lo == 0x00FF) // Simulate page boundary hardware bug
			{
				_addr_abs = (ushort)((_bus.Read((ushort)(ptr & 0xFF00)) << 8) | _bus.Read((ushort)(ptr + 0x00)));
			}
			else // Behave normally
			{
				_addr_abs = (ushort)((_bus.Read((ushort)(ptr + 0x01)) << 8) | _bus.Read((ushort)(ptr + 0x00)));
			}

			return 0x00;
		}

		// Indirect X
		byte IZX()
		{
			ushort t = _bus.Read(PC);
			PC++;

			ushort lo = _bus.Read((ushort)((ushort)(t + X) & 0x00FF));
			ushort hi = _bus.Read((ushort)((ushort)(t + X + 0x01) & 0x00FF));

			_addr_abs = (ushort)((hi << 8) | lo);

			return 0x00;
		}

		// Indirect Y
		byte IZY()
		{
			ushort t = _bus.Read(PC);
			PC++;

			ushort lo = _bus.Read((ushort)(t & 0x00FF));
			ushort hi = _bus.Read((ushort)((t + 0x01) & 0x00FF));

			_addr_abs = (ushort)((hi << 8) | lo);
			_addr_abs += Y;

			if ((_addr_abs & 0xFF00) != (hi << 8))
				return 0x01;
			else
				return 0x00;
		}


		//just fetch byte and return it
		byte Fetch()
		{
			if (!lookup[_opcode].GetAddrMode().Equals(IMP()))
			{
				_fetched = _bus.Read(_addr_abs);
			}

			return _fetched;
		}

		// INSTRUCTION IMPLEMENTATIONS

		// Add with Carry
		byte ADC()
		{
			Fetch();

			_temp = (ushort)(A + _fetched + GetFlag(FLAGS65C02.C));

			SetFlag(FLAGS65C02.C, _temp > 255);

			SetFlag(FLAGS65C02.Z, (_temp & 0x00FF) == 0);

			SetFlag(FLAGS65C02.V, (~(A ^ _fetched) & (A ^ _temp) & 0x0080) == 0x80);

			SetFlag(FLAGS65C02.N, (_temp & 0x80) == 0x80);

			A = (byte)(_temp & 0x00FF);

			return 0x01;
		}

		// Subtraction with Borrow In
		byte SBC()
		{
			Fetch();

			ushort value = (ushort)((_fetched) ^ 0x00FF);

			_temp = (ushort)(A + value + GetFlag(FLAGS65C02.C));

			SetFlag(FLAGS65C02.C, (_temp & 0xFF00) == 1);
			SetFlag(FLAGS65C02.Z, (_temp & 0x00FF) == 0);
			SetFlag(FLAGS65C02.V, ((_temp ^ A) & (_temp ^ value) & 0x0080) == 1);
			SetFlag(FLAGS65C02.N, (_temp & 0x0080) == 1);
			A = (byte)(_temp & 0x00FF);

			return 0x01;
		}

		// Bitwise Logic AND
		byte AND()
		{
			Fetch();
			A = (byte)(A & _fetched);
			SetFlag(FLAGS65C02.Z, A == 0x00);
			SetFlag(FLAGS65C02.N, (A & 0x80) == 0x80);

			return 0x01;
		}

		// Arithmetic Shift Left
		byte ASL()
		{
			Fetch();
			_temp = (ushort)(_fetched << 1);
			SetFlag(FLAGS65C02.C, (_temp & 0xFF00) > 0);
			SetFlag(FLAGS65C02.Z, (_temp & 0x00FF) == 0x00);
			SetFlag(FLAGS65C02.N, (_temp & 0x80) == 0x80);

			if (lookup[_opcode].GetAddrMode().Equals(IMP()))
				A = (byte)(_temp & 0x00FF);
			else
				_bus.Write(_addr_abs, (byte)(_temp & 0x00FF));

			return 0x00;
		}

		// Branch if Carry Clear
		byte BCC()
		{
			if (GetFlag(FLAGS65C02.C) == 0)
			{
				_cycles++;
				_addr_abs = (ushort)(PC + _addr_rel);

				if ((_addr_abs & 0xFF00) != (PC & 0xFF00))
				{
					_cycles++;
				}

				PC = _addr_abs;
			}
			
			return 0x00;
		}

		// Branch if Carry Set
		byte BCS()
		{
			if (GetFlag(FLAGS65C02.C) == 1)
			{
				_cycles++;
				_addr_abs = (ushort)(PC + _addr_rel);

				if ((_addr_abs & 0xFF00) != (PC & 0xFF00))
				{
					_cycles++;
				}

				PC = _addr_abs;
			}
			
			return 0x00;
		}

		// Branch if Equal
		byte BEQ()
		{
			if (GetFlag(FLAGS65C02.Z) == 1)
			{
				_cycles++;
				_addr_abs = (ushort)(PC + _addr_rel);

				if ((_addr_abs & 0xFF00) != (PC & 0xFF00))
				{
					_cycles++;
				}

				PC = _addr_abs;
			}
			
			return 0x00;
		}
		
		byte BIT()
		{
			Fetch();
			_temp = (ushort)(A & _fetched);
			SetFlag(FLAGS65C02.Z, (_temp & 0x00FF) == 0x00);
			SetFlag(FLAGS65C02.N, (_fetched & (1 << 7)) == 0x01);
			SetFlag(FLAGS65C02.V, (_fetched & (1 << 6)) == 0x01);
			
			return 0x00;
		}

		// Branch if Negative
		byte BMI()
		{
			if (GetFlag(FLAGS65C02.N) == 0x01)
			{
				_cycles++;
				_addr_abs = (ushort)(PC + _addr_rel);

				if ((_addr_abs & 0xFF00) != (PC & 0xFF00))
				{
					_cycles++;
				}

				PC = _addr_abs;
			}
			
			return 0x00;
		}

		// Branch if Not Equal
		byte BNE()
		{
			if (GetFlag(FLAGS65C02.Z) == 0)
			{
				_cycles++;
				_addr_abs = (ushort)(PC + _addr_rel);

				if ((_addr_abs & 0xFF00) != (PC & 0xFF00))
				{
					_cycles++;
				}

				PC = _addr_abs;
			}
			
			return 0x00;
		}

		// Branch if Positive
		byte BPL()
		{
			if (GetFlag(FLAGS65C02.N) == 0)
			{
				_cycles++;
				_addr_abs = (ushort)(PC + _addr_rel);

				if ((_addr_abs & 0xFF00) != (PC & 0xFF00))
				{
					_cycles++;
				}

				PC = _addr_abs;
			}
			
			return 0x00;
		}

		// Break
		byte BRK()
		{
			PC++;

			SetFlag(FLAGS65C02.I, true);
			_bus.Write((ushort)(0x0100 + SP), (byte)((PC >> 8) & 0x00FF));
			SP--;
			_bus.Write((ushort)(0x0100 + SP), (byte)(PC & 0x00FF));
			SP--;

			SetFlag(FLAGS65C02.B, true);
			_bus.Write((ushort)(0x0100 + SP), SR);
			SP--;
			SetFlag(FLAGS65C02.B, false);

			PC = (ushort)(_bus.Read(0xFFFE) | (_bus.Read(0xFFFF) << 8));
		
			return 0x00;
		}

		// Branch if Overflow Clear
		byte BVC()
		{
			if (GetFlag(FLAGS65C02.V) == 0)
			{
				_cycles++;
				_addr_abs = (ushort)(PC + _addr_rel);

				if ((_addr_abs & 0xFF00) != (PC & 0xFF00))
				{
					_cycles++;
				}

				PC = _addr_abs;
			}
			
			return 0x00;
		}

		// Branch if Overflow Set
		byte BVS()
		{
			if (GetFlag(FLAGS65C02.V) == 1)
			{
				_cycles++;
				_addr_abs = (ushort)(PC + _addr_rel);

				if ((_addr_abs & 0xFF00) != (PC & 0xFF00))
				{
					_cycles++;
				}

				PC = _addr_abs;
			}
			
			return 0x00;
		}

		// Clear Carry Flag
		byte CLC()
		{
			SetFlag(FLAGS65C02.C, false);
			
			return 0x00;
		}

		// Clear Decimal Flag
		byte CLD()
		{
			SetFlag(FLAGS65C02.D, false);
			
			return 0x00;
		}

		// Disable Interrupts / Clear Interrupt Flag
		byte CLI()
		{
			SetFlag(FLAGS65C02.I, false);
            
			return 0x00;
        }

		// Clear Overflow Flag
		byte CLV()
		{
			SetFlag(FLAGS65C02.V, false);
			
			return 0x00;
		}

		// Compare Accumulator
		byte CMP()
		{
			Fetch();
			_temp = (ushort)(A - _fetched);
			
			SetFlag(FLAGS65C02.C, A >= _fetched);
			SetFlag(FLAGS65C02.Z, (_temp & 0x00FF) == 0x0000);
			SetFlag(FLAGS65C02.N, (_temp & 0x0080) == 0x0001);
			
			return 0x01;
		}

		// Compare X Register
		byte CPX()
		{
			Fetch();
			_temp = (ushort)(X - _fetched);
			
			SetFlag(FLAGS65C02.C, X >= _fetched);
			SetFlag(FLAGS65C02.Z, (_temp & 0x00FF) == 0x0000);
			SetFlag(FLAGS65C02.N, (_temp & 0x0080) == 0x0001);
			
			return 0x00;
		}

		// Compare Y Register
		byte CPY()
		{
			Fetch();
			_temp = (ushort)(Y - _fetched);
			
			SetFlag(FLAGS65C02.C, Y >= _fetched);
			SetFlag(FLAGS65C02.Z, (_temp & 0x00FF) == 0x0000);
			SetFlag(FLAGS65C02.N, (_temp & 0x0080) == 0x0001);
			
			return 0x00;
		}

		// Decrement Value at Memory Location
		byte DEC()
		{
			Fetch();
			_temp = (ushort)(_fetched - 1);
			
			_bus.Write(_addr_abs, (byte)(_temp & 0x00FF));
			SetFlag(FLAGS65C02.Z, (_temp & 0x00FF) == 0x0000);
			SetFlag(FLAGS65C02.N, (_temp & 0x0080) == 0x0001);
			
			return 0x00;
		}

		// Decrement X Register
		byte DEX()
		{
			X--;
			
			SetFlag(FLAGS65C02.Z, X == 0x00);
			SetFlag(FLAGS65C02.N, (X & 0x80) == 0x80);
			
			return 0x00;
		}

		// Decrement Y Register
		byte DEY()
		{
			Y--;
			
			SetFlag(FLAGS65C02.Z, Y == 0x00);
			SetFlag(FLAGS65C02.N, (Y & 0x80) == 0x80);
			
			return 0x00;
		}

		// Bitwise Logic XOR
		byte EOR()
		{
			Fetch();
			A = (byte)(A ^ _fetched);
			
			SetFlag(FLAGS65C02.Z, A == 0x00);
			SetFlag(FLAGS65C02.N, (A & 0x80) == 0x80);
			
			return 0x01;
		}

		// Increment Value at Memory Location
		byte INC()
		{
			Fetch();
			_temp = (ushort)(_fetched + 1);
			_bus.Write(_addr_abs, (byte)(_temp & 0x00FF));
			SetFlag(FLAGS65C02.Z, (_temp & 0x00FF) == 0x0000);
			SetFlag(FLAGS65C02.N, (_temp & 0x0080) == 0x0001);
			
			return 0x00;
		}

		// Increment X Register
		byte INX()
		{
			X++;
			SetFlag(FLAGS65C02.Z, X == 0x00);
			SetFlag(FLAGS65C02.N, (X & 0x80) == 0x80);
			
			return 0x00;
		}

		// Increment Y Register
		byte INY()
		{
			Y++;
			
			SetFlag(FLAGS65C02.Z, Y == 0x00);
			SetFlag(FLAGS65C02.N, (Y & 0x80) == 0x80);
			
			return 0x00;
		}

		// Jump To Location
		byte JMP()
		{
			PC = _addr_abs;
			
			return 0x00;
		}

		// Jump To Sub-Routine
		byte JSR()
		{
			PC--;

			_bus.Write((ushort)(0x0100 + SP), (byte)((PC >> 8) & 0x00FF));
			SP--;
			_bus.Write((ushort)(0x0100 + SP), (byte)(PC & 0x00FF));
			SP--;

			PC = _addr_abs;
			
			return 0x00;
		}

		// Load The Accumulator
		byte LDA()
		{
			Fetch();
			A = _fetched;
			SetFlag(FLAGS65C02.Z, A == 0x00);
			SetFlag(FLAGS65C02.N, (A & 0x80) == 0x80);
			
			return 0x01;
		}

		// Load The X Register
		byte LDX()
		{
			Fetch();
			X = _fetched;
			SetFlag(FLAGS65C02.Z, X == 0x00);
			SetFlag(FLAGS65C02.N, (X & 0x80) == 0x80);
			
			return 0x01;
		}

		// Load The Y Register
		byte LDY()
		{
			Fetch();
			Y = _fetched;
			SetFlag(FLAGS65C02.Z, Y == 0x00);
			SetFlag(FLAGS65C02.N, (Y & 0x80) == 0x80);
			
			return 0x01;
		}
		
		byte LSR()
		{
			Fetch();
			SetFlag(FLAGS65C02.C, (_fetched & 0x0001) == 0x0001);
			_temp = (ushort)(_fetched >> 1);
			
			SetFlag(FLAGS65C02.Z, (_temp & 0x00FF) == 0x0000);
			SetFlag(FLAGS65C02.N, (_temp & 0x0080) == 0x0001);

			if (lookup[_opcode].GetAddrMode().Equals(IMP()))
				A = (byte)(_temp & 0x00FF);
			else
				_bus.Write(_addr_abs, (byte)(_temp & 0x00FF));
		
			return 0x00;
		}

		// Not all NOPs are equal
		// based on https://wiki.nesdev.com/w/index.php/CPU_unofficial_opcodes
		byte NOP()
		{
			switch (_opcode)
			{
				case 0x1C:
				case 0x3C:
				case 0x5C:
				case 0x7C:
				case 0xDC:
				case 0xFC:
					return 0x01;
			}
			
			return 0x00;
		}

		// Bitwise Logic OR
		byte ORA()
		{
			Fetch();
			A = (byte)(A | _fetched);
			
			SetFlag(FLAGS65C02.Z, A == 0x00);
			SetFlag(FLAGS65C02.N, (A & 0x80) == 0x80);
			
			return 0x01;
		}

		// Push Accumulator to Stack
		byte PHA()
		{
			_bus.Write((ushort)(0x0100 + SP), A);
			SP--;
			
			return 0x00;
		}

		// Push Status Register to Stack
		byte PHP()
		{
			_bus.Write((ushort)(0x0100 + SP), (byte)(SR | (byte)FLAGS65C02.B | (byte)FLAGS65C02.U));
			SetFlag(FLAGS65C02.B, false);
			SetFlag(FLAGS65C02.U, false);
			SP--;
			
			return 0x00;
		}

		// Pop Accumulator off Stack
		byte PLA()
		{
			SP++;
			A = _bus.Read((ushort)(0x0100 + SP));
			
			SetFlag(FLAGS65C02.Z, A == 0x00);
			SetFlag(FLAGS65C02.N, (A & 0x80) == 0x80);
			
			return 0x00;
		}

		// Pop Status Register off Stack
		byte PLP()
		{
			SP++;
			SR = _bus.Read((ushort)(0x0100 + SP));
			
			SetFlag(FLAGS65C02.U, true);
			
			return 0x00;
		}

		byte ROL()
		{
			Fetch();
			_temp = (ushort)((_fetched << 1) | GetFlag(FLAGS65C02.C));
			
			SetFlag(FLAGS65C02.C, (_temp & 0xFF00) == 0x01);
			SetFlag(FLAGS65C02.Z, (_temp & 0x00FF) == 0x0000);
			SetFlag(FLAGS65C02.N, (_temp & 0x0080) == 0x0001);
			
			if (lookup[_opcode].GetAddrMode().Equals(IMP()))
				A = (byte)(_temp & 0x00FF);
			else
				_bus.Write(_addr_abs, (byte)(_temp & 0x00FF));
			return 0;
		}
		
		byte ROR()
		{
			Fetch();
			_temp = (ushort)((GetFlag(FLAGS65C02.C) << 7) | (_fetched >> 1));
			SetFlag(FLAGS65C02.C, (_fetched & 0x01) == 0x01);
			SetFlag(FLAGS65C02.Z, (_temp & 0x00FF) == 0x00);
			SetFlag(FLAGS65C02.N, (_temp & 0x0080) == 0x01);

			if (lookup[_opcode].GetAddrMode().Equals(IMP()))
				A = (byte)(_temp & 0x00FF);
			else
				_bus.Write(_addr_abs, (byte)(_temp & 0x00FF));
			return 0;
		}
		
		//return from irq request
		byte RTI()
		{
			SP++;
			SR = _bus.Read((ushort)(0x0100 + SP));
			byte f = (byte)FLAGS65C02.B;
			SR &= (byte)~f;
			f = (byte)FLAGS65C02.U;
			SR &= (byte)~f;

			SP++;
			PC = _bus.Read((ushort)(0x0100 + SP));
			PC++;
			PC |= (byte)(_bus.Read((ushort)(0x0100 + SP)) << 8);
			
			return 0x00;
		}

		byte RTS()
		{
			SP++;
			PC = _bus.Read((ushort)(0x0100 + SP));
			
			SP++;
			PC |= (ushort)(_bus.Read((ushort)(0x0100 + SP)) << 8);

			PC++;
			
			return 0x00;
		}

		// Set Carry Flag
		byte SEC()
		{
			SetFlag(FLAGS65C02.C, true);
			
			return 0x00;
		}

		// Set Decimal Flag
		byte SED()
		{
			SetFlag(FLAGS65C02.D, true);
		
			return 0x00;
		}

		// Set Interrupt Flag / Enable Interrupts
		byte SEI()
		{
			SetFlag(FLAGS65C02.I, true);

			return 0x00;
		}

		// Store Accumulator at Address
		byte STA()
		{
			_bus.Write(_addr_abs, A);
			return 0x00;
		}

		// Store X Register at Address
		byte STX()
		{
			_bus.Write(_addr_abs, X);
			return 0x00;
		}

		// Store Y Register at Address
		byte STY()
		{
			_bus.Write(_addr_abs, Y);
			return 0x00;
		}

		// Transfer Accumulator to X Register
		byte TAX()
		{
			X = A;

			SetFlag(FLAGS65C02.Z, X == 0x00);
			SetFlag(FLAGS65C02.N, (X & 0x80) == 0x80);

			return 0x00;
		}

		// Transfer Accumulator to Y Register
		byte TAY()
		{
			Y = A;

			SetFlag(FLAGS65C02.Z, Y == 0x00);
			SetFlag(FLAGS65C02.N, (Y & 0x80) == 0x80);

			return 0x00;
		}

		// Transfer Stack Pointer to X Register
		byte TSX()
		{
			X = SP;

			SetFlag(FLAGS65C02.Z, X == 0x00);
			SetFlag(FLAGS65C02.N, (X & 0x80) == 0x80);

			return 0x00;
		}

		// Transfer X Register to Accumulator
		byte TXA()
		{
			A = X;

			SetFlag(FLAGS65C02.Z, A == 0x00);
			SetFlag(FLAGS65C02.N, (A & 0x80) == 0x80);

			return 0x00;
		}

		// Transfer X Register to Stack Pointer
		byte TXS()
		{
			SP = X;

			return 0x00;
		}

		// Transfer Y Register to Accumulator
		byte TYA()
		{
			A = Y;

			SetFlag(FLAGS65C02.Z, A == 0x00);
			SetFlag(FLAGS65C02.N, (A & 0x80) == 0x80);

			return 0x00;
		}

		// Capture all other opcodes and ignore them
		byte XXX() { return 0x00; }
    }

}
