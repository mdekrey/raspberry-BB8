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
var gamepadMapping = config.GetSection("gamepad").Get<GamepadMappingConfiguration>();
var bluetoothGamepadMacAddresses = (from deviceMapping in gamepadMapping.Devices
                                    where deviceMapping.Device.Bluetooth is string
                                    select deviceMapping.Device.Bluetooth).ToArray();
var motorConfig = config.GetSection("motion").Get<MotionConfiguration>();

var originalColor = Console.ForegroundColor;

IBluetoothController bluetoothController = new BluetoothController();
Pi.Init<BootstrapWiringPi>();

foreach (var pin in Pi.Gpio)
{
    Console.WriteLine($"{pin.BcmPin}: {((GpioPin)pin).Capabilities}");
}

var motors = motorConfig.Motors.Select(ConfiguredMotor.ToMotor).ToArray();

await using (var bluetoothGamepads = new BluetoothGamepads(bluetoothController, bluetoothGamepadMacAddresses))
await using (var motorBinding = new MotorBinding(Pi.Gpio, motorConfig.Serial, motors.ToDictionary(c => c.Motor, c => c.Configuration)))
{
    try
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(DateTime.Now.ToString());

        var controllerUpdates = bluetoothGamepads.GamepadStateChanges
            .Select((gamepad) => (gamepad.state, gamepad.eventArgs, mapping: gamepadMapping.Devices.FirstOrDefault(d => StringComparer.OrdinalIgnoreCase.Equals(d.Device.Name, gamepad.state.GamepadName))?.Mapping!))
            .Do(gamepad => Console.WriteLine(gamepad.state))
            .Where(gamepad => gamepad.mapping != null)
            .Select((gamepad) => (state: gamepad.state.Map(gamepad.mapping), eventArgs: gamepad.eventArgs.Map(gamepad.mapping).ToArray()))
            .Do(gamepad => Console.WriteLine(gamepad.state))
            .Do(gamepad =>
            {
                if (gamepad.eventArgs.Any(change => change is MappedButtonEventArgs(_, "disconnect", true)))
                {
                    Task.Run(() => bluetoothController.DisconnectAsync(bluetoothGamepadMacAddresses.First()));
                }
            })
            .TakeUntil(gamepad => gamepad.eventArgs.Any(change => change is MappedButtonEventArgs(_, "exit", true)))
            .Do(gamepad =>
            {
                var axisX = gamepad.state.Axis("moveX");
                var axisY = gamepad.state.Axis("moveY");
                motors[0].Motor.Update(axisX switch
                {
                    > 0 => new MotorState { Direction = MotorDirection.Forward, Speed = axisX },
                    < 0 => new MotorState { Direction = MotorDirection.Backward, Speed = -axisX },
                    _ => new MotorState { Direction = MotorDirection.Stopped },
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
