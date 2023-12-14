using System.Globalization;
public class Evaluator
{   //Modificar para poder acceder a las localizaciones de los errores semánticos
    private Node AST { get; set; }
    private Scope scope { get; set; }
    private Scope CurrentScope { get; set; }
    private Context context { get; set; }
    // private List<Scope>? currentcontext{get;set;}
    public List<Error> Semantic_Errors { get; set; }
    public bool PreviusLineWasImport { get; }

    public Evaluator(Context context, bool previuslineisimport)
    {
        this.context = context;
        scope = new Scope();
        Semantic_Errors = new List<Error>();
        AST = new Node();
        PreviusLineWasImport = previuslineisimport;
        CurrentScope = scope;
    }

    public void Tree_Reader(Node root)
    {
        AST = root;
        // currentcontext=new List<Scope>(){new Scope()};
    }

    public object StartEvaluation(Node node)
    {
        object result = GeneralEvaluation(node);
        return result;
    }
    public Context ResultingContext()
    {
      return context;
    }
    public List<Error> AllTheSemantic_Errors()
    {
        return Semantic_Errors;
    }

    public object GeneralEvaluation(Node node)
    {
        if (node.Type == Node.NodeType.Circle)
        {
            Point center = new Point(0, 0);
            Circle c = new Circle(center, 1);
            c.RandomCircle(context.ExistingCircles, context.ExistingPoints);
            context.ExistingCircles.Add(c);
            if (CurrentScope.Parent == null)
            {
                if (context.GlobalConstant.Keys.Contains(node.NodeExpression!.ToString()!))
                {
                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                }
                else context.GlobalConstant.Add(node.NodeExpression!.ToString()!, c);
            }
            else
            {
                if (CurrentScope.Variables.Keys.Contains(node.NodeExpression!.ToString()!))
                {
                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                }
                else CurrentScope.Variables.Add(node.NodeExpression!.ToString()!, c);
            }
            return "circle created";
        }
        else if (node.Type == Node.NodeType.Point)
        {
            Point p = new Point(0, 0);
            p.RandomPoint(context.ExistingPoints);
            context.ExistingPoints.Add(p);
            if (CurrentScope.Parent == null)
            {
                if (context.GlobalConstant.Keys.Contains(node.NodeExpression!.ToString()!))
                {
                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                }
                else
                    context.GlobalConstant.Add(node.NodeExpression!.ToString()!, p);
            }
            else
            {
                if (CurrentScope.Variables.Keys.Contains(node.NodeExpression!.ToString()!))
                {
                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                }
                else
                    CurrentScope.Variables.Add(node.NodeExpression!.ToString()!, p);
            }
            return "point created";
        }
        else if (node.Type == Node.NodeType.Line)
        {
            Point p1 = new Point(0, 0);
            Point p2 = new Point(0, 0);
            Line l = new Line(p1, p2);
            l.RandomLine(context.ExistingLines, context.ExistingPoints);
            context.ExistingLines.Add(l);
            if (CurrentScope.Parent == null)
            {
                if (context.GlobalConstant.Keys.Contains(node.NodeExpression!.ToString()!))
                {
                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                }
                else
                    context.GlobalConstant.Add(node.NodeExpression!.ToString()!, l);
            }
            else
            {
                if (CurrentScope.Variables.Keys.Contains(node.NodeExpression!.ToString()!))
                {
                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                }
                else
                    CurrentScope.Variables.Add(node.NodeExpression!.ToString()!, l);
            }
            return "line created";
        }
        else if (node.Type == Node.NodeType.Segment)
        {
            Point p1 = new Point(0, 0);
            Point p2 = new Point(0, 0);
            Segment s = new Segment(p1, p2);
            s.RandomSegment(context.ExistingSegments, context.ExistingPoints);
            context.ExistingSegments.Add(s);
            if (CurrentScope.Parent == null)
            {
                if (context.GlobalConstant.Keys.Contains(node.NodeExpression!.ToString()!))
                {
                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                }
                else
                    context.GlobalConstant.Add(node.NodeExpression!.ToString()!, s);
            }
            else
            {
                if (CurrentScope.Variables.Keys.Contains(node.NodeExpression!.ToString()!))
                {
                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                }
                else
                    CurrentScope.Variables.Add(node.NodeExpression!.ToString()!, s);
            }
            return "segment created";
        }
        else if (node.Type == Node.NodeType.Ray)
        {
            Point p1 = new Point(0, 0);
            Point p2 = new Point(0, 0);
            Ray r = new Ray(p1, p2);
            r.RandomRay(context.ExistingRays, context.ExistingPoints);
            context.ExistingRays.Add(r);
            if (CurrentScope.Parent == null)
            {
                if (context.GlobalConstant.Keys.Contains(node.NodeExpression!.ToString()!))
                {
                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                }
                else
                    context.GlobalConstant.Add(node.NodeExpression!.ToString()!, r);
            }
            else
            {
                if (CurrentScope.Variables.Keys.Contains(node.NodeExpression!.ToString()!))
                {
                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                }
                else
                    CurrentScope.Variables.Add(node.NodeExpression!.ToString()!, r);
            }
            return "ray created";
        }
        else if (node.Type == Node.NodeType.Point_Seq)
        {
            List<Point> elements = new List<Point>();
            Random r = new Random();
            int amount = r.Next(1, 30);
            for (int i = 0; i < amount; i++)
            {
                Point temp = new Point(0, 0);
                temp.RandomPoint(context.ExistingPoints);
                elements.Add(temp);
                context.ExistingPoints.Add(temp);
            }
            Finite_Sequence<Point> pts = new Finite_Sequence<Point>(elements);
            if (CurrentScope.Parent == null)
            {
                if (context.GlobalConstant.Keys.Contains(node.NodeExpression!.ToString()!))
                {
                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                }
                else
                    context.GlobalConstant.Add(node.NodeExpression!.ToString()!, pts);
            }
            else
            {
                if (CurrentScope.Variables.Keys.Contains(node.NodeExpression!.ToString()!))
                {
                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                }
                else
                    CurrentScope.Variables.Add(node.NodeExpression!.ToString()!, pts);
            }
            return "sequence of points created";
        }
        else if (node.Type == Node.NodeType.Line_Seq)
        {
            List<Line> elements = new List<Line>();
            Random r = new Random();
            int amount = r.Next(1, 30);
            for (int i = 0; i < amount; i++)
            {
                Point temp1 = new Point(0, 0);
                Point temp2 = new Point(0, 0);
                Line l = new Line(temp1, temp2);
                l.RandomLine(context.ExistingLines, context.ExistingPoints);
                elements.Add(l);
                context.ExistingLines.Add(l);
            }
            Finite_Sequence<Line> pts = new Finite_Sequence<Line>(elements);
            if (CurrentScope.Parent == null)
            {
                if (context.GlobalConstant.Keys.Contains(node.NodeExpression!.ToString()!))
                {
                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                }
                else
                    context.GlobalConstant.Add(node.NodeExpression!.ToString()!, pts);
            }
            else
            {
                if (CurrentScope.Variables.Keys.Contains(node.NodeExpression!.ToString()!))
                {
                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                }
                else
                    CurrentScope.Variables.Add(node.NodeExpression!.ToString()!, pts);
            }
            return "sequence of lines created";
        }
        else if (node.Type == Node.NodeType.GlobalVar)
        {
            string name = GeneralEvaluation(node.Branches[0]).ToString()!;
            object value = GeneralEvaluation(node.Branches[1]);
            if (CurrentScope.Parent == null)
            {
                if (context.GlobalConstant.Keys.Contains(name))
                {
                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                }
                else
                    context.GlobalConstant.Add(name, value);
            }
            else
            {
                if (CurrentScope.Variables.Keys.Contains(name))
                {
                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                }
                else
                    CurrentScope.Variables.Add(name, value);
            }
            return "global constant has been added";

        }
        else if (node.Type == Node.NodeType.Color)
        {
            if (context.UtilizedColors.Peek() != node.NodeExpression!.ToString()!)
            {
                context.UtilizedColors.Push(node.NodeExpression!.ToString()!);
            }
            return ("color changed to {0}", node.NodeExpression!.ToString()!);
        }
        else if (node.Type == Node.NodeType.Restore)
        {
            if (context.UtilizedColors.Count > 1)
            {
                context.UtilizedColors.Pop();
            }
            return ("Used color has been restore to {0}", context.UtilizedColors.Peek());
        }
        else if (node.Type == Node.NodeType.Draw)
        {
            object value = GeneralEvaluation(node.Branches[0]);
            string tag = GeneralEvaluation(node.Branches[1]).ToString()!;
            DrawObject d = new DrawObject(value, tag, context.UtilizedColors.Peek());
            if (!d.CheckValidType())
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "type,this type of object can't be draw", new Location("file", "line", "column")));
            }
            else context.ToDraw.Add(d);
            return "Function to draw added";
        }
        else if (node.Type == Node.NodeType.Fuction)
        {
            string name = node.Branches[0]!.ToString()!;
            Dictionary<string, object> arg = new Dictionary<string, object>();
            string par_name = "";
            foreach (var item in node.Branches[1].Branches)
            {
                par_name = (string)item.NodeExpression!;
                arg.Add(par_name, "");
            }
            Fuction func = new Fuction(node.Branches[0].NodeExpression!.ToString()!, node.Branches[2], arg);
            bool exist = false;
            foreach (var function in context.Available_Functions)
            {
                if (function.Name == name)
                {
                    exist = true;
                }
            }
            if (CurrentScope.Parent == null)
            {
                if (exist)
                {
                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "function name, already exist a preexistent function with the same name", new Location("file", "line", "column")));
                }
                else context.Available_Functions.Add(func);
                return ("{0} Function created and saved", name);
            }
            else
            {
                foreach (var function in CurrentScope.TemporalFunctions)
                {
                    if (function.Key == name)
                    {
                        exist = true;
                    }
                }
                if (exist)
                {
                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "function name, already exist a preexistent function with the same name", new Location("file", "line", "column")));
                }
                else CurrentScope.TemporalFunctions.Add(name, func);
                return ("{0} Function created and saved", name);
            }

        }
        else if (node.Type == Node.NodeType.GlobalSeq)
        {
            object value = GeneralEvaluation(node.Branches[1]);
            Type type = value.GetType();
            long amount_of_elements = node.Branches[0].Branches.Count;
            long index = 0;
            List<object> AlternativeSeq = new List<object>();
            if (value is Finite_Sequence<object>)
            {
                foreach (var subnode in node.Branches[0].Branches)
                {

                    if (subnode.Type == Node.NodeType.Low_Hyphen)
                    {
                        continue;
                    }
                    if (index == amount_of_elements - 1)
                    {
                        string name = GeneralEvaluation(subnode).ToString()!;
                        object valueofarg = ((Finite_Sequence<object>)value).ReturnValue();
                        if (valueofarg == default(object))
                        {
                            valueofarg = "{}";
                            if (CurrentScope.Parent == null)
                            {
                                if (context.GlobalConstant.Keys.Contains(name))
                                {
                                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                                }
                                else
                                    context.GlobalConstant.Add(name, valueofarg);
                            }
                            else
                            {
                                if (CurrentScope.Variables.Keys.Contains(name))
                                {
                                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                                }
                                else
                                    CurrentScope.Variables.Add(name, valueofarg);
                            }
                        }
                        else
                        {
                            AlternativeSeq.Add(valueofarg);
                            while (valueofarg != default(object))
                            {
                                valueofarg = ((Finite_Sequence<object>)value).ReturnValue();
                                if (valueofarg != default(object))
                                {
                                    AlternativeSeq.Add(valueofarg);
                                }
                            }
                            Finite_Sequence<object> rest = new Finite_Sequence<object>(AlternativeSeq);
                            valueofarg = rest;
                            if (CurrentScope.Parent == null)
                            {
                                if (context.GlobalConstant.Keys.Contains(name))
                                {
                                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                                }
                                else
                                    context.GlobalConstant.Add(name, valueofarg);
                            }
                            else
                            {
                                if (CurrentScope.Variables.Keys.Contains(name))
                                {
                                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                                }
                                else
                                    CurrentScope.Variables.Add(name, valueofarg);
                            }

                        }
                    }
                    else if (subnode.Type == Node.NodeType.VarName && index != amount_of_elements - 1)
                    {
                        string name = GeneralEvaluation(subnode).ToString()!;
                        object valueofarg = ((Finite_Sequence<object>)value).ReturnValue();
                        if (valueofarg == default(object))
                        {
                            valueofarg = "undefined";
                        }
                        if (CurrentScope.Parent == null)
                        {
                            if (context.GlobalConstant.Keys.Contains(name))
                            {
                                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                            }
                            else
                                context.GlobalConstant.Add(name, valueofarg);
                        }
                        else
                        {
                            if (CurrentScope.Variables.Keys.Contains(name))
                            {
                                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                            }
                            else
                                CurrentScope.Variables.Add(name, valueofarg);
                        }
                    }
                }
            }
            else if (value.ToString() == "undefined")
            {
                foreach (var subnode in node.Branches[0].Branches)
                {
                    if (subnode.Type == Node.NodeType.Low_Hyphen)
                    {
                        continue;
                    }
                    else if (subnode.Type == Node.NodeType.VarName)
                    {
                        string name = GeneralEvaluation(subnode).ToString()!;
                        if (CurrentScope.Parent == null)
                        {
                            if (context.GlobalConstant.Keys.Contains(name))
                            {
                                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                            }
                            else
                                context.GlobalConstant.Add(name, "undefined");
                        }
                        else
                        {
                            if (CurrentScope.Variables.Keys.Contains(name))
                            {
                                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                            }
                            else
                                CurrentScope.Variables.Add(name, "undefined");
                        }
                    }
                }
            }
            else if (value is Enclosed_Infinite_Sequence)
            {
                foreach (var subnode in node.Branches[0].Branches)
                {

                    if (subnode.Type == Node.NodeType.Low_Hyphen)
                    {
                        continue;
                    }
                    if (index == amount_of_elements - 1)
                    {
                        string name = GeneralEvaluation(subnode).ToString()!;
                        object valueofarg = ((Enclosed_Infinite_Sequence)value).ReturnValue();
                        if (valueofarg.Equals(long.MinValue))
                        {
                            valueofarg = "{}";
                            if (CurrentScope.Parent == null)
                            {
                                if (context.GlobalConstant.Keys.Contains(name))
                                {
                                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                                }
                                else
                                    context.GlobalConstant.Add(name, valueofarg);
                            }
                            else
                            {
                                if (CurrentScope.Variables.Keys.Contains(name))
                                {
                                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                                }
                                else
                                    CurrentScope.Variables.Add(name, valueofarg);
                            }
                        }
                        else
                        {
                            long newstart = Convert.ToInt64(valueofarg);
                            long end = ((Enclosed_Infinite_Sequence)value).EndsAd;
                            Enclosed_Infinite_Sequence rest = new Enclosed_Infinite_Sequence(newstart, end);
                            valueofarg = rest;
                            if (CurrentScope.Parent == null)
                            {
                                if (context.GlobalConstant.Keys.Contains(name))
                                {
                                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                                }
                                else
                                    context.GlobalConstant.Add(name, valueofarg);
                            }
                            else
                            {
                                if (CurrentScope.Variables.Keys.Contains(name))
                                {
                                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                                }
                                else
                                    CurrentScope.Variables.Add(name, valueofarg);
                            }

                        }
                    }
                    else if (subnode.Type == Node.NodeType.VarName && index != amount_of_elements - 1)
                    {
                        string name = GeneralEvaluation(subnode).ToString()!;
                        object valueofarg = ((Enclosed_Infinite_Sequence)value).ReturnValue();
                        if (valueofarg.Equals(long.MinValue))
                        {
                            valueofarg = "undefined";
                        }
                        if (CurrentScope.Parent == null)
                        {
                            if (context.GlobalConstant.Keys.Contains(name))
                            {
                                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                            }
                            else
                                context.GlobalConstant.Add(name, valueofarg);
                        }
                        else
                        {
                            if (CurrentScope.Variables.Keys.Contains(name))
                            {
                                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                            }
                            else
                                CurrentScope.Variables.Add(name, valueofarg);
                        }
                    }
                }
            }
            else if (value is Infinite_Sequence)
            {
                foreach (var subnode in node.Branches[0].Branches)
                {

                    if (subnode.Type == Node.NodeType.Low_Hyphen)
                    {
                        continue;
                    }
                    if (index == amount_of_elements - 1)
                    {
                        string name = GeneralEvaluation(subnode).ToString()!;
                        object valueofarg = ((Infinite_Sequence)value).ReturnValue();

                        long newstart = Convert.ToInt64(valueofarg);
                        Infinite_Sequence rest = new Infinite_Sequence(newstart);
                        valueofarg = rest;
                        if (CurrentScope.Parent == null)
                        {
                            if (context.GlobalConstant.Keys.Contains(name))
                            {
                                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                            }
                            else
                                context.GlobalConstant.Add(name, valueofarg);
                        }
                        else
                        {
                            if (CurrentScope.Variables.Keys.Contains(name))
                            {
                                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                            }
                            else
                                CurrentScope.Variables.Add(name, valueofarg);
                        }


                    }
                    else if (subnode.Type == Node.NodeType.VarName && index != amount_of_elements - 1)
                    {
                        string name = GeneralEvaluation(subnode).ToString()!;
                        object valueofarg = ((Infinite_Sequence)value).ReturnValue();
                        if (CurrentScope.Parent == null)
                        {
                            if (context.GlobalConstant.Keys.Contains(name))
                            {
                                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                            }
                            else
                                context.GlobalConstant.Add(name, valueofarg);
                        }
                        else
                        {
                            if (CurrentScope.Variables.Keys.Contains(name))
                            {
                                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,constants can't be modified", new Location("file", "line", "column")));
                            }
                            else
                                CurrentScope.Variables.Add(name, valueofarg);
                        }
                    }
                }
            }
            return "Requested values seted";
        }
        else if (node.Type == Node.NodeType.FucName)
        {
            return node.NodeExpression!;
        }
        else if (node.Type == Node.NodeType.ParName)
        {
            return node.NodeExpression!;
        }
        else if (node.Type == Node.NodeType.VarName)
        {
            return node.NodeExpression!;
        }
        if (node.Type == Node.NodeType.Text)
        {
            return node.NodeExpression!;
        }
        else if (node.Type == Node.NodeType.Number)
        {
            return node.NodeExpression!;
        }
        else if (node.Type == Node.NodeType.Sum)
        {
            Sum sum = new Sum();
            object left = GeneralEvaluation(node.Branches[0]);
            object right = GeneralEvaluation(node.Branches[1]);
            if ((left.GetType() != right.GetType()) || (!(left is double) && !(left is string) && !(left is Measure) && !(left is GenericSequence<object>)))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "valid values to operate", new Location("file", "line", "column")));
            }
            else
            {
                sum.Evaluate(left, right);
                return sum.Value!;
            }
        }
        else if (node.Type == Node.NodeType.Sub)
        {
            Substraction sub = new Substraction();
            object left = GeneralEvaluation(node.Branches[0]);
            object right = GeneralEvaluation(node.Branches[1]);
            if ((left.GetType() != right.GetType()) || (!(left is double) && !(left is Measure)))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "valid values to operate", new Location("file", "line", "column")));
            }
            sub.Evaluate(left, right);
            return sub.Value!;
        }
        else if (node.Type == Node.NodeType.Mul)
        {
            Multiplication mul = new Multiplication();
            object left = GeneralEvaluation(node.Branches[0]);
            object right = GeneralEvaluation(node.Branches[1]);
            if (!(left is double && right is Measure) && !(left is Measure && right is double) && !(left is double && right is double))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "valid values to operate", new Location("file", "line", "column")));
            }
            else
            {
                mul.Evaluate(left, right);
                return mul.Value!;
            }
        }
        else if (node.Type == Node.NodeType.Div)
        {
            Division div = new Division();
            object left = GeneralEvaluation(node.Branches[0]);
            object right = GeneralEvaluation(node.Branches[1]);
            if ((left.GetType() != right.GetType()) || (!(left is double) && !(left is Measure)))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "valid values to operate", new Location("file", "line", "column")));
            }
            else if ((Convert.ToDouble(right, CultureInfo.InvariantCulture) == 0) || ((Measure)right).Value == 0)
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "operation,can't divide by zero", new Location("file", "line", "column")));
                return left;
            }
            else
            {
                div.Evaluate(left, right);
                return div.Value!;
            }
        }
        else if (node.Type == Node.NodeType.Pow)
        {
            Power pow = new Power();
            object left = GeneralEvaluation(node.Branches[0]);
            object right = GeneralEvaluation(node.Branches[1]);
            if (!(left is double) || !(right is double))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "numerical values", new Location("file", "line", "column")));
            }
            else
            {
                pow.Evaluate(left, right);
                return pow.Value!;
            }
        }
        else if (node.Type == Node.NodeType.Module)
        {
            Module mod = new Module();
            object left = GeneralEvaluation(node.Branches[0]);
            object right = GeneralEvaluation(node.Branches[1]);
            if (!(left is double) || !(right is double))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "numerical values", new Location("file", "line", "column")));
            }
            else
            {
                mod.Evaluate(left, right);
                return mod.Value!;
            }
        }
        else if (node.Type == Node.NodeType.Var)
        {
            if (!CurrentScope.Variables.ContainsKey(node.NodeExpression!.ToString()!))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "variable", new Location("file", "line", "column")));
            }
            else return CurrentScope.Variables[node.NodeExpression!.ToString()!];
        }
        else if (node.Type == Node.NodeType.Declared_FucName)
        {
            return node.NodeExpression!;
        }
        else if (node.Type == Node.NodeType.Sin)
        {
            object arg = GeneralEvaluation(node.Branches[0]);
            if (!(arg is double))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "numerical values", new Location("file", "line", "column")));
            }
            else return context.Trig_functions["sin"](Convert.ToDouble(arg, CultureInfo.InvariantCulture));
        }
        else if (node.Type == Node.NodeType.Cos)
        {
            object arg = GeneralEvaluation(node.Branches[0]);
            if (!(arg is double))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "numerical values", new Location("file", "line", "column")));
            }
            else return context.Trig_functions["cos"](Convert.ToDouble(arg, CultureInfo.InvariantCulture));
        }
        else if (node.Type == Node.NodeType.Sqrt)
        {
            object arg = GeneralEvaluation(node.Branches[0]);
            if (!(arg is double))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "numerical values", new Location("file", "line", "column")));
            }
            else return context.Trig_functions["sqrt"](Convert.ToDouble(arg, CultureInfo.InvariantCulture));
        }
        else if (node.Type == Node.NodeType.Log)
        {
            object base_of = GeneralEvaluation(node.Branches[0]);
            object arg = GeneralEvaluation(node.Branches[1]);
            if (!(arg is double) || !(base_of is double))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "numerical values", new Location("file", "line", "column")));
            }
            else return context.Log["log"](Convert.ToDouble(base_of, CultureInfo.InvariantCulture), Convert.ToDouble(arg, CultureInfo.InvariantCulture));
        }
        else if (node.Type == Node.NodeType.PI)
        {
            return context.Math_value["PI"]();
        }
        else if (node.Type == Node.NodeType.E)
        {
            return context.Math_value["E"]();
        }
        else if (node.Type == Node.NodeType.Randoms)
        {
            IEnumerable<double> rand = context.Randoms["randoms"]();
            InfiniteDoubleSequence randoms = new InfiniteDoubleSequence(rand);
            return randoms;
        }
        else if (node.Type == Node.NodeType.Samples)
        {
            IEnumerable<Point> sam = context.Samples["samples"]();
            InfinitePointSequence samples = new InfinitePointSequence(sam);
            return samples;
        }
        else if (node.Type == Node.NodeType.Points)
        {
            object arg = GeneralEvaluation(node.Branches[0]);
            if (arg is Circle)
            {
                IEnumerable<Point> point = context.Points["points"]((Circle)arg);
                InfinitePointSequence samples = new InfinitePointSequence(point);
                return samples;
            }
            else
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "argument", new Location("line", "file", "column")));
            }

        }
        else if (node.Type == Node.NodeType.Count)
        {
            object arg = GeneralEvaluation(node.Branches[0]);
            if (arg is GenericSequence<object>)
            {
                long c = ((GenericSequence<object>)arg).count;
                if (c < 0)
                {
                    return "undefined";
                }
                return c;
            }

        }
        else if (node.Type == Node.NodeType.Undefined)
        {
            return "undefined";
        }
        else if (node.Type == Node.NodeType.Empty_Seq)
        {
            List<object> list = new List<object>();
            Finite_Sequence<object> seq = new Finite_Sequence<object>(list);
            return seq;
        }
        else if (node.Type == Node.NodeType.Infinite_Seq)
        {
            object value = GeneralEvaluation(node.Branches[0]);
            if (value is long)
            {
                Infinite_Sequence seq = new Infinite_Sequence((long)value);
                return seq;
            }
            else
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "argument", new Location("line", "file", "column")));
            }
        }
        else if (node.Type == Node.NodeType.Enclosed_Infinite_Seq)
        {
            object firstvalue = GeneralEvaluation(node.Branches[0]);
            object finalvalue = GeneralEvaluation(node.Branches[1]);
            if (firstvalue is long && finalvalue is long)
            {
                Enclosed_Infinite_Sequence seq = new Enclosed_Infinite_Sequence((long)firstvalue, (long)finalvalue);
                return seq;
            }
            else
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "boundries", new Location("line", "file", "column")));
            }
        }
        else if (node.Type == Node.NodeType.Finite_Seq)
        {
            object firstvalue = GeneralEvaluation(node.Branches[0]);
            Type t = firstvalue.GetType();
            long index = 0;
            List<object> valuesofseq = new List<object>();
            valuesofseq.Add(firstvalue);
            foreach (var item in node.Branches)
            {
                if (index == 0)
                {
                    continue;
                }
                object value = GeneralEvaluation(item);
                if (firstvalue.GetType() != value.GetType())
                {
                    Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "sequence, all values must belong to the same type", new Location("file", "line", "column")));
                    return "Invalid sequence";
                }
                valuesofseq.Add(value);
            }
            Finite_Sequence<object> seq = new Finite_Sequence<object>(valuesofseq);
            return seq;
        }
        else if (node.Type == Node.NodeType.Negation)
        {
            object value = GeneralEvaluation(node.Branches[0]);
            if (CheckTrueORFalse.Check(value))
            {
                return 0;
            }
            else return 1;
        }
        else if (node.Type == Node.NodeType.Minor)
        {
            Minor min = new Minor();
            object left = GeneralEvaluation(node.Branches[0]);
            object right = GeneralEvaluation(node.Branches[1]);
            if ((left.GetType() != right.GetType()) || (!(left is double) && !(left is Measure)))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "numeric or measure values", new Location("file", "line", "column")));
            }
            else
            {
                min.Evaluate(left, right);
                return min.Value!;
            }
        }
        else if (node.Type == Node.NodeType.Major)
        {
            Major maj = new Major();
            object left = GeneralEvaluation(node.Branches[0]);
            object right = GeneralEvaluation(node.Branches[1]);
            if ((left.GetType() != right.GetType()) || (!(left is double) && !(left is Measure)))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "numeric or measure values", new Location("file", "line", "column")));
            }
            else
            {
                maj.Evaluate(left, right);
                return maj.Value!;
            }
        }
        else if (node.Type == Node.NodeType.Equal_Major)
        {
            Equal_Major emaj = new Equal_Major();
            object left = GeneralEvaluation(node.Branches[0]);
            object right = GeneralEvaluation(node.Branches[1]);
            if ((left.GetType() != right.GetType()) || (!(left is double) && !(left is Measure)))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "numeric or measure values", new Location("file", "line", "column")));
            }
            else
            {
                emaj.Evaluate(left, right);
                return emaj.Value!;
            }
        }
        else if (node.Type == Node.NodeType.Equal_Minor)
        {
            Equal_Minor emin = new Equal_Minor();
            object left = GeneralEvaluation(node.Branches[0]);
            object right = GeneralEvaluation(node.Branches[1]);
            if ((left.GetType() != right.GetType()) || (!(left is double) && !(left is Measure)))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "numeric or measure values", new Location("file", "line", "column")));
            }
            else
            {
                emin.Evaluate(left, right);
                return emin.Value!;
            }
        }
        else if (node.Type == Node.NodeType.Or)
        {
            Or or = new Or();
            object left = GeneralEvaluation(node.Branches[0]);
            object right = GeneralEvaluation(node.Branches[1]);
            if ((left is null) && (right is null))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "valid values to operate", new Location("file", "line", "column")));
            }
            else
            {
                or.Evaluate(left!, right!);
                return or.Value!;
            }
        }
        else if (node.Type == Node.NodeType.And)
        {
            And and = new And();
            object left = GeneralEvaluation(node.Branches[0]);
            object right = GeneralEvaluation(node.Branches[1]);
            if ((left is null) && (right is null))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "valid values to operate", new Location("file", "line", "column")));
            }
            else
            {
                and.Evaluate(left!, right!);
                return and.Value!;
            }
        }
        else if (node.Type == Node.NodeType.Equal)
        {
            Equal eq = new Equal();
            object left = GeneralEvaluation(node.Branches[0]);
            object right = GeneralEvaluation(node.Branches[1]);
            if ((left is null) && (right is null))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "valid values to operate", new Location("file", "line", "column")));
            }
            else
            {
                eq.Evaluate(left!, right!);
                return eq.Value!;
            }
        }
        else if (node.Type == Node.NodeType.Diferent)
        {
            Diferent dif = new Diferent();
            object left = GeneralEvaluation(node.Branches[0]);
            object right = GeneralEvaluation(node.Branches[1]);
            if ((left is null) && (right is null))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "valid values to operate", new Location("file", "line", "column")));
            }
            else
            {
                dif.Evaluate(left!, right!);
                return dif.Value!;
            }
        }
        else if (node.Type == Node.NodeType.Conditional)
        {
            object condition = GeneralEvaluation(node.Branches[0]);
            if ((condition is null))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "valid value", new Location("file", "line", "column")));
                return null!;
            }
            Ternary Conditional = new Ternary();
            if (CheckTrueORFalse.Check(condition))
            {
                object if_part = GeneralEvaluation(node.Branches[1]);
                Conditional.Evaluate(condition, if_part, -1);
                return Conditional.Value!;
            }
            else
            {
                object else_part = GeneralEvaluation(node.Branches[2]);
                Conditional.Evaluate(condition, -1, else_part);
                return Conditional.Value!;
            }


        }
        else if (node.Type == Node.NodeType.Circle_Fuc)
        {
            object center = GeneralEvaluation(node.Branches[0]);
            object radio = GeneralEvaluation(node.Branches[1]);
            if (!(center is Point) || !(radio is Measure || radio is double))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "a valid center point and distance", new Location("file", "line", "column")));
            }
            else
            {
                if (radio is Measure)
                {
                    Circle temp = new Circle((Point)center, ((Measure)radio).Value);
                    context.ExistingCircles.Add(temp);
                    return temp;
                }
                else
                {
                    Circle temp = new Circle((Point)center, (double)radio);
                    context.ExistingCircles.Add(temp);
                    return temp;
                }
            }
        }
        else if (node.Type == Node.NodeType.Line_Fuc)
        {
            object p1 = GeneralEvaluation(node.Branches[0]);
            object p2 = GeneralEvaluation(node.Branches[1]);
            if (!(p1 is Point) || !(p2 is Point))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "valid points to declare a line", new Location("file", "line", "column")));
            }
            else
            {
                Line temp = new Line((Point)p1, (Point)p2);
                context.ExistingLines.Add(temp);
                return temp;
            }
        }
        else if (node.Type == Node.NodeType.Segment_Fuc)
        {
            object p1 = GeneralEvaluation(node.Branches[0]);
            object p2 = GeneralEvaluation(node.Branches[1]);
            if (!(p1 is Point) || !(p2 is Point))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "valid points to declare a segment", new Location("file", "line", "column")));
            }
            else
            {
                Segment temp = new Segment((Point)p1, (Point)p2);
                context.ExistingSegments.Add(temp);
                return temp;
            }
        }
        else if (node.Type == Node.NodeType.Ray_Fuc)
        {
            object p1 = GeneralEvaluation(node.Branches[0]);
            object p2 = GeneralEvaluation(node.Branches[1]);
            if (!(p1 is Point) || !(p2 is Point))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "valid points to declare a ray", new Location("file", "line", "column")));
            }
            else
            {
                Ray temp = new Ray((Point)p1, (Point)p2);
                context.ExistingRays.Add(temp);
                return temp;
            }
        }
        else if (node.Type == Node.NodeType.Measure_Fuc)
        {
            object p1 = GeneralEvaluation(node.Branches[0]);
            object p2 = GeneralEvaluation(node.Branches[1]);
            if (!(p1 is Point) || !(p2 is Point))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "valid points to declare a measure", new Location("file", "line", "column")));
            }
            else
            {
                Measure temp = new Measure((Point)p1, (Point)p2);
                return temp;
            }
        }
        else if (node.Type == Node.NodeType.Arc)
        {
            object p1 = GeneralEvaluation(node.Branches[0]);
            object p2 = GeneralEvaluation(node.Branches[1]);
            object p3 = GeneralEvaluation(node.Branches[2]);
            object m = GeneralEvaluation(node.Branches[3]);
            if (!(p1 is Point) || !(p2 is Point) || !(p3 is Point) || !(m is Measure || m is double))
            {
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, "valid points and distance to declare an arc", new Location("file", "line", "column")));
            }
            else
            {
                if (m is Measure)
                {
                    Arc temp = new Arc((Point)p1, (Point)p2, (Point)p3, ((Measure)m).Value);
                    return temp;
                }
                else if (m is double)
                {
                    Arc temp = new Arc((Point)p1, (Point)p2, (Point)p3, (double)m);
                    return temp;
                }

            }
        }
        else if (node.Type == Node.NodeType.Declared_Fuc)
        {
            string dfunc_name = node.Branches[0].NodeExpression!.ToString()!;
            Node func_parameters = node.Branches[1];
            bool exist = false;
            //Se comprueba si la función fue declarada con anterioridad
            foreach (var function in context.Available_Functions)
            {
                if (function.Name == dfunc_name)
                {
                    exist = true;
                }
            }
            foreach (var function in CurrentScope.TemporalFunctions)
            {
                if (function.Key == dfunc_name)
                {
                    exist = true;
                }
            }

            int index = -1;
            if (exist)
            {
                Scope func_scope = CurrentScope.Child();
                CurrentScope = func_scope;
                // currentcontext!.Add(func_scope);

                for (int i = 0; i < context.Available_Functions.Count; i++)
                {
                    //Se accede a los detalles de la función guardada
                    if (context.Available_Functions[i].Name == dfunc_name)
                    {
                        //Se comprueba si tienen la misma cantidad de argumentos
                        if (context.Available_Functions[i].Functions_Arguments.Count == func_parameters.Branches.Count)
                        {
                            //Se agregan los argumentos dados a el scope del cuerpo de la función
                            // foreach (var p_name in CurrentScope.Parent!.Variables.Keys)
                            // {
                            //     CurrentScope.Variables.Add(p_name, CurrentScope.Parent!.Variables[p_name]);
                            // }
                            int param_number = 0;
                            foreach (var p_name in context.Available_Functions[i].Functions_Arguments.Keys)
                            {
                                //Se asignan los argumentos dados a su correspondiente en los declarados por la función
                                context.Available_Functions[i].Functions_Arguments[p_name] = func_parameters.Branches[param_number].NodeExpression!;
                                //Se evaluan estos argumentos
                                if (CurrentScope.Variables.ContainsKey(p_name))
                                {

                                    CurrentScope.Variables[p_name] = GeneralEvaluation((Node)func_parameters.Branches[param_number].NodeExpression!);
                                    param_number++;
                                }
                                else
                                {
                                    object par_value = GeneralEvaluation((Node)func_parameters.Branches[param_number].NodeExpression!);
                                    CurrentScope.Variables.Add(p_name, par_value);
                                    param_number++;
                                }

                            }
                            index = i;
                            //Se evalua la función solicitada
                            object value = GeneralEvaluation(context.Available_Functions[index].Code);
                            //Se elimina el contexto creado para la función
                            Scope parent = CurrentScope.Parent!;
                            CurrentScope = parent;
                            // currentcontext!.Remove(currentcontext[currentcontext.Count - 1]);

                            return value;

                        }
                        else
                        {
                            //Se lanza un error si el número de parámetros no coincide
                            Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected, $"{context.Available_Functions[i].Functions_Arguments.Count} parameters but received {func_parameters.Branches.Count}", new Location("file", "line", "column")));
                        }

                    }

                }

            }
            else
            {
                //Se lanza error si se está llamando a una función que no existe
                Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "name,function has not been declared", new Location("file", "line", "column")));
                index = -1;
            }


        }
        else if (node.Type == Node.NodeType.Let_exp)
        {
            Scope exp_scope = CurrentScope.Child();
            CurrentScope = exp_scope;
            foreach (var instruction in node.Branches[0].Branches)
            {
                //Check this
                GeneralEvaluation(instruction);
            }
            object value = GeneralEvaluation(node.Branches[1]);
            Scope parent = CurrentScope.Parent!;
            CurrentScope = parent;
            return value;
        }
        else if (node.Type == Node.NodeType.Intersect)
        {

        }
        else if (node.Type == Node.NodeType.Import)
        {
           string archivename=node.NodeExpression!.ToString()!;
           string partialrute=Directory.GetParent(Path.Combine(Directory.GetCurrentDirectory()))!.FullName!;
           string rute=Path.Combine(partialrute,"Librerías de Geo");
           string[] archive=Directory.GetFiles(rute,archivename);
           if (archive.Length>1)
           {
            Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "file name,name must be unique", new Location("file", "line", "column")));
           }
           else if (archive.Length<=0)
           {
            Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid, "file name,file must exist", new Location("file", "line", "column")));
           }
           else
           {
            string code=File.ReadAllText(archive[0]);
            ArchiveAnalysis procesingfile=new ArchiveAnalysis(code,archivename);
            context=procesingfile.Analyze(context);
            return ("File {0} procesed",archivename);
           }
        }
        else
        {
            Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Unknown, "operation required", new Location("file", "line", "column")));
        }

        return "end";

    }



}