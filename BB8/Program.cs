using BB8;
using BB8.Bluetooth;
using BB8.Domain;
using BB8.RaspberryPi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

var isRaspberryPi = RuntimeInformation.RuntimeIdentifier.StartsWith("raspbian");

var hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder => builder
                .AddJsonFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"), optional: false, reloadOnChange: true)
                .AddJsonFile(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "bb8.json"), optional: true, reloadOnChange: true)
            )
            .ConfigureServices((ctx, services) =>
            {
                services.AddSingleton(sp => {
                    if (!isRaspberryPi)
                        return new FakeGpioController();
                    Pi.Init<BootstrapWiringPi>();
                    return Pi.Gpio;
                });
                services.AddSingleton(new CancellationTokenSource());
                services.Configure<GamepadMappingConfiguration>(ctx.Configuration.GetSection("gamepad"));
                services.Configure<MotionConfiguration>(ctx.Configuration.GetSection("motion"));
                services.Configure<MotorSerialControlPins>(ctx.Configuration.GetSection("motion:serial"));
                services.Configure<BbUnitConfiguration>(ctx.Configuration.GetSection("bbUnit"));
                services.AddSingleton<IBluetoothController, BluetoothController>();
                services.AddHostedService<MotorService>();
                services.AddHostedService<ControllerMappingService>();
                services.AddSingleton<MotorBinding>();
                services.AddTransient(sp => sp.GetRequiredService<MotorBinding>().Motors);
                services.AddSingleton<IGamepadProvider, EmptyGamepads>();
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    services.AddSingleton<IGamepadProvider>(sp => new LinuxBluetoothGamepads(sp.GetRequiredService<IBluetoothController>(), (from deviceMapping in sp.GetRequiredService<IOptions<GamepadMappingConfiguration>>().Value.Devices
                                                                                                                                             where deviceMapping.Device.Bluetooth is string
                                                                                                                                             select deviceMapping.Device.Bluetooth).ToArray()));
                }
                services.AddSingleton(sp => Observable.Merge(sp.GetRequiredService<IEnumerable<IGamepadProvider>>().Select(gamepads => gamepads.GamepadStateChanges)).Select(sp.GetRequiredService<IOptions<GamepadMappingConfiguration>>().Value.Devices).Replay(1).RefCount());
                services.AddSingleton(sp => Observable.CombineLatest(
                    sp.GetRequiredService<IObservable<IEnumerable<Motor>>>(),
                    sp.GetRequiredService<IObservable<EventedMappedGamepad>>().SelectVector("moveX", "moveY"),
                    sp.GetRequiredService<IOptionsMonitor<BbUnitConfiguration>>().Observe(),
                    (motors, direction, bbUnitConfiguration) => (motors: motors.ToArray(), direction, bbUnitConfiguration)
                )
                                                .Select(e => from entry in Enumerable.Zip(
                                                                        e.motors,
                                                                        from degrees in e.bbUnitConfiguration.MotorOrientation.Take(e.motors.Length)
                                                                        let radians = degrees * Math.PI / 180
                                                                        select radians is double r ? new Vector2(Math.Cos(r), Math.Sin(r)) : null,
                                                                        (motor, direction) => (motor, direction)
                                                                     )
                                                                     let speed = e.direction.Dot(entry.direction)
                                                                     select new MotorDriveState(entry.motor, state: speed switch
                                                                     {
                                                                         var speed when speed > 0 => new MotorState { Direction = MotorDirection.Forward, Speed = Math.Clamp(speed, double.Epsilon, 1) },
                                                                         var speed when speed < 0 => new MotorState { Direction = MotorDirection.Backward, Speed = Math.Clamp(-speed, double.Epsilon, 1) },
                                                                         _ => new MotorState { Direction = MotorDirection.Stopped },
                                                                     }))
                                                .Select(motorState => motorState.ToArray())
                                                .Replay(1).RefCount());

            })
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());

if (isRaspberryPi)
{
    hostBuilder = hostBuilder
            .ConfigureWebHost(host => host.UseKestrel(options => options.Listen(
                System.Net.IPAddress.Any,
                // TODO - I don't like having a hard-coded port here
                5001,
                // TODO - I don't like having a hard-coded path here
                listenOptions =>
                {
                    if (System.IO.File.Exists("/raspberrypi.pfx")) listenOptions.UseHttps("/raspberrypi.pfx");
                }
            )));
}

using var host = hostBuilder.Build();

Console.WriteLine("Starting diagnostics server...");
await host.RunAsync(host.Services.GetRequiredService<CancellationTokenSource>().Token);

Console.WriteLine("Ending");
