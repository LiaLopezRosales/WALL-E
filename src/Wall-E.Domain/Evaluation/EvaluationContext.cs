namespace Wall_E.Domain;

public class EvaluationContext
{
    public List<Fuction> Available_Functions { get; set; } = new();
    public Dictionary<string, object> GlobalConstant { get; set; } = new();
    public Dictionary<string, Func<double, double>> Trig_functions { get; }
    public Dictionary<string, Func<double>> Math_value { get; } = new()
    {
        ["PI"] = () => Math.PI,
        ["E"] = () => Math.E,
    };
    public Dictionary<string, Func<double, double, double>> Log { get; } = new()
    {
        ["log"] = (double Base, double argument) => Math.Log(argument, Base),
    };
    public Dictionary<string, Func<IEnumerable<double>>> Randoms { get; }
    public Dictionary<string, Func<IEnumerable<Point>>> Samples { get; }
    public Dictionary<string, Func<Circle, IEnumerable<Point>>> Points { get; }

    public List<object> Results { get; set; } = new();
    public bool HasErrors { get; set; }

    public EvaluationContext()
    {
        Trig_functions = new Dictionary<string, Func<double, double>>
        {
            ["sin"] = Sin,
            ["cos"] = Cos,
            ["sqrt"] = (double argument) => Math.Sqrt(argument),
            ["tan"] = (double a) => Math.Tan(a),
            ["atan"] = (double a) => Math.Atan(a),
            ["abs"] = (double a) => Math.Abs(a),
            ["floor"] = (double a) => Math.Floor(a),
            ["ceil"] = (double a) => Math.Ceiling(a),
        };
        Randoms = new Dictionary<string, Func<IEnumerable<double>>>
        {
            ["randoms"] = GenerateRandoms,
        };
        Samples = new Dictionary<string, Func<IEnumerable<Point>>>
        {
            ["samples"] = GenerateSamples,
        };
        Points = new Dictionary<string, Func<Circle, IEnumerable<Point>>>
        {
            ["points"] = GeneratePointsInFigure,
        };
    }

    public void Clear()
    {
        Available_Functions.Clear();
        GlobalConstant.Clear();
        Results.Clear();
        HasErrors = false;
    }

    private IEnumerable<double> GenerateRandoms()
    {
        Random r = RandomProvider.Instance;
        int count = 0;
        while (true)
        {
            if (count == 0)
                yield return 0.5;
            count++;
            yield return r.NextDouble();
        }
    }

    private IEnumerable<Point> GenerateSamples()
    {
        List<Point> points = new();
        int count = 0;
        while (true)
        {
            if (count == 0)
            {
                yield return new Point(1.3, 2.01);
                count++;
                continue;
            }
            Point tem = new(0, 0);
            tem.RandomPoint(points);
            points.Add(tem);
            yield return tem;
            count++;
        }
    }

    private IEnumerable<Point> GeneratePointsInFigure(Circle c)
    {
        List<Point> points = new();
        int count = 0;
        Point point = c.PointInsideFigure(points);
        points.Add(point);
        while (true)
        {
            if (count == 0)
                yield return point;
            count++;
            point = c.PointInsideFigure(points);
            points.Add(point);
            yield return point;
        }
    }

    double Cos(double argument)
    {
        if (Math.Abs(Math.Cos(argument)) < 0.0000001) return 0;
        else if (1 - Math.Cos(argument) < 0.0000001) return 1;
        else if (1 + Math.Cos(argument) < 0.0000001) return -1;
        else return Math.Cos(argument);
    }

    double Sin(double argument)
    {
        if (Math.Abs(Math.Sin(argument)) < 0.0000001) return 0;
        else if (1 - Math.Sin(argument) < 0.0000001) return 1;
        else if (1 + Math.Sin(argument) < 0.0000001) return -1;
        else return Math.Sin(argument);
    }
}
