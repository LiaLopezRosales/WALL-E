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
     public List<object>ToDraw{get;set;}
     public List<Circle>ExistingCircles{get;set;}
     public List<Point>ExistingPoints{get;set;}
     public List<Segment>ExistingSegments{get;set;}
     public List<Line>ExistingLines{get;set;}
     public List<Ray>ExistingRays{get;set;}
    public Context()
    {
        Available_Functions=new List<Fuction>();
        GlobalConstant=new Dictionary<string, object>();
        Trig_functions = new Dictionary<string, Func<double,double>>();
        Trig_functions.Add("sin",(double argument)=>Math.Sin(argument));
        Trig_functions.Add("cos",(double argument)=>Math.Cos(argument));
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
        ToDraw=new List<object>();
        ExistingCircles=new List<Circle>();
        ExistingLines=new List<Line>();
        ExistingPoints=new List<Point>();
        ExistingRays=new List<Ray>();
        ExistingSegments=new List<Segment>();
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
        while (true)
        {
            Point point=c.PointInsideFigure(points);
            points.Add(point);
            yield return point;
        }
    }
}