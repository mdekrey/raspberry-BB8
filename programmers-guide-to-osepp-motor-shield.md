# A Programmer's Guide to OSEPP Motor & Servo Sheild V1.0

The OSEPP Motor & Servo Sheild V1.0 is designed for Arduino users and electrical
engineers; I am neither of those things, but I'm going through decyphering what
little information I can find on the internet and compiling it into a single document.

Here's my board for reference, in case it changes in the future. Sorry for the
extra wires. I really don't want to lose my place!

-- TODO - add image --

[Here's the actual schematic.](./schematics/OSEPP_motor_shield_v1-0.svg)

## Terms

If you're not used to digital programming, here's some terms with which you may
not be familiar. I'm not going to straight up define them; I'd be wrong to some
degree. Instead, I'm going to provide tiny anecdotes that will hopefully provide
the information you need.

* Phase Width Modulation (or PWM) - Imagine a volume knob, where you can dial it
  from 0-10 (definitely not 11). If you set the knob to 5, the sound will be
  roughly half the volume constantly. This is known as "amplitude modulation".
  Unfortunately, given friction, resistances in voltage, etc., motors and servos
  do not work this way - giving a motor half the voltage it needs (or amps,
  etc.) wouldn't give half the speed. Instead, with a "5 of 10", we give full
  power for half the time, and half the power for half the time.
* Motor
* Servo
* Pin
* Serial
* Data
* Latch
* Clock
* Digital
* Analogue
* Voltage
* Ground

## Chips

This shield essentially wires together these two chips:

  * a 74x595 chip (specifically the 74HCT595D chip) https://www.nxp.com/documents/data_sheet/74HC_HCT595.pdf
    - This converts a serial data stream into 7 outputs and a carry. In this way, a total of 3 pins can be used to control several outputs. However, this shield
        doesn't provide access to the carry, instead using all 8 outputs to drive the 4 motors.
  * two L293D chips http://www.ti.com/lit/ds/symlink/l293.pdf
    - Each chip drives two motors by amplifying the PWM stream with an enable bit up to the input voltage for the motors.

## Pins

### Digital I/O

The digital I/O pins are along the top of the board, starting in the upper right
at 0, moving to the left for increasing numbers.

0. Unused (Schema is blank)
1. Unused (Schema is blank)
2. Unused (Schema is blank)
3. Phase Width Modulation of Motor 2 (TBD) (Schema is PWM2B)
4. Serial Clock Pin (Schema is DIR_CLK)
5. Phase Width Modulation of Motor 3 (TBD) (Schema is PWM0B)
6. Phase Width Modulation of Motor 4 (TBD) (Schema is PWM0A)
7. Ground, essentially. This is the "output enable" toggle, but negated due to the original chip. (TBD, how do you say this out loud?) (Schema is labelled DIR_EN)
8. Serial Data Pin (Schema is DIR_SER)
9. Phase Width Modulation of Servo 2 (TODO - verify) (Schema is PWM1A)
10. Phase Width Modulation of Servo 1 (TODO - verify) (Schema is PWM1B)
11. Phase Width Modulation of Motor 1 (TBD) (Schema is PWM2A)
12. Serial Latch (Schema is DIR_LATCH)
13. Unused (Schema is blank)
14. (labelled `gnd`) Ground
15. (labelled `ARef`) Unused

#### Serial Data

Serial data repeats 8 bits to enable/disable the motors.

0. M4 forward (hex 0x01)
1. M2 forward (hex 0x02)
2. M1 forward (hex 0x04)
3. M1 backward (hex 0x08)
4. M2 backward (hex 0x10)
5. M3 forward (hex 0x20)
6. M4 backward (hex 0x40)
7. M3 backward (hex 0x80)

Note here that the board is labelled M3 at the bottom, and the schematic said
that 5/7 went to M4. I chose to keep what was printed on my board rather than
what the schematic said.

### Analog In

Pins supported as a pass-through to the Arduino. Unused.

### Other Pins

From right-to-left, separate block of 6 from the "analog in" pins.

* `VIn`: Default low-voltage input for the board (see Jumpers)
* `Gnd`: Ground
* (unlabelled): Ground
* `5v`: 5 volts to use as reference for comparing serial data
* `3v`: Unused, but intended as a pass-through of 3 volts to another shield
* `RST`: Connects to ground when Reset is pushed. (Not applicable in this guide)

### Jumpers

* JP1:
    * When 1/2 are connected, uses VIn for voltage to the servos.
    * When 2/3 are connected, uses SERVO VSrc terminal (bottom left of board) for voltage to the servos.
* JP2:
    * When 1/2 are connected, uses VIn for voltage to the motors.
    * When 2/3 are connected, uses MOTOR VSrc for voltage to the motors.
