namespace BB8
{
    public record MotorDriveState(RaspberryPi.ConfiguredMotor motor, Domain.MotorState state);
}
