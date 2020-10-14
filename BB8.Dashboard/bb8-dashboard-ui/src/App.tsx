import React from 'react';
import { from, Observable } from 'rxjs';
import { switchMap, map, scan } from "rxjs/operators";
import './App.css';
import { useRobotConnection } from './robotServiceContext';
import { fromSignalR } from './utils/fromSignalR';
import { useRx } from './utils/useRx';
import { ControllerReply, GpioConfigurationReply, GpioStateReply, MotorDirection, MotorStateReply, UnitConfigurationReply } from './robotServiceTypes';
import { HubConnection } from '@microsoft/signalr';

function useRobotApi<T>(action: (connection: HubConnection) => Observable<T>) {
  const {connection, connected} = useRobotConnection();
  const connection$ = React.useMemo(() => from(connected).pipe(map(_ => connection)), [connection, connected]);

  return useRx(React.useMemo(() => connection$
  .pipe(
      switchMap(action)
  ), [connection$, action]), null);

}

function App() {
  const controller = useRobotApi(React.useCallback(connection => fromSignalR<ControllerReply>(connection.stream('GetController')), []));
  const motorStates = useRobotApi(React.useCallback(connection => fromSignalR<MotorStateReply>(connection.stream('GetMotorStates')), []));
  const gpioState = useRobotApi(React.useCallback(connection => fromSignalR<GpioStateReply>(connection.stream('GetGpioState')), []));
  const gpioConfig = useRobotApi(React.useCallback(connection => from(connection.invoke<GpioConfigurationReply>('GetGpioConfiguration')), []));
  const unitConfiguration = useRobotApi(React.useCallback(connection => from(connection.invoke<UnitConfigurationReply>('GetUnitConfiguration')), []));
  const motorZipped = motorStates === null || gpioState === null || unitConfiguration === null
    ? []
    : unitConfiguration.motorOrientation.reduce((prev, current, index) => {
      if (current === -1)
        return prev;
      prev.push({ orientation: current, state: motorStates.motors[index], gpioPower: gpioState.motorPower[index] })
      return prev;
    }, [] as { orientation: number, state: MotorStateReply["motors"][0], gpioPower: GpioStateReply["motorPower"][0] }[]);

  return (
    <div className="App">
      <header className="App-header">
        <div style={{ position: 'relative', width: '100px', height: '100px', backgroundColor: 'black' }}>
          <div style={{ backgroundColor: 'white', width: '50px', height: '50px', borderRadius: '50px', position: 'absolute', left: `${25 + 50*(controller?.axes.moveX || 0)}px`, top: `${25 + 50*(controller?.axes.moveY || 0)}px` }}></div>
        </div>
        <svg width={200} height={200}>
          {motorZipped.map((motor, index) =>
            <g key={index} transform={`translate(100, 100) rotate(${motor.orientation - 90})`}>
              <rect width={20} height={80} x={50} y={-40} fill="#000000" />
              {motor.gpioPower > 0
                ? <path d={`M-3,0 l6,0 v${motor.gpioPower * 30} h3 l-6,10 l-6,-10 h3 Z`}
                        transform={`translate(60, 0) scale(1, ${motor.state.direction === MotorDirection.FORWARD ? 1 : -1})`}
                        fill={`rgb(255,${Math.round(255*motor.gpioPower)},0)`} />
                : null}
            </g>
          )}
        </svg>
        <p>{Array(8).fill(0).map((_, index) => (1 << index)).map((bit) => <span key={bit}>{((gpioState?.serial || 0) & bit) ? 1 : 0}</span>)}</p>
        <table>
          <thead>
            <tr>
            <td>Pin</td><td>Capabilities</td>
            </tr>
          </thead>
          <tbody>
            {gpioConfig?.pins.map(pin => <tr key={pin.bcmPin}><td>{pin.bcmPin}</td><td>{pin.capabilities}</td></tr>)}
          </tbody>
        </table>
      </header>
    </div>
  );
}

export default App;
