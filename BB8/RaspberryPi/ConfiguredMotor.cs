using BB8.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB8.RaspberryPi
{
    public record ConfiguredMotor
    {
        public Motor Motor { get; }
        public MotorConfiguration Configuration { get; }

        public ConfiguredMotor(Motor motor, MotorConfiguration configuration)
        {
            Motor = motor;
            Configuration = configuration;
        }

        public static ConfiguredMotor ToMotor(MotorConfiguration configuration) =>
            new(new Motor(), configuration);
    }

}
