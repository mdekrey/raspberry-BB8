using Raspberry.IO.GeneralPurpose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BB8
{
    public class SerialDigitizer
    {
        // The chips involved support up to 100 MHz, though our polling code only supports 1 KHz. 
        // We could reduce this more if we don't need to read and got rid of Thread.Sleep.
        const int sleepDelay = 1;

        private readonly int bitCount;
        private readonly OutputPinConfiguration clockPin;
        private readonly OutputPinConfiguration dataPin;
        private readonly OutputPinConfiguration latchPin;
        private readonly GpioConnection connection;

        public SerialDigitizer(GpioConnection connection, OutputPinConfiguration data, OutputPinConfiguration clock, OutputPinConfiguration latch, int bitCount)
        {
            this.connection = connection;
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
                connection[latchPin] = false;
                Thread.Sleep(sleepDelay);
                for (int bit = bitCount - 1; bit >= 0; --bit)
                {
                    connection[dataPin] = (data & (1 << bit)) != 0;
                    connection[clockPin] = true;
                    Thread.Sleep(sleepDelay);
                    connection[clockPin] = false;
                    Thread.Sleep(sleepDelay);
                }
                connection[latchPin] = true;
                Thread.Sleep(sleepDelay);
            });
            result.Start();
            return result;
        }

    }
}
