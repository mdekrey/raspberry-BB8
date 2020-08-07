using BB8.PhaseWidthModulation;
using System;
using System.Diagnostics;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.WiringPi;

namespace BB8
{
    // GPIO Pins
    // 4 - 5V
    // 6 - Ground
    // 7 - GPIO 4 - PWM motor 2 (orange)
    // 9 - Ground - DIR_EN (green)
    // 11 - GPIO 17 - Serial Data (yellow)
    // 13 - GPIO 27 - Serial Latch (blue)
    // 15 - GPIO 22 - Clock (grey)

    public class Program
    {
        public static void Main(string[] args)
        {
            var originalColor = Console.ForegroundColor;
            
            Pi.Init<BootstrapWiringPi>();

            //var sensorPin = Pi.Gpio[18];

            var pwmOutput = Pi.Gpio[4];
            var serialDataPin = Pi.Gpio[17];
            var serialLatchPin = Pi.Gpio[27];
            var serialClockPin = Pi.Gpio[22];

            //var connection = new GpioConnection(new GpioConnectionSettings
            //{
            //    PollInterval = TimeSpan.FromMilliseconds(0.001)
            //}, sensorPin);

            var sw = new Stopwatch();
            //connection.PinStatusChanged += (sender, statusArgs) => 
            //{
            //    if (statusArgs.Configuration.Pin == sensorPin.Pin)
            //    {
            //        Console.Write(statusArgs.Enabled ? 1 : 0);
            //    }
            //};

            // bit 5 - motor header 1/2 pin 5
            // bit 2 - motor header 1/2 pin 4
            sw.Start();
            try
            {
                var serial = new SerialDigitizer(data: serialDataPin, latch: serialLatchPin, clock: serialClockPin, bitCount: 8);
                using (var pwm = new PwmThread())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.WriteLine(DateTime.Now.ToString());

                    double pw = 1;
                    pwm.SetPhaseWidth(pwmOutput, pw);
                    ConsoleKey key;
                    while ((key = Console.ReadKey().Key) != ConsoleKey.Enter)
                    {
                        if (key == ConsoleKey.LeftArrow)
                        {
                            serial.WriteData(2).Wait();
                        }
                        else if (key == ConsoleKey.RightArrow)
                        {
                            serial.WriteData(0x10).Wait();
                        }
                        else if (key == ConsoleKey.DownArrow)
                        {
                            pwm.SetPhaseWidth(pwmOutput, pw -= 0.01);
                        }
                        else if (key == ConsoleKey.UpArrow)
                        {
                            pwm.SetPhaseWidth(pwmOutput, pw += 0.01);
                        }
                        Console.WriteLine(pw);
                    }
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ending");

                    serial.WriteData(0).Wait();
                    // Give the Pi the time to process the signal so it actually shuts down
                    Thread.Sleep(1000);

                }
            }
            finally
            {
                Console.ForegroundColor = originalColor;
            }
        }
    }
}
