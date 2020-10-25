
using BB8.Bluetooth;
using BB8.Domain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BB8
{
    internal class ControllerMappingService : IHostedService, IDisposable
    {
        private readonly IObservable<EventedMappedGamepad> controllerUpdates;
        private readonly IObservable<MotorDriveState[]> moveMotors;
        private readonly GamepadMappingConfiguration gamepadMappingConfiguration;
        private readonly IBluetoothController bluetoothController;
        private readonly CancellationTokenSource cancellationTokenSource;
        private CompositeDisposable disposable = new();

        public ControllerMappingService(IObservable<EventedMappedGamepad> controllerUpdates, IObservable<MotorDriveState[]> moveMotors, IOptions<GamepadMappingConfiguration> gamepadMappingConfiguration, IBluetoothController bluetoothController, CancellationTokenSource cancellationTokenSource)
        {
            this.controllerUpdates = controllerUpdates;
            this.moveMotors = moveMotors;
            this.gamepadMappingConfiguration = gamepadMappingConfiguration.Value;
            this.bluetoothController = bluetoothController;
            this.cancellationTokenSource = cancellationTokenSource;
        }

        public void Dispose()
        {
            disposable.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            disposable.Add(
            controllerUpdates.Where(gamepad => gamepad.eventArgs.Any(change => change is MappedButtonEventArgs(_, "disconnect", true)))
    .Subscribe(_ =>
    {
        foreach (var entry in gamepadMappingConfiguration.Devices.Where(e => e.Device?.Bluetooth is string))
        {
            Task.Run(() => bluetoothController.DisconnectAsync(entry.Device!.Bluetooth!));
        }
    })); disposable.Add(
             controllerUpdates.Where(gamepad => gamepad.eventArgs.Any(change => change is MappedButtonEventArgs(_, "exit", true))).Subscribe(_ => cancellationTokenSource.Cancel()));
            disposable.Add(
            moveMotors.SelectMany(t => t).Subscribe(entry => entry.motor.Motor.Update(entry.state)));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

    }
}