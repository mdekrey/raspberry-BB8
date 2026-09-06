include <my-bb8-config.scad>;

blockSize = 20;
wallThickness=8;

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
        %camLockSlot();
        biscuitSlot();
    }
}


%translate([-50,0,0])
    {
        biscuitSlot();
    }