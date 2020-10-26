import React from 'react';
import { getMotionConfiguration } from './useRobotApi';
import styles from './MotionConfiguration.module.css';
import { useRobotConnection$ } from './robotServiceContext';
import { switchMap } from 'rxjs/operators';
import { MotionConfigurationMessage, MotorConfigurationMessage } from './robotServiceTypes';
import produce from 'immer';
import { useRx } from './utils/useRx';

function MotorConfigForm({value, setValue}: { value: MotorConfigurationMessage, setValue: React.Dispatch<React.SetStateAction<MotorConfigurationMessage>> }) {
  const idPrefix = React.useMemo(() => Math.random().toString(36).replace(/[^a-z]+/g, '').substr(0, 5), []);
  const setBuffer = React.useCallback((buffer) => setValue(produce(v => { v.buffer = buffer; })), [setValue]);
  const setDeadZone = React.useCallback((deadZone) => setValue(produce(v => { v.deadZone = deadZone; })), [setValue]);
  const setBoostFactor = React.useCallback((boostFactor) => setValue(produce(v => { v.boostFactor = boostFactor; })), [setValue]);
  return (
    <table>
      <tbody>
        <tr>
          <td><label htmlFor={`${idPrefix}-buffer`}>Buffer</label></td>
          <td><input id={`${idPrefix}-buffer`} value={value.buffer} onChange={ev => setBuffer(ev.currentTarget.value)} /></td>
        </tr>
        <tr>
          <td><label htmlFor={`${idPrefix}-dead-zone`}>Dead Zone</label></td>
          <td><input id={`${idPrefix}-dead-zone`} value={value.deadZone} onChange={ev => setDeadZone(ev.currentTarget.value)} /></td>
        </tr>
        <tr>
          <td><label htmlFor={`${idPrefix}-boost`}>Boost</label></td>
          <td><input id={`${idPrefix}-boost`} value={value.boostFactor} onChange={ev => setBoostFactor(ev.currentTarget.value)} /></td>
        </tr>
      </tbody>
    </table>
  );
}

export function MotionConfiguration() {
  const connection$ = useRobotConnection$();
  const connection = useRx(connection$, undefined);
  const [motionConfiguration, setConfiguration] = React.useState<MotionConfigurationMessage>(undefined as any as MotionConfigurationMessage);
  React.useEffect(() => {
    const subscription = connection$
      .pipe(switchMap(getMotionConfiguration))
      .subscribe(
        motionConfiguration => setConfiguration(motionConfiguration)
      );
    return () => subscription.unsubscribe();
  }, [connection$]);
  const setMotor = React.useCallback(
    (index: number, setStateAction: React.SetStateAction<MotorConfigurationMessage>) =>
      typeof setStateAction === 'function'
        ? setConfiguration(produce(config => { config.motors[index] = setStateAction(config.motors[index]); }))
        : setConfiguration(produce(config => { config.motors[index] = setStateAction; })),
    [setConfiguration]
  );
  const saveConfiguration = React.useCallback(
    async () => {
      if (connection) {
        const request: MotionConfigurationMessage = {
          motors: motionConfiguration.motors.map(m => ({
            pwmGpioPin: Number(m.pwmGpioPin),
            forwardBit: Number(m.forwardBit),
            backwardBit: Number(m.backwardBit),
            buffer: Number(m.buffer),
            deadZone: Number(m.deadZone),
            boostFactor: Number(m.boostFactor),
          })),
          serial: {
            clockPin: Number(motionConfiguration.serial.clockPin),
            latchPin: Number(motionConfiguration.serial.latchPin),
            dataPin: Number(motionConfiguration.serial.dataPin),
          }
        };
        await connection.send('SetMotionConfiguration', request);
      }
    },
    [connection, motionConfiguration]
  )
  if (!motionConfiguration)
    return null;

  return (
    <div className={styles["config-rows"]}>
      {motionConfiguration.motors.map((m, index) =>
        <MotorConfigForm key={m.pwmGpioPin} value={m} setValue={v => setMotor(index, v)} />
      )}
      <button onClick={saveConfiguration}>Save</button>
    </div>
  );
}
