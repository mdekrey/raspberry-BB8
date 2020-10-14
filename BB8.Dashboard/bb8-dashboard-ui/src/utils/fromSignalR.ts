import { Observable } from 'rxjs';
import { IStreamResult } from '@microsoft/signalr';
export function fromSignalR<T>(stream: IStreamResult<T>) {
    return new Observable<T>((subscriber) => {
        const result = stream.subscribe(subscriber);
        return () => result.dispose();
    });
}
