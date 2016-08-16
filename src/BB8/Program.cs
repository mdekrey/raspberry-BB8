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
    // 7 - Clock
    // 11 - Serial Data
    // 13 - Serial Latch
    // 15 - PWM motor 1

    public class Program
    {
        public static void Main(string[] args)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;

            Task.WaitAll(Hello("1"), Hello("2"), Hello("3"), Hello("4"));

            Console.WriteLine("Machine: {0}, OS: {1}, Processors: {2}",
                     Environment.GetEnvironmentVariable("COMPUTERNAME"),
                     Environment.GetEnvironmentVariable("OS"),
                     Environment.ProcessorCount);

            Console.WriteLine(DateTime.Now.ToString());
            Console.ForegroundColor = originalColor;
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
