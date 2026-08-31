// 08-animate.geo — Animación paramétrica (M12a)
// Muestra: animate(t from A to B) { ... } con 60 frames precomputados.
// `t` varía linealmente de A a B en cada frame; cada frame es una escena aislada.

color tomato;
grosor(2);

animate(t from 0 to 1) {
    // Punto recorriendo una parábola
    draw point(120 + t * 500, 480 - t * t * 380);

    // Círculo cuyo radio crece con t
    draw circle(point(320, 200), 20 + t * 110);
}

animate(t from 0 to 1) {
    // Dos puntos orbitando
    color steelblue;
    draw point(640 + 140 * cos(t * 6.28), 180 + 140 * sin(t * 6.28));
    color orange;
    draw point(640 + 140 * cos(t * 6.28 + 3.14), 180 + 140 * sin(t * 6.28 + 3.14));
}
