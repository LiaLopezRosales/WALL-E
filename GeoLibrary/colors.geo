// colors.geo — Sistema de colores completo

color red;
draw point(100, 100) "red";

color blue;
draw point(200, 100) "blue";

color green;
draw point(300, 100) "green";

rgb(255, 128, 0);
draw point(400, 100) "orange";

hsl(280, 80, 50);
draw point(500, 100) "purple";

color red;
lighten(30);
draw point(100, 200) "light red";

color blue;
darken(30);
draw point(200, 200) "dark blue";

color red;
mix(blue, 0.5);
draw point(300, 200) "purple mix";

color green;
complement();
draw point(400, 200) "complement";

color cyan;
fill;
draw circle(point(500, 200), 40) "filled";

color yellow;
fill;
draw polygon(point(100, 350), 50, 6) "hexagon";

color magenta;
fill linear(red, blue);
draw circle(point(300, 350), 60) "linear grad";

color cyan;
fill radial(green, yellow);
draw circle(point(500, 350), 60) "radial grad";
