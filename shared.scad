panelDegrees = 35; // https://rimstar.org/science_electronics_projects/bb-8_dimensions.htm
panelRadius = radius * sin(panelDegrees);

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

        rotate([0,-90,40])
        translate([0,0,-(radius - wallThickness - 0)])
        rotate([0,0,90])
        translate([0,-camlockBoltLength/2,0]) // needs bolt of length 40
        camLockSlot(boltLength=50);

        rotate([0,-90,50])
        translate([0,0,-(radius - wallThickness - 0)])
        rotate([0,0,90])
        translate([0,-camlockBoltLength/2,0]) // needs bolt of length 40
        camLockSlot(boltLength=50);

        rotate(a=-camlockOffset, v=[-1,0,1])
        rotate([0,-90,90])
        translate([0,0,-(radius - wallThickness - 0)])
        rotate([0,0,-45 + rotationDifference]) // bolt rotation
        translate([0,-camlockBoltLength/2,0]) // needs bolt of length 40
        camLockSlot(boltLength=50);

        rotate(a=-camlockOffset, v=[-1,0,1])
        rotate([0,-90,90])
        translate([0,0,-(radius - wallThickness - 0)])
        rotate([0,0,-45 - rotationDifference]) // bolt rotation
        translate([0,-camlockBoltLength/2,0]) // needs bolt of length 40
        camLockSlot(boltLength=50);

        rotate(a=camlockOffset, v=[0,-1,1])
        rotate([0,-90,0])
        translate([0,0,-(radius - wallThickness - 0)])
        rotate([0,0,-135 + rotationDifference]) // bolt rotation
        translate([0,-camlockBoltLength/2,0]) // needs bolt of length 40
        camLockSlot(boltLength=50);

        rotate(a=camlockOffset, v=[0,-1,1])
        rotate([0,-90,0])
        translate([0,0,-(radius - wallThickness - 0)])
        rotate([0,0,-135 - rotationDifference]) // bolt rotation
        translate([0,-camlockBoltLength/2,0]) // needs bolt of length 40
        camLockSlot(boltLength=50);
    }
}

module camLockSlot(boltLength = 50) {
    rotate([180, 0, 0])
    union() {
        translate([0,0,-camlockNutMaxDepth])
        cylinder(r1=camlockNutRadius, r2=camlockNutRadius, h=camlockNutThickness + camlockNutMaxDepth, $fn=$fn);

        rotate([90,0,0])
        translate([0, camlockNutThickness * 0.5, 0])
        cylinder(r1=camlockBoltRadius, r2=camlockBoltRadius, h=boltLength);
    }
}

module warn(text)
{
    echo(str("<span style='background-color: #ffffb0'>", text, "</span>"));
}