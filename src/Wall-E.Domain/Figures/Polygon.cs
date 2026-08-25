using System;
using System.Collections.Generic;

namespace Wall_E.Domain;

public class Polygon : Figure, IEquatable<Polygon>
{
    public Point Center { get; }
    public double Radius { get; }
    public int Sides { get; }

    public Polygon(Point center, double radius, int sides)
    {
        Center = center;
        Radius = radius;
        Sides = Math.Max(sides, 3);
    }

    public List<Point> Vertices()
    {
        var pts = new List<Point>(Sides);
        for (int i = 0; i < Sides; i++)
        {
            double angle = 2 * Math.PI * i / Sides - Math.PI / 2;
            pts.Add(new Point(
                Center.x + Radius * Math.Cos(angle),
                Center.y + Radius * Math.Sin(angle)));
        }
        return pts;
    }

    public override bool ContainPoint(Point p)
    {
        var verts = Vertices();
        for (int i = 0; i < verts.Count; i++)
        {
            var a = verts[i];
            var b = verts[(i + 1) % verts.Count];
            double cross = (b.x - a.x) * (p.y - a.y) - (b.y - a.y) * (p.x - a.x);
            if (cross < 0) return false;
        }
        return true;
    }

    public override GenericSequence<Point> FigurePoints()
    {
        var seq = new Finite_Sequence<Point>(Vertices());
        seq.type = Finite_Sequence<Point>.SeqType.point;
        return seq;
    }

    public override Finite_Sequence<Point> Intersect(Figure fig)
    {
        var result = new List<Point>();
        var verts = Vertices();
        for (int i = 0; i < verts.Count; i++)
        {
            var seg = new Segment(verts[i], verts[(i + 1) % verts.Count]);
            var inter = seg.Intersect(fig);
            foreach (var pt in inter.Sequence!)
                result.Add(pt);
        }
        var seq = new Finite_Sequence<Point>(result);
        seq.type = Finite_Sequence<Point>.SeqType.point;
        return seq;
    }

    public bool Equals(Polygon? other)
    {
        if (other is null) return false;
        return Center.Equals(other.Center) && Radius == other.Radius && Sides == other.Sides;
    }

    public override string ToString() => $"Polygon center={Center} r={Radius} n={Sides}";
}
