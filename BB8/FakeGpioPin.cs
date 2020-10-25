using System;
using Unosquare.RaspberryIO.Abstractions;

namespace BB8
{
    internal class FakeGpioPin : IGpioPin
    {
        public BcmPin BcmPin => (BcmPin)BcmPinNumber;

        public int BcmPinNumber { get; init; }

        public int PhysicalPinNumber => BcmPinNumber;

        public GpioHeader Header => GpioHeader.None;

        public GpioPinDriveMode PinMode { get; set; }
        public GpioPinResistorPullMode InputPullMode { get; set; }
        public bool Value { get; set; }

        public bool Read() => false;

        public void RegisterInterruptCallback(EdgeDetection edgeDetection, Action callback)
        {
        }

        public void RegisterInterruptCallback(EdgeDetection edgeDetection, Action<int, int, uint> callback)
        {
        }

        public void RemoveInterruptCallback(EdgeDetection edgeDetection, Action callback)
        {
        }

        public void RemoveInterruptCallback(EdgeDetection edgeDetection, Action<int, int, uint> callback)
        {
        }

        public bool WaitForValue(GpioPinValue status, int timeOutMillisecond)
        {
            return false;
        }

        public void Write(bool value)
        {
        }

        public void Write(GpioPinValue value)
        {
        }
    }
}