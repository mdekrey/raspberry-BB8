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
panelOverlapFactor = 0;
maxLip = 0;

include <shared.scad>;
*tFrameThird();
translate([0,0, panelHeight - radius])
*panelRingQuarter(false);
*translate([0,0, -radius * cos(panelDegrees)]) panel();
*panelMainBottom(4);

panel = 5;
module toolPanel() {
    panelMainTop(true) children();
    panelMainTop(false) children();
    *panelMainBottom(true);
    *panelMainBottom(false);
}

//rotate([0,0,90])
translate([0,0, panelHeight - radius])
{
    toolPanel()
    union()
    {
        panelDesignEmboss(panel);

        panelCutout(panel);
    }

    panelDesignCurved(panel);
}

*rotate([180, 0, 0])
translate([0,0, panelHeight - radius])
panelGlueBracket();

*translate([0,0, radius - panelDesignDepth])
linear_extrude(height=panelDesignDepth, twist=-360, slices=panelDesignDepth*5)
intersection() {
    circle(r=60);

    // translate([-60,0])
    square([120,60]);
}

module panelCutout(panel) {
    if (panel == 5 || panel == 6) {
        intersection() {
            panelDesignEmboss(str(panel, "-cutout"));

            if (panel == 5) {
                union() {

                    translate([0,-44, radius + 0 + 18.5 - 18])
                    scale([1,1,0.25])
                    sphere(r=24);

                    depth = 1;
                    intersection() {
                        union() {
                        rotate([0,0,-45])
                        translate([0,0, radius + 2.5])
                        for (i = [0:1 / 15 :1]) {
                        translate([0,0,i * 1])
                        rotate([0,0,i * 180])
                        rotate(-10, v = [1,-1,0])

                        linear_extrude(height=10)
                        intersection() {
                            circle(r=60);
                            square([60,60]);
                        }
                        }
                        }

                        rotate([0,0,45])
                        translate([-60, 0, radius - panelDesignDepth])
                        cube([120, 60, radius]);
                    }
                }
            }
        }
    }
}