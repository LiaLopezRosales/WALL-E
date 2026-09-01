// spiral.geo — Espiral de puntos con secuencias y funciones matemáticas

seed(42);

f(i) = i * 3;

repeat(120) {
    double a = f(PI);
    double r = 2 + count({1});
    draw point(300 + r * cos(a), 300 + r * sin(a));
}

draw point(300, 300) "center";
