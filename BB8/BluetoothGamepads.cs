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

        this.CurrentGamepadState = Observable.Create<IGamepad>(async (observer, cancellationToken) =>
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

                        foreach (var notConnected in bluetoothGamepadMacAddresses.Except(connectedDevices))
                        {
                            if (await bluetoothController.ConnectAsync(notConnected, cancellationToken))
                            {
                                break;
                            }
                        }
                        await Task.Delay(500);
                    } while (bluetoothGamepadMacAddresses.Except(connectedDevices).Any());
                }
            } while (!joysticks.Any());

            observer.OnNext(new Gamepad(joysticks.First()));

            return () => { };
        })
            .TakeUntil(disposed)
            .SelectMany(controller => controller.GamepadStateChanged)
            .StartWith(GamepadState.Empty)
            .Retry();
    }

    public IObservable<GamepadState> CurrentGamepadState { get; }

    public async ValueTask DisposeAsync()
    {
        disposed.OnNext(Unit.Default);
        foreach (var bt in bluetoothGamepadMacAddresses)
        {
            await bluetoothController.DisconnectAsync(bt);
        }
    }
}