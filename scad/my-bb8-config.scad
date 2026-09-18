$fnBody = 360 / ($preview ? 5 : 2);
$fn = 360/ ($preview ? 10 : 5);
$fnDetail = $preview ? 15 : 60;

smallPrintBed = false;

millisPerInch = 25.4;
radius = 253; // rimstar.org has it at diameter of both 506 and 508 in different spots, but this is what I started at, so I'm keeping it for the moment.
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
panelOverlapFactor = 0;
maxLip = 0;
ringLocksPerQuadrant = 6;

include <shared.scad>;
