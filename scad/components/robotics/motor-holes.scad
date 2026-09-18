
module motorHoles() {
    holeOffsetX = (3.6 + m3holeRadius - motorPlateSize[0]/2);
    holeOffsetY = (7.75 + m3holeRadius - motorPlateSize[1]/2);

    for (x = [-1, 1])
    for (y = [-1, 1])
    {
        translate([x * holeOffsetX, y * holeOffsetY])
        children();
    }
}
