include <motor-holes.scad>;

module motorAndWheel() {
    rotate([0,180,0]) {
        // motor
        translate([0,-motorPlateYOffset, motorRadius + motorZOffset])
        rotate([-90])
        cylinder(r=motorRadius, h=motorLength);

        // motor face plate
        translate([-20, -motorPlateYOffset - motorPlateThickness, 40])
        rotate([-90,0,0])
        linear_extrude(motorPlateThickness)
        square([40,40]);

        // motor anchor plate, positioned under origin at [0,0]
        linear_extrude(motorPlateThickness)
        difference() {
            translate([-motorPlateSize[0]/2,-motorPlateYOffset - motorPlateThickness])
            square(motorPlateSize);

            motorHoles()
            circle(r=m3holeRadius);
        };

        // power lead attachments
        translate([0,-motorPlateYOffset + leadLength/2 + motorLength, motorRadius + motorZOffset])
        rotate([-90, 0, 0])
        cube([5,motorRadius*2,leadLength], center=true);

        // wheel
        translate([0,-motorPlateYOffset - motorPlateThickness - wheelSpaceFromMotor, motorZOffset + motorShaftOffset])
        rotate([90])
        cylinder(r=wheelRadius, h=wheelThickness);
    }
}
