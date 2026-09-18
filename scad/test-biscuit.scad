include <my-bb8-config.scad>;
include <components/biscuit/config.scad>;
include <components/biscuit/biscuit.scad>;
include <components/biscuit/slot.scad>;

blockSize = 20;
wallThickness=5;

translate([0,0,biscuitThickness / 2])
biscuit();

rotate([90,0,0])
for(offset=[0,1])
translate([blockSize,0,offset * wallThickness * 1.2])
difference()
{
    translate([-blockSize / 2,0,-wallThickness])
    cube([
        blockSize,
        blockSize,
        wallThickness
    ]);

    {
        %camLockSlot(boltLength=camlockBoltLength);
        biscuitSlot();
    }
}


%translate([0,0,0])
    {
        biscuitSlot();
    }