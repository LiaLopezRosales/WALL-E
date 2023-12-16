public static class RandomExtensions
    {
        public static double NextDouble(this Random random,double min,double max)
        {
            //return min +(random.NextDouble()*(max-min));
            return random.NextDouble()*(max-min)+min;
        }
    }
    public static class CheckTrueORFalse
    {
        public static bool Check(object x)
        {
            Console.WriteLine("inside checking");
            if (x is double )
            {
                Console.WriteLine("inside double");
                if (((double)x)==0)
                {
                    return false;
                }
            }
            if (x is int )
            {
                if (((int)x)==0)
                {
                    return false;
                }
            }
            if (x is long )
            {
                if (((long)x)==0)
                {
                    return false;
                }
            }
            if (x is string)
            {
                Console.WriteLine("inside string");
                if (((string)x)=="undefined")
                {
                    return false;
                }
            }
            if (x is Finite_Sequence<object>)
            {
                if (((Finite_Sequence<object>)x).Sequence.Count()==0)
                {
                    return false;
                }
            }
            // if (x.Equals("{}")||x.Equals("undefined"))
            // {
            //     return false;
            // }
            return true;
        }
    }

    public static class GeometricTools
    {
        public static double Pendient(Point p1,Point p2)
        {
            return (p1.y-p2.y)/(p1.x-p2.x);
        }
        public static double N_of_Equation(Point p1,Point p2)
        {
            double m=Pendient(p1,p2);
            return p1.y - m*p1.x;
        }
        public static bool BelongToSegment(Point p1,Point p2,double x_cd)
        {
            double aprox_range=0.3;
            if (Math.Abs(x_cd - p2.x)<=aprox_range || Math.Abs(x_cd - p1.x)<=aprox_range)
            {
                return true;
            }
            double reason=(x_cd - p1.x)/(p2.x - x_cd);
            return reason >=0;
        }
        public static double PointsDistance(Point p1,Point p2)
        {
            double distance=Math.Sqrt(Math.Pow(p1.x-p2.x,2)+Math.Pow(p1.y-p2.y,2));
            return distance;
        }
        public static double Point_LineDistance(Point p,Line line)
        {
            double num=Math.Abs(Pendient(line.generalpoint1,line.generalpoint2)*p.x-p.y+N_of_Equation(line.generalpoint1,line.generalpoint2));
            double den=Math.Sqrt(Math.Pow(Pendient(line.generalpoint1,line.generalpoint2),2)+1);
            return num/den;
        }
        public static double VectorsAngle(Point c,Point p1,Point p2)
        {
            double vector1_x=p1.x-c.x;
            double vector1_y=p1.y-c.y;
            double vector2_x=p2.x-c.x;
            double vector2_y=p2.y-c.y;
            double product=EscalarProduct(vector1_x,vector1_y,vector2_x,vector2_y);
            double centerdistance1=PointsDistance(c,p1);
            double centerdistance2=PointsDistance(c,p2);
            double angle=Math.Acos(product/(centerdistance1*centerdistance2));
            angle=(angle/Math.PI)*180;
            return angle;
        }
        public static double EscalarProduct(double x1,double y1,double x2,double y2)
        {
            return x1*x2+y1*y2;
        }
        public static int Quadrant(Point center,double radio,Point p)
        {
            Point rightside=new Point(center.x+radio,center.y);
            Point upside=new Point(center.x,center.y-radio);
            if (p.x==upside.x)
            {
                if (p.y==upside.y)
                {
                    return 4;
                }
                else return 2;
            }
            else if (p.y==rightside.y)
            {
                if (p.x==rightside.x)
                {
                    return 1;
                }
                else return 3;
            }
            else if (p.x>upside.x)
            {
                if (p.y>upside.y)
                {
                    return 1;
                }
                else return 4;
            }
            else if (p.y>rightside.y)
            {
                return 2;
            }
            else return 3;
        }
        public static double FindY(double m,double n,double x)
        {
            double y=m*x+n;
            if (double.IsFinite(m))
            {
                Random random=new Random();
                y=RandomExtensions.NextDouble(random,0,120);
            }
            return y;
        }
        public static Point CircleRay_Intersection(Point center,Point end_of_ray,double radio,double m,double n)
        {
            double a=Math.Pow(m,2)+1;
            double b=2*(center.x-m*(n-center.y));
            double c=Math.Pow(n-center.y,2)-Math.Pow(radio,2)+Math.Pow(center.x,2);
            double d=Math.Pow(b,2)-4*a*c;
            double x1=(b+Math.Sqrt(d))/(2*a);
            double x2=(b-Math.Sqrt(d))/(2*a);
            if (BelongToSegment(center,end_of_ray,x1))
            {
                double y1=FindY(m,n,x1);
                return new Point(x1,y1);
            }
            else if (BelongToSegment(center,end_of_ray,x2))
            {
                double y2=FindY(m,n,x2);
                return new Point(x2,y2);
            }
            return new Point(0,0);
        }
    }