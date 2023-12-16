public class Arc:Figure,IEquatable<Arc>
{
    public Point center{get;set;}
    public Point point_of_semirect1{get;set;}
    public Point point_of_semirect2{get;set;}

    public double measure{get;set;}
    public double MainAngle{get;}
    public double SweepAngle{get;}
    public Point CircleRay1_Intersection{get;}
    public Point CircleRay2Intersection{get;}

    public Arc(Point c,Point p1,Point p2,double m)
    {
        center=c;
        point_of_semirect1=p1;
        point_of_semirect2=p2;
        measure=m;
        (CircleRay1_Intersection,CircleRay2Intersection)=GetCircleRayIntersection();
        (MainAngle,SweepAngle)=GetAngles();

    }

    public bool Equals(Arc? arc)
    {
        if (center.Equals(arc!.center)&&point_of_semirect1.Equals(arc.point_of_semirect1)&&point_of_semirect2.Equals(arc.point_of_semirect2)&&measure==arc.measure)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool ContainPoint(Point p)
    {
       double angle=(Math.Atan2(p.y - this.point_of_semirect1.y,p.x - this.point_of_semirect1.x)-Math.Atan2(this.point_of_semirect2.y-this.point_of_semirect1.y,this.point_of_semirect2.x-this.point_of_semirect1.x))*180/Math.PI;
            if (angle==this.measure)
            {
                return true;
            }
            else
            {
                return false;
            }
    }
    private (Point,Point) GetCircleRayIntersection()
    {
        Ray r1=new Ray(center,point_of_semirect1);
        Ray r2=new Ray(center,point_of_semirect2);
        Point p_left=GeometricTools.CircleRay_Intersection(center,r1.CreateRelativeEnd(),measure,GeometricTools.Pendient(r1.StartIn,r1.PassFor),GeometricTools.N_of_Equation(r1.StartIn,r1.PassFor));
        Point p_right=GeometricTools.CircleRay_Intersection(center,r2.CreateRelativeEnd(),measure,GeometricTools.Pendient(r2.StartIn,r2.PassFor),GeometricTools.N_of_Equation(r2.StartIn,r2.PassFor));
        return (p_left,p_right);
    }
    private (double,double) GetAngles()
    {
        Ray r1=new Ray(center,point_of_semirect1);
        Ray r2=new Ray(center,point_of_semirect2);
        Point origin=new Point(center.x+measure,center.y);
        double sweepangle=GeometricTools.VectorsAngle(center,CircleRay1_Intersection,CircleRay2Intersection);
        double LeftAngle=GeometricTools.VectorsAngle(center,CircleRay1_Intersection,origin);
        double RightAngle=GeometricTools.VectorsAngle(center,CircleRay2Intersection,origin);
        double startangle=0;
        int leftquadrant=GeometricTools.Quadrant(center,measure,CircleRay1_Intersection);
        int rightquadrant=GeometricTools.Quadrant(center,measure,CircleRay2Intersection);
        if (leftquadrant==rightquadrant)
        {
            if (leftquadrant<3)
            {
                startangle=Math.Min(LeftAngle,RightAngle);
            }
            else startangle=-Math.Max(LeftAngle,RightAngle);
        }
        else
        {
            if (leftquadrant==1 || rightquadrant==1)
            {
                double m=0;
                double n=0;
                Point point=new Point(0,0);
                if (leftquadrant==1)
                {
                    startangle=LeftAngle;
                    m=GeometricTools.Pendient(r1.StartIn,r1.PassFor);
                    n=GeometricTools.N_of_Equation(r1.StartIn,r1.PassFor);
                    //Revisar este point
                    point=CircleRay2Intersection;
                }
                else 
                {
                    startangle=RightAngle;
                    m=GeometricTools.Pendient(r2.StartIn,r2.PassFor);
                    n=GeometricTools.N_of_Equation(r2.StartIn,r2.PassFor);
                    point=CircleRay1_Intersection;
                }
                double evaluation=GeometricTools.FindY(m,n,point.x);
                if (point.y<=evaluation)
                {
                    sweepangle=360-sweepangle;
                }

            }
            else if(leftquadrant==2 || rightquadrant==2)
            {
                double m=0;
                double n=0;
                Point point=new Point(0,0);
                if (leftquadrant==2)
                {
                    startangle=LeftAngle;
                    m=GeometricTools.Pendient(r1.StartIn,r1.PassFor);
                    n=GeometricTools.N_of_Equation(r1.StartIn,r1.PassFor);
                    //Revisar este point
                    point=CircleRay2Intersection;
                }
                else 
                {
                    startangle=RightAngle;
                    m=GeometricTools.Pendient(r2.StartIn,r2.PassFor);
                    n=GeometricTools.N_of_Equation(r2.StartIn,r2.PassFor);
                    point=CircleRay1_Intersection;
                }
                double evaluation=GeometricTools.FindY(m,n,point.x);
                if (point.y>evaluation)
                {
                    sweepangle=360-sweepangle;
                }
            }
            else if(leftquadrant==3 || rightquadrant==3)
            {
                if (leftquadrant==2)
                {
                    startangle=-LeftAngle;
                }
                else 
                {
                    startangle=-RightAngle;
                }
            }
        }
        return (startangle,sweepangle);

    }
    public override GenericSequence<Point> FigurePoints()
    {
        IEnumerable<Point> Arc_PointsSeq()
        {
            while (true)
            {
                Random random = new Random();
                double x = RandomExtensions.NextDouble(random,center.x-measure-1,center.x+measure+1);
                double y1 =FindYofXinCircle(x)[0];
                double y2 = FindYofXinCircle(x)[1];
                Point p1=new Point(x,y1);
                Point p2=new Point(x,y2);
                var intersect1=p1.Intersect(this);
                var intersect2=p2.Intersect(this);
                if (intersect1.count>0)
                {
                    yield return p1;
                }
                else if (intersect2.count>0)
                {
                    yield return p2;
                }
            }

        }
        IEnumerable<Point> seq=Arc_PointsSeq();
        return new InfinitePointSequence(seq);
    }
    public List<double> FindYofXinCircle(double x)
    {
        double atdistance1=(Math.Sqrt(Math.Pow(this.measure,2)-Math.Pow(x-this.center.x,2)))+this.center.y;
        double atdistance2=(-Math.Sqrt(Math.Pow(this.measure,2)-Math.Pow(x-this.center.x,2)))+this.center.y;
        return new List<double>(){atdistance1,atdistance2};
    }
    public override Finite_Sequence<Point> Intersect(Figure fig)
    {
        if (fig is Point)
        {
            return ((Point)fig).Intersect(this);
        }
        else if (fig is Line)
        {
            return ((Line)fig).Intersect(this);
        }
        else if (fig is Segment)
        {
            return ((Segment)fig).Intersect(this);
        }
        else if (fig is Ray)
        {
            return ((Ray)fig).Intersect(this);
        }
        else if (fig is Circle)
        {
            return ((Circle)fig).Intersect(this);
        }
        else if (fig is Arc)
        {
            return this.IntersectArc((Arc)fig);
        }
        Finite_Sequence<Point> temp=new Finite_Sequence<Point>(new List<Point>());
        temp.type=Finite_Sequence<Point>.SeqType.point;
        return temp;
        
    }
    private Finite_Sequence<Point> IntersectArc(Arc arc)
    {
        if (this.Equals(arc))
        {
            return null!;
        }
        List<Point> points=new List<Point>();
        Circle thisrelativecircle=new Circle(this.center,this.measure);
        Circle relativecircle=new Circle(arc.center,arc.measure);
        var intersect=thisrelativecircle.Intersect(relativecircle);
        if (intersect is null)
        {
            Segment s1=new Segment(this.point_of_semirect1,this.point_of_semirect2);
            Segment s2=new Segment(arc.point_of_semirect1,arc.point_of_semirect2);
            var secondintersect=s1.Intersect(s2);
            return (secondintersect.count>0)?null!:new Finite_Sequence<Point>(new List<Point>());
        }
        else if (intersect.count>0)
        {
            foreach (var item in intersect.Sequence)
            {
                var p1=item.Intersect(this);
                var p2=item.Intersect(arc);
                if (p1.count>0 && p2.count>0)
                {
                    points.Add(item);
                }
            }
        }
        Finite_Sequence<Point> temp=new Finite_Sequence<Point>(points);
        temp.type=Finite_Sequence<Point>.SeqType.point;
        return temp;
    }
}