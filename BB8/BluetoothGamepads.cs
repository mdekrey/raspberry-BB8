using BB8.Bluetooth;
using BB8.Gamepad;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

internal class BluetoothGamepads : IAsyncDisposable
{
    private readonly IBluetoothController bluetoothController;
    private readonly string[] bluetoothGamepadMacAddresses;
    private readonly Subject<Unit> disposed = new Subject<Unit>();

    public BluetoothGamepads(IBluetoothController bluetoothController, string[] bluetoothGamepadMacAddresses)
    {
        this.bluetoothController = bluetoothController;
        this.bluetoothGamepadMacAddresses = bluetoothGamepadMacAddresses;
        if (!bluetoothGamepadMacAddresses.Any())
            throw new InvalidOperationException("Must provide at least one bluetooth gamepad to use this class.");

        this.GamepadStateChanges = Observable.Create<IGamepad>(async (observer, cancellationToken) =>
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
                        Console.WriteLine("Checking bluetooth devices...");
                        connectedDevices = await bluetoothController.GetConnectedBluetoothDevicesAsync(cancellationToken).ConfigureAwait(false);

                        foreach (var notConnected in bluetoothGamepadMacAddresses.Except(connectedDevices))
                        {
                            if (await bluetoothController.ConnectAsync(notConnected, cancellationToken).ConfigureAwait(false))
                            {
                                break;
                            }
                        }
                        // Raspberry Pi 3 models cannot run wifi and bluetooth at the same time. This delay attempts to keep wifi alive for remote control. TODO - upgrade and remove.
                        Console.WriteLine("Delay connecting bluetooth...");
                        await Task.Delay(500).ConfigureAwait(false);
                    } while (bluetoothGamepadMacAddresses.Except(connectedDevices).Any());
                }
            } while (!joysticks.Any());

            var joystick = joysticks.First();
            var macAddress = await bluetoothController.GetMacAddress(joystick, cancellationToken).ConfigureAwait(false);
            Console.WriteLine($"Joystick {joystick} is {macAddress}");
            var name = bluetoothGamepadMacAddresses.FirstOrDefault(known => StringComparer.OrdinalIgnoreCase.Equals(known, macAddress));
            observer.OnNext(new Gamepad(name: name ?? "default", deviceFile: joystick));

            return () => { };
        })
            .Catch((Exception ex) => {
                Console.WriteLine(ex);
                return Observable.Throw<IGamepad>(ex);
            })
            .TakeUntil(disposed)
            .SelectMany(controller => controller.GamepadStateChanged)
            .Buffer(TimeSpan.FromMilliseconds(10), System.Reactive.Concurrency.TaskPoolScheduler.Default)
            .Where(changes => changes.Any())
            .Select(stateChanges => (state: stateChanges.Last().state, eventArgs: stateChanges.Select(change => change.eventArgs).ToArray()))
            .StartWith((state: GamepadState.Empty, eventArgs: Array.Empty<GamepadEventArgs>()))
            .Retry()
            .Publish()
            .RefCount();
    }

    public IObservable<(GamepadState state, GamepadEventArgs[] eventArgs)> GamepadStateChanges { get; }

    public async ValueTask DisposeAsync()
    {
        disposed.OnNext(Unit.Default);
        foreach (var bt in bluetoothGamepadMacAddresses)
        {
            await bluetoothController.DisconnectAsync(bt);
        }
    }
}