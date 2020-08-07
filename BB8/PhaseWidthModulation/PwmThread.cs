using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO.Abstractions;

namespace BB8.PhaseWidthModulation
{
    /// <summary>
    /// Creates a constantly running thread that controls the given GPIO pins
    /// for phase-width modulation. That is, given a 
    /// `phaseWidthCycleMilliseconds` of 10, setting a `phaseWidth` on a pin to
    /// 0.5, it will be on for 5ms followed by 5ms off being off.
    /// </summary>
    public class PwmThread : IDisposable
    {
        private readonly double phaseWidthCycleMilliseconds = 10;

        private bool disposedValue = false; // To detect redundant calls
        private Thread thread;
        private readonly ConcurrentDictionary<IGpioPin, double> phaseWidths 
            = new ConcurrentDictionary<IGpioPin, double>(new EqualityComparer<IGpioPin, int>((pinA) => pinA.BcmPinNumber));

        public PwmThread(double phaseWidthCycleMilliseconds = 10)
        {
            this.phaseWidthCycleMilliseconds = phaseWidthCycleMilliseconds;
            thread = new Thread(RunPwmThread);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        public double? GetPhaseWidth(IGpioPin pinConfig)
        {
            double phaseWidth;
            if (phaseWidths.TryGetValue(pinConfig, out phaseWidth))
            {
                return phaseWidth;
            }
            return null;
        }

        public void SetPhaseWidth(IGpioPin pinConfig, double phaseWidth)
        {
            phaseWidths.AddOrUpdate(pinConfig, phaseWidth, delegate { return phaseWidth; });
        }

        public void ClearPhaseWidth(IGpioPin pinConfig)
        {
            phaseWidths.TryRemove(pinConfig, out _);
        }

        private void RunPwmThread()
        {
            var sw = new Stopwatch();
            while (!disposedValue)
            {
                var list = phaseWidths
                    .ToArray()
                    .OrderBy(e => e.Value)
                    .Select(e => (e.Key, Value: e.Value * phaseWidthCycleMilliseconds))
                    .ToArray();
                var nextIndex = 0;
                foreach (var e in list)
                {
                    e.Key.Write(true);
                }
                sw.Restart();
                while (sw.ElapsedMilliseconds < phaseWidthCycleMilliseconds)
                {
                    while (nextIndex < list.Length && list[nextIndex].Value < sw.ElapsedMilliseconds)
                    {
                        list[nextIndex].Key.Write(false);
                        nextIndex++;
                    }
                }
                sw.Stop();
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
