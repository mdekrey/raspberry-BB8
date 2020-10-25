using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace BB8
{
    public static class OptionsExtensions
    {
        public static IObservable<T> Observe<T>(this IOptionsMonitor<T> options) =>
            Observable.Create<T>(observer => options.OnChange(observer.OnNext)).StartWith(options.CurrentValue);
    }
}
