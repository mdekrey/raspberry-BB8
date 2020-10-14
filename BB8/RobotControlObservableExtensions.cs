using BB8.Domain;
using BB8.Gamepad;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BB8
{
    public static class RobotControlObservableExtensions
    {
        public static IObservable<Vector2> SelectVector(this IObservable<EventedMappedGamepad> mappedGamepad, string xAxis, string yAxis) =>
            mappedGamepad.Select(gamepad => new Vector2(gamepad.state.Axis(xAxis), gamepad.state.Axis(yAxis)).MaxUnit());

        public static IObservable<(Action cancel, Task task)> ThrottledTask<T>(this IObservable<T> input, Func<T, Task> continuation) =>
            input.Scan((cancel: (Action)(() => { }), task: Task.CompletedTask), (prev, nextValue) =>
            {
                prev.cancel();
                var cancellation = new CancellationTokenSource();
                return (cancel: cancellation.Cancel, task: prev.task.ContinueWith(async _ =>
                {
                    if (!cancellation.IsCancellationRequested)
                        await continuation(nextValue);
                }).Unwrap());
            })
                .StartWith((cancel: (Action)(() => { }), task: Task.CompletedTask))
                .Replay(1).RefCount();

        public static IObservable<T> TakeUntil<T>(this IObservable<T> input, CancellationToken cancellationToken) =>
            input.TakeUntil(Observable.Create<Unit>(observer => cancellationToken.Register(() => observer.OnNext(Unit.Default))));
    }
}
