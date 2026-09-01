// basic.geo — Figuras básicas del DSL GeoWall-E

A = point(100, 300);
B = point(300, 100);
C = point(500, 300);

l1 = line(A, B);
l2 = line(B, C);
l3 = line(A, C);

c1 = circle(point(300, 300), 150);
c2 = circle(A, 50);
c3 = circle(C, 50);

draw l1;
draw l2;
draw l3;
draw c1;
draw c2 "A";
draw c3 "C";
draw A "A";
draw B "B";
draw C "C";
