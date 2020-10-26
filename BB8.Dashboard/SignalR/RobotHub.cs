using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using static BB8.Services.RoboDiagnostics;

namespace BB8.Dashboard.SignalR
{
    public class RobotHub : Hub
    {
        private readonly RoboDiagnosticsClient client;

        public RobotHub(RoboDiagnosticsClient client)
        {
            this.client = client;
        }

        public ChannelReader<BB8.Services.ControllerReply> GetController(CancellationToken cancellationToken) =>
            Observable.Return(new Services.EmptyRequest())
                .AsGrpc(client.GetController)
                .AsSignalRChannel(cancellationToken);


        public ChannelReader<BB8.Services.MotorStateReply> GetMotorStates(CancellationToken cancellationToken) =>
            Observable.Return(new Services.EmptyRequest())
                .AsGrpc(client.GetMotorState)
                .AsSignalRChannel(cancellationToken);

        public async Task<BB8.Services.GpioConfigurationReply> GetGpioConfiguration() =>
            await client.GetGpioConfigurationAsync(new Services.EmptyRequest()).ResponseAsync;

        public ChannelReader<BB8.Services.GpioStateReply> GetGpioState(CancellationToken cancellationToken) =>
            Observable.Return(new Services.EmptyRequest())
                .AsGrpc(client.GetGpioState)
                .AsSignalRChannel(cancellationToken);

        public ChannelReader<BB8.Services.UnitConfigurationReply> GetUnitConfiguration(CancellationToken cancellationToken) =>
            Observable.Return(new Services.EmptyRequest())
                .AsGrpc(client.GetUnitConfiguration)
                .AsSignalRChannel(cancellationToken);

        public ChannelReader<BB8.Services.MotionConfigurationMessage> GetMotionConfiguration(CancellationToken cancellationToken) =>
            Observable.Return(new Services.EmptyRequest())
                .AsGrpc(client.GetMotionConfiguration)
                .AsSignalRChannel(cancellationToken);

        public async Task<BB8.Services.MotionConfigurationMessage> SetMotionConfiguration(BB8.Services.MotionConfigurationMessage message) =>
            await client.SetMotionConfigurationAsync(message).ResponseAsync;
    }
}
