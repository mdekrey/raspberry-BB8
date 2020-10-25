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
using Unosquare.WiringPi;

Pi.Init<BootstrapWiringPi>();

using var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder => builder
                .AddJsonFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"), optional: false, reloadOnChange: true)
                .AddJsonFile(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "bb8.json"), optional: true, reloadOnChange: true)
            )
            .ConfigureServices((ctx, services) =>
            {
                services.AddSingleton(new CancellationTokenSource());
                services.Configure<GamepadMappingConfiguration>(ctx.Configuration.GetSection("gamepad"));
                services.Configure<MotionConfiguration>(ctx.Configuration.GetSection("motion"));
                services.Configure<BbUnitConfiguration>(ctx.Configuration.GetSection("bbUnit"));
                services.AddSingleton<IBluetoothController, BluetoothController>();
                services.AddHostedService<MotorService>();
                services.AddHostedService<ControllerMappingService>();
                services.AddSingleton(sp => new MotorBinding(Pi.Gpio, sp.GetRequiredService<IOptions<MotionConfiguration>>().Value.Serial, sp.GetRequiredService<List<ConfiguredMotor>>()));
                services.AddSingleton(sp => new BluetoothGamepads(sp.GetRequiredService<IBluetoothController>(), (from deviceMapping in sp.GetRequiredService<IOptions<GamepadMappingConfiguration>>().Value.Devices
                                                                                                                  where deviceMapping.Device.Bluetooth is string
                                                                                                                  select deviceMapping.Device.Bluetooth).ToArray()));
                services.AddSingleton(sp => sp.GetRequiredService<IOptions<MotionConfiguration>>().Value.Motors.Select(ConfiguredMotor.ToMotor).ToList());
                services.AddSingleton(sp => sp.GetRequiredService<BluetoothGamepads>().GamepadStateChanges.Select(sp.GetRequiredService<IOptions<GamepadMappingConfiguration>>().Value.Devices).Replay(1).RefCount());
                services.AddSingleton(sp => sp.GetRequiredService<IObservable<EventedMappedGamepad>>().SelectVector("moveX", "moveY")
                                                .Select(direction => from entry in Enumerable.Zip(
                                                                        sp.GetRequiredService<List<ConfiguredMotor>>(),
                                                                        from degrees in sp.GetRequiredService<IOptions<BbUnitConfiguration>>().Value.MotorOrientation
                                                                        let radians = degrees * Math.PI / 180
                                                                        select radians is double r ? new Vector2(Math.Cos(r), Math.Sin(r)) : null,
                                                                        (configuredMotor, direction) => (configuredMotor, direction)
                                                                     )
                                                                     let speed = direction.Dot(entry.direction)
                                                                     select new MotorDriveState(entry.configuredMotor, state: speed switch
                                                                     {
                                                                         var speed when speed > 0 => new MotorState { Direction = MotorDirection.Forward, Speed = Math.Clamp(speed, double.Epsilon, 1) },
                                                                         var speed when speed < 0 => new MotorState { Direction = MotorDirection.Backward, Speed = Math.Clamp(-speed, double.Epsilon, 1) },
                                                                         _ => new MotorState { Direction = MotorDirection.Stopped },
                                                                     }))
                                                .Select(motorState => motorState.ToArray())
                                                .Replay(1).RefCount());

            })
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
            .ConfigureWebHost(host => host.UseKestrel(options => options.Listen(
                System.Net.IPAddress.Any,
                // TODO - I don't like having a hard-coded port here
                5001,
                // TODO - I don't like having a hard-coded path here
                listenOptions => listenOptions.UseHttps("/raspberrypi.pfx")
            )))
            .Build();

Console.WriteLine("Starting diagnostics server...");
await host.RunAsync(host.Services.GetRequiredService<CancellationTokenSource>().Token);

Console.WriteLine("Ending");
