import React from 'react';
import styles from './App.module.css';
import { GpioPinConfiguration } from './GpioPinConfiguration';
import { JoystickDisplay } from './JoystickDisplay';
import { MotionConfiguration } from './MotionConfiguration';
import { WheelDisplay } from './WheelDisplay';

function App() {
  return (
    <div className={styles["App"]}>
      <div className={styles["status"]}>
        <JoystickDisplay />
        <WheelDisplay />
      </div>
      <div className={styles["pins"]}>
        <GpioPinConfiguration />
      </div>
      <div className={styles["config"]}>
        <MotionConfiguration />
      </div>
    </div>
  );
}

export default App;
