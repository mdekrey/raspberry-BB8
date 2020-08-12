using System;
using System.Collections.Immutable;
using System.Linq;

namespace BB8.Gamepad
{
    public abstract record GamepadEventArgs
    {
    }
    public record AxisEventArgs(byte Axis, short Value) : GamepadEventArgs;
    public record ButtonEventArgs(byte Button, bool Pressed) : GamepadEventArgs;
    public record AxisConfigurationEventArgs(byte Axis) : GamepadEventArgs;
    public record ButtonConfigurationEventArgs(byte Button) : GamepadEventArgs;

    public record GamepadState(ImmutableDictionary<byte, bool> Buttons, ImmutableDictionary<byte, short> Axis)
    {
        public static readonly GamepadState Empty = new (ImmutableDictionary<byte, bool>.Empty, ImmutableDictionary<byte, short>.Empty);

        public GamepadState HandleGamepadEvent(object sender, GamepadEventArgs ev)
        {
            return ev switch
            {
                AxisEventArgs(var axis, var value) => this with { Axis = Axis.SetItem(axis, value) },
                ButtonEventArgs(var button, var pressed) => this with { Buttons = Buttons.SetItem(button, pressed) },

                AxisConfigurationEventArgs(var axis) => this with { Axis = Axis.SetItem(axis, 0) },
                ButtonConfigurationEventArgs(var button) => this with { Buttons = Buttons.SetItem(button, false) },

                _ => this,
            };
        }

        public override string ToString()
        {
            return $"{string.Join(' ', Axis.Select(axis => $"A{axis.Key}: {axis.Value,6}"))} B{string.Join("", Buttons.Select(button => button.Value ? 1 : 0))}";
        }
    }


    public interface IGamepadController
    {
        IObservable<GamepadState> GamepadStateChanged { get; }
    }
}