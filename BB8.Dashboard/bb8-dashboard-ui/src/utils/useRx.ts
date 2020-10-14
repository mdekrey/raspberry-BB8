import { useState, useEffect } from 'react';
import { Observable } from 'rxjs';

export function useRx<T>(target: Observable<T>, initial: T) {
    const [current, setCurrent] = useState(initial);
    useEffect(() => {
        const subscription = target.subscribe(setCurrent);
        return () => subscription.unsubscribe();
    }, [target]);
    return current;
}
