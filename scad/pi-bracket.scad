xBetweenPi = 58;
yBetweenPi = 49;
piBoard = [85, 56];
standRadiusPi = 6 / 2;
pinRadiusPi = 2.75 / 2;
outerRadiusPi = 3.5;
piBoardMiddle = piBoard - [outerRadiusPi,outerRadiusPi] * 2;
caseHeightAbovePiBoard = 18.3;
arduinoCoord = 0.001 * 25.4;
arduinoMountingHoleRadius = 125 / 2 * arduinoCoord;
insertionTolerance = 0.3;
m3holeRadius = 1.5;
boardThickness = 1.6;
arduinoBoard = [2700 * arduinoCoord, 2100 * arduinoCoord];
zipTieHole = [7,3];

bb8OuterRadius = 253;
bb8Radius = bb8OuterRadius - 25.4; // 0.5 wall thickness, twice

batteryVerticalBuffer = 25.4/2;
motorRadius = 186.4;
angle = asin(motorRadius / bb8Radius);

anker = [3.62, 2.36, 0.87] * 25.4;

outwardMotor = 23.9 - m3holeRadius + 11 + 23;
motorSizeDown = 60 - 23;
motorSizeUp = 60 + 23;
motorPlateSize = [40,42.5];

rot30 = rotationMatrix (30);
rot60 = rotationMatrix (60);
rot90 = rotationMatrix (90);
rot120 = rotationMatrix (120);
rot150 = rotationMatrix (150);
rot180 = rotationMatrix (180);
rot210 = rotationMatrix (210);
rot240 = rotationMatrix (240);
rot270 = rotationMatrix (270);
rot300 = rotationMatrix (300);
rot330 = rotationMatrix (330);

motorBracketThickness = 0.4 * 25.4;
arcSmoothness = motorRadius*2;
arcCenter = [arcSmoothness,0];
motorPlateRadius = motorRadius - outwardMotor;
motorPlateCenter = [motorPlateRadius, 0];
motorPlateLeftOuter = motorPlateCenter + [motorPlateSize[0],-motorPlateSize[1]]/2;
motorPlateRightOuter = motorPlateCenter + motorPlateSize/2;
motorPlateLeftInner = motorPlateCenter + [-motorPlateSize[0],-motorPlateSize[1]]/2;
motorPlateRightInner = motorPlateCenter + [-motorPlateSize[0],motorPlateSize[1]]/2;
function dist(v) = sqrt(v * v);
function norm(v) = v / dist(v);
armBase = [motorPlateLeftOuter[0] - motorPlateLeftOuter[1] / sin(30) * cos(30), 0];
leftArm = armBase * rot120 - armBase;
rightArm = armBase * rot240 - armBase;

armDistance = dist((motorPlateLeftInner * rot120) - motorPlateRightInner);
function rotationMatrix (angle) = [
    [cos(angle), -sin(angle)],
    [sin(angle),  cos(angle)]
];
motorCenter = motorPlateRightOuter * rotationMatrix(60);
motorOuterFactor = (motorPlateLeftOuter - armBase).x / leftArm.x;
motorInnerFactor = (motorPlateLeftInner - armBase).x / leftArm.x;
motorWidthFactor = ((motorPlateLeftOuter * rot120 - motorPlateRightOuter * rot120) * rightArm) / (rightArm * rightArm);

armThickness = 0.4 * 25.4;
arcRadius = sqrt((arcCenter - motorCenter) * (arcCenter - motorCenter));

batteryDimensions = [70, 90, 101];
batteryDimensionsHalf = batteryDimensions / 2;
batteryCaseThickness = 3;
circuitryToBatteryBracketMounts = [-0.5, 0, 0.47] * batteryDimensions.x;
batteryBracketBackMounts = [-0.4, 0, 0.4] * batteryDimensions.x;

centerPlatformMiddleRadius = 70 - armThickness;

bottomRiding = sin(acos((motorRadius) / bb8Radius)) * bb8Radius;

$fn = 30;

module piHoles() {
    translate([outerRadiusPi + 0, outerRadiusPi + 0]) children();
    translate([outerRadiusPi + xBetweenPi, outerRadiusPi + 0]) children();
    translate([outerRadiusPi + 0, outerRadiusPi + yBetweenPi]) children();
    translate([outerRadiusPi + xBetweenPi, outerRadiusPi + yBetweenPi]) children();
}

module piBoard() {
    linear_extrude(height = boardThickness)
    difference() {
        translate([outerRadiusPi, outerRadiusPi])
        offset(r = outerRadiusPi) {
            square(piBoardMiddle);
        }

        piHoles() circle(r=pinRadiusPi);
    }
}

module arduinoHoles(availableHoles = [0,1,2,3], arduinoCoord = arduinoCoord) {
    holes = [[550, 100],[2600, 300],[2600, 1400],[600, 2000]];

    for (index = availableHoles)
    translate(holes[index] * arduinoCoord)
    children();
}

module arduinoBoard(availableHoles = [0,1,2,3]) {
    linear_extrude(height = 18, center=true)
    scale([arduinoCoord, arduinoCoord])
    difference()
    {
        polygon([[0,0],[2600,0],[2600,100],[2700,200],[2700,1490],[2600,1590],[2600,2040],[2540,2100],[0,2100]]);

        arduinoHoles(availableHoles, arduinoCoord=1)
        circle(r=arduinoMountingHoleRadius / arduinoCoord);
    }
}

circuitrySpaceBetween = zipTieHole.y * 3;
module circuitryBracketHoles() {

    translate([circuitrySpaceBetween/2 + piBoard.y, -piBoard.x / 2,10])
    rotate([0,0,90])
    {
        piHoles()
        circle(r=m3holeRadius + insertionTolerance);

        triangles([outerRadiusPi, outerRadiusPi], [xBetweenPi+outerRadiusPi, yBetweenPi+outerRadiusPi], 3);
    }

    translate([-circuitrySpaceBetween/2,0,10])
    translate([-arduinoBoard.x, piBoard.x / 2, 0])
    {
        rotate([180,0,0])
        arduinoHoles([1,3])
        circle(r=m3holeRadius + insertionTolerance);

        translate([0, -arduinoBoard.y, 0])
        arduinoHoles([1,3])
        circle(r=m3holeRadius + insertionTolerance);
    }
}

module board() {
    difference() {
        offset(r = outerRadiusPi)
        hull() {
            children();
        }

        children();
    }
}

module circuitryBracket() {
    separatorHeight = 2;

    midSpace = 20;
    topSpace = 16;

    color("green")
    linear_extrude(height = separatorHeight)
    board() {
        circuitryBracketHoles();
        children();
    }

    %translate([0,0, separatorHeight + insertionTolerance * 2])
    union() {
        translate([circuitrySpaceBetween/2 + piBoard.y, -piBoard.x / 2,10])
        rotate([0,0,90])
        piBoard();

        translate([-circuitrySpaceBetween/2,0,10])
        translate([-arduinoBoard.x, piBoard.x / 2, 0])
        {
            rotate([180,0,0])
            *arduinoBoard([1,3]);

            translate([0, -arduinoBoard.y, 0])
            arduinoBoard([1,3]);
        }
    }

}

module ankerBracket() {
    separatorHeight = 2;
    color("green")
    linear_extrude(height = separatorHeight)
    board() {
        rotate([0,0,-90])
        triangles(anker / -2, anker / 2, 6);

        for (x = [-0.5, 0.5])
            for (y = [-0.35, 0, 0.35])
                translate([x * anker.y, y * anker.x])
                rotate(90)
                square(zipTieHole, center=true);
        children();
    }

    %translate([0,0, separatorHeight + insertionTolerance * 2])
    rotate(90)
    translate([anker.x, anker.y, 0] * -0.5)
    phoneBattery();
}

module batteryZipTieHoles() {
    for (x = [-0.35, 0.35]* batteryDimensions.x)
        for (y = [-0.5, 0.5] * (batteryDimensions.y + batteryBraceThickness * 2))
            translate([x, y])
            children();

    for (i = [[-0.5, -1/3],[-0.5,1/3], [0.5,0.2], [0.5,-0.2]])
        translate([i.x * (batteryDimensions.x + batteryBraceThickness), i.y * batteryDimensions.y])
        rotate(90)
        children();

}

batteryBuffer = 10;
batteryBracketThickness = motorBracketThickness;
batteryLowerBracketThickness = 3;
batterySpaceBetween = zipTieHole.y * 3;
batteryInnerHeight = centerPlatformMiddleRadius * sin(120) * 2;
module batteryAnchor() {

    color("grey")
    linear_extrude(height = batteryBracketThickness)
    difference() {
        offset(r = outerRadiusPi)
        square([batteryDimensions.x + batterySpaceBetween * 2, max(batteryDimensions.y, batteryInnerHeight) + outerRadiusPi + batterySpaceBetween], center=true);


        triangles([-sin(45) * centerPlatformMiddleRadius, -sin(45) * centerPlatformMiddleRadius], [sin(45) * centerPlatformMiddleRadius, sin(45) * centerPlatformMiddleRadius], 6);

        for (i = [0,90,180,270])
            rotate(i)
            translate([sin(45) * centerPlatformMiddleRadius, 0])
            circle(r=m3holeRadius + insertionTolerance);

        for (i = [0,120,240])
            rotate(i)
            translate([centerPlatformMiddleRadius, 0])
            circle(r=m3holeRadius + insertionTolerance);

        batteryZipTieHoles()
        square(zipTieHole, center = true);
    }

}

batteryBraceThickness = 6;
module singleBatteryBracket() {
    batteryBaseDimensions = [batteryDimensions.x, batteryDimensions.y];

    linear_extrude(height = batteryLowerBracketThickness)
    difference() {
        offset(r = outerRadiusPi)
        square([batteryDimensions.x + batterySpaceBetween * 2, max(batteryDimensions.y, batteryInnerHeight) + outerRadiusPi + batterySpaceBetween], center=true);

        triangles(-batteryBaseDimensions/2, batteryBaseDimensions/2, batteryBraceThickness);

        batteryZipTieHoles()
        square(zipTieHole, center = true);
    }

    translate([0,0, -batteryBuffer])
    linear_extrude(height = batteryBuffer + batteryLowerBracketThickness)
    {
        difference() {
            union() {
        translate([batteryDimensions.x / 2, 0])
        square([6, batteryDimensions.y], center=true);

        for (i = [-0.2, 0.2])
        translate([0, i * batteryDimensions.y])
        square([batteryDimensions.x, 6], center=true);
            }

            batteryZipTieHoles()
            square([9,9], center=true);
        }
    }

    translate([0,0, -batteryBuffer-batteryBraceThickness])
    difference() {
        linear_extrude(height = batteryBuffer+batteryBraceThickness)
        difference()
        {
            union() {
                translate([(batteryDimensions.x + batteryBraceThickness) / 2 + insertionTolerance / 2, 0])
                square([batteryBraceThickness - insertionTolerance, batteryDimensions.y+batteryBraceThickness*3], center=true);

                for (i = [-1, 1])
                    translate([0, i * ((batteryDimensions.y / 2 + batteryBraceThickness) + insertionTolerance)])
                    square([batteryDimensions.x+batteryBraceThickness*2, batteryBraceThickness], center=true);
            }

            batteryZipTieHoles()
            square([9,9], center=true);
        }

        for (i = circuitryToBatteryBracketMounts)
            for (x = [-1, 1])
                translate([
                    i,
                    ((batteryDimensions.y / 2 + batteryBraceThickness) + insertionTolerance) * x,
                    (batteryBuffer+batteryBraceThickness) - (outerRadiusPi + m3holeRadius + insertionTolerance * 3)
                ])
                rotate([90,0,0])
                cylinder(r=m3holeRadius + insertionTolerance, h=batteryBraceThickness * 2, center=true);
    }
}

module batteryBracket() {
    batteryBaseDimensions = [batteryDimensions.x, batteryDimensions.y];

    color("blue")
    translate([0,0,-batteryLowerBracketThickness])
    {
        singleBatteryBracket();

        translate([0,0, - batteryDimensions.z - insertionTolerance * 2])
        rotate([180,0,0])
        translate([0,0,batteryBuffer * 2])
        {
            singleBatteryBracket();
        }
    }


    %translate([0,0,-batteryBuffer -batteryDimensions.z / 2 - batteryLowerBracketThickness - insertionTolerance])
    battery();
}

module battery() {
    cube(batteryDimensions, center=true);
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

module motorHoles() {
    translate([-20,-23.9 + m3holeRadius])
    {
        translate([3.6 + m3holeRadius, 42.5 - (30.6 + m3holeRadius)])
        children();

        translate([40 - (3.6 + m3holeRadius), 42.5 - (30.6 + m3holeRadius)])
        children();

        translate([3.6 + m3holeRadius, 42.5 - (6.6 + m3holeRadius)])
        children();

        translate([40 - (3.6 + m3holeRadius), 42.5 - (6.6 + m3holeRadius)])
        children();
    }
}

module motor() {
    translate([0,m3holeRadius - 23.9, 28.5])
    rotate([-90])
    cylinder(r=37/2, h=60);

    translate([0, 0, 28.5])
    rotate([-90])
    translate([-20, -20 + 8.5, m3holeRadius - 23.9])
    linear_extrude(2.9)
    square([40,40]);

    linear_extrude(2.9)
    difference() {
        translate([-motorPlateSize[0]/2,m3holeRadius - 23.9])
        square(motorPlateSize);

        translate([0,0])
        motorHoles()
        circle(r=m3holeRadius);
    };

    translate([0,m3holeRadius - 23.9, 28.5])
    rotate([-90])
    translate([0,0,65])
    cube([5,30,10], center=true);

    translate([0,-23.9 + m3holeRadius - 11,23])
    rotate([90])
    cylinder(r=60, h=23);
}

module phoneBattery()
{
    hull() {
        translate([0,anker.z / 2,0])
        cube(anker - [0,anker.z,0]);

        translate([0,anker.z / 2,anker.z / 2])
        rotate([0,90,0])
        cylinder(r=anker.z / 2, h=anker.x);

        translate([0,anker.y - anker.z / 2,anker.z / 2])
        rotate([0,90,0])
        cylinder(r=anker.z / 2, h=anker.x);
    }
}

module motorBracket() {
    color("white")
    difference() {
        linear_extrude(motorBracketThickness)
        {
            intersection() {
                difference() {
                    union() {
                        for (i=[0,120,240]) rotate(i)
                        {
                            // plate for motor
                            polygon([
                                motorPlateLeftOuter,
                                motorPlateRightOuter,
                                motorPlateRightInner,
                                motorPlateLeftInner,
                            ]);

                            // curved outer arms
                            rotate(180)
                            translate(arcCenter)
                            difference() {
                                circle(r=arcRadius+armThickness,$fn=120);
                                circle(r=arcRadius,$fn=120);
                            }

                            // arm at plate
                            translate([(armBase + rightArm * motorInnerFactor).x-(insertionTolerance+armThickness)/2,motorPlateLeftInner.y-motorPlateRightInner.y+armThickness / 2])
                            square([armThickness, abs(motorPlateLeftInner.y-motorPlateRightInner.y)*2], center=true);
                        }

                        difference() {
                            circle(r=centerPlatformMiddleRadius+armThickness);

                            circle(r=centerPlatformMiddleRadius-armThickness);

                            for (i=[0,120,240]) rotate(i)
                            {
                                rotate(180)
                                translate(arcCenter)
                                circle(r=arcRadius+armThickness + insertionTolerance,$fn=120);
                            }
                        }
                    }

                    for (i=[0,120,240]) rotate(i)
                    {
                        // plate holes for motor
                        translate(motorPlateCenter)
                        rotate([0,0,90])
                        motorHoles()
                        circle(r=m3holeRadius+insertionTolerance);

                        // hole in plate for nut control
                        polygon([
                            (motorPlateCenter + motorPlateLeftOuter) / 2,
                            (motorPlateCenter + motorPlateRightOuter) / 2,
                            (motorPlateCenter + motorPlateRightInner) / 2,
                            (motorPlateCenter + motorPlateLeftInner) / 2,
                        ]);

                        // zip tie holes in center platform
                        translate([0, centerPlatformMiddleRadius])
                        square(zipTieHole, center=true);
                        translate([0, -centerPlatformMiddleRadius])
                        square(zipTieHole, center=true);

                        // m3Holes in center platform
                        translate([centerPlatformMiddleRadius, 0])
                        circle(r=m3holeRadius);

                        // slice to separate arm from plate
                        translate([(armBase + rightArm * motorInnerFactor).x-insertionTolerance,(motorPlateLeftInner.y-motorPlateRightInner.y)*2])
                        square([insertionTolerance, abs(motorPlateLeftInner.y-motorPlateRightInner.y)*2], center=false);

                        rotate(180)
                        translate(arcCenter)
                        circle(r=arcRadius,$fn=120);
                    }
                }

                polygon([
                    armBase + leftArm * motorOuterFactor,
                    armBase + leftArm * (1 - motorOuterFactor),
                    armBase + leftArm * (1 - motorOuterFactor) + rightArm * motorWidthFactor,
                    armBase + rightArm * (1 - motorOuterFactor) + leftArm * motorWidthFactor,
                    armBase + rightArm * (1 - motorOuterFactor),
                    armBase + rightArm * motorOuterFactor,
                ]);
            }

        }
        translate([0,0,motorBracketThickness / 2]) {

            for (i=[0,120,240]) rotate(i)
            {
                // bolt from plate to arm
                translate([armBase.x + leftArm.x * motorInnerFactor, 0, 0])
                rotate([0,90,0])
                cylinder(r=m3holeRadius+insertionTolerance, h=armThickness * 3, center=true);

                // hole from center platform to arms
                translate([-70,0,0])
                rotate([0,90,0])
                cylinder(r=m3holeRadius+insertionTolerance, h=70, center=true);
            }
        }
    }

    *circle(r=motorRadius);

}

translate([0,0, -bottomRiding])
{
    translate(
        [
            0,
            batteryDimensions.y / 2 + batteryBraceThickness + insertionTolerance *2 + 5,
            motorSizeUp
            - piBoard.x / 2 - circuitrySpaceBetween
            - (m3holeRadius - insertionTolerance) * 2 - outerRadiusPi
            - batteryLowerBracketThickness
            - insertionTolerance
        ]

    )
    rotate([-90,0,0])
    circuitryBracket()
    {
        for (i = circuitryToBatteryBracketMounts)
            translate([i, -piBoard.x / 2 - circuitrySpaceBetween])
            circle(r=m3holeRadius + insertionTolerance);
    };

    ankerBracketAdjustment = [10,0,-12];
    translate(
        [
            0,
            -batteryDimensions.y / 2 - batteryBraceThickness - insertionTolerance *2 - 5,
            motorSizeUp
            - piBoard.x / 2 - circuitrySpaceBetween
            - (m3holeRadius - insertionTolerance) * 2 - outerRadiusPi
            - batteryLowerBracketThickness
            - insertionTolerance
        ] + ankerBracketAdjustment
    )
    rotate([90,90,0])
    ankerBracket()
    {
        for (i = circuitryToBatteryBracketMounts)
            translate([-piBoard.x / 2 - circuitrySpaceBetween + ankerBracketAdjustment.z, i - ankerBracketAdjustment.x])
            circle(r=m3holeRadius + insertionTolerance);
    }

    translate([0, 0, motorSizeUp + motorBracketThickness + insertionTolerance])
    batteryAnchor();

    translate([0,0, motorSizeUp - insertionTolerance])
    batteryBracket();

    %for (i = [0,1,2]) {
        rotate([0,0,i*120+90])
        translate([0,-motorRadius,0])
        //rotate([-angle])
        translate([0,outwardMotor,motorSizeUp])
        rotate([0,180,0])
        motor();
    }

    translate([0,0,motorSizeUp])
        //for (i=[0,120,240])
            //rotate([0,0,i])
            motorBracket();

    %translate([0,0,bottomRiding])
    difference() {
        sphere(r=bb8OuterRadius, $fn=120);
        sphere(r=bb8Radius, $fn=120);

        *cylinder(center=true, r=bb8OuterRadius * sin(35), h=bb8OuterRadius*2);
        *rotate([90,0,0]) cylinder(center=true, r=bb8OuterRadius * sin(35), h=bb8OuterRadius*2);
        *rotate([0,90,0]) cylinder(center=true, r=bb8OuterRadius * sin(35), h=bb8OuterRadius*2);

        translate([0,0,-bottomRiding])
        translate([0,0,bb8OuterRadius])
        cube([1,1,1]*bb8OuterRadius*2, center=true);
    }
}

*translate([40,0,0])
cube([9*25.4,6*25.4,6*25.4], center=true);
