panelDegrees = 35; // https://rimstar.org/science_electronics_projects/bb-8_dimensions.htm
panelRadius = radius * sin(panelDegrees);
insertionTolerance = 0.3;

visibleBoltHole = 4 / 2;
visibleBoltOuterRadius = radius * 0.07734 / 2;
visibleBoltCoverRadius = visibleBoltOuterRadius / 2;
visibleBoltBezelDepth = visibleBoltOuterRadius * 0.2;
visibleBoltBezel = visibleBoltOuterRadius * 0.2;

panelRingDegrees = panelDegrees * 4/15;
panelRingInnerDegrees = panelDegrees - panelRingDegrees;
panelRingInnerRadius = radius * sin(panelRingInnerDegrees);
panelRingOuterOverlap = 3;
panelRingInnerOverlap = 1;
panelArmDegrees = panelDegrees * 0.6;
panelArmBoltDegrees = panelDegrees * 0.52;

if (camlockBoltRadius * 2 + 2 > camlockNutThickness)
    warn("Camlock Bolt/Nut sizes invalid");

panelRadiusOffset = radius * cos(panelDegrees);
panelRotateLockOffset = 5;
rotateLockDegrees = 6;

panelRingInnerActualDegrees = asin((panelRingInnerRadius - insertionTolerance) / radius);
panelRingInnerInternalDegrees = asin((panelRingInnerRadius + panelRingInnerOverlap - insertionTolerance)
    / (radius - wallThickness));
panelHeight = radius - cos(panelRingInnerInternalDegrees) * (radius - wallThickness);
ringThickness = cos(panelRingInnerActualDegrees) * radius - cos(panelRingInnerInternalDegrees) * (radius - wallThickness) - 0.8;

panelDesignDepth = radius - cos(asin((panelRingInnerRadius * 0.92) / radius)) * radius + insertionTolerance;
panelDesignRadius = wallThickness - 0.2 * millisPerInch;

module bodySphere(additionalWallThickness = 0) {
    difference() {
        sphere(radius, $fn=$fnBody);
        sphere(radius - wallThickness - additionalWallThickness, $fn=$fn);
    };
}

module panelRing() {
    intersection() {
        bodySphere(additionalWallThickness = cos(panelDegrees) * maxLip);

        difference() {
            union() {
                translate([0,0, (radius) * cos(panelDegrees) - wallThickness])
                cylinder(r=panelRadius - insertionTolerance - panelRingOuterOverlap, h=radius * 2);
                translate([0,0,radius + cos(panelDegrees) * radius])
                cube([(panelRadius - insertionTolerance + panelRingOuterOverlap) * 2, (panelRadius - insertionTolerance + panelRingOuterOverlap) * 2, radius * 2], center=true);
            }
            intersection() {
                cylinder(r=panelRingInnerRadius, h=radius * 2);

                difference() {
                    cube([radius*2,radius*2,radius*2], center=true);

                    rotate([0,0,-30+panelRotateLockOffset+rotateLockDegrees])
                    linear_extrude(height=radius)
                    import("panel-x.svg", center=true, dpi=2611.8439045872/panelRingInnerRadius);
                }

            }

            intersection() {
                sphere(radius - wallThickness/2+insertionTolerance, $fn=$fn);
                cylinder(r=panelRingInnerRadius + panelRingInnerOverlap + insertionTolerance, h=radius);
            }
        }
    }

}

panelAdditionalWallThickness = 0; // cos(panelDegrees) * maxLip;
panelInnerWall = radius - wallThickness - panelAdditionalWallThickness;
panelLayerWall = radius - (radius - panelInnerWall) * 0.6;
module panel() {
    difference() {
        rotate([0,0,-15 - rotateLockDegrees - panelRotateLockOffset])
        intersection() {
            bodySphere(additionalWallThickness = panelAdditionalWallThickness);

            union() {
                intersection() {
                    cylinder(r=panelRingInnerRadius - insertionTolerance, h=radius * 2);

                    difference() {
                        cube([radius*2,radius*2,radius*2], center=true);
                        sphere(radius - wallThickness/2 - insertionTolerance, $fn=$fn);

                        rotate([0,0,-30+panelRotateLockOffset+rotateLockDegrees])
                        linear_extrude(height=radius)
                        offset(r=insertionTolerance)
                        import("panel-x.svg", center=true, dpi=2611.8439045872/panelRingInnerRadius);
                    }
                }

                intersection() {
                    sphere(radius - wallThickness/2 - insertionTolerance, $fn=$fn);
                    cylinder(r=panelRingInnerRadius + panelRingInnerOverlap - insertionTolerance, h=radius);
                }
            }

        }

        difference() {
            children();
            sphere(r=1);

            sphere(r=radius - panelDesignRadius, $fn=$fnDetail);
        }

        for (i = [0 : 90 : 360]) {
            rotate([0, panelArmBoltDegrees, i])
            translate([0, 0, radius - wallThickness - panelAdditionalWallThickness])
            rotate([0, 180, 0])
            visibleBoltHole();
        }
    }
}

module panelMain() {
    difference() {
        panel() children();

        translate([
            0, 0,
            radius * 2 - panelHeight + ringThickness])
        rotate([0,0,90 + 20])
        translate([- insertionTolerance / 2, 0, 0])
        cube([insertionTolerance, radius*2, radius*2], center=true);

        translate([
            0, 0,
            radius - panelHeight + ringThickness - insertionTolerance / 2])
        cube([radius*2, radius*2, insertionTolerance], center=true);


        translate([
            0, 0,
            -panelHeight + ringThickness])
        linear_extrude(height=radius)
        for (i = [45,180 + 45])
        rotate([0,0,i])
        polygon([
            [+insertionTolerance / 2, +(panelRingInnerRadius + panelRingInnerOverlap)],
            [+insertionTolerance / 2, +(panelRingInnerRadius + panelRingInnerOverlap - 5)],
            [+insertionTolerance / 2 + 5, +(panelRingInnerRadius + panelRingInnerOverlap - 5)],
            [+insertionTolerance / 2 + 5, +(panelRingInnerRadius + panelRingInnerOverlap - 10)],
            [+insertionTolerance / 2, +(panelRingInnerRadius + panelRingInnerOverlap - 10)],
            [+insertionTolerance / 2, 0],
            [-insertionTolerance / 2, 0],
            [-insertionTolerance / 2, +(panelRingInnerRadius + panelRingInnerOverlap - 10 + insertionTolerance)],
            [-insertionTolerance / 2 + 5, +(panelRingInnerRadius + panelRingInnerOverlap - 10 + insertionTolerance)],
            [-insertionTolerance / 2 + 5, +(panelRingInnerRadius + panelRingInnerOverlap - 5 - insertionTolerance)],
            [-insertionTolerance / 2, +(panelRingInnerRadius + panelRingInnerOverlap - 5 - insertionTolerance)],
            [-insertionTolerance / 2, +(panelRingInnerRadius + panelRingInnerOverlap)],
        ]);
    }
}

module panelMainBottom() {
    *difference() {
        intersection() {
            panel() children();
            translate([0, 0, -panelHeight + ringThickness - insertionTolerance])
            cube([radius * 2, radius*2, radius*2], center=true);
        }
        linear_extrude(height=radius)
        for (i = [45,180 + 45])
        rotate([0,0,i])
        polygon([
            [+insertionTolerance / 2, +(panelRingInnerRadius + panelRingInnerOverlap)],
            [+insertionTolerance / 2, +(panelRingInnerRadius + panelRingInnerOverlap - 5)],
            [+insertionTolerance / 2 + 5, +(panelRingInnerRadius + panelRingInnerOverlap - 5)],
            [+insertionTolerance / 2 + 5, +(panelRingInnerRadius + panelRingInnerOverlap - 10)],
            [+insertionTolerance / 2, +(panelRingInnerRadius + panelRingInnerOverlap - 10)],
            [+insertionTolerance / 2, 0],
            [-insertionTolerance / 2, 0],
            [-insertionTolerance / 2, +(panelRingInnerRadius + panelRingInnerOverlap - 10 + insertionTolerance)],
            [-insertionTolerance / 2 + 5, +(panelRingInnerRadius + panelRingInnerOverlap - 10 + insertionTolerance)],
            [-insertionTolerance / 2 + 5, +(panelRingInnerRadius + panelRingInnerOverlap - 5 - insertionTolerance)],
            [-insertionTolerance / 2, +(panelRingInnerRadius + panelRingInnerOverlap - 5 - insertionTolerance)],
            [-insertionTolerance / 2, +(panelRingInnerRadius + panelRingInnerOverlap)],
        ]);
    }

}

module panelDesign(panelNumber) {
    intersection() {
        difference() {
            resize(newsize = [panelRingInnerRadius*2,panelRingInnerRadius*2])
            import(str("tool-panel-",panelNumber,".svg"), center=true, dpi=200);

            offset(r=insertionTolerance)
            import("panel-x.svg", center=true, dpi=2611.8439045872/panelRingInnerRadius);
        }

        translate([panelRingInnerRadius*2 / 200, -panelRingInnerRadius*2 / 200])
        square([panelRingInnerRadius*2, panelRingInnerRadius*2], center=true);
    }
}

module panelDesignEmboss(panelDesign) {
        rotate(45)
        translate([0,0, radius - panelDesignDepth])
        linear_extrude(height=panelDesignDepth)
        offset(insertionTolerance)
        panelDesign(panelDesign);

}

module panelDesignCurved(panelDesign) {
    intersection() {
        rotate(45)
        translate([0,0, radius - panelDesignDepth + insertionTolerance])
        linear_extrude(height=panelDesignDepth)
        panelDesign(panelDesign);

        difference() {
            sphere(r=radius, $fn=$fnBody);
            sphere(r=radius - panelDesignRadius + insertionTolerance, $fn=$fnDetail);
        }
    }
}

module tFrame() {
    difference() {
        bodySphere();

        rotate([90, 0, 0])
        cylinder(r1=panelRadius - panelRingOuterOverlap, r2=panelRadius - panelRingOuterOverlap, h=radius * 2, center=true);
        rotate([0, 90, 0])
        cylinder(r1=panelRadius - panelRingOuterOverlap, r2=panelRadius - panelRingOuterOverlap, h=radius * 2, center=true);
        cylinder(r1=panelRadius - panelRingOuterOverlap, r2=panelRadius - panelRingOuterOverlap, h=radius * 2, center=true);

        translate([0,0,radius + cos(panelDegrees) * radius - insertionTolerance])
        cube([(panelRadius + panelRingOuterOverlap) * 2, (panelRadius + panelRingOuterOverlap) * 2, radius * 2], center=true);

        rotate([-90, 0, 0])
        translate([0,0,radius + cos(panelDegrees) * radius - insertionTolerance])
        cube([(panelRadius + panelRingOuterOverlap) * 2, (panelRadius + panelRingOuterOverlap) * 2, radius * 2], center=true);

        rotate([0, 90, 0])
        translate([0,0,radius + cos(panelDegrees) * radius - insertionTolerance])
        cube([(panelRadius + panelRingOuterOverlap) * 2, (panelRadius + panelRingOuterOverlap) * 2, radius * 2], center=true);
    }
}

module tFrameThird() {
    rotationDifference = 30.5;
    difference() {
        intersection() {
            polyhedron(
            points=[ [radius,radius,radius],[2*radius,0,0],[0,2*radius,0], // the three points at base
                    [0,0,0]  ],                                 // the apex point
            faces=[ [0,1,3],[1,2,3],
                        [2,0,3],[2,1,0] ]
            );

            tFrame();
        }

        // bottom holes
        rotate([0,-90,40])
        translate([0,0,-(radius - wallThickness - 0)])
        rotate([0,0,90])
        camLockSlot(boltLength=camlockBoltLength);

        rotate([0,-90,50])
        translate([0,0,-(radius - wallThickness - 0)])
        rotate([0,0,90])
        camLockSlot(boltLength=camlockBoltLength);

        // adjacent tri holes
        for (camlockOffset = [adjacentCamlockOffsetStart:adjacentCamlockOffsetStep:adjacentCamlockOffsetMax]) {
            rotate(a=-camlockOffset - panelDegrees, v=[-1,0,1])
            rotate([0,-90,90])
            translate([0,0,-(radius - wallThickness - 0)])
            rotate([0,0,-45 + rotationDifference]) // bolt rotation
            camLockSlot(boltLength=camlockBoltLength);

            rotate(a=-camlockOffset - panelDegrees, v=[-1,0,1])
            rotate([0,-90,90])
            translate([0,0,-(radius - wallThickness - 0)])
            rotate([0,0,-45 - rotationDifference]) // bolt rotation
            camLockSlot(boltLength=camlockBoltLength);

            rotate(a=camlockOffset + panelDegrees, v=[0,-1,1])
            rotate([0,-90,0])
            translate([0,0,-(radius - wallThickness - 0)])
            rotate([0,0,-135 + rotationDifference]) // bolt rotation
            camLockSlot(boltLength=camlockBoltLength);

            rotate(a=camlockOffset + panelDegrees, v=[0,-1,1])
            rotate([0,-90,0])
            translate([0,0,-(radius - wallThickness - 0)])
            rotate([0,0,-135 - rotationDifference]) // bolt rotation
            camLockSlot(boltLength=camlockBoltLength);
        }

        // outer-ring holes
        for (position = [15:15:31]){
            rotate([position,0,0])
            rotate([0,-90,panelDegrees])
            translate([0,0,-(radius - wallThickness - 0)])
            rotate([0,0,180]) // bolt rotation
            camLockSlot(boltLength=camlockBoltLength);

            rotate([0,-position,0])
            rotate([90,0,-panelDegrees])
            translate([0,0,-(radius - wallThickness - 0)])
            rotate([0,0,90]) // bolt rotation
            camLockSlot(boltLength=camlockBoltLength);
        }
        // end outer-ring holes

        // visible bolt-hole cover
        rotate([0, 90 - 35 / 2, 45])
        translate([0, 0, radius])
        visibleBoltHole();

        rotate([0, 90 - 35 / 2, 45])
        translate([0, 0, radius - wallThickness])
        rotate([180, 0, 0])
        visibleBoltHole();
    }
}

module panelRingQuarter(split = false) {
    panelRingCenterDegrees = (panelDegrees - panelRingDegrees / 2);
    endHoleOffset = 180-panelDegrees + panelRingDegrees *0.625;
    difference() {
        intersection() {
            cube([radius, radius, radius]);

            difference() {
                cube([radius*2, radius*2, radius*2], center=true);

                rotate([0,0,0])
                cube([insertionTolerance, radius*2, radius*2], center=true);

                rotate([0,0,-90])
                cube([insertionTolerance, radius*2, radius*2], center=true);

                if (split) {
                    rotate([0,0,-45])
                    cube([insertionTolerance, radius*2, radius*2], center=true);
                }
            }

            panelRing();
        }

        // end holes
        for (end = [-1 : (split ? 1 : 2) : 1]) {
            rotate([endHoleOffset,0,-45 + 45 * end])
            translate([0,0,-(radius - camlockNutThickness)])
            rotate([0,90,0]) // bolt rotation
            cylinder(r=pinRadius + insertionTolerance / 2, h=pinLength + insertionTolerance, center=true);
        }

        // rotate lock holes
        for(loop = [panelRotateLockOffset : 15 : 90]) {
            rotate([0,0,loop])
            translate([0,0, radius * cos(panelDegrees)])
            rotateLockSlot(boltLength=camlockBoltLength, radius = panelRadius, angle = rotateLockDegrees, downwardAngle = panelDegrees);
        }

        // visible bolt
        rotate([0, panelArmBoltDegrees, 15+panelRotateLockOffset+rotateLockDegrees])
        translate([0, 0, radius])
        rotate([0, 0, 0])
        visibleBoltHole();
    }
}


module panelGlueBracket() {
    panelRingCenterDegrees = (panelDegrees - panelRingDegrees / 2);
    endHoleOffset = 180-panelDegrees + panelRingDegrees *0.625;
    difference() {
        translate([panelRingInnerRadius - 20,0,radius - panelHeight / 2])
        cube([60, 40, panelHeight], center=true);

        translate([panelRingInnerRadius + 10,0,radius - panelHeight / 2])
        scale([0.6,1,1])
        rotate([0,0,45])
        cube([5, 5, panelHeight+1], center=true);

        translate([panelRingInnerRadius + 10,0,radius - panelHeight / 2])
        scale([0.6,1,1])
        rotate([0,45,0])
        cube([5, 40+1, 5], center=true);

        translate([0,0,1])
        difference() {
            cylinder(r=panelRingInnerRadius, h=radius);

            rotate([0,0,-45])
            linear_extrude(height=radius)
            import("panel-x.svg", center=true, dpi=2611.8439045872/panelRingInnerRadius);

        }

        intersection() {
            sphere(radius - wallThickness/2+insertionTolerance, $fn=$fn);
            cylinder(r=panelRingInnerRadius + panelRingInnerOverlap + insertionTolerance, h=radius);
        }

        // visible bolt
        rotate([0, panelArmBoltDegrees, 0])
        translate([0, 0, radius + wallThickness])
        rotate([0, 0, 0])
        translate([0, 0, -wallThickness * 2])
        cylinder(r = visibleBoltHole, h=wallThickness * 5, $fn=$fn);
    }
}

module rotateLockSlot(boltLength, radius, angle, downwardAngle) {
    offset = -wallThickness + camlockNutThickness * 0.5;
    yOffset = offset * cos(downwardAngle);
    xOffset = offset * sin(downwardAngle);
    translate([0,0,yOffset])
    union() {
        hull() {
            rotate([0,90,angle])
            translate([0, 0, radius+xOffset])
            rotate([0,downwardAngle,0])
            cylinder(r=(camlockBoltRadius + insertionTolerance / 2), h=boltLength, center=true, $fn=$fnDetail);

            rotate([0,90,0])
            translate([0, 0, radius+xOffset])
            rotate([0,downwardAngle,0])
            cylinder(r=(camlockBoltRadius + insertionTolerance / 2), h=boltLength, center=true, $fn=$fnDetail);

            rotate_extrude(angle = angle, $fn=$fnDetail) {
                translate([radius+xOffset,0])
                rotate(-downwardAngle)
                translate([- boltLength/2, -(camlockBoltRadius + insertionTolerance / 2)])
                square([boltLength, 2*(camlockBoltRadius + insertionTolerance / 2)]);
            }
        }

        rotate([0,90,0])
        translate([0, 0, radius+xOffset])
        rotate([0,downwardAngle,0])
        translate([(camlockBoltRadius + insertionTolerance / 2) + wallThickness/2, 0, 0])
        cube([(camlockBoltRadius + insertionTolerance / 2)*2 + wallThickness,(camlockBoltRadius + insertionTolerance / 2)*2, boltLength], center=true);
    }
}

module camLockSlot(boltLength) {
    translate([0,-boltLength/2,0])
    rotate([180, 0, 0])
    union() {
        translate([0,0,-camlockNutMaxDepth])
        cylinder(r1=camlockNutRadius, r2=camlockNutRadius, h=camlockNutThickness + camlockNutMaxDepth, $fn=$fn);

        rotate([90,0,0])
        translate([0, camlockNutThickness * 0.5, 0])
        cylinder(r1=(camlockBoltRadius + insertionTolerance / 2), r2=(camlockBoltRadius + insertionTolerance / 2), h=boltLength);
    }
}

module visibleBoltHole() {
    translate([0, 0, -visibleBoltBezelDepth])
    cylinder(r1=visibleBoltOuterRadius - visibleBoltBezel * 2, r2=visibleBoltOuterRadius, h=visibleBoltBezelDepth * 2, $fn=$fnDetail);

    translate([0, 0, -wallThickness * 2])
    cylinder(r = visibleBoltHole, h=wallThickness * 2, $fn=$fn);
}

module toolPanel(panel) {
    panelMain()
    union()
    {
        panelDesignEmboss(panel);

        panelCutout(panel);
    }

    panelDesignCurved(panel);
}

module panelCutout(panel) {
    if (panel != 3 && panel != 4) {
        intersection() {
            if (panel != 2)
            panelDesignEmboss(str(panel, "-cutout"));

            if (panel == 1) {
                intersection() {
                    union() {
                    rotate([0,0,-45])
                    translate([0,0, radius + 2.5])
                    for (i = [0:1 / 30 :1]) {
                    rotate([0,0,i * 360])
                    rotate(-10, v = [1,-1,0])

                    linear_extrude(height=10)
                    intersection() {
                        circle(r=60);
                        square([60,60]);
                    }
                    }
                    }
                }
            }
            if (panel == 5) {
                union() {

                    translate([0,-36, radius - 2.75])
                    rotate([8,0,0])
                    scale([1,1,1/15.5])
                    sphere(r=15.5);

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
            if (panel == 2) {
                union() {
                    rotate([0, 11.193381591600671, 45 + 79.94651307773178])
                    translate([0, 0, radius - 6])
                    difference() {
                        cylinder(r=radius * sin(3.2278906006438186) + insertionTolerance, h=panelDesignDepth * 2);
                        translate([0,0,insertionTolerance])
                        cylinder(r=radius * sin(3.2278906006438186), h=panelDesignDepth * 2);
                    }


                    rotate([0, 4.210262618975445, 45 + 33.815798010531786])
                    translate([0, 0, radius])
                    scale([1,1, 1 / (radius * sin(3.2278906006438186))])
                    sphere(r=radius * sin(3.2278906006438186));
                }
            }
        }
    }
}

module warn(text)
{
    echo(str("<span style='background-color: #ffffb0'>", text, "</span>"));
}