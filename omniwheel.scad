radius = 120/2;
axleDiameter = 5;
axleLength = 25;
axleWheelBuffer = 0.1;
axleDiscBuffer = 0.3;
wheelCount = 5;
spokeWidth = 7;
spokeBuffer = 2;
tipBuffer = 10;
wheelBuffer = 3;
ribSize = 0.6;
ribSpacing = 3;
discThickness = 8;
boltRadius = 3.5/2;
boltHeadRadius = 5.5 / 2;
hexTolerance = 0.1;

// Corresponds to Pololu Universal Aluminum Mounting Hub
// hubRadius = 9.5;

*preview();
*wheel();
wheelHalf();
*rotate([180,0,0]) disc(true);
*rotate([180,0,0]) disc(false);

function circleRadius(r, $fn) = r / cos(360 / 2 / $fn);

wheelRotation = 360 / wheelCount;
shift = radius * cos(wheelRotation / 2);

wheelHeight = 2 * radius * sin(wheelRotation / 2) - tipBuffer;

wheelMaxRadius = (radius - shift);

module preview() {
    for (i = [0:wheelCount]) {
        rotate([0,wheelRotation * i, 0])
        translate([shift,0,0])
        wheel();
    }
    
    rotate([90,0,0])
    disc(true);
    
    rotate([-90,0,0])
    disc(false);
}

module wheel() {
    spokeOffset = (spokeWidth + spokeBuffer) / 2;
    
    translate([0, 0, spokeOffset])
    wheelHalf();
    
    rotate([180, 0, 0])
    translate([0, 0, spokeOffset])
    wheelHalf();
    
    axle();
}

module wheelHalf() {
    spokeOffset = (spokeWidth + spokeBuffer) / 2;
    
    rotate_extrude(convexity=10,$fn=60)
    {
        intersection()
        {
            union() {
                for(y=[ceil(spokeOffset / ribSize / ribSpacing)*ribSize*ribSpacing-ribSize/3*ribSpacing:ribSize*ribSpacing:wheelHeight / 2]) {
                    for (m=[-1:2:2]) { // positive and negative
                        translate([cos(asin(y / radius)) * radius - shift - 0.1, y*m-spokeOffset])
                        rotate(asin(y / radius) + 45)
                        square([ribSize, ribSize], center=true);
                    }
                }
                intersection()
                {
                    translate([-shift,-spokeOffset]) circle(r=radius,$fn=60);
                    translate([shift,-spokeOffset]) circle(r=radius,$fn=60);
                }
            }
        
            union() {
                
                translate([0,(axleLength) / 2 - spokeOffset])
                    square([wheelMaxRadius,wheelHeight / 2 - axleLength / 2]);
                translate([axleDiameter / 2 + axleWheelBuffer,0])
                    square([wheelMaxRadius,wheelHeight / 2 - spokeOffset]);
            }
        }
    }
}

module axle() {
    actualAxleLength = max(axleLength, (spokeWidth + spokeBuffer) / 2 + 1);
    
    cylinder(r=axleDiameter / 2, h=actualAxleLength, center=true, $fn=60);
}

module disc(hexHole) {
    difference() {
        linear_extrude(height = discThickness / 2) {
            difference() {
                circle(r=radius - wheelMaxRadius);
                
                for (i = [0:wheelCount]) {
                    rotate(wheelRotation * i)
                    translate([shift + radius - wheelMaxRadius - wheelBuffer, 0])
                    circle(r = radius);
                }
                
                if (hexHole) circle(r=circleRadius(r=6, $fn=6) + hexTolerance, $fn=6);
                else circle(r=boltRadius, $fn=60);
            
                *for (i = [0:6]) {
                    rotate(30+60*i)
                    translate([hubRadius, 0])
                    circle(r=boltRadius, $fn=60);
                }
            }
            
            for (i = [0:wheelCount]) {
                difference() {
                    rotate(wheelRotation * i)
                    translate([radius/2, -spokeWidth / 2])
                    square([radius/2 - wheelMaxRadius + spokeWidth / 2 + axleDiameter / 2, spokeWidth]);
                    
                    
                    rotate(wheelRotation * i)
                    translate([radius - wheelMaxRadius * 2 - max(0, boltHeadRadius - spokeWidth / 2), 0])
                    circle(r=boltRadius, $fn=60);
                }
            }
            
        }
        
        for (i = [0:wheelCount]) {            
            rotate([90, 0, wheelRotation * i])
            translate([radius - wheelMaxRadius, 0, 0])
            cylinder(r=axleDiameter / 2 + axleDiscBuffer, h=spokeWidth + 1, center=true, $fn=60);
        }
        
    }
}
