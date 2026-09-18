wheelRadius = 3*millisPerInch;
wheelThickness = 23;

platformExtra = 0.6; // enough for 3 layers of filament

// A metal dowel runs through the middle of the 3d-printed frames for added
// strength/weight distribution since multiple prints are pieced together due to
// the size.
robotFrameDowelDiameter = 0.5*millisPerInch; // 1/2" metal dowel

robotFrameDowelLength = radius;

robotPlatformThickness = robotFrameDowelDiameter + platformExtra * 2;
