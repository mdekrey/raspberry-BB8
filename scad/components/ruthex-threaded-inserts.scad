ruthexM3ThreadedInsertRadius = 3.9 / 2;
ruthexM3ThreadedInsertDepthMin = 8; // TODO: this isn't right

module ruthexM3InsertHole(depth = 0) {
    actualDepth = max(depth, ruthexM3ThreadedInsertDepthMin);
    cylinder(h = actualDepth, r = ruthexM3ThreadedInsertRadius);
}

ruthexM4ThreadedInsertRadius = 5.5 / 2;
ruthexM4ThreadedInsertDepthMin = 9.1;

module ruthexM4InsertHole(depth = 0) {
    actualDepth = max(depth, ruthexM4ThreadedInsertDepthMin);
    cylinder(h = actualDepth, r = ruthexM4ThreadedInsertRadius);
}
