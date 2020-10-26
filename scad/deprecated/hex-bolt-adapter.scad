hexTolerance = 0.1;
motorShaftTolerance = 0.15;
nutSideToSideDiameter = 5.5;
nutThickness = 2.5;
nutTolerance = 0.2;
shaftRadius = 3;

/*
Ultimately, this hex-bolt-adapter didn't work for long. The plastic warped and
split due to pressure of holding the motor, so metal ones are required.
*/

function circleRadius(r, $fn) = r / cos(360 / 2 / $fn);

difference() {
    // outer
    union() {
        translate([0,0, -6])
        cylinder(r=circleRadius(r=6, $fn=6) - hexTolerance, h=6, $fn=6);

        cylinder(r=circleRadius(r=shaftRadius + nutThickness + 3, $fn=360), h=12, $fn=360);
    }

    // vertical shaft
    union() {
        translate([0,0,-7])
        cylinder(r=3.5 / 2, h=8, $fn=120);

        cylinder(r=circleRadius(r=shaftRadius, $fn=120) + motorShaftTolerance, h=13, $fn=120);

        translate([0,0,-nutThickness])
        cylinder(r=circleRadius(r=nutSideToSideDiameter/2, $fn=6) + nutTolerance, h=nutThickness, $fn=6);
    }

    // horizontal setting bolt
    translate([0,0,5])
    rotate([-90,0,0])
    union() {
        cylinder(r=circleRadius(r=nutSideToSideDiameter/2, $fn=6) + nutTolerance, h=nutThickness + shaftRadius, $fn=6);

        cylinder(r=3.5 / 2, h=15, $fn=120);
    }

}
