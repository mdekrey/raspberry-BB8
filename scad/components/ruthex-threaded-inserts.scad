ruthexM3ThreadedInsertRadius = 3.9 / 2;

ruthexM4ThreadedInsertRadius = 5.5 / 2;
ruthexM4ThreadedInsertDepthMin = 9.1;

module ruthexM4InsertHole(depth = 0) {
    actualDepth = max(depth, ruthexM4ThreadedInsertDepthMin);
    cylinder(h = actualDepth, r = ruthexM4ThreadedInsertRadius);
}
