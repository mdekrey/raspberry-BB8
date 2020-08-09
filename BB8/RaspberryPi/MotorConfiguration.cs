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
        public double DeadZone { get; init; } = 0.1;

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
                _ => (1 - DeadZone) * state.Speed + DeadZone,
            };
    }
}
