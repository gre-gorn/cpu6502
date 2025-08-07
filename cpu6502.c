#include "cpu6502.h"
#include <stddef.h>

void cpu6502_init(cpu6502_t *cpu, cpu6502_bus_t bus) {
    if (!cpu) return;
    cpu->A = 0;
    cpu->X = 0;
    cpu->Y = 0;
    cpu->PC = 0;
    cpu->SP = 0xFD;
    cpu->SR = 0x00 | FLAG_U;
    cpu->fetched = 0;
    cpu->temp = 0;
    cpu->addr_abs = 0;
    cpu->addr_rel = 0;
    cpu->opcode = 0;
    cpu->cycles = 0;
    cpu->clock_count = 0;
    cpu->bus = bus;
}

void cpu6502_reset(cpu6502_t *cpu) {
    if (!cpu) return;
    // Read reset vector (0xFFFC & 0xFFFD)
    uint16_t lo = cpu->bus.read(cpu->bus.ctx, 0xFFFC);
    uint16_t hi = cpu->bus.read(cpu->bus.ctx, 0xFFFD);
    cpu->PC = (hi << 8) | lo;
    cpu->A = 0;
    cpu->X = 0;
    cpu->Y = 0;
    cpu->SP = 0xFD;
    cpu->SR = 0x00 | FLAG_U;
    cpu->addr_abs = 0;
    cpu->addr_rel = 0;
    cpu->fetched = 0;
    cpu->cycles = 8;
}

void cpu6502_irq(cpu6502_t *cpu) {
    if (!cpu) return;
    if (!(cpu->SR & FLAG_I)) {
        cpu->bus.write(cpu->bus.ctx, 0x0100 + cpu->SP, (cpu->PC >> 8) & 0xFF); cpu->SP--;
        cpu->bus.write(cpu->bus.ctx, 0x0100 + cpu->SP, cpu->PC & 0xFF);        cpu->SP--;
        cpu->cpu->bus.write(cpu->bus.ctx, 0x0100 + cpu->SP, cpu->SR & ~FLAG_B & ~FLAG_U); cpu->SP--;
        cpu->SR |= FLAG_I;
        uint16_t lo = cpu->bus.read(cpu->bus.ctx, 0xFFFE);
        uint16_t hi = cpu->bus.read(cpu->bus.ctx, 0xFFFF);
        cpu->PC = (hi << 8) | lo;
        cpu->cycles = 7;
    }
}

void cpu6502_nmi(cpu6502_t *cpu) {
    if (!cpu) return;
    cpu->bus.write(cpu->bus.ctx, 0x0100 + cpu->SP, (cpu->PC >> 8) & 0xFF); cpu->SP--;
    cpu->bus.write(cpu->bus.ctx, 0x0100 + cpu->SP, cpu->PC & 0xFF);        cpu->SP--;
    cpu->bus.write(cpu->bus.ctx, 0x0100 + cpu->SP, cpu->SR & ~FLAG_B & ~FLAG_U); cpu->SP--;
    cpu->SR |= FLAG_I;
    uint16_t lo = cpu->bus.read(cpu->bus.ctx, 0xFFFA);
    uint16_t hi = cpu->bus.read(cpu->bus.ctx, 0xFFFB);
    cpu->PC = (hi << 8) | lo;
    cpu->cycles = 8;
}

void cpu6502_clock(cpu6502_t *cpu) {
    if (!cpu) return;
    if (cpu->cycles == 0) {
        // TODO: Fetch & execute opcode
    }
    cpu->clock_count++;
    if (cpu->cycles > 0)
        cpu->cycles--;
}

bool cpu6502_complete(const cpu6502_t *cpu) {
    return cpu && cpu->cycles == 0;
}

uint8_t cpu6502_get_flag(const cpu6502_t *cpu, cpu6502_flags_t flag) {
    return (cpu->SR & flag) ? 1 : 0;
}

void cpu6502_set_flag(cpu6502_t *cpu, cpu6502_flags_t flag, bool v) {
    if (v)
        cpu->SR |= flag;
    else
        cpu->SR &= ~flag;
}
