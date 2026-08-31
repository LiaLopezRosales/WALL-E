// 03-sequences.geo — Secuencias y generadores
// Muestra: literales { ... }, secuencias infinitas {a...b}, count,
// concatenación con +, bucles for y los generadores randoms()/samples()/points().

seed(2024);

// 1. Secuencia finita por extensión y su conteo
print(count({1, 2, 3, 4, 5}));          // -> 5

// 2. Secuencia infinita acotada por el contador (MaxElements)
print(count({1...40}));                 // -> 40

// 3. Concatenación de dos secuencias finitas
print(count({1, 2} + {3, 4}));          // -> 4

// 4. Iterar una secuencia finita y dibujar con ella
color steelblue;
grosor(2);
for n in {1, 2, 3, 4, 5} {
    draw point(100 + n * 70, 140);
}

// 5. Generadores randoms()/samples() sobre el contorno de un círculo
randoms();
color darkslategray;
print(count(points(circle(point(300, 300), 190))));

samples();
color seagreen;
print(count(points(circle(point(300, 300), 260))));
