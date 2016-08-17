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
    // 7 - GPIO 4 - Clock
    // 11 - GPIO 17 - Serial Data
    // 13 - GPIO 27 - Serial Latch
    // 15 - GPIO 22 - PWM motor 1

    public class Program
    {
        public static void Main(string[] args)
        {
            var originalColor = Console.ForegroundColor;

            // Thanks to http://blog.bennymichielsen.be/2016/03/14/getting-up-and-running-with-mono-and-raspberry-pi-3/
            var driver = GpioConnectionSettings.DefaultDriver;

            // Reads pin 11, labelled "GPIO 17" on the pin layout
            var pin2 = ConnectorPin.P1Pin11.ToProcessor();
            var pin2Sensor = pin2.Input();
            Console.WriteLine((int)pin2); // shows "17" to match the GPIO number

            var pwmOutput = ((ProcessorPin)4).Output();

            var connection = new GpioConnection(pin2Sensor);
            var pwmConnection = new GpioConnection(pwmOutput);

            var sw = new Stopwatch();
            connection.PinStatusChanged += (sender, statusArgs)
                                => Console.WriteLine("Pin changed {0}", sw.ElapsedMilliseconds);
            sw.Start();
            try
            {
                using (var pwm = new PwmThread())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.WriteLine(DateTime.Now.ToString());

                    pwm.SetPhaseWidth(pwmConnection, 0.5);

                    Console.ReadKey();
                }
            }
            finally
            {
                connection.Close();
                Console.ForegroundColor = originalColor;
            }
        }
    }
}
