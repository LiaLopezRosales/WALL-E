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
}