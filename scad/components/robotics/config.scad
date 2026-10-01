
motorPlateSize = [40,42.5];
motorRadius = 37/2;
motorLength = 57.1;
motorPlateThickness = 2.9;
motorPlateYOffset = 19.5;
motorZOffset = 10;
wheelSpaceFromMotor = 11;
motorShaftOffset = 13;
leadLength = 10;

wheelEdgeCenter=[wheelRadius,-motorPlateYOffset - motorPlateThickness - wheelSpaceFromMotor - wheelThickness / 2,-motorZOffset - motorShaftOffset];

robotFrameDowelOffset = [
    -radius * 0.2, // the amount it passes the center line of the robot
    wheelEdgeCenter[1], // runs directly under the motor mount
    wheelEdgeCenter[2] - 0.5*robotPlatformThickness // running directly through the platform itself
];
robotPlatformMotorDistance = radius-wallThickness-wheelEdgeCenter[0] + motorPlateSize[0]/2;
robotPlatformPartWidth = -wheelEdgeCenter[1] + motorPlateSize[1]/2;
