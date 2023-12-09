public static class RandomExtensions
    {
        public static double NextDouble(this Random random,double min,double max)
        {
            return min +(random.NextDouble()*(max-min));
        }
    }
    

    // public class Intercept
    // {
    //     public static Point PointCircle(Point p,Circle c)
    //     {
    //         double distance=Math.Sqrt(Math.Pow(p.x-c.center.x,2)+Math.Pow(p.y-c.center.y,2));
    //         if (distance==c.radio)
    //         {
    //             return p;
    //         }
    //         else return null!;
    //     }

    //     public static Point PointLine(Point p,Line l)
    //     {
    //         double m=(l.generalpoint2.y-l.generalpoint1.y)/(l.generalpoint2.x-l.generalpoint1.x);
    //         double b=l.generalpoint1.y-m*l.generalpoint1.x;
    //         if ((p.y-m*p.x)-b==0)
    //         {
    //             return p;
    //         }
    //         else return null!;
    //     }

    //     public static Point PointSegment(Point p,Segment l)
    //     {
    //         double m=(l.EndsIn.y-l.StartIn.y)/(l.EndsIn.x-l.StartIn.x);
    //         double b=l.StartIn.y-m*l.StartIn.x;
    //         if ((p.y-m*p.x)-b==0)
    //         {
    //             double minX=Math.Min(l.StartIn.x,l.EndsIn.x);
    //             double maxX=Math.Max(l.StartIn.x,l.EndsIn.x);
    //             double minY=Math.Min(l.StartIn.y,l.EndsIn.y);
    //             double maxY=Math.Min(l.StartIn.y,l.EndsIn.y);
    //             if (p.x>=minX&&p.x<=maxX&&p.y>=minY&&p.y<=maxY)
    //             {
    //                 return p;
    //             }
    //         }
    //         return null!;
    //     }
        
    //      public static Point PointRay(Point p,Ray l)
    //     {
    //         double m=(l.PassFor.y-l.StartIn.y)/(l.PassFor.x-l.StartIn.x);
    //         double b=l.StartIn.y-m*l.StartIn.x;
    //         if ((p.y-m*p.x)-b==0)
    //         {
    //             if (l.StartIn.x<=l.PassFor.x)
    //             {
    //                 if (l.StartIn.y<=l.PassFor.y)
    //                 {
    //                     if (p.x>=l.StartIn.x&&p.y>=l.PassFor.y)
    //                     {
    //                         return p;
    //                     }
    //                 }
    //                 else
    //                 {
    //                     if (p.x>=l.StartIn.x&&p.y<=l.PassFor.y)
    //                     {
    //                         return p;
    //                     }
    //                 }
    //             }
    //             else
    //             {
    //                 if (l.StartIn.y<=l.PassFor.y)
    //                 {
    //                     if (p.x<=l.StartIn.x&&p.y>=l.PassFor.y)
    //                     {
    //                         return p;
    //                     }
    //                 }
    //                 else
    //                 {
    //                     if (p.x<=l.StartIn.x&&p.y<=l.PassFor.y)
    //                     {
    //                         return p;
    //                     }
    //                 }
    //             }
    //         }
    //         return null!;
    //     }  

    //     //Check this
    //     public static Point PointArc(Point p,Arc a)
    //     {
    //         double angle=(Math.Atan2(p.y - a.point_of_semirect1.y,p.x - a.point_of_semirect1.x)-Math.Atan2(a.point_of_semirect2.y-a.point_of_semirect1.y,a.point_of_semirect2.x-a.point_of_semirect1.x))*180/Math.PI;
    //         if (angle==a.measure)
    //         {
    //             return p;
    //         }
    //         else
    //         {
    //             return null!;
    //         }
    //     }

    //}