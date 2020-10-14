export type ControllerReply = {
  name: string;
  buttons: Record<string, boolean>;
  axes: Record<string, number>;
};

export type MotorStateReply = {
  motors: MotorState[];
};

export enum MotorDirection {
  STOPPED,
  FORWARD,
  BACKWARD,
}
export type MotorState = {
  direction: MotorDirection;
  speed: number;
};

export type GpioConfigurationReply = {
  pins: GpioConfiguration[];
};
export type GpioConfiguration = {
  bcmPin: string;
  capabilities: string;
};

export type GpioStateReply = {
  serial: number;
  motorPower: number[];
};

export type UnitConfigurationReply = {
  motorOrientation: number[];
}