
using BB8.Bluetooth;
using BB8.Domain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BB8
{
    internal class ControllerMappingService : IHostedService, IDisposable
    {
        private readonly IOptionsMonitor<GamepadMappingConfiguration> gamepadMappingConfiguration;
        private readonly IBluetoothController bluetoothController;
        private readonly CancellationTokenSource cancellationTokenSource;
        public IObservable<EventedMappedGamepad> ControllerUpdates { get; }
        public IObservable<MotorDriveState[]> MotorStates { get; }
        private CompositeDisposable disposable = new();
        public IObservable<IReadOnlyList<Motor>> Motors { get; }

        public ControllerMappingService(IEnumerable<IGamepadProvider> gamepadProviders, IOptionsMonitor<BbUnitConfiguration> bbUnitConfiguration, IOptionsMonitor<GamepadMappingConfiguration> gamepadMappingConfiguration, IBluetoothController bluetoothController, CancellationTokenSource cancellationTokenSource)
        {
            this.gamepadMappingConfiguration = gamepadMappingConfiguration;
            this.bluetoothController = bluetoothController;
            this.cancellationTokenSource = cancellationTokenSource;

            this.Motors = bbUnitConfiguration.Observe()
                .Select(cfg => cfg.MotorOrientation.Length)
                .Scan(ImmutableList<Motor>.Empty, (motors, totalCount) => Enumerable.Range(0, Math.Max(0, totalCount - motors.Count)).Aggregate(motors, (motors, _) => motors.Add(new Motor())))
                .Replay(1)
                .RefCount();

            this.ControllerUpdates =
                gamepadMappingConfiguration.Observe()
                    .Select(c => c.Devices)
                    .Select(devices =>
                        Observable.Merge(
                            gamepadProviders
                                .Select(gamepads => gamepads.GamepadStateChanges)
                        )
                            .Select(devices)
                            .Replay(1)
                            .RefCount()
                    )
                    .Switch();

            this.MotorStates = Observable.CombineLatest(
                    Motors,
                    ControllerUpdates.SelectVector("moveX", "moveY"),
                    bbUnitConfiguration.Observe(),
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
                                                .Replay(1).RefCount();
        }

        public void Dispose()
        {
            disposable.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            disposable.Add(
                ControllerUpdates
                    .Where(gamepad => gamepad.eventArgs.Any(change => change is MappedButtonEventArgs(_, "disconnect", true)))
                    .Subscribe(_ =>
                    {
                        foreach (var entry in gamepadMappingConfiguration.CurrentValue.Devices.Where(e => e.Device?.Bluetooth is string))
                        {
                            Task.Run(() => bluetoothController.DisconnectAsync(entry.Device!.Bluetooth!));
                        }
                    })
            );
            disposable.Add(
                ControllerUpdates
                    .Where(gamepad => gamepad.eventArgs.Any(change => change is MappedButtonEventArgs(_, "exit", true)))
                    .Subscribe(_ => cancellationTokenSource.Cancel())
            );
            disposable.Add(MotorStates.SelectMany(t => t).Subscribe(entry => entry.motor.Update(entry.state)));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

    }
}