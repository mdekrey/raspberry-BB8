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
var bluetoothGamepads = config.GetSection("gamepad:bluetoothDevices").Get<string[]>();
var motorConfig = config.GetSection("motion").Get<MotionConfiguration>();

var originalColor = Console.ForegroundColor;

IBluetoothController bluetoothController = new BluetoothController();
Pi.Init<BootstrapWiringPi>();

foreach (var pin in Pi.Gpio)
{
    Console.WriteLine($"{pin.BcmPin}: {((GpioPin)pin).Capabilities}");
}

var motor = new Motor();
await using (var motorBinding = new MotorBinding(Pi.Gpio, motorConfig.Serial, new Dictionary<Motor, MotorConfiguration> { { motor, motorConfig.Motors.First(m => m != null)! } }))
{
    try
    {
        Console.ForegroundColor = ConsoleColor.Yellow;

        Console.WriteLine(DateTime.Now.ToString());
        using var controllerUpdates = AnyGamepadState(bluetoothController, bluetoothGamepads).Subscribe(gamepad => Console.WriteLine(gamepad), ex => Console.WriteLine(ex));
        MainLoop(motor);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Ending");

        await DisconnectGamepads(bluetoothController, bluetoothGamepads);

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
        switch (Console.ReadKey(true).Key)
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

IObservable<GamepadState> AnyGamepadState(IBluetoothController bluetoothController, string[] bluetoothGamepads)
{
    return Observable.Create<IGamepad>(async (observer, cancellationToken) =>
    {
        string[] joysticks;
        do
        {
            joysticks = Gamepad.GetDeviceNames();
            if (!joysticks.Any())
            {
                string[] connectedDevices;
                do
                {
                    connectedDevices = await bluetoothController.GetConnectedBluetoothDevicesAsync(cancellationToken);

                    foreach (var notConnected in bluetoothGamepads.Except(connectedDevices))
                    {
                        if (await bluetoothController.ConnectAsync(notConnected, cancellationToken))
                            break;
                    }
                } while (bluetoothGamepads.Except(connectedDevices).Any());
            }
        } while (!joysticks.Any());

        observer.OnNext(new Gamepad(joysticks.First()));

        return () => { };
    })
        .SelectMany(controller => controller.GamepadStateChanged)
        .StartWith(GamepadState.Empty)
        .Retry();
}

async System.Threading.Tasks.Task DisconnectGamepads(IBluetoothController bluetoothController, string[] bluetoothGamepads)
{
    foreach (var bt in bluetoothGamepads)
    {
        await bluetoothController.DisconnectAsync(bt);
    }
}
