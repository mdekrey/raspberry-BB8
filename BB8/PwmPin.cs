using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace BB8
{
    internal static class PwmPin
    {
        public static IPwmPin ToPwmPin(this IGpioPin pin)
        {
            var gpioPin = (GpioPin)pin;
            if ((gpioPin.Capabilities & PinCapability.PWM) != 0)
            {
                return new HardwarePwmPin(gpioPin);
            }
            else
            {
                return new SoftwarePwmPin(gpioPin);
            }
        }
    }

    internal class SoftwarePwmPin : IPwmPin
    {
        private GpioPin gpioPin;

        public SoftwarePwmPin(GpioPin gpioPin)
        {
            this.gpioPin = gpioPin;
            gpioPin.StartSoftPwm(0, 1000);
        }

        public uint PwmRange => (uint)gpioPin.SoftPwmRange;
        public uint PwmValue { get => (uint)gpioPin.SoftPwmValue; set => gpioPin.SoftPwmValue = (int)value; }
    }

    internal class HardwarePwmPin : IPwmPin
    {
        private GpioPin gpioPin;

        public HardwarePwmPin(GpioPin gpioPin)
        {
            this.gpioPin = gpioPin;
        }

        public uint PwmRange => gpioPin.PwmRange;

        public uint PwmValue { get => (uint)gpioPin.PwmRegister; set => gpioPin.PwmRegister = (int)value; }
    }
}