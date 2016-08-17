using BB8.PhaseWidthModulation;
using Raspberry.IO.GeneralPurpose;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BB8
{
    // GPIO Pins
    // 4 - 5V
    // 6 - Ground
    // 7 - GPIO 4 - PWM motor 1
    // 11 - GPIO 17 - Serial Data
    // 13 - GPIO 27 - Serial Latch
    // 15 - GPIO 22 - Clock

    public class Program
    {
        public static void Main(string[] args)
        {
            var originalColor = Console.ForegroundColor;

            // Thanks to http://blog.bennymichielsen.be/2016/03/14/getting-up-and-running-with-mono-and-raspberry-pi-3/
            var driver = GpioConnectionSettings.DefaultDriver;

            var sensorPin = ((ProcessorPin)18).Input();

            var pwmOutput = ((ProcessorPin)4).Output();
            var serialDataPin = ((ProcessorPin)17).Output();
            var serialLatchPin = ((ProcessorPin)27).Output();
            var serialClockPin = ((ProcessorPin)22).Output();

            var connection = new GpioConnection(new GpioConnectionSettings
            {
                PollInterval = TimeSpan.FromMilliseconds(0.001)
            }, sensorPin, serialDataPin, serialLatchPin, serialClockPin);
            var pwmConnection = new GpioConnection(pwmOutput);
            connection.Open();
            pwmConnection.Open();

            var sw = new Stopwatch();
            connection.PinStatusChanged += (sender, statusArgs) => 
            {
                if (statusArgs.Configuration.Pin == sensorPin.Pin)
                {
                    Console.Write(statusArgs.Enabled ? 1 : 0);
                }
            };

            sw.Start();
            try
            {
                var serial = new SerialDigitizer(connection, serialDataPin, serialLatchPin, serialClockPin, 8);
                using (connection)
                using (pwmConnection)
                using (var pwm = new PwmThread())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.WriteLine(DateTime.Now.ToString());

                    pwm.SetPhaseWidth(pwmConnection, 0.8);

                    while (Console.ReadKey().Key != ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        serial.WriteData(0xaa).Wait();
                        Console.WriteLine();
                    }
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ending");
                }
            }
            finally
            {
                Console.ForegroundColor = originalColor;
            }
        }
    }
}
