import React from 'react';
import styles from "./GpioPinConfiguration.module.css";
import { useGpioConfiguration } from './useRobotApi';

export function GpioPinConfiguration() {
  const gpioConfig = useGpioConfiguration();
  return (
    <table className={styles["gpio-table"]}>
      <thead>
        <tr>
          <td>Pin</td><td>Capabilities</td>
        </tr>
      </thead>
      <tbody>
        {gpioConfig?.pins.map(pin => <tr key={pin.bcmPin}><td>{pin.bcmPin}</td><td>{pin.capabilities}</td></tr>)}
      </tbody>
    </table>
  );
}
