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
var bbUnitConfig = config.GetSection("bbUnit").Get<BbUnitConfiguration>();

var originalColor = Console.ForegroundColor;

IBluetoothController bluetoothController = new BluetoothController();
Pi.Init<BootstrapWiringPi>();

foreach (var pin in Pi.Gpio)
{
    Console.WriteLine($"{pin.BcmPin}: {((GpioPin)pin).Capabilities}");
}

var motors = motorConfig.Motors.Select(ConfiguredMotor.ToMotor).ToArray();
var motorDirection = bbUnitConfig.MotorOrientation.Select(degrees => degrees * Math.PI / 180).Select(radians => new Vector2(Math.Cos(radians), Math.Sin(radians)));

await using (var bluetoothGamepads = new BluetoothGamepads(bluetoothController, bluetoothGamepadMacAddresses))
await using (var motorBinding = new MotorBinding(Pi.Gpio, motorConfig.Serial, motors.ToDictionary(c => c.Motor, c => c.Configuration)))
{
    try
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(DateTime.Now.ToString());

        var controllerUpdates = bluetoothGamepads.GamepadStateChanges
            .Select((gamepad) => (gamepad.state, gamepad.eventArgs, mapping: gamepadMapping.Devices.FirstOrDefault(d => StringComparer.OrdinalIgnoreCase.Equals(d.Device.Name, gamepad.state.GamepadName))?.Mapping!))
            //.Do(gamepad => Console.WriteLine(gamepad.state))
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
            .Select(gamepad => new Vector2(gamepad.state.Axis("moveX"), gamepad.state.Axis("moveY")).MaxUnit())
            .Do(direction =>
            {
                foreach (var entry in motorDirection.Zip(motors, (direction, motor) => (direction, motor: motor.Motor)))
                {
                    entry.motor.Update(direction.Dot(entry.direction) switch
                    {
                        var speed when speed > 0 => new MotorState { Direction = MotorDirection.Forward, Speed = Math.Clamp(speed, double.Epsilon, 1) },
                        var speed when speed < 0 => new MotorState { Direction = MotorDirection.Backward, Speed = Math.Clamp(-speed, double.Epsilon, 1) },
                        _ => new MotorState { Direction = MotorDirection.Stopped },
                    });
                }
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

record Vector2(double X, double Y)
{
    public double Dot(Vector2 other) => X * other.X + Y * other.Y;

    public Vector2 MaxUnit() =>
        (X * X + Y * Y) switch
        {
            < 1 => this,
            var norm => this.Multiply(1 / Math.Sqrt(norm))
        };

    public Vector2 Multiply(double factor) => new(X * factor, Y * factor);
}