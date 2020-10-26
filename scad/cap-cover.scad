visibleBoltHole = 4 / 2;
visibleBoltOuterRadius = 253 * 0.07734 / 2;
visibleBoltCoverRadius = visibleBoltOuterRadius / 2;
visibleBoltBezelDepth = visibleBoltOuterRadius * 0.2;
visibleBoltBezel = visibleBoltOuterRadius * 0.2;

boltRadius = 3 / 2;

nutSideToSideDiameter = 5.5;
nutThickness = 2.5;
nutTolerance = 0.2;
nutCornerRadius = circleRadius(r=nutSideToSideDiameter/2, $fn=6);

capHeight = nutThickness + 1.6;

capBase = visibleBoltOuterRadius - visibleBoltBezel * 2;
capRadius = 10.8;
nutOffset = capRadius * cos(asin(nutCornerRadius / capRadius));
capOffset = nutOffset - 0.8 - nutThickness;
echo(nutOffset - 0.8);
echo((capRadius - capOffset) - nutThickness);

function circleRadius(r, $fn) = r / cos(360 / 2 / $fn);

difference() {

    rotate_extrude($fn=60)
    intersection() {
        difference() {
            translate([0,-capOffset])
            circle(r=capRadius);

            *square([nutCornerRadius, nutThickness]);
            *square([boltRadius, nutThickness]);
        }

        square([capRadius, nutOffset - capOffset]);

        difference() {
            square([capBase,20]);
            translate([capBase-nutTolerance*2, 0])
            square([nutTolerance*2, nutTolerance*2]);
        }

    }

    translate([0,0,-nutTolerance])
    cylinder(r=nutCornerRadius + nutTolerance, h=nutThickness + nutTolerance, $fn=6);
}