
module motorPosition() {
    translate([radius-wallThickness,0,0])
    rotate([180,0,0])
    translate(-wheelEdgeCenter)
    children();
}

module dowelPosition() {
    translate(robotFrameDowelOffset)
    rotate([0,90,0])
    children();
}

module batteryPosition() {
    // TODO: this translation amount should becalculated as a variable
    translate([0,0,-50])
    children();
}