hexTolerance = 0.1;
motorShaftTolerance = 0.15;


function circleRadius(r, $fn) = r / cos(360 / 2 / $fn);

difference() {
    translate([0,0, -6])
    cylinder(r=circleRadius(r=6, $fn=6) - hexTolerance, h=18, $fn=6);
    
    translate([0,0,-7])
    cylinder(r=3 / 2, h=8, $fn=120);
    
    intersection() {
        translate([0,0,0])
        cylinder(r=circleRadius(r=3, $fn=120) + motorShaftTolerance, h=13, $fn=120);
        
        *translate([-3 - motorShaftTolerance,-3 - motorShaftTolerance,0])
        cube([6 + motorShaftTolerance * 2,5.6 + motorShaftTolerance,13]);
    }
    
    
    translate([0,0,5])
    rotate([-90,0,0])
    cylinder(r=3 / 2, h=15, $fn=120);
    
}
