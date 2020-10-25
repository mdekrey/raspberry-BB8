namespace BB8.Gamepad
{
    public abstract record GamepadEventArgs { }
    public record ConnectedEventArgs : GamepadEventArgs
    {
        public static readonly GamepadEventArgs Instance = new ConnectedEventArgs();
    }
    public record AxisEventArgs(byte Axis, short Value) : GamepadEventArgs;
    public record ButtonEventArgs(byte Button, bool Pressed) : GamepadEventArgs;
    public record AxisConfigurationEventArgs(byte Axis) : GamepadEventArgs;
    public record ButtonConfigurationEventArgs(byte Button) : GamepadEventArgs;
    public record DisconnectedEventArgs : GamepadEventArgs
    {
        public static readonly GamepadEventArgs Instance = new DisconnectedEventArgs();
    }

    public record EventedGamepad(string name, GamepadState state, GamepadEventArgs[] eventArgs);
}