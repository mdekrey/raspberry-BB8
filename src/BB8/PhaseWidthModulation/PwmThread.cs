using Raspberry.IO.GeneralPurpose;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BB8.PhaseWidthModulation
{
    public class PwmThread : IDisposable
    {
        private const double phaseWidthCycleMilliseconds = 5000;

        private bool disposedValue = false; // To detect redundant calls
        private Thread thread;
        private readonly ConcurrentDictionary<GpioConnection, double> phaseWidths 
            = new ConcurrentDictionary<GpioConnection, double>(new EqualityComparer<GpioConnection, ProcessorPin>((pinA) => pinA.Pins.Single().Configuration.Pin));

        public PwmThread()
        {
            thread = new Thread(RunPwmThread);
            
            thread.Start();
        }

        public double? GetPhaseWidth(GpioConnection pinConfig)
        {
            double phaseWidth;
            if (phaseWidths.TryGetValue(pinConfig, out phaseWidth))
            {
                return phaseWidth;
            }
            return null;
        }

        public void SetPhaseWidth(GpioConnection pinConfig, double phaseWidth)
        {
            phaseWidths.AddOrUpdate(pinConfig, phaseWidth, delegate { return phaseWidth; });
        }

        public void ClearPhaseWidth(GpioConnection pinConfig)
        {
            double phaseWidth;
            phaseWidths.TryRemove(pinConfig, out phaseWidth);
        }

        private void RunPwmThread()
        {
            while (true)
            {
                var list = phaseWidths
                    .ToArray()
                    .OrderBy(e => e.Value)
                    .Select(e => new KeyValuePair<GpioConnection, double>(e.Key, e.Value * phaseWidthCycleMilliseconds))
                    .ToArray();
                var nextIndex = 0;
                foreach (var e in list)
                { 
                    Console.WriteLine("On");
                    e.Key[e.Key.Pins.Single().Configuration] = true;
                }
                var sw = new Stopwatch();
                sw.Start();
                while (sw.ElapsedMilliseconds < phaseWidthCycleMilliseconds)
                {
                    while (nextIndex < list.Length && list[nextIndex].Value < sw.ElapsedMilliseconds)
                    {
                        Console.WriteLine("Off");
                        list[nextIndex].Key[list[nextIndex].Key.Pins.Single().Configuration] = false;
                        nextIndex++;
                    }
                }
            }
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.thread.Abort();
                }

                disposedValue = true;
            }
        }

        ~PwmThread()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        class EqualityComparer<T, U> : IEqualityComparer<T>
        {
            private readonly Func<T, U> selector;

            public EqualityComparer(Func<T, U> selector)
            {
                this.selector = selector;
            }

            public bool Equals(T x, T y)
            {
                return selector(x).Equals(selector(y));
            }

            public int GetHashCode(T obj)
            {
                return selector(obj).GetHashCode();
            }
        }
    }
}
