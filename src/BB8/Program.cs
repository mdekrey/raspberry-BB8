using Raspberry.IO.GeneralPurpose;
using System;
using System.Collections.Generic;
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

            GpioConnection connection = new GpioConnection(pin2Sensor);
            connection.PinStatusChanged += (sender, statusArgs)
                                => Console.WriteLine("Pin changed {0}", statusArgs.Configuration.Name);

            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;

                Task.WaitAll(Hello("1"), Hello("2"), Hello("3"), Hello("4"));
                
                Console.WriteLine(DateTime.Now.ToString());

                Console.ReadKey();
            }
            finally
            {
                connection.Close();
                Console.ForegroundColor = originalColor;
            }
        }

        private static async Task Hello(string v)
        {
            await Task.Yield();

            Speak(v);

            Thread.Sleep(1000);

            Speak(v);

            await Task.Delay(1000).ConfigureAwait(false);

            Speak(v);
        }

        private static void Speak(string v)
        {
            Console.WriteLine("Hello " + v);
        }
    }
}
