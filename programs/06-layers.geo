// 06-layers.geo — Capas, ocultar/mostrar y snap (M9)
// Muestra: layer N (z-order), hide/show de etiquetas y snap configurable.

color steelblue;
fill;
draw circle(point(200, 200), 110) "principal";

// Capas: los objetos siguientes se dibujan en un z-order superior
layer 1;
color crimson;
fill;
draw polygon(point(200, 200), 60, 6) "hexagono";

layer 2;
color gold;
draw circle(point(200, 200), 70) "anillo";

// hide/show manipulan el panel de etiquetas
draw point(360, 260) "puntoA";
hide puntoA;

color orange;
draw point(120, 330) "puntoB";
show puntoB;

// snap: redondea las coordenadas a la rejilla
color purple;
snap 0.5;
draw point(127.3, 253.7) "snap";
