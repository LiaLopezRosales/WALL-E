// 05-figures.geo — Polígonos, elipses, estilos de línea y relleno (M7)
// Muestra: polygon, ellipse, grosor, dashed/dotted/dashdot/solid y fill.

// Polígonos regulares
color green;
fill;
draw polygon(point(140, 140), 90, 6) "hexagono";

color crimson;
grosor(3);
draw polygon(point(380, 140), 80, 3) "triangulo";

color royalblue;
grosor(1);
draw polygon(point(600, 140), 70, 8) "octogono";

// Elipses con ejes horizontal y vertical
color darkorange;
fill;
draw ellipse(point(170, 320), 130, 60);

// Estilos de línea
color darkslateblue;
grosor(2);
draw segment(point(380, 300), point(580, 300));
dashed;
draw segment(point(380, 340), point(580, 340));
dotted;
draw segment(point(380, 380), point(580, 380));
dashdot;
draw segment(point(380, 420), point(580, 420));
solid;
draw segment(point(380, 460), point(580, 460));
