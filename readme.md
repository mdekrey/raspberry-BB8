# Raspberry-BB8

One developer's adventure into building a BB-8 replica, with help from his wife and friends.

## Equipment

I'm using:

* Raspberry Pi 3 model B

    ![GPIO out for Raspberry Pi](http://i.stack.imgur.com/yWGmW.png)

* OSEPP Motor and Servo Shield v1.0 http://osepp.com/wp-content/uploads/2013/07/OSEPP_motor_shield_v1-0.pdf, which has on it
  * a 74x595 chip https://www.nxp.com/documents/data_sheet/74HC_HCT595.pdf
  * two L293D chips http://www.ti.com/lit/ds/symlink/l293.pdf

## Preparing the Raspberry Pi

First, a few things need to be installed on the Pi. This is assuming Raspbian.

    sudo apt-get update
    sudo apt-get upgrade
    sudo apt-get install mono-complete
    sudo apt-get install libmono-system-core4.0-cil

Package the BB8 project in this repository using `dotnet`. This has been tested from Windows.

    dotnet publish

Copy the "Windows" assemblies from `\src\BB8\bin\Debug\net46\win7-x64` via SFTP to the Pi.

Run the project via SUDO:

    sudo mono BB8.exe

