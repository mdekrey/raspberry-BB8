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
        // The chips involved support up to 100 MHz, though our polling code only supports 1 KHz. 
        // We could reduce this more if we don't need to read and got rid of Thread.Sleep.
        const int sleepDelay = 1;

        private readonly int bitCount;
        private readonly IGpioPin clockPin;
        private readonly IGpioPin dataPin;
        private readonly IGpioPin latchPin;

        public SerialDigitizer(IGpioPin data, IGpioPin clock, IGpioPin latch, int bitCount)
        {
            this.dataPin = data;
            this.clockPin = clock;
            this.latchPin = latch;
            this.bitCount = bitCount;
        }

        public Task WriteData(int data)
        {
            var result = new Task(async () =>
            {
                await Task.Yield();
                latchPin.Write(false);
                Thread.Sleep(sleepDelay);
                for (int bit = bitCount - 1; bit >= 0; --bit)
                {
                    dataPin.Write((data & (1 << bit)) != 0);
                    clockPin.Write(true);
                    Thread.Sleep(sleepDelay);
                    clockPin.Write(false);
                    Thread.Sleep(sleepDelay);
                }
                latchPin.Write(true);
                Thread.Sleep(sleepDelay);
            });
            result.Start();
            return result;
        }

    }
}
