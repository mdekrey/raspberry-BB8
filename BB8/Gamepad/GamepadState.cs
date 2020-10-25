using System.Collections.Immutable;
using System.Linq;

namespace BB8.Gamepad
{
    public record GamepadState(bool Connected, ImmutableDictionary<byte, bool> Buttons, ImmutableDictionary<byte, short> Axes, string GamepadName = "default")
    {
        public static readonly GamepadState Empty = new (Connected: true, ImmutableDictionary<byte, bool>.Empty, ImmutableDictionary<byte, short>.Empty);

        public short Axis(byte axis) =>
            Axes.ContainsKey(axis) ? Axes[axis] : (short)0;

        public bool Button(byte button) =>
            Buttons.ContainsKey(button) ? Buttons[button] : false;

        public static GamepadState Apply(GamepadState previous, GamepadEventArgs ev) =>
            ev switch
            {
                AxisEventArgs(var axis, var value) => previous with { Connected = true, Axes = previous.Axes.SetItem(axis, value) },
                ButtonEventArgs(var button, var pressed) => previous with { Connected = true, Buttons = previous.Buttons.SetItem(button, pressed) },

                AxisConfigurationEventArgs(var axis) => previous with { Connected = true, Axes = previous.Axes.SetItem(axis, 0) },
                ButtonConfigurationEventArgs(var button) => previous with { Connected = true, Buttons = previous.Buttons.SetItem(button, false) },

                DisconnectedEventArgs _ => Empty with { Connected = false },

                _ => previous,
            };

        public override string ToString() =>
            Connected 
                ? $"{GamepadName}: {string.Join(' ', Axes.Select(axis => $"A{axis.Key}: {axis.Value,6}"))} {(Buttons.Any() ? "B" : "")}{string.Join("", Buttons.OrderBy(button => button.Key).Select(button => button.Value ? 1 : 0))}"
                : $"{GamepadName}: Disconnected";
    }
}