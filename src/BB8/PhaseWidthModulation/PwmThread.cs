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
        private const double phaseWidthCycleMilliseconds = 10;

        private bool disposedValue = false; // To detect redundant calls
        private Thread thread;
        private readonly ConcurrentDictionary<OutputPinConfiguration, double> phaseWidths 
            = new ConcurrentDictionary<OutputPinConfiguration, double>(new EqualityComparer<OutputPinConfiguration, ProcessorPin>((pinA) => pinA.Pin));
        private readonly GpioConnection connection;

        public PwmThread(GpioConnection connection)
        {
            this.connection = connection;
            thread = new Thread(RunPwmThread);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        public double? GetPhaseWidth(OutputPinConfiguration pinConfig)
        {
            double phaseWidth;
            if (phaseWidths.TryGetValue(pinConfig, out phaseWidth))
            {
                return phaseWidth;
            }
            return null;
        }

        public void SetPhaseWidth(OutputPinConfiguration pinConfig, double phaseWidth)
        {
            phaseWidths.AddOrUpdate(pinConfig, phaseWidth, delegate { return phaseWidth; });
        }

        public void ClearPhaseWidth(OutputPinConfiguration pinConfig)
        {
            double phaseWidth;
            phaseWidths.TryRemove(pinConfig, out phaseWidth);
        }

        private void RunPwmThread()
        {
            while (!disposedValue)
            {
                var list = phaseWidths
                    .ToArray()
                    .OrderBy(e => e.Value)
                    .Select(e => new KeyValuePair<OutputPinConfiguration, double>(e.Key, e.Value * phaseWidthCycleMilliseconds))
                    .ToArray();
                var nextIndex = 0;
                foreach (var e in list)
                {
                    connection[e.Key] = true;
                }
                var sw = new Stopwatch();
                sw.Start();
                while (sw.ElapsedMilliseconds < phaseWidthCycleMilliseconds)
                {
                    while (nextIndex < list.Length && list[nextIndex].Value < sw.ElapsedMilliseconds)
                    {
                        connection[list[nextIndex].Key] = false;
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
