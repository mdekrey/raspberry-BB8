include <my-bb8-config.scad>;

biscuit();

blockSize = 20;
wallThickness=8;
echo(wallThickness);

translate([50,0,0])
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