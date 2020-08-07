# Raspberry-BB8

One developer's adventure into building a BB-8 replica, with help from his wife and friends.

## Equipment

I'm using:

* Raspberry Pi 3 model B

    ![GPIO out for Raspberry Pi](./schematics/pi3modelB.png)

* OSEPP Motor and Servo Shield v1.0

    ![OSEPP Motor and Servo Shield v1.0 Schematic](./schematics/OSEPP_motor_shield_v1-0.svg)

## Building

Package the BB8 project in this repository using `dotnet`.

    dotnet publish

Copy the "Windows" assemblies from `\src\BB8\bin\Debug\net46\win7-x64` via SFTP to the Pi.

Run the project via SUDO:

    sudo BB8.exe

