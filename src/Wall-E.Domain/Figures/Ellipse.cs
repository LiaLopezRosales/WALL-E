using System;
using System.Collections.Generic;

namespace Wall_E.Domain;

public class Ellipse : Figure, IEquatable<Ellipse>
{
    public Point Center { get; }
    public double Rx { get; }
    public double Ry { get; }

    public Ellipse(Point center, double rx, double ry)
    {
        Center = center;
        Rx = Math.Abs(rx);
        Ry = Math.Abs(ry);
    }

    public override bool ContainPoint(Point p)
    {
        double dx = p.x - Center.x;
        double dy = p.y - Center.y;
        if (Rx < 1e-9 || Ry < 1e-9) return false;
        return (dx * dx) / (Rx * Rx) + (dy * dy) / (Ry * Ry) <= 1.0;
    }

    public override GenericSequence<Point> FigurePoints()
    {
        const int Steps = 64;
        var pts = new List<Point>(Steps);
        for (int i = 0; i < Steps; i++)
        {
            double t = 2 * Math.PI * i / Steps;
            pts.Add(new Point(
                Center.x + Rx * Math.Cos(t),
                Center.y + Ry * Math.Sin(t)));
        }
        var seq = new Finite_Sequence<Point>(pts);
        seq.type = Finite_Sequence<Point>.SeqType.point;
        return seq;
    }

    public override Finite_Sequence<Point> Intersect(Figure fig)
    {
        var result = new List<Point>();
        const int Steps = 64;
        for (int i = 0; i < Steps; i++)
        {
            double t1 = 2 * Math.PI * i / Steps;
            double t2 = 2 * Math.PI * (i + 1) / Steps;
            var a = new Point(Center.x + Rx * Math.Cos(t1), Center.y + Ry * Math.Sin(t1));
            var b = new Point(Center.x + Rx * Math.Cos(t2), Center.y + Ry * Math.Sin(t2));
            var seg = new Segment(a, b);
            var inter = seg.Intersect(fig);
            foreach (var pt in inter.Sequence!)
                result.Add(pt);
        }
        var seq = new Finite_Sequence<Point>(result);
        seq.type = Finite_Sequence<Point>.SeqType.point;
        return seq;
    }

    public bool Equals(Ellipse? other)
    {
        if (other is null) return false;
        return Center.Equals(other.Center) && Rx == other.Rx && Ry == other.Ry;
    }

    public override string ToString() => $"Ellipse center={Center} rx={Rx} ry={Ry}";
}
