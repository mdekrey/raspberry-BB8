
module biscuit() {
    hull() {
        for(x = [-1,1])
        for(y = [-1,1])
            translate([
                x * (biscuitWidth - biscuitRadius)/2,
                y * (biscuitLength - biscuitRadius) / 2,
                0
            ])
            cylinder(biscuitThickness, r=biscuitRadius, center=true);
    }
}
