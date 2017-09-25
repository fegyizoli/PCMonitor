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
- [ ] Basic custom character handling (only 8 chars)
- [ ] Advanced custom character handling (any chars)
- [ ] Feedback to host
- [ ] Host application to PC (basic C# application with openhardwaremonitor.dll)
- [ ] connect 4x4 pinpad
- [ ] basic menu feature
- [ ] fan control with pinpad

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


### Potentiometer connection:

     Vcc
     _|_
    |   |
    |   |<--- V0
    |_ _|
      |
     GND
     
## Software features

### Arduino firmware

To communicate with the device the host shall connect to it.
The connection shall be renewed with a command or the conne



| Command code (Chr) | Hex     | Description                    |
| :-------------------------- |:-------:|:----------------------------- |
|  +                                | 0x2B  | Connect or ping device  |
|  -                                 | 0x2D  | Disconnect                     |
|  B                                | 0x42  | Set backlight (0-255)     |
|  C                                | 0x43  | Clear screen                  |
|  [                                 | 0x5B  | Command begin            |
|  ]                                 | 0x5D  | Command end               |
|  a                                | 0x61  | Write line 0                    |
|  b                                | 0x62  | Write line 1                    |
|  c                                 | 0x63  | Write line 2                    |
|  d                                | 0x64  | Write line 3                    |

Command format:
"["<command-code><attribute0><attribute1>...<attributeN>"]"

Examples:

- Write "Hello world!" to the first line: 
> CHR: +[aHello world!] 
> HEX: {0x2B, 0x5B, 0x61, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x77, 0x6F, 0x72, 0x6C, 0x64, 0x21, 0x5D}

- Write "Hello world!" to the second line: 
> CHR:[bHello world!]
> HEX: {0x5B, 0x62, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x77, 0x6F, 0x72, 0x6C, 0x64, 0x21, 0x5D}

- Clear screen (interpreter does not care with the attribute): 
> CHR: [C0]
> HEX: {0x5B, 0x43, 0x30, 0x5D}

- Set backlight to intensity 50: 
> CHR: [B2]
> HEX: {0x5B, 0x42, 0x32, 0x5D}



