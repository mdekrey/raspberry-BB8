using BB8;
using BB8.Domain;
using BB8.RaspberryPi;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

var config = new ConfigurationBuilder()
    .AddJsonFile("config.json")
    .AddJsonFile(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "bb8.json"), optional: true)
    .Build();
var motorConfig = config.GetSection("motors").Get<MotionConfiguration>();

var originalColor = Console.ForegroundColor;
            
Pi.Init<BootstrapWiringPi>();


foreach (var pin in Pi.Gpio)
{
    Console.WriteLine($"{pin.BcmPin}: {((GpioPin)pin).Capabilities}");
}

var motor = new Motor();
await using (var motorBinding = new MotorBinding(Pi.Gpio, motorConfig.Serial, new Dictionary<Motor, MotorConfiguration> { { motor, new() { PwmGpioPin = motorConfig.GpioPwmMotor, ForwardBit = 1, BackwardBit = 4 } } }))
{

    try
    {
        Console.ForegroundColor = ConsoleColor.Yellow;

        Console.WriteLine(DateTime.Now.ToString());
        MainLoop(motor);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Ending");

        // Give the Pi the time to process the signal so it actually shuts down
        Thread.Sleep(1000);
    }
    finally
    {
        Console.ForegroundColor = originalColor;
    }
}


void MainLoop(Motor motor)
{
    while (true)
    {
        switch (Console.ReadKey().Key)
        {
            case ConsoleKey.Enter:
                return;
            case var key when key is ConsoleKey.LeftArrow or ConsoleKey.RightArrow:
                var shift = key is ConsoleKey.LeftArrow ? -1 : 1;
                var value = motor.Current switch
                {
                    { Direction: MotorDirection.Stopped } => 0,
                    { Direction: MotorDirection.Backward, Speed: var s } => -s,
                    { Direction: MotorDirection.Forward, Speed: var s } => s,
                    _ => 0
                };
                var newState = (Math.Round(value * 100) + shift) switch
                {
                    > 100 => new MotorState { Direction = MotorDirection.Forward, Speed = 1 },
                    < -100 => new MotorState { Direction = MotorDirection.Backward, Speed = 1 },
                    var speed when speed > 0 => new MotorState { Direction = MotorDirection.Forward, Speed = speed / 100.0 },
                    var speed when speed < 0 => new MotorState { Direction = MotorDirection.Backward, Speed = speed / -100.0 },
                    _ => new MotorState { Direction = MotorDirection.Stopped },
                };
                motor.Update(newState);
                break;
        }
        Console.WriteLine(motor.Current switch
        {
            { Direction: MotorDirection.Stopped }  => $"Stopped",
            { Direction: var dir, Speed: var speed } => $"{dir} @ {speed:0.00}",
        });
    }
}