// samples.geo — Showcase de funcionalidades del lenguaje

seed(123);

// Polígono y elipse
color green;
fill;
draw polygon(point(300, 300), 120, 6) "hexagono";
draw ellipse(point(300, 300), 180, 100);

// Triángulo con colores
color red;
grosor(2);
draw polygon(point(300, 100), 80, 3) "triangulo";

// Círculos concéntricos
color blue;
grosor(1);
draw circle(point(300, 300), 30);
draw circle(point(300, 300), 60);
draw circle(point(300, 300), 90);
draw circle(point(300, 300), 120);
draw circle(point(300, 300), 150);

// Labels
color white;
label(point(300, 300), "GeoWall-E", 24);
