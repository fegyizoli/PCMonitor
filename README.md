# PCMonitor

This is a basic 'arduino-based' module which contains:
- Arduino Nano (with Atmega328)
- 4x20 LCD with HD44780 driver IC

## Features

- [x] Communication via UART with the PC
- [x] PWM driven Backlight (can be set value with serial commands)
- [x] Display strings in every row
- [x] Clear entire screen
- [x] Debug mode (messages via UART)

## Hardware description

### LCD Connection:

| LCD pins | Arduino Nano pins | 
|:--------:|:----------------:|
|  1 Vss    | GND                   |
|  2 Vcc    | +5V                   | 
|  3 V0      |                         | 
|  4 RS      | D12                   |
|  5 RW     | GND                   | 
|  6 E       | D11                    |
|  7 DB0    | Not Connected     |
|  8 DB1    | Not Connected     |
|  9 DB2    | Not Connected     |
| 10 DB3    | Not Connected     |
| 11 DB4    | D5                     |
| 12 DB5    | D4                     |
| 13 DB6    | D3                     |
| 14 DB7    | D2                     |
| 15 LED+   | D6                     |
| 16 LED-   | GND                   |


###Potentiometer connection:

     Vcc
     _|_
    |   |
    |   |<--- V0
    |_ _|
      |
    GND