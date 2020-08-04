$fnBody = 360 / 3;
$fn = 360 / 5;
$fnDetail = 60;
millisPerInch = 25.4;
radius = 253/2;
wallThickness = 0.4 * millisPerInch;
camlockNutRadius = 2.6;
camlockNutThickness = wallThickness*0.7;
camlockNutWallThickness = 1.6;
camlockNutMaxDepth = 10;
camlockNutGripSize = 1;
camlockBoltRadius = 2.6;
camlockBoltLength = 20;
fittedTolerance = 0.3;
pinRadius = 1.5;
pinLength = 15;

include <shared.scad>;
// panelRingOverlap = 0; // hidden during test renders for now

tFrameThird();
//%translate([0,0, -radius * cos(panelDegrees)]) panelRing();
// tFrame();
// rotate([-90,-90,0])
//translate([0,0, -radius * cos(panelDegrees)]) 
// panelRingQuarter();

// camlockBolt(15);
// rotate([90,0,0]) cylinder(r=pinRadius, h=pinLength);
//rotateLockSlot(20, 72.5574, 6, 35);
