import React from 'react';
import { MotorDirection } from './robotServiceTypes';
import { useGpioState, useMotorStates, useUnitConfiguration } from './useRobotApi';

export function WheelDisplay({ size = 100 }: { size?: number }) {
  const motorStates = useMotorStates();
  const gpioState = useGpioState();
  const unitConfiguration = useUnitConfiguration();
  const motorZipped = gpioState === null || unitConfiguration === null
    ? []
    : unitConfiguration.motorOrientation
      .map((current, index) => ({ current, index }))
      .filter(({ current }) => current !== -1)
      .map(({ current, index }) => ({
        orientation: current,
        state: motorStates ? motorStates.motors[index] : { direction: MotorDirection.STOPPED },
        gpioPower: gpioState.motorPower[index]
      }));
  return (
    <div>
      <svg width={size} height={size}>
        {motorZipped.map((motor, index) => <g key={index} transform={`translate(${size * 0.5}, ${size * 0.5}) rotate(${motor.orientation - 90})`}>
          <rect width={size * 0.1} height={size * 0.4} x={size * 0.25} y={size * -0.2} fill="#000000" />
          {motor.gpioPower > 0
            ? <path d={`M-${size * 0.015},0 l${size * 0.03},0 v${motor.gpioPower * size * 0.15} h${size * 0.015} l-${size * 0.03},${size * 0.05} l-${size * 0.03},-${size * 0.05} h${size * 0.015} Z`}
              transform={`translate(${size * 0.3}, 0) scale(1, ${motor.state.direction === MotorDirection.FORWARD ? 1 : -1})`}
              fill={`rgb(255,${Math.round(255 * motor.gpioPower)},0)`} />
            : null}
        </g>
        )}
      </svg>
      <p>{Array(8).fill(0).map((_, index) => (1 << index)).map((bit) => <span key={bit}>{((gpioState?.serial || 0) & bit) ? 1 : 0}</span>)}</p>
    </div>
  );
}
