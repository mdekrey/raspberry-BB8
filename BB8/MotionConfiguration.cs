using BB8.RaspberryPi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB8
{
    public record MotionConfiguration
    {
        public MotorSerialControlPins Serial { get; init; } = new MotorSerialControlPins();

        public List<MotorConfiguration?> Motors { get; init; } = new();

    }
}
