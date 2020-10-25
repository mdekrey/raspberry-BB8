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
                services.AddSingleton<ControllerMappingService>();
                services.AddHostedService<ControllerMappingService>();
                services.AddTransient(sp => sp.GetRequiredService<ControllerMappingService>().MotorStates);
                services.AddTransient(sp => sp.GetRequiredService<ControllerMappingService>().ControllerUpdates);
                services.AddSingleton<MotorBinding>();
                services.AddTransient(sp => sp.GetRequiredService<MotorBinding>().Motors);
                services.AddSingleton<IGamepadProvider, EmptyGamepads>();
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    services.AddSingleton<IGamepadProvider, LinuxBluetoothGamepads>();
                }
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
