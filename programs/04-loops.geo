// 04-loops.geo — Bucles repeat, for y anidados (M7)
// Muestra: repeat(n){...}, for x in seq {...}, bucles anidados y print.

color dimgray;

// repeat: cuerpo ejecutado n veces
repeat(4) {
    draw point(100, 100);
}

// repeat con expresión de conteo
repeat(4 + 4) {
    draw point(300, 60);
}

// for: itera sobre una secuencia finita
for i in {1, 2, 3, 4, 5} {
    color red;
    draw point(80 + i * 90, 220);
}
print("fin del for");

// for con cuenta derivada de otra secuencia
for k in {10, 20, 30} {
    print(k);
}

// Bucles anidados: cuadrícula de puntos
color teal;
for r in {0, 1, 2, 3} {
    for c in {0, 1, 2, 3} {
        draw point(420 + c * 50, 420 + r * 50);
    }
}
