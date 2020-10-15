using BB8.Domain;
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
        private readonly IGpioPin serialDataPin;
        private readonly IGpioPin serialLatchPin;
        private readonly IGpioPin serialClockPin;
        private readonly IDisposable subscriptions;
        private readonly Subject<Unit> completed = new();
        private readonly SerialDigitizer serial;
        private readonly IObservable<(Action cancel, Task task)> serialObservable;
        private readonly Subject<(int index, double power)> motorPower = new();
        private readonly Subject<byte> sentSerialData = new();

        public IObservable<byte> SerialData => sentSerialData.AsObservable();
        public IObservable<IReadOnlyList<double>> MotorPower { get; }

        public MotorBinding(IGpioController gpioPins, MotorSerialControlPins serialConfiguration, IEnumerable<ConfiguredMotor> motors)
        {
            serialDataPin = gpioPins[serialConfiguration.GpioData];
            serialLatchPin = gpioPins[serialConfiguration.GpioLatch];
            serialClockPin = gpioPins[serialConfiguration.GpioClock];
            serialDataPin.PinMode = GpioPinDriveMode.Output;
            serialLatchPin.PinMode = GpioPinDriveMode.Output;
            serialClockPin.PinMode = GpioPinDriveMode.Output;

            serial = new SerialDigitizer(serialDataPin, serialClockPin, serialLatchPin, 8);

            var serialObservable = Observable.CombineLatest(motors.Select(kvp => kvp.Motor.Select(state => (MotorState: state, Configuration: kvp.Configuration))))
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

            var subscriptions = new CompositeDisposable();
            this.subscriptions = subscriptions;
            subscriptions.Add(serialObservable.Subscribe());

            MotorPower = motorPower
                .Scan(motors.Select(_ => 0.0).ToImmutableList(), (prev, next) => prev.SetItem(next.index, next.power))
                .StartWith(motors.Select(_ => 0.0).ToImmutableList())
                .Replay(1)
                .RefCount();
            subscriptions.Add(MotorPower.Subscribe());

            foreach (var entry in motors.Select((m, index) => (m.Motor, m.Configuration, index)))
            {
                var pwmOutput = gpioPins[entry.Configuration.PwmGpioPin].ToPwmPin();
                subscriptions.Add(entry.Motor
                    .Select(state => (state, pwm: pwmOutput))
                    .Subscribe(state => {
                        var speed = entry.Configuration.ToSpeed(state.state);
                        state.pwm.PwmValue = (uint)(pwmOutput.PwmRange * speed);
                        motorPower.OnNext((entry.index, speed));
                    }));
            }
        }

        private static byte ToFlag((MotorState MotorState, MotorConfiguration Configuration) state) => 
            state.Configuration.ToFlag(state.MotorState);

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
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
