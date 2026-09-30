
module biscuitSlot() {
    translate([0,0,-biscuitSlotDepth])
    cube([
        biscuitWidth + biscuitRadius + insertionTolerance * 2,
        biscuitLength * 1.05 + biscuitRadius + insertionTolerance * 2,
        biscuitThickness + insertionTolerance
    ], center = true);
}
