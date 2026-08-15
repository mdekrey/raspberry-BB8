include <my-bb8-config.scad>;

echo("print dimensions: ");
echo(x = headRadius * 2, y = headRadius * 2, z = headRadius + headBaseHeight/2);

translate([0,0, -headOffset])
headDetail();
