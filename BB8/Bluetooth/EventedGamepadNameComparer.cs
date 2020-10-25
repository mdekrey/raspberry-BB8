using BB8.Gamepad;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BB8.Bluetooth
{
    internal class EventedGamepadNameComparer : IEqualityComparer<EventedGamepad>
    {
        public bool Equals(EventedGamepad? x, EventedGamepad? y) => (x == null && y == null) || (x?.name == y?.name);

        public int GetHashCode([DisallowNull] EventedGamepad obj) => obj.name.GetHashCode();
    }
}