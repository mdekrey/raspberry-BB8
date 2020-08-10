$fnBody = 360 / 2;
$fn = 360/5;
$fnDetail = 60;
millisPerInch = 25.4;
radius = 253;
wallThickness = 0.5 * millisPerInch;
camlockNutRadius = 2.6;
camlockNutThickness = wallThickness * 0.6;
camlockNutWallThickness = 1.6;
camlockNutMaxDepth = 10;
camlockNutGripSize = 1;
camlockBoltRadius = 2.5;
camlockBoltLength = 25;
fittedTolerance = 0.3;
pinRadius = camlockBoltRadius;
pinLength = camlockBoltLength;
adjacentCamlockOffsetStart = 7.5;
adjacentCamlockOffsetStep = 7.5;
adjacentCamlockOffsetMax = 20;
panelOverlapFactor = 28;

include <shared.scad>;
tFrameThird();
panelRingQuarter();
*translate([0,0, -radius * cos(panelDegrees)]) panel();
* panelMainBottom(4);


*translate([0,0, -radius * cos(panelDegrees)]) panelMainTop(4);
