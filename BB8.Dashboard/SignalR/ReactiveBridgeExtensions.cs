using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using grpc = global::Grpc.Core;

namespace BB8.Dashboard.SignalR
{
    public static class ReactiveBridgeExtensions
    { 
        public static IObservable<TResult> AsGrpc<T, TResult>(this IObservable<T> request, Func<T, grpc::CallOptions, grpc::AsyncServerStreamingCall<TResult>> getAsyncServerStreamingCall)
        {
            return request.Select(request => Observable.Create<TResult>(async (observer, cancellation) =>
            {
                var result = getAsyncServerStreamingCall(request, new (cancellationToken: cancellation));
                while (await result.ResponseStream.MoveNext(cancellation))
                    observer.OnNext(result.ResponseStream.Current);
            })).Switch();
        }

        public static ChannelReader<T> AsSignalRChannel<T>(this IObservable<T> observable, CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<T>();

            observable
                .Aggregate(Task.CompletedTask, (prevTask, next) => prevTask.ContinueWith(t => channel.Writer.WriteAsync(next).AsTask()).Unwrap())
                .Select(next => Observable.FromAsync(() => next))
                .Concat()
                .Subscribe(_ => { }, ex => channel.Writer.Complete(ex), () => channel.Writer.Complete(), cancellationToken);

            return channel.Reader;
        }

    }
}
