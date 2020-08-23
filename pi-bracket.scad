xBetweenPi = 58;
boardXPi = 78;
yBetweenPi = 49;
standRadiusPi = 6 / 2;
pinRadiusPi = 2.75 / 2;
outerRadiusPi = 3.5;
caseHeightAbovePiBoard = 18.3;
arduinoCoord = 0.001 * 25.4;
arduinoMountingHoleRadius = 125 / 2 * arduinoCoord;
insertionTolerance = 0.3;
m3holeRadius = 1.5;
gripThickness = 2;
gripSpacing = 2;
boardThickness = 1.6;
boardYArduino = 2100 * arduinoCoord;
boardXArduino = 2700 * arduinoCoord;

bb8OuterRadius = 253;
bb8Radius = bb8OuterRadius - 25.4; // 0.5 wall thickness, twice

batteryVerticalBuffer = 3;
motorRadius = 186.4;
angle = asin(motorRadius / bb8Radius);

$fn = 30;

module piHoles() {
    translate([0, 0]) children();
    translate([xBetweenPi, 0]) children();
    translate([0, yBetweenPi]) children();
    translate([xBetweenPi, yBetweenPi]) children();
}

module piBoard() {
    linear_extrude(height = boardThickness, center=true)
    translate([outerRadiusPi, outerRadiusPi])
    difference() {
        offset(r = outerRadiusPi) {
            square([boardXPi, yBetweenPi]);
        }

        piHoles() circle(r=pinRadiusPi);
    }
}

module arduinoHoles(availableHoles = [0,1,2,3], arduinoCoord = arduinoCoord) {
    holes = [[550, 100],[2600, 300],[2600, 1400],[600, 2000]];

    for (index = availableHoles)
    translate([holes[index][0] * arduinoCoord, holes[index][1] * arduinoCoord])
    children();
}

module arduinoBoard(availableHoles = [0,1,2,3]) {
    linear_extrude(height = boardThickness, center=true)
    scale([arduinoCoord, arduinoCoord])
    difference()
    {
        polygon([[0,0],[2600,0],[2600,100],[2700,200],[2700,1490],[2600,1590],[2600,2040],[2540,2100],[0,2100]]);

        arduinoHoles(availableHoles, arduinoCoord=1)
        circle(r=arduinoMountingHoleRadius / arduinoCoord);
    }
}

module piPostScrewTop(height = 10, insertion = 5) {
    union() {
        linear_extrude(height = insertion)
        difference() {
            circle(r = standRadiusPi);

            circle(r = pinRadiusPi);
        }

        translate([0,0,insertion])
        linear_extrude(height = height - insertion)
        difference() {
            circle(r = standRadiusPi);
            circle(r = m3holeRadius);
        }
    }
}

module piPost(height = 10, insertion = 5) {
    linear_extrude(height = height)
    difference() {
        circle(r = standRadiusPi);

        circle(r = pinRadiusPi);
    }

    translate([0,0,insertion])
    linear_extrude(height = height - insertion)
    difference() {
        circle(r = standRadiusPi);
    }

    translate([0,0,insertion])
    linear_extrude(height = height - insertionTolerance)
    difference() {
        circle(r = pinRadiusPi - insertionTolerance / 2);
    }
}

module piStub(height = 3, insertion = 5) {
    linear_extrude(height = height)
    circle(r = standRadiusPi);

    linear_extrude(height = height + insertion - insertionTolerance)
    difference() {
        circle(r = pinRadiusPi - insertionTolerance / 2);
    }
}

module arduinoPost(height = 10) {
    linear_extrude(height = height)
    difference() {
        circle(r = standRadiusPi);

        circle(r = m3holeRadius);
    }
}

module arduinoPosts(height) {
    arduinoHoles()
    arduinoPost(height);
}

module gripSide() {
    translate([0,0,standRadiusPi])
    rotate([90, 0, 0])
    linear_extrude(height=gripThickness, center=true) {
        difference() {
            hull() {
                translate([0, -standRadiusPi / 2])
                square([standRadiusPi*2, standRadiusPi], center=true);
                circle(r=standRadiusPi);
            }
            circle(r=pinRadiusPi);
        }
    }
}

module grip() {
    translate([0, -gripSpacing / 2 - gripThickness/2, 0])
    gripSide();
    translate([0, gripSpacing / 2 + gripThickness/2, 0])
    gripSide();
}

module sideMount() {
    translate([0,0,-2])
    linear_extrude(height=2)
    offset(r = outerRadiusPi) {
        square([xBetweenPi + outerRadiusPi * 2, 40 + outerRadiusPi * 2]);
    }
    translate([0,outerRadiusPi,0])
    grip();
    translate([0,40+outerRadiusPi,0])
    grip();

    translate([-outerRadiusPi,outerRadiusPi,0])
    %rotate([90, 0, 0])
    piBoard();

    %translate([-600*arduinoCoord,40+outerRadiusPi,2100*arduinoCoord])
    rotate([-90, 0, 0])
    arduinoBoard([1,3]);

}

*arduinoPosts();


module constructionPiPost(bottomSpace, midSpace) {
    translate([0,0,bottomSpace + insertionTolerance + boardThickness])
    piPostScrewTop(height = midSpace);
    piStub(bottomSpace);
}

module construction() {
    separatorHeight = 2;
    bottomSpace = 5;
    midSpace = 20;
    topSpace = 16;
    arduinoOffset = 0.5*outerRadiusPi;

    translate([0,0,-separatorHeight])
    linear_extrude(height = separatorHeight)
    difference() {
        offset(r = outerRadiusPi)
        square([boardXPi, yBetweenPi]);

        offset(r = -outerRadiusPi*1.5)
        square([xBetweenPi, yBetweenPi]);

        translate([xBetweenPi, 0])
        offset(r = -outerRadiusPi*1.5)
        square([boardXPi - xBetweenPi, yBetweenPi]);

        translate([outerRadiusPi/4, yBetweenPi/2-15]) circle(r=m3holeRadius + insertionTolerance);
        translate([outerRadiusPi/4, yBetweenPi/2+15]) circle(r=m3holeRadius + insertionTolerance);
        translate([xBetweenPi, yBetweenPi/2-15]) circle(r=m3holeRadius + insertionTolerance);
        translate([xBetweenPi, yBetweenPi/2+15]) circle(r=m3holeRadius + insertionTolerance);
        translate([boardXPi -outerRadiusPi/4, yBetweenPi/2-15]) circle(r=m3holeRadius + insertionTolerance);
        translate([boardXPi -outerRadiusPi/4, yBetweenPi/2+15]) circle(r=m3holeRadius + insertionTolerance);
    }
    piHoles() constructionPiPost(bottomSpace, midSpace);

    translate([0,0,midSpace+bottomSpace + boardThickness + insertionTolerance * 2])
    linear_extrude(height = separatorHeight)
    difference() {
        offset(r = outerRadiusPi)
        difference() {
            square([boardXArduino-arduinoOffset, yBetweenPi]);

            translate([outerRadiusPi / 2, outerRadiusPi])
            square([boardXArduino - arduinoOffset - outerRadiusPi*2, yBetweenPi - outerRadiusPi*2]);
        }


        piHoles() circle(r=m3holeRadius + insertionTolerance);
        translate([-arduinoOffset, -arduinoOffset])
        arduinoHoles() circle(r=m3holeRadius + insertionTolerance);
    }

    %translate([-outerRadiusPi,-outerRadiusPi,bottomSpace + boardThickness / 2])
    piBoard();

    translate([-arduinoOffset, -arduinoOffset, midSpace+bottomSpace + boardThickness + insertionTolerance * 3 + separatorHeight])
    arduinoPosts(topSpace);

    %translate([-arduinoOffset,-arduinoOffset,midSpace+bottomSpace+topSpace + boardThickness*1.5 + insertionTolerance * 4 + 2])
    arduinoBoard([1,3]);

}

batteryDimensions = [152, 3.86*25.4, 3.7*25.4];
batteryDimensionsHalf = batteryDimensions / 2;
batteryCaseThickness = 3;
module battery() {
    cube(batteryDimensions, center=true);
}

module batteryCaseSides() {
    *translate([-(batteryDimensions[0] + batteryCaseThickness)/2, 0, 0])
    rotate([0,90,0])
    linear_extrude(height = batteryCaseThickness, center=true)
    offset(batteryCaseThickness)
    children();

    translate([(batteryDimensions[0] + batteryCaseThickness)/2, 0, 0])
    rotate([0,90,0])
    linear_extrude(height = batteryCaseThickness, center=true)
    offset(batteryCaseThickness)
    children();
}

module batteryCaseFrontBack() {
    translate([0, -(batteryDimensions[1] + batteryCaseThickness)/2, 0])
    rotate([90,0,0])
    linear_extrude(height = batteryCaseThickness, center=true)
    offset(batteryCaseThickness)
    children();

    translate([0, (batteryDimensions[1] + batteryCaseThickness)/2, 0])
    rotate([90,0,0])
    linear_extrude(height = batteryCaseThickness, center=true)
    offset(batteryCaseThickness)
    children();
}

module triangles(x1, x2, y1, y2, offset=3) {
    midX = (x1+x2) / 2;
    midY = (y1+y2) / 2;
    offset(-offset)
    polygon([[midX,midY],[x1,y2],[x1,y1]]);
    offset(-offset)
    polygon([[midX,midY],[x2,y2],[x2,y1]]);

    offset(-offset)
    polygon([[midX,midY],[x2,y1],[x1,y1]]);
    offset(-offset)
    polygon([[midX,midY],[x2,y2],[x1,y2]]);
}

module batteryCase() {

    translate([0,0,-(batteryDimensions[2] + batteryCaseThickness)/2])
    linear_extrude(height = batteryCaseThickness, center=true)
    offset(batteryCaseThickness)
    difference() {
        square([batteryDimensions[0], batteryDimensions[1]], center=true);

        triangles(batteryDimensionsHalf[0], 0, batteryDimensionsHalf[1], -batteryDimensionsHalf[1]);
        triangles(0, -batteryDimensionsHalf[0], batteryDimensionsHalf[1], -batteryDimensionsHalf[1]);
    }

    batteryCaseSides()
    difference() {
        square([batteryDimensions[2], batteryDimensions[1]], center=true);

        triangles(batteryDimensionsHalf[2], -batteryDimensionsHalf[2], 0, -batteryDimensionsHalf[1]);
        triangles(batteryDimensionsHalf[2], -batteryDimensionsHalf[2], batteryDimensionsHalf[1], 0);

    }

    batteryCaseFrontBack()
    difference() {
        square([batteryDimensions[0], batteryDimensions[2]], center=true);

        triangles(batteryDimensionsHalf[0], 0, batteryDimensionsHalf[2], -batteryDimensionsHalf[2]);
        triangles(0, -batteryDimensionsHalf[0], batteryDimensionsHalf[2], -batteryDimensionsHalf[2]);
    }

    //cube(batteryDimensions, center=true);
    for (i = [[-1,-1],[-1,1],[1,-1],[1,1]])
        translate([(batteryDimensions[0] + batteryCaseThickness)/2 * i[0], (batteryDimensions[1] + batteryCaseThickness)/2 * i[1], 0])
        cube([batteryCaseThickness,batteryCaseThickness,batteryDimensions[2]+batteryCaseThickness*2], center=true);
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

outwardMotor = 23.9 - m3holeRadius + 11 + 23;
motorSizeDown = 60 - 23;
motorSizeUp = 60 + 23;
motorPlateSize = [40,42.5];

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

centerPlatformOuterRadius = 70 - armThickness;

module motorBracket() {

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
                            circle(r=centerPlatformOuterRadius+armThickness);

                            circle(r=centerPlatformOuterRadius-armThickness);

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
                        translate([0, centerPlatformOuterRadius])
                        square([7,3], center=true);
                        translate([0, -centerPlatformOuterRadius])
                        square([7,3], center=true);

                        // m3Holes in center platform
                        translate([centerPlatformOuterRadius, 0])
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

*construction();


%translate([0,0,-3.7*25.4 / 2 + motorSizeUp - motorBracketThickness - batteryVerticalBuffer])
battery();

translate([0,0,-3.7*25.4 / 2 + motorSizeUp - motorBracketThickness - batteryVerticalBuffer])
*batteryCase();

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

bottomRiding = sin(acos((motorRadius) / bb8Radius)) * bb8Radius;
*translate([0,0,bottomRiding])
%difference() {
    *sphere(r=bb8OuterRadius, $fn=120);
    sphere(r=bb8Radius, $fn=120);

    *cylinder(center=true, r=bb8OuterRadius * sin(35), h=bb8OuterRadius*2);
    *rotate([90,0,0]) cylinder(center=true, r=bb8OuterRadius * sin(35), h=bb8OuterRadius*2);
    *rotate([0,90,0]) cylinder(center=true, r=bb8OuterRadius * sin(35), h=bb8OuterRadius*2);
}

*translate([40,0,0])
cube([9*25.4,6*25.4,6*25.4], center=true);