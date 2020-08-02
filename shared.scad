panelDegrees = 35; // https://rimstar.org/science_electronics_projects/bb-8_dimensions.htm
panelRadius = radius * sin(panelDegrees);
insertionTolerance = 0.3;

visibleBoltHole = 4 / 2;
visibleBoltOuterRadius = radius * 0.07734 / 2;
visibleBoltCoverRadius = visibleBoltOuterRadius / 2;
visibleBoltBezelDepth = visibleBoltOuterRadius * 0.2;
visibleBoltBezel = visibleBoltBezelDepth;

if (camlockBoltRadius * 2 + 2 > camlockNutThickness)
    warn("Camlock Bolt/Nut sizes invalid");

panelRadiusOffset = radius * cos(panelDegrees);

module bodySphere() {
    difference() {
        sphere(radius, $fn=$fnBody);
        sphere(radius - wallThickness, $fn=$fn);
    };
}

module tFrame() {
    difference() {
        bodySphere();

        rotate([90, 0, 0])
        cylinder(r1=panelRadius, r2=panelRadius, h=radius * 2, center=true);
        rotate([0, 90, 0])
        cylinder(r1=panelRadius, r2=panelRadius, h=radius * 2, center=true);
        cylinder(r1=panelRadius, r2=panelRadius, h=radius * 2, center=true);
    }
}

module tFrameThird() {
    camlockOffset = 42.5;
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
        rotate(a=-camlockOffset, v=[-1,0,1])
        rotate([0,-90,90])
        translate([0,0,-(radius - wallThickness - 0)])
        rotate([0,0,-45 + rotationDifference]) // bolt rotation
        camLockSlot(boltLength=camlockBoltLength);

        rotate(a=-camlockOffset, v=[-1,0,1])
        rotate([0,-90,90])
        translate([0,0,-(radius - wallThickness - 0)])
        rotate([0,0,-45 - rotationDifference]) // bolt rotation
        camLockSlot(boltLength=camlockBoltLength);

        rotate(a=camlockOffset, v=[0,-1,1])
        rotate([0,-90,0])
        translate([0,0,-(radius - wallThickness - 0)])
        rotate([0,0,-135 + rotationDifference]) // bolt rotation
        camLockSlot(boltLength=camlockBoltLength);

        rotate(a=camlockOffset, v=[0,-1,1])
        rotate([0,-90,0])
        translate([0,0,-(radius - wallThickness - 0)])
        rotate([0,0,-135 - rotationDifference]) // bolt rotation
        camLockSlot(boltLength=camlockBoltLength);

        // outer-ring holes
        rotate([15,0,0])
        rotate([0,-90,panelDegrees])
        translate([0,0,-(radius - wallThickness - 0)])
        rotate([0,0,180]) // bolt rotation
        camLockSlot(boltLength=camlockBoltLength);

        rotate([30,0,0])
        rotate([0,-90,panelDegrees])
        translate([0,0,-(radius - wallThickness - 0)])
        rotate([0,0,180]) // bolt rotation
        camLockSlot(boltLength=camlockBoltLength);

        rotate([0,-15,0])
        rotate([90,0,-panelDegrees])
        translate([0,0,-(radius - wallThickness - 0)])
        rotate([0,0,90]) // bolt rotation
        camLockSlot(boltLength=camlockBoltLength);

        rotate([0,-30,0])
        rotate([90,0,-panelDegrees])
        translate([0,0,-(radius - wallThickness - 0)])
        rotate([0,0,90]) // bolt rotation
        camLockSlot(boltLength=camlockBoltLength);
        // end outer-ring holes

        // visible bolt-hole cover
        translate([-depth + visibleBoltBezelDepth, 0, 0])
        rotate([0, 90 - 35 / 2, 45])
        translate([0, 0, radius])
        visibleBoltHole();
    }
}

module camLockSlot(boltLength = 50) {
    translate([0,-boltLength/2,0])
    rotate([180, 0, 0])
    union() {
        translate([0,0,-camlockNutMaxDepth])
        cylinder(r1=camlockNutRadius, r2=camlockNutRadius, h=camlockNutThickness + camlockNutMaxDepth, $fn=$fn);

        rotate([90,0,0])
        translate([0, camlockNutThickness * 0.5, 0])
        cylinder(r1=camlockBoltRadius, r2=camlockBoltRadius, h=boltLength);
    }
}

module camlockBolt(boltLength = camlockBoltLength) {
    endcapLength = camlockNutRadius - fittedTolerance * 2 - camlockNutWallThickness;
    endcapOffsetLength = camlockNutRadius + fittedTolerance;
    boltRadius = camlockBoltRadius - insertionTolerance / 2;
    endcapConnectorRadius = boltRadius - camlockNutGripSize;
    bezel = endcapLength * 0.25;
    echo(endcapLength);

    cylinder(r1=endcapConnectorRadius, r2=endcapConnectorRadius, h=boltLength);

    translate([0, 0, endcapOffsetLength + bezel])
    cylinder(r1=boltRadius, r2=boltRadius, h=boltLength - endcapOffsetLength * 2 - bezel * 2);
    translate([0, 0, endcapOffsetLength])
    cylinder(r1=endcapConnectorRadius, r2=boltRadius, h=bezel);
    translate([0, 0, boltLength - endcapOffsetLength - bezel])
    cylinder(r1=boltRadius, r2=endcapConnectorRadius, h=bezel);

    cylinder(r1=boltRadius, r2=boltRadius, h=endcapLength-bezel);
    translate([0, 0, endcapLength - bezel])
    cylinder(r1=boltRadius, r2=endcapConnectorRadius, h=bezel);

    translate([0, 0, boltLength - endcapLength + bezel])
    cylinder(r1=boltRadius, r2=boltRadius, h=endcapLength-bezel);
    translate([0, 0, boltLength - endcapLength])
    cylinder(r1=endcapConnectorRadius, r2=boltRadius, h=bezel);
}

module camlockNut() {
    camlockRadius = camlockNutRadius - fittedTolerance;
    camlockInnerRadius = camlockNutRadius - fittedTolerance - camlockNutWallThickness;
    endcapOpeningRadius = camlockBoltRadius - camlockNutGripSize;

    difference() {
        translate([0,0,insertionTolerance])
        cylinder(r1=camlockRadius, r2=camlockRadius, h=camlockNutThickness, center=true);

        intersection() {
            union() {
                translate([camlockRadius - camlockInnerRadius, 0, - camlockNutThickness / 2 + camlockBoltRadius])
                cylinder(r1=camlockInnerRadius, r2=camlockInnerRadius, h=camlockNutThickness, center=true);
                cylinder(r1=camlockRadius + 5, r2=camlockRadius + 5, h=endcapOpeningRadius * 2, center=true);
            }
            union() {
                rotate([0,0,-90])
                translate([-(camlockRadius*2-camlockBoltRadius),-camlockRadius,-camlockNutThickness / 2])
                cube([camlockRadius * 2, camlockRadius * 2, camlockNutThickness]);

                translate([camlockRadius - camlockInnerRadius * 2 + 2, 0, 0])
                rotate([0, 90, 0])
                cylinder(r1=camlockBoltRadius, r2=camlockBoltRadius, h=camlockNutRadius + camlockInnerRadius *2);
            }
        }
        // side bolt-hole
        rotate([0, 90, 0])
        cylinder(r1=camlockBoltRadius, r2=camlockBoltRadius, h=camlockNutRadius + camlockInnerRadius *2);

        // bottom bolt-hole
        translate([camlockRadius - 1.5, 0, -camlockNutThickness])
        cube([camlockRadius * 2, camlockBoltRadius * 2, camlockNutThickness * 2], center = true);

        // rotate slot for cap opening
        rotate([0, -90, 0])
        cylinder(r1=endcapOpeningRadius, r2=endcapOpeningRadius, h=camlockNutRadius);

        // screwdriver slot
        translate([0, 0, camlockNutThickness - 0.6 + insertionTolerance])
        cube([camlockRadius * 3, 1.2, camlockNutThickness], center=true);
    }
}

module visibleBoltHole() {
    translate([0, 0, -visibleBoltBezelDepth])
    cylinder(r1=visibleBoltOuterRadius - visibleBoltBezel * 2, r2=visibleBoltOuterRadius, h=visibleBoltBezelDepth * 2, $fn=$fnDetail);

    translate([0, 0, -wallThickness * 2])
    cylinder(r = visibleBoltHole, h=wallThickness * 2, $fn=$fn);
}

module warn(text)
{
    echo(str("<span style='background-color: #ffffb0'>", text, "</span>"));
}