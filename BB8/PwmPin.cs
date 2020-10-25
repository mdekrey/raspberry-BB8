using Microsoft.AspNetCore.Routing.Matching;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace BB8
{
    internal static class PwmPin
    {
        public static IPwmPin ToPwmPin(this IGpioPin pin)
        {
            if (pin is GpioPin gpioPin)
            {
                if ((gpioPin.Capabilities & PinCapability.PWM) != 0)
                {
                    return new HardwarePwmPin(gpioPin);
                }
                else
                {
                    return new SoftwarePwmPin(gpioPin);
                }
            }
            else
            {
                return new FakePwmPin(pin);
            }
        }
    }

    internal class SoftwarePwmPin : IPwmPin
    {
        private GpioPin gpioPin;

        public SoftwarePwmPin(GpioPin gpioPin)
        {
            this.gpioPin = gpioPin;
            if (!gpioPin.IsInSoftPwmMode)
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
            this.gpioPin.PinMode = GpioPinDriveMode.PwmOutput;
        }

        public uint PwmRange => gpioPin.PwmRange;

        public uint PwmValue { get => (uint)gpioPin.PwmRegister; set => gpioPin.PwmRegister = (int)value; }
    }

    internal class FakePwmPin : IPwmPin
    {
        private IGpioPin pin;

        public FakePwmPin(IGpioPin pin)
        {
            this.pin = pin;
        }

        public uint PwmRange { get; set; }

        public uint PwmValue { get; set; }
    }
}