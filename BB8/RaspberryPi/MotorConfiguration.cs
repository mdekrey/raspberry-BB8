using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB8.RaspberryPi
{
    public record MotorConfiguration
    {
        public int PwmGpioPin { get; set; }
        public byte ForwardBit { get; set; }
        public byte BackwardBit { get; set; }
    }
}
