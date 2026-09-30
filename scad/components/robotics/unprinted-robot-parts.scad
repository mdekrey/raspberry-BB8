include <motor-and-wheel.scad>;

module unprintedRobotParts() {
    for(wheelDeg = [0,120,240])
    rotate([0,0,wheelDeg])
    {
        union(){
            // m3 inserts for motor holes
            translate([radius-wallThickness,0,0])
            translate(-wheelEdgeCenter)
            motorAndWheel();

            // dowel for structure
            translate(robotFrameDowelOffset)
            rotate([0,90,0])
            cylinder(h = radius, r = 0.5*robotFrameDowelDiameter);;

            // TODO: holes to mount battery, servo, circuitry, etc.
        }
    }
}