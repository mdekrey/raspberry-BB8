using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO.Abstractions;

namespace BB8
{
    public class SerialDigitizer
    {
        private readonly int bitCount;
        private readonly IGpioPin clockPin;
        private readonly IGpioPin dataPin;
        private readonly IGpioPin latchPin;

        public SerialDigitizer(IGpioPin data, IGpioPin clock, IGpioPin latch, int bitCount)
        {
            data.PinMode = GpioPinDriveMode.Output;
            latch.PinMode = GpioPinDriveMode.Output;
            clock.PinMode = GpioPinDriveMode.Output;
            this.dataPin = data;
            this.clockPin = clock;
            this.latchPin = latch;
            this.bitCount = bitCount;
        }

        public Task WriteDataAsync(int data) =>
            // Running under Mono, this needed a sleepDelay of >= 1 to work, but .NET 5 doesn't seem to require it.
            Task.Factory.StartNew(() =>
            {
                latchPin.Write(false);
                //Thread.Sleep(sleepDelay);
                for (int bit = bitCount - 1; bit >= 0; --bit)
                {
                    dataPin.Write((data & (1 << bit)) != 0);
                    clockPin.Write(true);
                    //Thread.Sleep(sleepDelay);
                    clockPin.Write(false);
                    //Thread.Sleep(sleepDelay);
                }
                latchPin.Write(true);
                //Thread.Sleep(sleepDelay);
            }, TaskCreationOptions.LongRunning);
    }
}
