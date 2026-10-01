include <battery.scad>;

module lowerZipTieHoles() {
    for (x = lowerZipTieHolesX)
        for (y = lowerZipTieHolesY)
            translate([x, y])
            children();
}

module bracketWalls() {
    translate([(batteryDimensions.x + batteryBraceThickness) / 2 + insertionTolerance / 2, 0])
    square([batteryBraceThickness - insertionTolerance, batteryDimensions.y+batteryBraceThickness*2], center=true);

    for (i = [-1, 1])
        translate([0, i * ((batteryDimensions.y + batteryBraceThickness) / 2 + insertionTolerance)])
        square([batteryDimensions.x+batteryBraceThickness*2, batteryBraceThickness], center=true);
}

module singleBatteryBracket() {
    batteryBaseDimensions = [batteryDimensions.x, batteryDimensions.y];

    linear_extrude(height = batteryBracketThickness, convexity = 8)
    difference() {
        offset(r = outerRadiusPi)
        hull() {
            square([batteryDimensions.x + batterySpaceBetween * 2, batteryDimensions.y + outerRadiusPi + batterySpaceBetween], center=true);
            bracketWalls();

            lowerZipTieHoles()
            square(zipTieHole, center = true);

            // hole for zip tie to secure battery
            translate([-batteryBaseDimensions.x / 2 - 5, 0])
            rotate(90)
            square(zipTieHole, center = true);
        }

        triangles(-batteryBaseDimensions/2, batteryBaseDimensions/2, batteryBraceThickness);

        lowerZipTieHoles()
        square(zipTieHole, center = true);

        // hole for zip tie to secure battery
        translate([-batteryBaseDimensions.x / 2 - 5, 0])
        rotate(90)
        square(zipTieHole, center = true);
    }

    translate([0,0, -batteryBraceHeight])
    difference() {
        linear_extrude(height = batteryBraceHeight)
        difference()
        {
            bracketWalls();

            lowerZipTieHoles()
            square(zipTieHole * 2.2, center=true);
        }
    }
}

module triangles(corner1, corner2, offset=3) {
    mid = ([corner1.x, corner1.y] + [corner2.x, corner2.y]) / 2;
    offset(-offset)
    polygon([mid,[corner1.x,corner2.y],[corner1.x,corner1.y]]);
    offset(-offset)
    polygon([mid,[corner2.x,corner2.y],[corner2.x,corner1.y]]);

    offset(-offset)
    polygon([mid,[corner2.x,corner1.y],[corner1.x,corner1.y]]);
    offset(-offset)
    polygon([mid,[corner2.x,corner2.y],[corner1.x,corner2.y]]);
}

module batteryBracket() {
    batteryBaseDimensions = [batteryDimensions.x, batteryDimensions.y];

    color("blue")
    translate([0,0,-batteryBracketThickness])
    {
        singleBatteryBracket();

        translate([0,0, - batteryDimensions.z - insertionTolerance * 2])
        rotate([180,0,0])
        {
            singleBatteryBracket();
        }
    }


    translate([0,0,-batteryDimensions.z / 2 - batteryBracketThickness - insertionTolerance])
    battery();
}
