using BB8.Domain;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO.Abstractions;

namespace BB8.RaspberryPi
{
    public class MotorBinding : IAsyncDisposable
    {
        private bool disposedValue;
        private readonly IDisposable subscriptions;
        private readonly Subject<Unit> completed = new();
        private SerialDigitizer serial;
        private readonly IObservable<(Action cancel, Task task)> serialObservable;
        private readonly Subject<(int index, double power)> motorPower = new();
        private readonly Subject<byte> sentSerialData = new();

        public IObservable<byte> SerialData { get; }
        public IObservable<IReadOnlyList<double>> MotorPower { get; }

        public MotorBinding(IGpioController gpioPins, IOptionsMonitor<MotionConfiguration> motionConfiguration, IObservable<IReadOnlyList<Motor>> motors)
        {
            var subscriptions = new CompositeDisposable();

            serial = BuildSerialDigitizer(gpioPins, motionConfiguration.CurrentValue.Serial);
            subscriptions.Add(
                motionConfiguration.Observe()
                    .Select(c => c.Serial)
                    .Subscribe(cfg =>
                    {
                        serial = BuildSerialDigitizer(gpioPins, cfg);
                    })
            );

            var motorConfiguration = Observable.CombineLatest(motionConfiguration.Observe().Select(config => config.Motors),
                motors,
                (configurations, motors) => Enumerable.Zip(
                    configurations.Take(motors.Count),
                    motors.Take(configurations.Count),
                    (configuration, motor) => (configuration, motor)
                )
                    .Select((e, index) => (e.configuration, e.motor, index))
            )
                    .Replay(1)
                    .RefCount();

            SerialData = sentSerialData.StartWith((byte)0).Replay(1).RefCount();
            subscriptions.Add(SerialData.Subscribe());

            var serialObservable = motorConfiguration.Select(e => Observable.CombineLatest(e.Select(kvp => kvp.motor.Select(state => (MotorState: state, Configuration: kvp.configuration)))))
                .Switch()
                .TakeUntil(completed)
                .Buffer(TimeSpan.FromMilliseconds(15), 3)
                .Where(v => v.Any())
                .Select(v => v.Last())
                .Select(states => states.Select(ToFlag).Aggregate((byte)0, (prev, flag) => (byte)(prev | flag)))
                .DistinctUntilChanged()
                .ThrottledTask(async nextByte =>
                {
                    await serial.WriteDataAsync(nextByte);
                    sentSerialData.OnNext(nextByte);
                });
            this.serialObservable = serialObservable;

            this.subscriptions = subscriptions;
            subscriptions.Add(serialObservable.Subscribe(_ => { }, ex => Console.WriteLine(ex)));

            MotorPower = motorPower
                .Scan(ImmutableDictionary<int, double>.Empty, (prev, next) => prev.SetItem(next.index, next.power))
                .StartWith(ImmutableDictionary<int, double>.Empty)
                .Select(dict => Enumerable.Range(0, dict.Keys.DefaultIfEmpty(-1).Max() + 1).Select(key => dict.TryGetValue(key, out var value) ? value : 0).ToArray())
                .Replay(1)
                .RefCount();
            subscriptions.Add(MotorPower.Subscribe());

            subscriptions.Add(
                motorConfiguration.SelectMany(configuredMotors => configuredMotors)
                    .SelectMany(e => e.motor.Select(state => (state, speed: e.configuration.ToSpeed(state), pwm: gpioPins[e.configuration.PwmGpioPin].ToPwmPin(), index: e.index)))
                    .Subscribe(e =>
                    {
                        e.pwm.PwmValue = (uint)(e.pwm.PwmRange * e.speed);
                        motorPower.OnNext((e.index, e.speed));
                    }, ex => Console.WriteLine(ex))
            );
        }

        private static SerialDigitizer BuildSerialDigitizer(IGpioController gpioPins, MotorSerialControlPins cfg)
        {
            return new SerialDigitizer(gpioPins[cfg.GpioData], gpioPins[cfg.GpioClock], gpioPins[cfg.GpioLatch], 8);
        }

        private static byte ToFlag((MotorState MotorState, MotorConfiguration Configuration) state) =>
            state.Configuration.ToSpeed(state.MotorState) > 0
                ? state.Configuration.ToFlag(state.MotorState)
                : 0;

        ~MotorBinding()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            DisposeAsyncCore(disposing: false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore(disposing: true);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    completed.OnNext(Unit.Default);
                    var last = await serialObservable.LastAsync();
                    last.cancel();
                    await Task.WhenAll(last.task.ContinueWith(_ => { }));
                    subscriptions.Dispose();
                    completed.Dispose();
                    motorPower.Dispose();
                    sentSerialData.Dispose();
                }

                await serial.WriteDataAsync(0);
                disposedValue = true;
            }

        }
    }
}
