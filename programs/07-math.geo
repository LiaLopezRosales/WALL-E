// 07-math.geo — Matemáticas y determinismo (M6)
// Muestra: constantes (PI, E, phi, sqrt2), funciones (sqrt, abs, floor, ceil,
// tan, atan, log, sin, cos) y seed/print para depuración determinista.

seed(7);

print(PI);
print(E);
print(phi);
print(sqrt2);

print(sqrt(144));
print(abs(-5));
print(floor(3.9));
print(ceil(3.1));

// Ángulo recto en radianes: tan(PI/4) == 1 y atan(1) == PI/4
print(tan(PI / 4));
print(atan(1));

color darkgreen;
grosor(1);
draw point(150 + 100 * cos(PI / 4), 240 - 100 * sin(PI / 4)) "cos+sin";
draw point(150 + cos(0), 240 + cos(0)) "cos(0)";
