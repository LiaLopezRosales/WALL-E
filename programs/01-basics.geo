// 01-basics.geo — Figuras básicas y etiquetas
// Muestra: point, segment, line, ray, circle, arc y labels.

color steelblue;
A = point(120, 260);
B = point(320, 110);
C = point(520, 260);

// Punto, segmento, semirrecta y recta
draw point(60, 380) "P";
draw segment(B, C) ;
draw ray(A, C) ;
draw line(A, B) ;

// Círculos con centro y radio
color orangered;
c1 = circle(A, 90);
c2 = circle(C, 55);
draw c1;
draw c2;

// Arco definido por tres puntos y un ángulo
color seagreen;
draw arc(point(120, 470), point(260, 470), point(200, 560), 90) "arco";

// Etiquetas de los vértices
color white;
label(A, "A", 15);
label(B, "B", 15);
label(C, "C", 15);
