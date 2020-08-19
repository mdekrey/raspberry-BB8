using Swan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BB8.Bluetooth
{
    class BluetoothController : IBluetoothController
    {
        private const string HciToolCommand = "hcitool";
        private const string BcCommand = "bluetoothctl";
        private const string UdevAdminCommand = "udevadm";

        private static readonly Regex MacAddressPattern = new Regex("([0-9A-F][0-9A-F]:){5}[0-9A-F][0-9A-F]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex MacAddressUdevPattern = new Regex("ATTRS{uniq}==\"(?<mac>([0-9A-Fa-f][0-9A-Fa-f]:){5}[0-9A-Fa-f][0-9A-Fa-f])\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Swan.ProcessRunner
        // ```hcitool con``` - lists connected devices
        // ```bluetoothctl connect 00:0B:E4:81:35:F5``` - tries to connect to a specified device
        // ```bluetoothctl disconnect 00:0B:E4:81:35:F5``` - tries to disconnect from a specified device
        //var controllers = await Pi.Bluetooth.ListControllers();
        // await Pi.Bluetooth.Connect(controllers.First(), "00:0B:E4:81:35:F5");
        //await Pi.Bluetooth.DeviceInfo("00:0B:E4:81:35:F5");
        //return;

        public async Task<string[]> GetConnectedBluetoothDevicesAsync(CancellationToken cancellationToken = default)
        {
            var output = await ProcessRunner.GetProcessOutputAsync(HciToolCommand, $"con", null, cancellationToken)
                    .ConfigureAwait(false);

            return MacAddressPattern.Matches(output).Select(match => match.Value).ToArray();
        }

        public async Task<bool> ConnectAsync(string macAddress, CancellationToken cancellationToken = default)
        {
            if (!MacAddressPattern.IsMatch(macAddress))
                throw new InvalidOperationException($"An invalid MAC address was provided: {macAddress}");

            var result = await ProcessRunner.GetProcessResultAsync(BcCommand, $"connect {macAddress}", cancellationToken)
                    .ConfigureAwait(false);
            return result.ExitCode == 0;
        }

        public async Task<bool> DisconnectAsync(string macAddress, CancellationToken cancellationToken = default)
        {
            if (!MacAddressPattern.IsMatch(macAddress))
                throw new InvalidOperationException($"An invalid MAC address was provided: {macAddress}");

            var result = await ProcessRunner.GetProcessResultAsync(BcCommand, $"disconnect {macAddress}", cancellationToken)
                    .ConfigureAwait(false);
            return result.ExitCode == 0;
        }

        public async Task<string?> GetMacAddress(string deviceHandle, CancellationToken cancellationToken = default)
        {
            var output = await ProcessRunner.GetProcessOutputAsync(UdevAdminCommand, $"info -a {deviceHandle.Replace("/dev", "/sys/class")}", null, cancellationToken)
                    .ConfigureAwait(false);
            var result = MacAddressUdevPattern.Match(output)?.Groups["mac"]?.Value;
            if (result is not { Length: > 0 })
                Console.WriteLine(BitConverter.ToString(Encoding.ASCII.GetBytes(output)).Replace("-", string.Empty));
            return result;
        }
    }
}
