// grid.geo — Cuadrícula de puntos con capas

color lightgray;
draw point(100, 100);
draw point(200, 100);
draw point(300, 100);
draw point(400, 100);
draw point(500, 100);
draw point(100, 200);
draw point(200, 200);
draw point(300, 200);
draw point(400, 200);
draw point(500, 200);
draw point(100, 300);
draw point(200, 300);
draw point(300, 300);
draw point(400, 300);
draw point(500, 300);

layer 1;
color red;
draw point(300, 200) "center";

layer 2;
color blue;
draw circle(point(300, 200), 100);

layer 3;
color green;
draw segment(point(200, 200), point(400, 200));
