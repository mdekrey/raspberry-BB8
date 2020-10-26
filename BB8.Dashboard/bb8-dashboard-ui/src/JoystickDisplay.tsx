import React from 'react';
import { useController } from './useRobotApi';

export function JoystickDisplay() {
  const controller = useController();
  return (
    <div style={{ position: 'relative', width: '100px', height: '100px', backgroundColor: 'black', borderRadius: '100px' }}>
      <div style={{ backgroundColor: 'white', width: '50px', height: '50px', borderRadius: '50px', position: 'absolute', left: `${25 + 50 * (controller?.axes.moveX || 0)}px`, top: `${25 + 50 * (controller?.axes.moveY || 0)}px` }}></div>
    </div>
  );
}
