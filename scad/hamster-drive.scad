include <lib/bb8-config/full.scad>;
include <lib/robot-config/no3.scad>;
include <components/robotics/_.scad>;
include <shared.scad>;
include <components/ruthex-threaded-inserts.scad>;

robotFrameDowelOffset = [
    -radius * 0.2, // the amount it passes the center line of the robot
    -wheelEdgeCenter[1], // runs directly over the motor mount
    -wheelEdgeCenter[2] + 0.5*robotPlatformThickness // running directly through the platform itself
];
robotPlatformMotorDistance = radius-wallThickness-wheelEdgeCenter[0] + motorPlateSize[0]/2;
robotPlatformPartWidth = -wheelEdgeCenter[1] + motorPlateSize[1]/2;
echo("Print dimensions:", robotPlatformPartWidth * (1+sin(120)), robotPlatformMotorDistance);
echo([-cos(120), sin(120)]);

%intersection() {
    bodySphere();
    translate([0,0,-radius])
    cube([radius*2, radius*2, radius*2], center = true);
}

%unprintedRobotParts();

for(wheelDeg = $preview ? [0,120,240] : [0])
rotate([0,0,wheelDeg])
{
    difference() {
        translate([0,0,-wheelEdgeCenter[2]])
        linear_extrude(height = robotPlatformThickness)
        #polygon([
            [0,0],
            [0,robotPlatformPartWidth],
            // arm of the platform that goes around the motor platform staying clear of the wheel
            [robotPlatformMotorDistance,robotPlatformPartWidth],
            [robotPlatformMotorDistance,robotPlatformPartWidth-motorPlateSize[1]],
            [robotPlatformMotorDistance - wheelRadius - motorPlateSize[0]/2,robotPlatformPartWidth-motorPlateSize[1]],
            // extend for the other dowel to slot into
            [robotPlatformPartWidth*sin(120), robotPlatformPartWidth*cos(120)],
        ], convexity = 2);

        unprintedRobotParts();

        // TODO: m3 inserts for motor holes
        // TODO: holes to mount battery, servo, circuitry, etc.
    }
}
