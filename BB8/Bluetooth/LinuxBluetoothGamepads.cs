﻿using BB8.Bluetooth;
using BB8.Domain;
using BB8.Gamepad;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace BB8.Bluetooth
{
    using LinuxGamepad = BB8.Gamepad.LinuxGamepad;

    internal class LinuxBluetoothGamepads : IAsyncDisposable, IGamepadProvider
    {
        private readonly IBluetoothController bluetoothController;
        private readonly IOptionsMonitor<GamepadMappingConfiguration> gamepadMapping;
        private readonly Subject<Unit> disposed = new Subject<Unit>();

        public LinuxBluetoothGamepads(IBluetoothController bluetoothController, IOptionsMonitor<GamepadMappingConfiguration> gamepadMapping)
        {
            this.bluetoothController = bluetoothController;
            this.gamepadMapping = gamepadMapping;

            this.GamepadStateChanges = gamepadMapping.Observe()
                .Select(GetMacAddresses)
                .Select(bluetoothGamepadMacAddresses =>
                    bluetoothGamepadMacAddresses.Any()
                        ? Observable.Create<IEnumerable<IGamepad>>(async (observer, cancellationToken) =>
                        {
                            string[] joysticks;
                            while (!cancellationToken.IsCancellationRequested)
                            {
                                joysticks = LinuxGamepad.GetDeviceNames();
                                if (joysticks.Any())
                                {
                                    var gamepads = await Task.WhenAll(joysticks.Select(async joystick =>
                                    {
                                        var macAddress = await bluetoothController.GetMacAddress(joystick, cancellationToken).ConfigureAwait(false);
                                        Console.WriteLine($"Joystick {joystick} is {macAddress}");
                                        var name = bluetoothGamepadMacAddresses.FirstOrDefault(known => StringComparer.OrdinalIgnoreCase.Equals(known, macAddress));
                                        return new LinuxGamepad(name: name ?? "default", deviceFile: joystick);
                                    })).ConfigureAwait(false);
                                    observer.OnNext(gamepads);

                                    await Task.Delay(5000).ConfigureAwait(false);
                                }
                                else
                                {
                                    string[] connectedDevices;
                                    do
                                    {
                                        Console.WriteLine("Checking bluetooth devices...");
                                        connectedDevices = await bluetoothController.GetConnectedBluetoothDevicesAsync(cancellationToken).ConfigureAwait(false);

                                        foreach (var notConnected in bluetoothGamepadMacAddresses.Except(connectedDevices))
                                        {
                                            if (await bluetoothController.ConnectAsync(notConnected, cancellationToken).ConfigureAwait(false))
                                            {
                                                break;
                                            }
                                        }
                                        // Raspberry Pi 3 models have power issues constantly
                                        // trying to connect bluetooth. This short delay doesn't
                                        // seem to affect my controller, but helps the Pi
                                        // immensley.
                                        Console.WriteLine("Delay connecting bluetooth...");
                                        await Task.Delay(500).ConfigureAwait(false);
                                    } while (bluetoothGamepadMacAddresses.Except(connectedDevices).Any());
                                }
                            }
                        })
                        : Observable.Empty<IEnumerable<IGamepad>>())
                .Switch()
                .TakeUntil(disposed)
                .SelectMany(gamepads => Observable.Merge(gamepads.Select(gamepad => gamepad.BufferEvents(TimeSpan.FromMilliseconds(10)))))
                .Publish()
                .RefCount();
        }

        private static IEnumerable<string> GetMacAddresses(GamepadMappingConfiguration value)
        {
            return (from deviceMapping in value.Devices
                    where deviceMapping.Device.Bluetooth is string
                    select deviceMapping.Device.Bluetooth);
        }

        public IObservable<EventedGamepad> GamepadStateChanges { get; }

        public async ValueTask DisposeAsync()
        {
            disposed.OnNext(Unit.Default);
            foreach (var bt in GetMacAddresses(gamepadMapping.CurrentValue))
            {
                await bluetoothController.DisconnectAsync(bt);
            }
        }
    }
}