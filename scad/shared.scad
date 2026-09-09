include <lib/computed.scad>;

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
            difference() {
                cylinder(r=panelRingInnerRadius, h=radius * 2);

                linear_extrude(height=radius)
                panelX();
            }

            intersection() {
                sphere(radius - wallThickness/2+insertionTolerance, $fn=$fn);
                cylinder(r=panelRingInnerRadius + panelRingInnerOverlap + insertionTolerance, h=radius);
            }
        }
    }

}

module panelX(offset = 0) {
    for (arm=[0:90:359]) {
        rotate([0,0,arm+45])
        translate([-83, 83, 0])
        offset(r=offset)
        import("panel-x.svg", center=true, dpi=2611.8439045872/panelRingInnerRadius);
    }
}

module panel() {
    difference() {
        intersection() {
            bodySphere(additionalWallThickness = panelAdditionalWallThickness);

            union() {
                intersection() {
                    cylinder(r=panelRingInnerRadius - insertionTolerance, h=radius * 2);

                    difference() {
                        cube([radius*2,radius*2,radius*2], center=true);
                        sphere(radius - wallThickness/2 - insertionTolerance, $fn=$fn);

                        linear_extrude(height=radius)
                        panelX(offset=insertionTolerance);
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

        if (smallPrintBed) {
            translate([
                0, 0,
                radius * 2 - panelHeight + ringThickness])
            rotate([0,0,90 + 20])
            translate([- insertionTolerance / 2, 0, 0])
            cube([insertionTolerance, radius*2, radius*2], center=true);
        }

        translate([
            0, 0,
            radius - panelHeight + ringThickness - insertionTolerance / 2])
        cube([radius*2, radius*2, insertionTolerance], center=true);


        if (smallPrintBed) {
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
}

module panelDesign(panelNumber) {
    intersection() {
        difference() {
            resize(newsize = [panelRingInnerRadius*2,panelRingInnerRadius*2])
            import(str("tool-panel-",panelNumber,".svg"), center=true, dpi=200);

            rotate([0,0,45])
            panelX(insertionTolerance);
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

module tFrameTriangle() {
    rotationDifference = 30.5;
    difference() {
        intersection() {
            polyhedron(
            points=[ [0,0,2*radius],[2*radius,0,0],[0,2*radius,0], // the three points at base
                    [0,0,0]  ],                                 // the apex point
            faces=[ [0,1,3],[1,2,3],
                        [2,0,3],[2,1,0] ]
            );

            tFrame();
        }

        // interlocking holes for other frame triangles
        // when facing from the outside, bottom, left, right
        for (rotationAxis = [[0,0,0],[1,0,0],[0,-1,0]])
            rotate(90, rotationAxis)
            for (interlock = [-5, 5])
                rotate([0,-90,45 + interlock])
                translate([0,0,-(radius - wallThickness - 0)])
                rotate([0,0,90])
                camLockSlot(boltLength=camlockBoltLength);

        // outer-ring holes - when facing from the outside, left, right, top
        for (side = [[0,-90,0], [90,0,0], [180,0,90]])
            for (position = [panelLockBoltDegrees:panelLockBoltDegrees:89])
                rotate(side)
                rotate([0,0,-position])
                rotate([panelDegrees,0,0])
                translate([0,0,-(radius - wallThickness - 0)])
                camLockSlot(boltLength=camlockBoltLength);
        // end outer-ring holes

        // visible bolt-hole covers (bottom, left, right)
        for (rotationAxis = [[0,0,0],[1,0,0],[0,-1,0]])
            rotate(45, rotationAxis)
            rotate(45, [0, 0, 1])
            rotate(90-35/2, [0, 1, 0])
            translate([0, 0, radius])
            outerWallBoltHole();

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
        // TODO: change this first param to offset with panelLockBoltDegrees*0.5
        for (position = [panelLockBoltDegrees:panelLockBoltDegrees:44]){
            rotate([0,-90,0])
            rotate([0,0,-position])
            rotate([panelDegrees,0,0])
            translate([0,0,-(radius - wallThickness - 0)])
            rotate([0,0,180]) // bolt rotation
            camLockSlot(boltLength=camlockBoltLength);

            rotate([90,0,0])
            rotate([0,0,-position -45])
            rotate([panelDegrees,0,0])
            translate([0,0,-(radius - wallThickness - 0)])
            rotate([0,0,180]) // bolt rotation
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
    quarterRingRotation = -(panelLockBoltDegrees+panelRotateLockOffset+rotateLockDegrees);
    difference() {
        intersection() {
            rotate([0,0,quarterRingRotation])
            translate([insertionTolerance*0.5,insertionTolerance*0.5,0])
            cube([radius, radius, radius]);

            panelRing();
        }

        rotate([0,0,quarterRingRotation])
        {
            // end holes
            for (end = [-1 : (split ? 1 : 2) : 1]) {
                rotate([endHoleOffset,0,-45 + 45 * end])
                translate([0,0,-(radius - camlockNutThickness)])
                rotate([0,90,0]) // bolt rotation
                cylinder(r=pinRadius + insertionTolerance / 2, h=pinLength + insertionTolerance, center=true);
            }

            // rotate lock holes
            for(loop = [panelRotateLockOffset : panelLockBoltDegrees : 90]) {
                // The hole in line with the panel arm does not exist to ensure proper alignment of the pieces
                if (loop != panelRotateLockOffset + panelLockBoltDegrees) {
                    rotate([0,0,loop])
                    translate([0,0, radius * cos(panelDegrees)])
                    rotateLockSlot(boltLength=camlockBoltLength, radius = panelRadius, angle = rotateLockDegrees, downwardAngle = panelDegrees);
                }
            }

            // visible bolt
            // TODO: the arm and bolt should line up with the lock hole, regardless of the number of bolts
            rotate([0, panelArmBoltDegrees, 15+panelRotateLockOffset+rotateLockDegrees])
            translate([0, 0, radius])
            rotate([0, 0, 0])
            visibleBoltHole();
        }
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

        translate([0,-boltLength,-camlockNutMaxDepth])
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

module outerWallBoltHole() {
    // front inset
    translate([0, 0, -visibleBoltBezelDepth])
    cylinder(r1=visibleBoltOuterRadius - visibleBoltBezel * 2, r2=visibleBoltOuterRadius, h=visibleBoltBezelDepth * 2, $fn=$fnDetail);

    translate([0, 0, -wallThickness * 2])
    cylinder(r = visibleBoltHole, h=wallThickness * 2, $fn=$fn);

    // back inset
    translate([0, 0, - wallThickness])
    rotate([180, 0, 0])
    translate([0, 0, -visibleBoltBezelDepth])
    cylinder(r1=visibleBoltOuterRadius - visibleBoltBezel * 2, r2=visibleBoltOuterRadius, h=visibleBoltBezelDepth * 2, $fn=$fnDetail);
}

module toolPanel(panel) {
    if (panel == 0)
        panelMain();
    else {
        panelMain()
        union()
        {
            panelDesignEmboss(panel);
            panelCutout(panel);
        }
        panelDesignCurved(panel);
    }
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

module head()
{
    translate([0,0, headOffset])
    union()
    {
        translate([0,0, headBaseHeight + headConeHeight])
        intersection() {
            difference(){
                sphere(headRadius, $fn=$fnBody);

                // make the head hollow.
                sphere(r=headInnerRadius);
            }
            // Just the top half
            translate([-headRadius, -headRadius, 0])
            cube([headRadius*2, headRadius*2, headRadius]);
        }

        // The base has a portion that is purely a cylinder
        color("white")
        translate([0,0, headConeHeight])
        difference() {
            cylinder(headBaseHeight, headRadius, headRadius, $fn=$fnBody);

            translate([0,0, - insertionTolerance])
            cylinder(headBaseHeight + insertionTolerance*2, r=headInnerRadius, $fn=$fnBody);
        }

        // ... followed by a cone that comes down to the base itself
        difference() {
            cylinder(headConeHeight, headConeRadius, headRadius, $fn=$fnBody);

            // And hollow out the cone, but end in a cylinder so we don't have a sharp edge
            translate([0,0, headBaseHeight*0.2 - insertionTolerance])
            cylinder(headBaseHeight*0.8 + insertionTolerance*2, headConeRadius - wallThickness, headInnerRadius, $fn=$fnBody);
            translate([0,0, - insertionTolerance])
            cylinder(headBaseHeight + insertionTolerance*2, r=headConeRadius - wallThickness, $fn=$fnBody);
        }
    }
}

module headShell()
{
    render()
    {
        translate([0,0, headBaseHeight + headConeHeight])
        intersection() {
            difference(){
                sphere(headRadius + insertionTolerance, $fn=$fnBody);

                sphere(headRadius - headInsetMinThickness, $fn=$fnBody);
            }
            // Just the top half
            translate([-headRadius, -headRadius, 0])
            cube([headRadius*2, headRadius*2, headRadius]);
        }

        // The base has a portion that is purely a cylinder
        translate([0,0, headConeHeight + headBaseHeight / 2])
        difference() {
            cylinder(headBaseHeight, r=headRadius+ insertionTolerance, $fn=$fnBody, center=true);

            cylinder(headBaseHeight + insertionTolerance*2, r=headRadius-headInsetMinThickness, $fn=$fnBody, center=true);
        }

        // ... followed by a cone that comes down to the base itself
        difference() {
            cylinder(headConeHeight, r1=headConeRadius + insertionTolerance, r2=headRadius + insertionTolerance, $fn=$fnBody);

            tmpoffset = (headRadius-headConeRadius)/headConeHeight * insertionTolerance;
            translate([0,0,-insertionTolerance])
            cylinder(headConeHeight+ insertionTolerance * 2, r1=headConeRadius-headInsetMinThickness - tmpoffset, r2=headRadius-headInsetMinThickness+tmpoffset, $fn=$fnBody);
        }
    }
}

module headWedge(deg1=0, deg2=1, a=45)
{
    h1 = sin(deg1);
    h2 = sin(deg2);

    translate([0,0, headBaseHeight + headConeHeight])
    polyhedron(
        [
            [0,0,headRadius*h1],
            [0,0,headRadius*h2],
            [0,-headRadius,headRadius*h1],
            [0,-headRadius,headRadius*h2],
            [tan(a)*headRadius,-headRadius,headRadius*h1],
            [tan(a)*headRadius,-headRadius,headRadius*h2]
        ],
        faces = [
            [0,1,3,2],
            [2,3,5,4],
            [1,0,4,5],
            [0,2,4],
            [1,5,3]
        ],
        convexity=1
    );
}

headLowerRingTopY = -headBaseHeight * 0.0;
headLowerRingBottomY = -headBaseHeight * 0.9;
headTopGreyRingBottomDeg = 90-35;
headTopGreyRingTopDeg = 90-22;
headPartialOrangeRingTopDeg = 90-39;
headPartialOrangeRingBottomDeg = 90-43;
headBaseOrangeRingTopDeg = 8;

module headInlays(part = 0)
{
    translate([0,0, headOffset])
    intersection()
    {
        headShell();

        union()
        {
            // Orange top ring inlay. Splits in two parts for printing.
            if(part == 0 || part == 1 || part == 2)
                difference() {
                    translate([-headRadius*1.5, -headRadius*(part == 0 ? 1.5 : part == 1 ? 0 : 3), headY(deg=headPartialOrangeRingBottomDeg)])
                    cube([headRadius*3, headRadius*3, headY(deg=headPartialOrangeRingTopDeg)-headY(deg=headPartialOrangeRingBottomDeg)]);

                    rotate([0,0,90])
                    polyhedron(
                        [
                            [0,0,headY(deg=headPartialOrangeRingBottomDeg)],
                            [tan(19.5)*headRadius,-headRadius,headY(deg=headPartialOrangeRingBottomDeg)],
                            [tan(-32.5)*headRadius,-headRadius,headY(deg=headPartialOrangeRingBottomDeg)],
                            [tan(-6.5)*headRadius,-headRadius,headY(deg=90)],
                        ],
                        faces = [
                            [0,3,2],
                            [2,3,1],
                            [0,1,3],
                            [0,2,1]
                        ],
                        convexity=1
                    );
                }

            #rotate([0,0,90])
            if (part == 0 || part == 3) {
                rotate([0,0,45+2])
                headWedge(deg2=headBaseOrangeRingTopDeg, a=10);
                rotate([0,0,45+2+10+3])
                headWedge(deg2=headBaseOrangeRingTopDeg, a=6);
                rotate([0,0,45+2+10+3+6+3])
                headWedge(deg2=headBaseOrangeRingTopDeg, a=6);
                rotate([0,0,45+2+10+3+6+3+6+3])//78
                headWedge(deg2=headBaseOrangeRingTopDeg, a=27); // 105
                rotate([0,0,105+3])
                headWedge(deg2=headBaseOrangeRingTopDeg, a=6);
                rotate([0,0,105+3+6+3])
                headWedge(deg2=headBaseOrangeRingTopDeg, a=24);

                // lined up with the vertical slice near the back of the head
                rotate([0,0,148.5])
                headWedge(deg2=headBaseOrangeRingTopDeg, a=3);

                rotate([0,0,148.5+3+6])
                headWedge(deg2=headBaseOrangeRingTopDeg, a=24);
                rotate([0,0,148.5+3+6+24+3])
                headWedge(deg2=headBaseOrangeRingTopDeg, a=6);
                rotate([0,0,148.5+3+6+24+3+6+3])
                headWedge(deg2=headBaseOrangeRingTopDeg, a=27);
                rotate([0,0,148.5+3+6+24+3+6+3+27+4])
                headWedge(deg2=headBaseOrangeRingTopDeg, a=6);
                rotate([0,0,148.5+3+6+24+3+6+3+27+4+6+3])
                headWedge(deg2=headBaseOrangeRingTopDeg, a=15);
                rotate([0,0,148.5+3+6+24+3+6+3+27+4+6+3+15+4])
                headWedge(deg2=headBaseOrangeRingTopDeg, a=6);

                rotate([0,0,148.5+3+6+24+3+6+3+27+4+6+3+15+4+6+4])
                // This has the blue light on the sphero BB-8, which is the processor display (grid LCD as seen on R2-D2)
                headWedge(deg2=headBaseOrangeRingTopDeg, a=27);

                rotate([0,0,148.5+3+6+24+3+6+3+27+4+6+3+15+4+6+4+27+4])
                headWedge(deg2=headBaseOrangeRingTopDeg, a=6);

                rotate([0,0,148.5+3+6+24+3+6+3+27+4+6+3+15+4+6+4+27+4+6+4])
                headWedge(deg2=headBaseOrangeRingTopDeg, a=24);

                rotate([0,0,148.5+3+6+24+3+6+3+27+4+6+3+15+4+6+4+27+4+6+4+24+3])
                headWedge(deg2=headBaseOrangeRingTopDeg, a=3);

            }
        }
    }
}

function headY(y=0, deg=0) =
    (deg == 0 ? y : sin(deg) * headRadius)
        + headConeHeight + headBaseHeight;

module headEtchings()
{
    translate([0,0, headOffset])
    intersection()
    {
        headShell();

        union()
        {
            // horizontal lines near base
            for (ring = [0.15,0.95])
                translate([
                    0, 0,
                    headConeHeight + headBaseHeight * ring])
                cube([headRadius*2, headRadius*2, headBaseHeight * 0.1], center=true);

            // lines on grey top ring
            greyRingH = headY(deg = headTopGreyRingTopDeg) - headY(deg = headTopGreyRingBottomDeg);
            intersection() {
                translate([0,0,headY(deg=headTopGreyRingBottomDeg)+greyRingH/2])
                cube([headRadius*3, headRadius*3, greyRingH], center=true);

                union()
                for (i=[0:1:7])

                    rotate([0,0,22.5 * i - (7.5 * (i%2))])
                    cube([headRadius*2, etchLineThickness, headRadius*3], center=true);
            }

            // vertical etch lines in white
            for (vertLineRotation=[88.5,91.5,148.5,151.5,238.5,241.5])
                rotate([0,0,vertLineRotation])
                translate([0,etchLineThickness/2,headY(deg=8)])
                cube([headRadius*2, etchLineThickness, headY(deg=headPartialOrangeRingBottomDeg)-headY(deg=8)]);

            // add front eye etchings
            // TODO: consider https://github.com/alidaf/3D-Printing/blob/main/Curved%20SVG%20Images/Curved%20SVG%20Images.scad
            #translate([0,0,headY()])
            rotate([0,0,96.5])
            scale([1.27,1,1])
            translate([-65,0,0])
            rotate([90,0,0])
            linear_extrude(height = headRadius)
            offset(r=etchLineThickness/2)
            scale(headRadius/87)
            import("head-eye-outline.svg", dpi=72);

            // TODO: add horizontal panels
            // TODO: add top etchings
        }
    }
}

module headHorizontalSlice(y=0, deg=0, isUp=false)
{
    y = headY(y=y, deg=deg);
    outerX = cos(deg) * headRadius;
    innerX = cos(asin(sin(deg) * headRadius / headInnerRadius)) * headInnerRadius;
    x = (outerX*1 + innerX*2) / 3;
    translate([0,0,y])
    {
        difference() {
            cube([headRadius*3, headRadius*3, insertionTolerance], center=true);
            cylinder(y * 2, r=x - insertionTolerance, center=true);
        }

        translate([0,0, (isUp?1:-1) * (headCutHeight / 2)])
        difference()
        {
            cylinder(insertionTolerance * 2 + headCutHeight, r=x, center=true);
            cylinder(insertionTolerance * 4 + headCutHeight, r=x - insertionTolerance, center=true);
        }

        translate([0,0, (isUp?1:-1) * (headCutHeight + insertionTolerance)])
        cylinder(insertionTolerance, r=x);
    }
}

module headCuts()
{
    translate([0,0, headOffset])
    {
        headHorizontalSlice(deg = headTopGreyRingBottomDeg);
        headHorizontalSlice(deg = headTopGreyRingTopDeg);

        headHorizontalSlice(y = headLowerRingTopY, isUp=true);
        headHorizontalSlice(y = headLowerRingBottomY);
    }
}

module headDetail()
{
    difference()
    {
        head();

        headInlays();
        headEtchings();
        headCuts();
    }
}

module warn(text)
{
    echo(str("<span style='background-color: #ffffb0'>", text, "</span>"));
}