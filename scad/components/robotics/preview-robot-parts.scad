include <motor-and-wheel.scad>;
include <positioning.scad>;

module previewRobotParts() {
    for(wheelDeg = [0,120,240])
    rotate([0,0,wheelDeg])
    {
        union(){
            // motors
            motorPosition()
            motorAndWheel();

            // dowel for structure
            dowelPosition()
            cylinder(h = radius, r = 0.5*robotFrameDowelDiameter);;

            // TODO: servo, circuitry, etc.
        }
    }
}