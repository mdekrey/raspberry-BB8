include <lib/bb8-config/full.scad>;
include <lib/robot-config/no3.scad>;
include <components/robotics/_.scad>;
include <shared.scad>;
include <components/ruthex-threaded-inserts.scad>;
include <components/battery/_.scad>;


echo("Print dimensions:", robotPlatformPartWidth * (1+sin(120)), robotPlatformMotorDistance);

%render() intersection() {
    bodySphere();
    translate([0,0,-radius])
    cube([radius*2, radius*2, radius*2], center = true);
}

%render() previewRobotParts();

batteryPosition()
batteryBracket();

for(wheelDeg = $preview ? [0,120,240] : [0])
rotate([0,0,wheelDeg])
{
    difference() {
        rotate([180,0,0])
        translate([0,0,-wheelEdgeCenter[2]])
        linear_extrude(height = robotPlatformThickness, convexity=2)
        polygon([
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
    }
}

