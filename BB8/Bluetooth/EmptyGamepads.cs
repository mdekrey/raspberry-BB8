using BB8.Gamepad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace BB8.Bluetooth
{
    public class EmptyGamepads : IGamepadProvider
    {
        public IObservable<EventedGamepad> GamepadStateChanges => Observable.Never<EventedGamepad>();
    }
}
