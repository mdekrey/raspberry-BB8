using System;

namespace BB8.Gamepad
{
    public interface IGamepad
    {
        IObservable<GamepadState> GamepadStateChanged { get; }
    }
}