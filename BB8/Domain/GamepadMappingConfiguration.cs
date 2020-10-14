using BB8.Gamepad;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB8.Domain
{
    using GamepadMapping = Dictionary<string, GamepadMappingConfiguration.Mapping>;

    public class GamepadMappingConfiguration
    {
        public record Device
        {
            public bool Default { get; init; }
            public string? Bluetooth { get; init; }

            public string Name => Bluetooth ?? "default";
        }

        public record MappingAxis
        {
            public double Positive { get; init; }
            public double Negative { get; init; }
        }

        public record Mapping
        {
            public byte? Button { get; init; }
            public byte? Axis { get; init; }

            public short? ToButtonThreshold { get; init; }
            public MappingAxis? AxisFactors { get; init; }
            public bool Invert { get; init; }
        }

        public record DeviceMapping
        {
            public Device Device { get; init; } = new() { Default = true };
            public GamepadMapping Mapping { get; init; } = new GamepadMapping();
        }

        public DeviceMapping[] Devices { get; init; } = Array.Empty<DeviceMapping>();
    }

    public record GamepadMappedState(ImmutableDictionary<string, bool> Buttons, ImmutableDictionary<string, double> Axes, string GamepadName = "default")
    {
        public double Axis(string axis) =>
            Axes.ContainsKey(axis) ? Axes[axis] : 0;

        public bool Button(string button) =>
            Buttons.ContainsKey(button) ? Buttons[button] : false;


        public override string ToString() =>
            $"{GamepadName}: {string.Join(", ", Axes.Select(axis => $"{axis.Key}: {axis.Value:0.0000}").Concat(Buttons.Select(button => $"{button.Key}: {button.Value,6}")))}";
    }

    public abstract record MappedGamepadEventArgs(GamepadEventArgs Original) { }
    public record MappedAxisEventArgs(GamepadEventArgs Original, string Axis, double Value) : MappedGamepadEventArgs(Original)
    {
        public override string ToString() =>
            $"Axis: {Axis} {Value:0.000}";
    }
    public record MappedButtonEventArgs(GamepadEventArgs Original, string Button, bool Pressed) : MappedGamepadEventArgs(Original)
    {
        public override string ToString() =>
            $"Button: {Button} {Pressed}";
    }

    public record EventedMappedGamepad(GamepadMappedState state, MappedGamepadEventArgs[] eventArgs);

    public static class GamepadMappingExtensions
    {
        public static GamepadMappedState Map(this GamepadState state, GamepadMapping mappings) =>
            new GamepadMappedState(
                GamepadName: state.GamepadName,
                Buttons: mappings.Aggregate(ImmutableDictionary<string, bool>.Empty, (prev, mapping) => 
                    mapping.Value switch
                    {
                        { Button: byte button, Invert: var invert } => prev.SetItem(mapping.Key, state.Button(button) != invert),
                        { Axis: byte axis, ToButtonThreshold: short threshold, Invert: var invert } => prev.SetItem(mapping.Key, (state.Axis(axis) > threshold) != invert),
                        _ => prev
                    }),
                Axes: mappings.Aggregate(ImmutableDictionary<string, double>.Empty, (prev, mapping) =>
                    mapping.Value switch
                    {
                        { Axis: byte axis, ToButtonThreshold: null, AxisFactors: var axisFactors, Invert: var invert } => prev.SetItem(mapping.Key, axisFactors.Map(state.Axis(axis)) * (invert ? -1 : 1)),
                        _ => prev
                    })
            );

        public static IEnumerable<MappedGamepadEventArgs> Map(this IEnumerable<GamepadEventArgs> original, GamepadMapping mappings) =>
            original.Select(e => e.Map(mappings)).Where(e => e != null)!;

        public static MappedGamepadEventArgs? Map(this GamepadEventArgs original, GamepadMapping mappings) =>
            mappings
                .Select(mapping => (original, mapping.Value) switch
                {
                    (AxisEventArgs args, { Axis: byte axis, ToButtonThreshold: short threshold, Invert: var invert }) when axis == args.Axis => 
                        (MappedGamepadEventArgs)new MappedButtonEventArgs(original, mapping.Key, (args.Value > threshold) != invert),
                    (ButtonEventArgs args, { Button: byte button, Invert: var invert }) when button == args.Button =>
                        (MappedGamepadEventArgs)new MappedButtonEventArgs(original, mapping.Key, args.Pressed != invert),

                    (AxisEventArgs { Value: var value, Axis: byte axis1 }, { Axis: byte axis2, AxisFactors: var axisFactors, Invert: var invert }) when axis1 == axis2 =>
                        (MappedGamepadEventArgs)new MappedAxisEventArgs(original, mapping.Key, axisFactors.Map(value) * (invert ? -1 : 1)),
                    _ => null
                })
                .Where(v => v != null)
                .FirstOrDefault();

        static double Map(this GamepadMappingConfiguration.MappingAxis? mappingAxis, short value) =>
            Math.Clamp(
            (value, mappingAxis) switch
            {
                (_, null) => value,
                (> 0, { Positive: var positive }) => value * positive,
                (< 0, { Negative: var negative }) => value * negative,
                _ => value
            }, -1, 1);

        public static IObservable<EventedMappedGamepad> Select(
            this IObservable<EventedGamepad> gamepadStateChanges, GamepadMappingConfiguration.DeviceMapping[] deviceMappings
        ) => gamepadStateChanges
            .Select((gamepad) => (gamepad.state, gamepad.eventArgs, mapping: deviceMappings.FirstOrDefault(d => StringComparer.OrdinalIgnoreCase.Equals(d.Device.Name, gamepad.state.GamepadName))?.Mapping!))
            .Where(gamepad => gamepad.mapping != null)
            .Select((gamepad) => new EventedMappedGamepad(state: gamepad.state.Map(gamepad.mapping), eventArgs: gamepad.eventArgs.Map(gamepad.mapping).ToArray()));
    }
}
