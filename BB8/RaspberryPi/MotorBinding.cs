using BB8.Domain;
using System;
using System.Collections.Generic;
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
    class MotorBinding : IAsyncDisposable
    {
        private readonly IGpioPin serialDataPin;
        private readonly IGpioPin serialLatchPin;
        private readonly IGpioPin serialClockPin;
        private readonly IDisposable subscriptions;
        private readonly Subject<Unit> completed = new();
        private readonly SerialDigitizer serial;
        private readonly IObservable<(Action cancel, Task task)> serialObservable;

        public MotorBinding(IGpioController gpioPins, MotorSerialControlPins serialConfiguration, IReadOnlyDictionary<Motor, MotorConfiguration> motors)
        {
            serialDataPin = gpioPins[serialConfiguration.GpioData];
            serialLatchPin = gpioPins[serialConfiguration.GpioLatch];
            serialClockPin = gpioPins[serialConfiguration.GpioClock];
            serialDataPin.PinMode = GpioPinDriveMode.Output;
            serialLatchPin.PinMode = GpioPinDriveMode.Output;
            serialClockPin.PinMode = GpioPinDriveMode.Output;

            serial = new SerialDigitizer(serialDataPin, serialClockPin, serialLatchPin, 8);

            var serialObservable = Observable.CombineLatest(motors.Select(kvp => kvp.Key.Select(state => (MotorState: state, Configuration: kvp.Value))))
                .TakeUntil(completed)
                .Buffer(TimeSpan.FromMilliseconds(15), 3)
                .Where(v => v.Any())
                .Select(v => v.Last())
                .Select(states => states.Select(ToFlag).Aggregate((byte)0, (prev, flag) => (byte)(prev | flag)))
                .DistinctUntilChanged()
                .Scan((cancel: (Action)(() => { }), task: Task.CompletedTask), (prev, nextByte) =>
                {
                    prev.cancel();
                    var cancellation = new CancellationTokenSource();
                    return (cancel: cancellation.Cancel, task: prev.task.ContinueWith(async _ =>
                    {
                        if (!cancellation.IsCancellationRequested)
                            await serial.WriteDataAsync(nextByte);
                    }).Unwrap());
                })
                .Replay(1);
            this.serialObservable = serialObservable;

            var subscriptions = new CompositeDisposable();
            this.subscriptions = subscriptions;
            subscriptions.Add(serialObservable.Connect());
            subscriptions.Add(serialObservable.Subscribe());

            foreach (var kvp in motors)
            {
                var pwmOutput = gpioPins[kvp.Value.PwmGpioPin].ToPwmPin();
                subscriptions.Add(kvp.Key
                    .Select(state => (state, pwm: pwmOutput))
                    .Subscribe(state => state.pwm.PwmValue = (uint)(pwmOutput.PwmRange * kvp.Value.ToSpeed(state.state))));
            }
        }

        private static byte ToFlag((MotorState MotorState, MotorConfiguration Configuration) state) => 
            state.Configuration.ToFlag(state.MotorState);

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
        protected virtual async ValueTask DisposeAsyncCore()
        {
            completed.OnNext(Unit.Default);
            var last = await serialObservable.LastAsync();
            last.cancel();
            await Task.WhenAll(last.task.ContinueWith(_ => { }));
            subscriptions.Dispose();
            completed.Dispose();

            await serial.WriteDataAsync(0);
        }
    }
}
