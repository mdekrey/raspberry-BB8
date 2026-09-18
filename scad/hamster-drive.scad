include <lib/bb8-config/full.scad>;
include <components/robotics/_.scad>;
include <shared.scad>;

%intersection() {
    bodySphere();
    translate([0,0,-radius])
    cube([radius*2, radius*2, radius*2], center = true);
}

for(wheelDeg = [0,120,240])
{
    %
    rotate([0,0,wheelDeg])
    translate([radius-wallThickness,0,0])
    translate(-wheelEdgeCenter)
    motorAndWheel();
}
