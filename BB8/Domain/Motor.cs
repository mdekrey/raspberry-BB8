using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace BB8.Domain
{
    public enum MotorDirection
    {
        Stopped,
        Forward,
        Backward
    }

    public record MotorState
    {
        public MotorDirection Direction { get; init; }
        public double Speed { get; init; }

        public bool IsValid =>
            Direction switch
            {
                MotorDirection.Stopped => Speed == 0,
                _ => Speed > 0 && Speed <= 1
            };

        public override string ToString() =>
            Direction switch
            {
                MotorDirection.Stopped => "00.00",
                MotorDirection.Forward => $"+{Speed:0.00}",
                MotorDirection.Backward => $"-{Speed:0.00}",
                _ => throw new NotSupportedException()
            };
    }

    public sealed class Motor : IObservable<MotorState>, IDisposable
    {
        private readonly BehaviorSubject<MotorState> state = new(new() { Direction = MotorDirection.Stopped });
        private bool disposedValue;

        public IDisposable Subscribe(IObserver<MotorState> observer)
        {
            return ((IObservable<MotorState>)state).Subscribe(observer);
        }

        public MotorState Current => state.Value;

        public void Update(MotorState motorState)
        {
            if (motorState.Equals(state.Value))
                return;
            if (!motorState.IsValid)
                throw new InvalidOperationException("Provided motor state not valid.");
            state.OnNext(motorState);
        }

        #region Dispose
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    state.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
