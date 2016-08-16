# Raspberry-BB8

One developer's adventure into building a BB-8 replica, with help from his wife and friends.

## Preparing the BB8

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

