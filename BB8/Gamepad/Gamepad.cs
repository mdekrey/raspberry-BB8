using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BB8.Gamepad
{

    public class Gamepad : IGamepad
    {
        public IObservable<(GamepadState state, GamepadEventArgs eventArgs)> GamepadStateChanged { get; }

        public static string[] GetDeviceNames()
        {
            return Directory.GetFiles("/dev/input", "js*");
        }

        public Gamepad(string deviceFile = "/dev/input/js0")
        {
            if (!File.Exists(deviceFile))
                throw new ArgumentException(nameof(deviceFile), $"The device {deviceFile} does not exist");

            this.GamepadStateChanged =
                System.Reactive.Linq.Observable.Create(ProcessMessages(deviceFile))
                    .Scan((state: GamepadState.Empty, eventArgs: ConnectedEventArgs.Instance), (prev, next) => (GamepadState.Apply(prev.state, next), next))
                    .DistinctUntilChanged()
                    .Replay(1)
                    .RefCount();
        }

        private Func<IObserver<GamepadEventArgs>, Action> ProcessMessages(string deviceFile)
        {
            CancellationTokenSource cancellationTokenSource = new();
            return observer =>
            {
                // Create the Task that will constantly read the device file, process its bytes and fire events accordingly
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        using var fs = new FileStream(deviceFile, FileMode.Open);

                        var token = cancellationTokenSource.Token;
                        var message = new byte[8];

                        while (!token.IsCancellationRequested)
                        {
                            // Read chunks of 8 bytes at a time.
                            fs.Read(message, 0, 8);

                            if (HasConfiguration(message))
                                ProcessConfiguration(message, observer.OnNext);

                            ProcessValues(message, observer.OnNext);
                        }
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                }, TaskCreationOptions.LongRunning);

                return cancellationTokenSource.Cancel;
            };
        }

        private void ProcessConfiguration(byte[] message, Action<GamepadEventArgs> dispatchEvent)
        {
            var address = GetAddress(message);
            if (IsButton(message))
                dispatchEvent(new ButtonConfigurationEventArgs(address));
            else if (IsAxis(message))
                dispatchEvent(new AxisConfigurationEventArgs(address));
        }

        private void ProcessValues(byte[] message, Action<GamepadEventArgs> dispatchEvent)
        {
            var address = GetAddress(message);
            if (IsButton(message))
                dispatchEvent(new ButtonEventArgs(address, IsButtonPressed(message)));
            else if (IsAxis(message))
                dispatchEvent(new AxisEventArgs(address, GetAxisValue(message)));
        }


        public static bool HasConfiguration(byte[] message)
        {
            return IsFlagSet(message[6], 0x80); // 0x80 in byte 6 means it has Configuration information
        }

        public static bool IsButton(byte[] message)
        {
            return IsFlagSet(message[6], 0x01); // 0x01 in byte 6 means it is a Button
        }

        public static bool IsAxis(byte[] message)
        {
            return IsFlagSet(message[6], 0x02); // 0x01 in byte 6 means it is a Axis
        }

        public static bool IsButtonPressed(byte[] message)
        {
            return message[4] == 0x01; // byte 4 contains the status (0x01 means pressed, 0x00 means released)
        }

        public static byte GetAddress(byte[] message)
        {
            return message[7]; // Address is stored in byte 7
        }

        public static short GetAxisValue(byte[] message)
        {
            return BitConverter.ToInt16(message, 4); // Value is stored in bytes 4 and 5
        }

        /// <summary>
        /// Checks if bits that are set in flag are set in value.
        /// </summary>
        private static bool IsFlagSet(byte value, byte flag)
        {
            byte c = (byte)(value & flag);
            return c == flag;
        }

    }
}
