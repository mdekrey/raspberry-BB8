import React from 'react';
import { from, Observable } from 'rxjs';
import { switchMap } from "rxjs/operators";
import { useRobotConnection$ } from './robotServiceContext';
import { useRx } from './utils/useRx';
import { HubConnection } from '@microsoft/signalr';
import { fromSignalR } from './utils/fromSignalR';
import { ControllerReply, GpioConfigurationReply, GpioStateReply, MotorStateReply, UnitConfigurationReply, MotionConfigurationMessage } from './robotServiceTypes';

export function useRobotApi<T>(action: (connection: HubConnection) => Observable<T>) {
    const connection$ = useRobotConnection$();
    return useRx(React.useMemo(() => connection$
        .pipe(
            switchMap(action)
        ), [connection$, action]), null);
}

const getController = (connection: HubConnection) => fromSignalR<ControllerReply>(connection.stream('GetController'));
const getMotorStates = (connection: HubConnection) => fromSignalR<MotorStateReply>(connection.stream('GetMotorStates'));
const getGpioState = (connection: HubConnection) => fromSignalR<GpioStateReply>(connection.stream('GetGpioState'));
const getGpioConfiguration = (connection: HubConnection) => from(connection.invoke<GpioConfigurationReply>('GetGpioConfiguration'));
const getUnitConfiguration = (connection: HubConnection) => fromSignalR<UnitConfigurationReply>(connection.stream('GetUnitConfiguration'));
export const getMotionConfiguration = (connection: HubConnection) => fromSignalR<MotionConfigurationMessage>(connection.stream('GetMotionConfiguration'));

export const useController = () => useRobotApi(getController);
export const useMotorStates = () => useRobotApi(getMotorStates);
export const useGpioState = () => useRobotApi(getGpioState);
export const useGpioConfiguration = () => useRobotApi(getGpioConfiguration);
export const useUnitConfiguration = () => useRobotApi(getUnitConfiguration);
export const useMotionConfiguration = () => useRobotApi(getMotionConfiguration);
