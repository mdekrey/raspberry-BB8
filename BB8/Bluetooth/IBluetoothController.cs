using System.Threading;
using System.Threading.Tasks;

namespace BB8.Bluetooth
{
    interface IBluetoothController
    {
        Task<bool> ConnectAsync(string macAddress, CancellationToken cancellationToken = default);
        Task<bool> DisconnectAsync(string macAddress, CancellationToken cancellationToken = default);
        Task<string[]> GetConnectedBluetoothDevicesAsync(CancellationToken cancellationToken = default);

        Task<string?> GetMacAddress(string deviceHandle, CancellationToken cancellationToken = default);
    }
}