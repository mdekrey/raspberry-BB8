import React from 'react';
import { useMotionConfiguration } from './useRobotApi';

export function MotionConfiguration() {
  const motionConfiguration = useMotionConfiguration();
  return (
    <pre>
      {JSON.stringify(motionConfiguration, null, 4)}
    </pre>
  );
}
