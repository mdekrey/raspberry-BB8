using BB8;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

var localConfig = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "bb8.json");
var config = new ConfigurationBuilder()
    .AddJsonFile("config.json")
    .AddJsonFile(localConfig, optional: true)
    .Build();
var motorConfig = config.GetSection("motors").Get<MotorConfiguration>();

Console.WriteLine(localConfig);
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(motorConfig));

var originalColor = Console.ForegroundColor;
            
Pi.Init<BootstrapWiringPi>();

var pwmOutput = Pi.Gpio[motorConfig.GpioPwmMotor].ToPwmPin();
var serialDataPin = Pi.Gpio[motorConfig.Serial.GpioData];
var serialLatchPin = Pi.Gpio[motorConfig.Serial.GpioLatch];
var serialClockPin = Pi.Gpio[motorConfig.Serial.GpioClock];

foreach (var pin in Pi.Gpio)
{
    Console.WriteLine($"{pin.BcmPin}: {((GpioPin)pin).Capabilities}");
}
serialDataPin.PinMode = GpioPinDriveMode.Output;
serialLatchPin.PinMode = GpioPinDriveMode.Output;
serialClockPin.PinMode = GpioPinDriveMode.Output;

var sw = new Stopwatch();
sw.Start();
try
{
    var serial = new SerialDigitizer(data: serialDataPin, latch: serialLatchPin, clock: serialClockPin, bitCount: 8);
    Console.ForegroundColor = ConsoleColor.Yellow;

    Console.WriteLine(DateTime.Now.ToString());

    double pw = 1;
    pwmOutput.PwmValue = (uint)(pwmOutput.PwmRange * pw);
    ConsoleKey key;
    while ((key = Console.ReadKey().Key) != ConsoleKey.Enter)
    {
        if (key == ConsoleKey.LeftArrow)
        {
            serial.WriteData(2).Wait();
        }
        else if (key == ConsoleKey.RightArrow)
        {
            serial.WriteData(0x10).Wait();
        }
        else if (key == ConsoleKey.DownArrow)
        {
            pw = Math.Max(0, pw - 0.01);
            pwmOutput.PwmValue = (uint)(pwmOutput.PwmRange * pw);
        }
        else if (key == ConsoleKey.UpArrow)
        {
            pw = Math.Min(1, pw + 0.01);
            pwmOutput.PwmValue = (uint)(pwmOutput.PwmRange * pw);
        }
        Console.WriteLine($"{pw} * {pwmOutput.PwmRange}");
    }
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Ending");

    serial.WriteData(0).Wait();
    // Give the Pi the time to process the signal so it actually shuts down
    Thread.Sleep(1000);
}
finally
{
    Console.ForegroundColor = originalColor;
}
