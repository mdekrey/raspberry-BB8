using BB8.Domain;
using BB8.RaspberryPi;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BB8.Services
{
    public class RoboDiagnosticsService : RoboDiagnostics.RoboDiagnosticsBase
    {
        private readonly ILogger<RoboDiagnosticsService> _logger;
        private readonly IObservable<EventedMappedGamepad> gamepad;
        private readonly IObservable<MotorDriveState[]> motorStates;
        private readonly MotorBinding motorBinding;
        private readonly IOptionsMonitor<BbUnitConfiguration> unitConfiguration;

        public RoboDiagnosticsService(ILogger<RoboDiagnosticsService> logger, IObservable<EventedMappedGamepad> gamepad, IObservable<MotorDriveState[]> motorStates, MotorBinding motorBinding, IOptionsMonitor<BbUnitConfiguration> unitConfiguration)
        {
            _logger = logger;
            this.gamepad = gamepad;
            this.motorStates = motorStates;
            this.motorBinding = motorBinding;
            this.unitConfiguration = unitConfiguration;
        }

        public override Task GetController(EmptyRequest request, IServerStreamWriter<ControllerReply> responseStream, ServerCallContext context) =>
            gamepad
                .Select(gamepad => new ControllerReply()
                {
                    Name = gamepad.state.GamepadName,
                    Axes = { gamepad.state.Axes },
                    Buttons = { gamepad.state.Buttons },
                })
                .Subscribe(responseStream, context);

        public override Task GetMotorState(EmptyRequest request, IServerStreamWriter<MotorStateReply> responseStream, ServerCallContext context) =>
            motorStates
                .Select(states => new MotorStateReply()
                {
                    Motors = {
                        from state in states
                        select new MotorState
                        {
                            Direction = state.state.Direction switch
                            {
                                MotorDirection.Forward => MotorState.Types.MotorDirection.Forward,
                                MotorDirection.Backward => MotorState.Types.MotorDirection.Backward,
                                _ => MotorState.Types.MotorDirection.Stopped
                            },
                            Speed = state.state.Speed
                        }
                    }
                })
                .Subscribe(responseStream, context);

        public override Task<GpioConfigurationReply> GetGpioConfiguration(EmptyRequest request, ServerCallContext context) =>
            Task.FromResult(
                new GpioConfigurationReply()
                {
                    Pins =
                    {
                        from pin in Unosquare.RaspberryIO.Pi.Gpio
                        select new GpioConfiguration { BcmPin = $"{pin.BcmPin}", Capabilities = $"{((Unosquare.WiringPi.GpioPin)pin).Capabilities}" }
                    }
                }
            );

        public override Task GetGpioState(EmptyRequest request, IServerStreamWriter<GpioStateReply> responseStream, ServerCallContext context) =>
            Observable
                .WithLatestFrom(motorBinding.SerialData, motorBinding.MotorPower, (serialData, motorPower) => new GpioStateReply { Serial = serialData, MotorPower = { motorPower } })
                .Subscribe(responseStream, context);

        public override Task<UnitConfigurationReply> GetUnitConfiguration(EmptyRequest request, ServerCallContext context)
        {
            // TODO - stream changes
            return Task.FromResult(new UnitConfigurationReply
            {
                MotorOrientation = { unitConfiguration.CurrentValue.MotorOrientation.Select(n => n ?? -1) }
            });
        }
    }
}
