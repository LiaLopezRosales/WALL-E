// 02-colors.geo — Sistema de color completo (M8)
// Muestra: nombres CSS, hex, rgb/rgba, hsl, lighten/darken/mix/complement,
// fill solid, gradientes linear/radial.

color red;
draw point(100, 100) "red";

// Formato hexadecimal (short y largo)
color #f0f;
draw point(200, 100) "#f0f";
color #ff8800;
draw point(300, 100) "#ff8800";

// RGB y RGBA
rgb(120, 60, 220);
draw point(400, 100) "rgb";
rgba(20, 200, 120, 160);
draw point(500, 100) "rgba";

// HSL
hsl(200, 80, 50);
draw point(100, 190) "hsl";

// Operaciones cromáticas
color blue;
lighten(30);
draw point(200, 190) "lighten";
color blue;
darken(30);
draw point(300, 190) "darken";
color red;
mix(yellow, 0.5);
draw point(400, 190) "mix";
color steelblue;
complement();
draw point(500, 190) "complement";

// Relleno sólido
color tomato;
fill;
draw circle(point(200, 310), 60) "solid";

// Gradiente lineal
color magenta;
fill linear(red, blue);
draw circle(point(360, 310), 60) "linear";

// Gradiente radial
color cyan;
fill radial(green, yellow);
draw circle(point(520, 310), 60) "radial";
