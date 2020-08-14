namespace BB8.Gamepad
{
    public abstract record GamepadEventArgs { }
    public record AxisEventArgs(byte Axis, short Value) : GamepadEventArgs;
    public record ButtonEventArgs(byte Button, bool Pressed) : GamepadEventArgs;
    public record AxisConfigurationEventArgs(byte Axis) : GamepadEventArgs;
    public record ButtonConfigurationEventArgs(byte Button) : GamepadEventArgs;
}