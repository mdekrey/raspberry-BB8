
import React, { createContext, useContext, useMemo, useEffect } from "react";
import { HubConnectionBuilder, HubConnection } from "@microsoft/signalr";
import { from } from "rxjs";
import { map } from "rxjs/operators";

export function useNewRobotConnection() {
    const connection = useMemo(() => new HubConnectionBuilder().withUrl("/robot").build(), []);
    const connected = useMemo(() => connection.start(), [connection]);

    useEffect(() => {
        return () => { connection.stop() };
    }, [connection]);

    return [connection, connected] as [HubConnection, Promise<void>];
}

export type RobotConnectionContextType = {
    connection: HubConnection;
    connected: Promise<void>;
}

const RobotConnectionContext = createContext({} as RobotConnectionContextType);
export function useRobotConnection() {
    return useContext(RobotConnectionContext);
}

export function RobotConnectionScope(props: { children: React.ReactNode }) {
    const [connection, connected] = useNewRobotConnection();
    const context = useMemo((): RobotConnectionContextType => ({
        connection,
        connected,
    }), [connection, connected]);

    return (
        <RobotConnectionContext.Provider value={context}>
            {props.children}
        </RobotConnectionContext.Provider>
    )
}

export function useRobotConnection$() {
    const { connection, connected } = useRobotConnection();
    const connection$ = React.useMemo(() => from(connected).pipe(map(_ => connection)), [connection, connected]);

    return connection$;
}
