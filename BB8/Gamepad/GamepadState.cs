using System.Collections.Immutable;
using System.Linq;

namespace BB8.Gamepad
{
    public record GamepadState(ImmutableDictionary<byte, bool> Buttons, ImmutableDictionary<byte, short> Axis)
    {
        public static readonly GamepadState Empty = new (ImmutableDictionary<byte, bool>.Empty, ImmutableDictionary<byte, short>.Empty);

        public static GamepadState Apply(GamepadState previous, GamepadEventArgs ev) =>
            ev switch
            {
                AxisEventArgs(var axis, var value) => previous with { Axis = previous.Axis.SetItem(axis, value) },
                ButtonEventArgs(var button, var pressed) => previous with { Buttons = previous.Buttons.SetItem(button, pressed) },

                AxisConfigurationEventArgs(var axis) => previous with { Axis = previous.Axis.SetItem(axis, 0) },
                ButtonConfigurationEventArgs(var button) => previous with { Buttons = previous.Buttons.SetItem(button, false) },

                _ => previous,
            };

        public override string ToString() =>
            $"{string.Join(' ', Axis.Select(axis => $"A{axis.Key}: {axis.Value,6}"))} B{string.Join("", Buttons.Select(button => button.Value ? 1 : 0))}";
    }
}