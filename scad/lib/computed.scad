panelDegrees = 35; // https://rimstar.org/science_electronics_projects/bb-8_dimensions.htm
panelRadius = radius * sin(panelDegrees);
insertionTolerance = 0.3;

visibleBoltHole = 4 / 2;
visibleBoltOuterRadius = radius * 0.07734 / 2;
visibleBoltCoverRadius = visibleBoltOuterRadius / 2;
visibleBoltBezelDepth = visibleBoltOuterRadius * 0.2;
visibleBoltBezel = visibleBoltOuterRadius * 0.2;

panelRingDegrees = panelDegrees * 4/15;
panelRingInnerDegrees = panelDegrees - panelRingDegrees;
panelRingInnerRadius = radius * sin(panelRingInnerDegrees);
panelRingOuterOverlap = 3;
panelRingInnerOverlap = 1;
panelArmDegrees = panelDegrees * 0.6;
panelArmBoltDegrees = panelDegrees * 0.52;

if (camlockBoltRadius * 2 + 2 > camlockNutThickness)
    warn("Camlock Bolt/Nut sizes invalid");

panelLockBoltDegrees = 90/ringLocksPerQuadrant;
panelRadiusOffset = radius * cos(panelDegrees);
rotateLockDegrees = panelLockBoltDegrees * 0.6; // The number of degrees to rotate the lock
panelRotateLockOffset = (panelLockBoltDegrees - rotateLockDegrees) / 2; // The number of degrees offset from the start position for the initial lock

panelRingInnerActualDegrees = asin((panelRingInnerRadius - insertionTolerance) / radius);
panelRingInnerInternalDegrees = asin((panelRingInnerRadius + panelRingInnerOverlap - insertionTolerance)
    / (radius - wallThickness));
panelHeight = radius - cos(panelRingInnerInternalDegrees) * (radius - wallThickness);
ringThickness = cos(panelRingInnerActualDegrees) * radius - cos(panelRingInnerInternalDegrees) * (radius - wallThickness) - 0.8;

panelDesignDepth = radius - cos(asin((panelRingInnerRadius * 0.92) / radius)) * radius + insertionTolerance;
panelDesignRadius = wallThickness - 0.2 * millisPerInch;

headRadius = radius * 295/506;
headWallThickness = 1.6*3;
headBaseHeight = 20 * headRadius / 147.5;
headConeHeight = 20 * headRadius / 147.5;
headConeRadius = 111.5 * headRadius / 147.5;
headOffset = cos(asin(headConeRadius / radius)) * radius;
headInnerRadius = headRadius - headWallThickness;
headInnerHeight = headRadius - headWallThickness;
headInsetMinThickness = 1.6;
headCutHeight = headWallThickness / 2;
etchLineThickness = 1;

panelAdditionalWallThickness = 0;
panelInnerWall = radius - wallThickness - panelAdditionalWallThickness;
panelLayerWall = radius - (radius - panelInnerWall) * 0.6;
