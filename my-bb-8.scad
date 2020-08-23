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
maxLip = 0;

include <shared.scad>;
*tFrameThird();
translate([0,0, panelHeight - radius])
*panelRingQuarter(false);
*translate([0,0, -radius * cos(panelDegrees)]) panel();
*panelMainBottom(4);

rotate([0,0,90])
translate([0,0, panelHeight - radius])
union() {
    *panel();
    panelMainTop(5, true);
    panelMainTop(5, false);
    panelMainBottom(5, true);
    panelMainBottom(5, false);
    panelDesignCurved(5);

}

*rotate([180, 0, 0])
translate([0,0, panelHeight - radius])
panelGlueBracket();
