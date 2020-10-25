using BB8.Gamepad;
using System;
using System.Collections.Generic;

namespace BB8.Bluetooth
{
    internal interface IGamepadProvider
    {
        IObservable<EventedGamepad> GamepadStateChanges { get; }
    }
}