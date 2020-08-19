using BB8.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB8.RaspberryPi
{
    public record MotorConfiguration
    {
        public int PwmGpioPin { get; init; }
        public byte ForwardBit { get; init; }
        public byte BackwardBit { get; init; }
        public double Buffer { get; init; } = 0.05;
        public double DeadZone { get; init; } = 0.1;
        public double BoostFactor { get; init; } = 1.0;

        public byte ToFlag(MotorState state) =>
            state.Direction switch
            {
                MotorDirection.Forward => (byte)(1 << ForwardBit),
                MotorDirection.Backward => (byte)(1 << BackwardBit),
                _ => 0
            };

        public double ToSpeed(MotorState state) =>
            state.Direction switch
            {
                MotorDirection.Stopped => 0,
                _ when state.Speed < Buffer => 0,
                _ => Math.Clamp(((1 - DeadZone) * state.Speed + DeadZone) * BoostFactor, 0.0, 1.0),
            };
    }
}
