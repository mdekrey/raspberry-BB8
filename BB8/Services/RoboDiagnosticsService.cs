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
using System.Text.Json.Serialization;
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
        private readonly IOptionsMonitor<MotionConfiguration> motionConfiguration;

        public RoboDiagnosticsService(ILogger<RoboDiagnosticsService> logger, IObservable<EventedMappedGamepad> gamepad, IObservable<MotorDriveState[]> motorStates, MotorBinding motorBinding, IOptionsMonitor<BbUnitConfiguration> unitConfiguration, IOptionsMonitor<MotionConfiguration> motionConfiguration)
        {
            _logger = logger;
            this.gamepad = gamepad;
            this.motorStates = motorStates;
            this.motorBinding = motorBinding;
            this.unitConfiguration = unitConfiguration;
            this.motionConfiguration = motionConfiguration;
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

        public override Task GetUnitConfiguration(EmptyRequest request, IServerStreamWriter<UnitConfigurationReply> responseStream, ServerCallContext context) =>
            unitConfiguration.Observe()
                .Select(v => new UnitConfigurationReply
                {
                    MotorOrientation = { v.MotorOrientation.Select(n => n ?? -1) }
                })
            .Subscribe(responseStream, context);

        public override Task GetMotionConfiguration(EmptyRequest request, IServerStreamWriter<MotionConfigurationMessage> responseStream, ServerCallContext context) =>
            motionConfiguration.Observe()
                .Select(v => ConfigurationToMessage(v))
            .Subscribe(responseStream, context);

        private static MotionConfigurationMessage ConfigurationToMessage(MotionConfiguration v) => 
            new MotionConfigurationMessage
            {
                Motors = { v.Motors.Select(m => new MotorConfigurationMessage { BackwardBit = m.BackwardBit, BoostFactor = m.BoostFactor, Buffer = m.Buffer, DeadZone = m.DeadZone, ForwardBit = m.ForwardBit, PwmGpioPin = m.PwmGpioPin }) },
                Serial = new SerialConfigurationMessage { ClockPin = v.Serial.GpioClock, DataPin = v.Serial.GpioData, LatchPin = v.Serial.GpioLatch },
            };

        public override async Task<MotionConfigurationMessage> SetMotionConfiguration(MotionConfigurationMessage request, ServerCallContext context)
        {
            var config = new MotionConfiguration
            {
                Motors = request.Motors
                    .Select(m => new MotorConfiguration { BackwardBit = (byte)m.BackwardBit, BoostFactor = m.BoostFactor, Buffer = m.Buffer, DeadZone = m.DeadZone, ForwardBit = (byte)m.ForwardBit, PwmGpioPin = m.PwmGpioPin })
                    .ToList(),
                Serial = new MotorSerialControlPins { GpioClock = request.Serial.ClockPin, GpioData = request.Serial.DataPin, GpioLatch = request.Serial.LatchPin }
            };

            // TODO - I don't like having a hard-coded path here...
            var jsonPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "bb8.json");
            var data = System.IO.File.Exists(jsonPath)
                ? System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(await System.IO.File.ReadAllTextAsync(jsonPath)) ?? new Dictionary<string, object>()
                : new Dictionary<string, object>();
            data["motion"] = config;
            var json = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            await System.IO.File.WriteAllTextAsync(jsonPath, json);

            return ConfigurationToMessage(motionConfiguration.CurrentValue);
        }
    }
}
