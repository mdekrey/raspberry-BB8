using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB8
{
    public class MotorConfiguration
    {
        public MotorSerialControlPins Serial { get; set; }

        public int TestMotor { get; set; }

        public int GpioPwmMotor { get; set; }
    }
}
