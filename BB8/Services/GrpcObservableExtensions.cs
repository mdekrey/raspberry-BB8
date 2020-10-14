using Grpc.Core;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace BB8.Services
{
    static class GrpcObservableExtensions
    {
        public static async Task Subscribe<T>(this IObservable<T> source, IServerStreamWriter<T> target, ServerCallContext context)
        {
            var last = await source
                .TakeUntil(context.CancellationToken)
                .ThrottledTask(reply => target.WriteAsync(reply));
            await last.task;
        }
    }
}
