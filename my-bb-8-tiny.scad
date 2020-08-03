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

include <shared.scad>;
// panelRingOverlap = 0; // hidden during test renders for now

// tFrameThird();
//%translate([0,0, -radius * cos(panelDegrees)]) panelRing();
// tFrame();
// rotate([-90,-90,0])
//translate([0,0, -radius * cos(panelDegrees)]) 
panelRingQuarter();

// camlockBolt(15);
// cylinder(r=camlockBoltRadius - insertionTolerance / 2, h=15);
//rotateLockSlot(20, 72.5574, 6, 35);
