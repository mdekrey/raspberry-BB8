$fnBody = 360 / 3;
$fn = 360/5;
millisPerInch = 25.4;
radius = 253;
wallThickness = 0.5 * millisPerInch;
camlockNutRadius = 0.25 * millisPerInch;
camlockNutThickness = wallThickness * 0.6;
camlockNutWallThickness = 1.6;
camlockNutMaxDepth = 10;
camlockNutGripSize = 1;
camlockBoltRadius = 2.6;
camlockBoltLength = 40;
fittedTolerance = 0.3;

include <shared.scad>;

%union() {
    translate([-camlockNutRadius*1.5-insertionTolerance,0,0])
    rotate([0, 90, 0])
    camlockBolt(20);

    translate([-camlockNutRadius*1.5-insertionTolerance,0,0])
    camlockNut();
}

%difference() {
    translate([-camlockNutRadius*1.5,0,0])
    cube([camlockNutRadius*3,camlockNutRadius*3,camlockNutThickness*1.5], center=true);

    translate([0,0,camlockNutThickness * 0.5])
    rotate([0, 0, -90])
    camLockSlot(20);
}


translate([-camlockNutRadius*1.5-insertionTolerance,0,0])
rotate([0, 90, 0])
camlockBolt(20);