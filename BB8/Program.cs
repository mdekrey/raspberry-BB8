using BB8;
using BB8.Bluetooth;
using BB8.Domain;
using BB8.Gamepad;
using BB8.RaspberryPi;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.WiringPi;

var config = new ConfigurationBuilder()
    .AddJsonFile("config.json")
    .AddJsonFile(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "bb8.json"), optional: true)
    .Build();
var bluetoothGamepadMacAddresses = config.GetSection("gamepad:bluetoothDevices").Get<string[]>();
var motorConfig = config.GetSection("motion").Get<MotionConfiguration>();

var originalColor = Console.ForegroundColor;

IBluetoothController bluetoothController = new BluetoothController();
Pi.Init<BootstrapWiringPi>();

foreach (var pin in Pi.Gpio)
{
    Console.WriteLine($"{pin.BcmPin}: {((GpioPin)pin).Capabilities}");
}

var motors = motorConfig.Motors.Select(ConfiguredMotor.ToMotor).ToArray();

const int axisIndex = 3;

await using (var bluetoothGamepads = new BluetoothGamepads(bluetoothController, bluetoothGamepadMacAddresses))
await using (var motorBinding = new MotorBinding(Pi.Gpio, motorConfig.Serial, motors.ToDictionary(c => c.Motor, c => c.Configuration)))
{
    try
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(DateTime.Now.ToString());

        var controllerUpdates = bluetoothGamepads.CurrentGamepadState
            .Do(gamepad => Console.WriteLine(gamepad))
            .Do(gamepad =>
            {
                if (gamepad.Buttons.TryGetValue(3, out var pressed) && pressed)
                {
                    Task.Run(() => bluetoothController.DisconnectAsync(bluetoothGamepadMacAddresses.First()));
                }
            })
            .TakeUntil(gamepad => gamepad.Buttons.TryGetValue(0, out var pressed) ? pressed : false)
            .Do(gamepad =>
            {
                motors[0].Motor.Update((gamepad.Axis.TryGetValue(axisIndex, out var axis) ? axis : 0) switch
                {
                    0 => new MotorState { Direction = MotorDirection.Stopped },
                    > 0 => new MotorState { Direction = MotorDirection.Forward, Speed = axis / (double)short.MaxValue },
                    < 0 => new MotorState { Direction = MotorDirection.Backward, Speed = axis / (double)short.MinValue },
                });
            });
        await controllerUpdates;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Ending");
    }
    finally
    {
        Console.ForegroundColor = originalColor;
    }
}
// Give the Pi the time to process the signal so it actually shuts down
// TODO - is the Sleep still necessary?
Thread.Sleep(1000);
