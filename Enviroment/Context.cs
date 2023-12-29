using System;
public class Context
{
	public List<Fuction> Available_Functions{get;set;}
    public Dictionary<string,Func<double,double>> Trig_functions{get;}
    public Dictionary<string,Func<double>> Math_value{get;}
    public Dictionary<string,Func<double,double,double>> Log{get;}
    public Dictionary<string,object> GlobalConstant{get;set;}
    public Dictionary<string,Func<IEnumerable<double>>> Randoms {get;}
    public Dictionary<string,Func<IEnumerable<Point>>> Samples {get;}
    public Dictionary<string,Func<Circle,IEnumerable<Point>>> Points{get;}
    public List<DrawObject>ToDraw{get;set;}
    public List<Circle>ExistingCircles{get;set;}
    public List<Point>ExistingPoints{get;set;}
    public List<Segment>ExistingSegments{get;set;}
    public List<Line>ExistingLines{get;set;}
    public List<Ray>ExistingRays{get;set;}
    public Stack<string> UtilizedColors{get;set;}
    public bool issuedcontext{get;set;}
    public Context()
    {
        Available_Functions=new List<Fuction>();
        GlobalConstant=new Dictionary<string, object>();
        Trig_functions = new Dictionary<string, Func<double,double>>();
        Trig_functions.Add("sin",(double argument)=>Sin(argument));
        Trig_functions.Add("cos",(double argument)=>Cos(argument));
        Trig_functions.Add("sqrt",(double argument)=>Math.Sqrt(argument));
        Math_value = new Dictionary<string, Func<double>>();
        Math_value.Add("PI",()=>Math.PI);
        Math_value.Add("E",()=>Math.E);
        Log = new Dictionary<string, Func<double, double, double>>();
        Log.Add("log",(double Base,double argument)=> Math.Log(argument,Base));
        Randoms=new Dictionary<string, Func<IEnumerable<double>>>();
        Randoms.Add("randoms",GenerateRandoms);
        Samples=new Dictionary<string, Func<IEnumerable<Point>>>();
        Samples.Add("samples",GenerateSamples);
        Points=new Dictionary<string, Func<Circle, IEnumerable<Point>>>();
        Points.Add("points",GeneratePointsInFigure);
        ToDraw=new List<DrawObject>();
        ExistingCircles=new List<Circle>();
        ExistingLines=new List<Line>();
        ExistingPoints=new List<Point>();
        ExistingRays=new List<Ray>();
        ExistingSegments=new List<Segment>();
        UtilizedColors=new Stack<string>();
        UtilizedColors.Push("black");
        issuedcontext=false;
    }

    private IEnumerable<double> GenerateRandoms()
    {
        Random r=new Random();
        int count=0;
        while (true)
        {
            if (count==0)
            {
                yield return (0.5);
            }
            count++;
            yield return r.NextDouble()*(1-0)+0;
        }   
    }
    private IEnumerable<Point> GenerateSamples()
    {
        List<Point> points=new List<Point>();
        int count=0;
        Point tem=new Point(0,0);
        while (true)
        {
            if (count==0)
            {
                yield return new Point(1.3,2.01);
            }
            count++;
            tem.RandomPoint(points);
            points.Add(tem);
            yield return tem;
        }
    }
    private IEnumerable<Point> GeneratePointsInFigure(Circle c)
    {
        List<Point> points=new List<Point>();
        int count=0;
        Point point=c.PointInsideFigure(points);
        points.Add(point);
        while (true)
        {
            if (count==0)
            {
                yield return point;
            }
            count++;
            point=c.PointInsideFigure(points);
            points.Add(point);
            yield return point;
        }
    }
    double Cos(double argument)
        {
           if (Math.Abs(Math.Cos(argument))<0.0000001)
           {
             return 0;
           }
           else if (1-Math.Cos(argument)<0.0000001)
           {
             return 1;
           }
           else if (1+Math.Cos(argument)<0.0000001)
           {
             return -1;
           }
           else return Math.Cos(argument);
        }
        double Sin(double argument)
        {
           if (Math.Abs(Math.Sin(argument))<0.0000001)
           {
             return 0;
           }
           else if (1-Math.Sin(argument)<0.0000001)
           {
             return 1;
           }
           else if (1+Math.Sin(argument)<0.0000001)
           {
             return -1;
           }
           else return Math.Sin(argument);
        }

}