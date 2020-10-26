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
camlockBoltRadius = 2.5;
camlockBoltLength = 20;
fittedTolerance = 0.3;
pinRadius = 1.5;
pinLength = 15;
adjacentCamlockOffsetStart = 7.5;
adjacentCamlockOffsetStep = 20;
adjacentCamlockOffsetMax = 20;
panelOverlapFactor = 14;
maxLip = 0;

include <shared.scad>;

*tFrameThird();
*translate([0,0, -radius * cos(panelDegrees)]) panelRing();
*tFrame();
// rotate([-90,-90,0])
//translate([0,0, -radius * cos(panelDegrees)]) 
*panelRingQuarter();
*rotate([0,0,-15 - rotateLockDegrees - panelRotateLockOffset]) translate([0,0, -radius * cos(panelDegrees)]) panel();

// camlockBolt(20);
// diagonal bolt makes the bolt stronger from perpendicular sheer strength
// while still remaining easy to print
*intersection() {
    rotate([-15, 0, 0]) translate([camlockBoltRadius, camlockBoltRadius, 0]) bolt();
    cube([camlockBoltLength,camlockBoltLength,camlockBoltLength]);
}
// rotate([90,0,0]) pin();
//rotateLockSlot(20, 72.5574, 6, 35);

rotate([0,0,90])
translate([0,0, panelHeight - radius])
union() {
    *panel();
    panelMainTop(4, true);
    panelMainTop(4, false);
    panelMainBottom(4, true);
    panelMainBottom(4, false);
    panelDesignCurved(4);
}
