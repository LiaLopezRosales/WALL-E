namespace Wall_E.Domain;

/// <summary>Holds the mutable evaluation state: functions, constants, built-in tables, and results.</summary>
public class EvaluationContext
{
    /// <summary>User-defined functions declared by the program (and imports).</summary>
    public List<Fuction> Available_Functions { get; set; } = new();
    /// <summary>Global constants and variables bound by the program.</summary>
    public Dictionary<string, object> GlobalConstant { get; set; } = new();
    /// <summary>Built-in unary math functions (sin, cos, sqrt, tan, atan, abs, floor, ceil).</summary>
    public Dictionary<string, Func<double, double>> Trig_functions { get; }
    /// <summary>Built-in numeric constants (PI, E).</summary>
    public Dictionary<string, Func<double>> Math_value { get; } = new()
    {
        ["PI"] = () => Math.PI,
        ["E"] = () => Math.E,
    };
    /// <summary>Built-in logarithmic function (log(base, argument)).</summary>
    public Dictionary<string, Func<double, double, double>> Log { get; } = new()
    {
        ["log"] = (double Base, double argument) => Math.Log(argument, Base),
    };
    /// <summary>Built-in random double-sequence generator (randoms).</summary>
    public Dictionary<string, Func<IEnumerable<double>>> Randoms { get; }
    /// <summary>Built-in random point-sample generator (samples).</summary>
    public Dictionary<string, Func<IEnumerable<Point>>> Samples { get; }
    /// <summary>Built-in points-inside-circle generator (points).</summary>
    public Dictionary<string, Func<Circle, IEnumerable<Point>>> Points { get; }

    /// <summary>Evaluated values of every top-level statement, in program order.</summary>
    public List<object> Results { get; set; } = new();
    /// <summary>Output produced by the print statement.</summary>
    public List<string> PrintOutput { get; set; } = new();
    /// <summary>Becomes true when the run accumulated any semantic error.</summary>
    public bool HasErrors { get; set; }

    /// <summary>Creates a context seeded with the built-in function and constant tables.</summary>
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
        PrintOutput.Clear();
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
