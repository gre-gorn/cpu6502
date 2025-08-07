#ifndef CPU6502_H
#define CPU6502_H

#include <stdint.h>
#include <stdbool.h>

// Flagi statusu
#define FLAG_C 0x01
#define FLAG_Z 0x02
#define FLAG_I 0x04
#define FLAG_D 0x08
#define FLAG_B 0x10
#define FLAG_U 0x20
#define FLAG_V 0x40
#define FLAG_N 0x80

// Struktura CPU
typedef struct {
    uint8_t  A, X, Y;
    uint16_t PC;
    uint8_t  SP;
    uint8_t  status;
} CPU6502;

// Interfejs busa
void bus_init();
void bus_write(uint16_t address, uint8_t data);
uint8_t bus_read(uint16_t address);

// API
void cpu_init(CPU6502 *cpu);
void cpu_reset(CPU6502 *cpu);
void cpu_execute(CPU6502 *cpu);

// Flagi
uint8_t cpu_get_flag(CPU6502 *cpu, uint8_t flag);
void cpu_set_flag(CPU6502 *cpu, uint8_t flag, int value);

#endif // CPU6502_H
