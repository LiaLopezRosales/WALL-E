using System.Threading;
namespace Wall_E.Domain;

public class EvaluatorVisitor : INodeVisitor<EvaluationResult>
{
    private EvaluationContext _context;
    private FigureRepository _figures;
    private RenderScene _scene;
    private readonly Scope _rootScope;
    private Scope _currentScope;
    private readonly string _file;
    private string _line = "0";
    private readonly List<Error> _semanticErrors = new();
    public CancellationToken CancellationToken { get; set; }

    public IReadOnlyList<Error> SemanticErrors => _semanticErrors;
    public Scope CurrentScope => _currentScope;

    public EvaluatorVisitor(EvaluationContext context, FigureRepository figures, RenderScene scene, string file)
    {
        _context = context;
        _figures = figures;
        _scene = scene;
        _file = file;
        _rootScope = new Scope();
        _currentScope = _rootScope;
    }

    public void SetLine(string line) => _line = line;
    public void SetCurrentScope(Scope scope) => _currentScope = scope;

    public EvaluationResult Visit(Node node) => node.Type switch
    {
        Node.NodeType.Instructions => VisitInstructions(node),
        Node.NodeType.GlobalVar => VisitGlobalVar(node),
        Node.NodeType.GlobalSeq => VisitGlobalSeq(node),
        Node.NodeType.VarName => VisitVarName(node),
        Node.NodeType.Assigment => VisitAssigment(node),
        Node.NodeType.Low_Hyphen => VisitLowHyphen(node),
        Node.NodeType.Let_exp => VisitLetExp(node),
        Node.NodeType.Draw => VisitDraw(node),
        Node.NodeType.Conditional => VisitConditional(node),
        Node.NodeType.IF => VisitIf(node),
        Node.NodeType.Else => VisitElse(node),
        Node.NodeType.FucName => VisitFucName(node),
        Node.NodeType.Declared_FucName => VisitDeclaredFucName(node),
        Node.NodeType.Declared_Fuc => VisitDeclaredFuc(node),
        Node.NodeType.ParName => VisitParName(node),
        Node.NodeType.Negation => VisitNegation(node),
        Node.NodeType.Var => VisitVar(node),
        Node.NodeType.parameters => VisitParameters(node),
        Node.NodeType.Fuction => VisitFuction(node),
        Node.NodeType.Concat => VisitConcat(node),
        Node.NodeType.And => VisitAnd(node),
        Node.NodeType.Or => VisitOr(node),
        Node.NodeType.Minor => VisitMinor(node),
        Node.NodeType.Major => VisitMajor(node),
        Node.NodeType.Equal_Minor => VisitEqualMinor(node),
        Node.NodeType.Equal_Major => VisitEqualMajor(node),
        Node.NodeType.Equal => VisitEqual(node),
        Node.NodeType.Diferent => VisitDiferent(node),
        Node.NodeType.Sum => VisitSum(node),
        Node.NodeType.Sub => VisitSub(node),
        Node.NodeType.Mul => VisitMul(node),
        Node.NodeType.Div => VisitDiv(node),
        Node.NodeType.Module => VisitModule(node),
        Node.NodeType.Pow => VisitPow(node),
        Node.NodeType.Number => VisitNumber(node),
        Node.NodeType.Circle => VisitCircle(node),
        Node.NodeType.Point => VisitPoint(node),
        Node.NodeType.Line => VisitLine(node),
        Node.NodeType.Ray => VisitRay(node),
        Node.NodeType.Segment => VisitSegment(node),
        Node.NodeType.Arc => VisitArc(node),
        Node.NodeType.Point_Seq => VisitPointSeq(node),
        Node.NodeType.Line_Seq => VisitLineSeq(node),
        Node.NodeType.Color => VisitColor(node),
        Node.NodeType.Restore => VisitRestore(node),
        Node.NodeType.Import => VisitImport(node),
        Node.NodeType.Point_Fuc => VisitPointFuc(node),
        Node.NodeType.Line_Fuc => VisitLineFuc(node),
        Node.NodeType.Segment_Fuc => VisitSegmentFuc(node),
        Node.NodeType.Ray_Fuc => VisitRayFuc(node),
        Node.NodeType.Circle_Fuc => VisitCircleFuc(node),
        Node.NodeType.Polygon_Fuc => VisitPolygonFuc(node),
        Node.NodeType.Ellipse_Fuc => VisitEllipseFuc(node),
        Node.NodeType.Measure => VisitMeasure(node),
        Node.NodeType.Measure_Fuc => VisitMeasureFuc(node),
        Node.NodeType.Intersect => VisitIntersect(node),
        Node.NodeType.Count => VisitCount(node),
        Node.NodeType.Text => VisitText(node),
        Node.NodeType.Cos => VisitCos(node),
        Node.NodeType.Sin => VisitSin(node),
        Node.NodeType.Log => VisitLog(node),
        Node.NodeType.Sqrt => VisitSqrt(node),
        Node.NodeType.Tan => VisitTan(node),
        Node.NodeType.Atan => VisitAtan(node),
        Node.NodeType.Abs => VisitAbs(node),
        Node.NodeType.Floor => VisitFloor(node),
        Node.NodeType.Ceil => VisitCeil(node),
        Node.NodeType.Phi => VisitPhi(node),
        Node.NodeType.Sqrt2 => VisitSqrt2(node),
        Node.NodeType.Seed => VisitSeed(node),
        Node.NodeType.Print => VisitPrint(node),
        Node.NodeType.ColorRgb => VisitColorRgb(node),
        Node.NodeType.ColorRgba => VisitColorRgba(node),
        Node.NodeType.ColorHsl => VisitColorHsl(node),
        Node.NodeType.Lighten => VisitLighten(node),
        Node.NodeType.Darken => VisitDarken(node),
        Node.NodeType.MixColors => VisitMixColors(node),
        Node.NodeType.Complement => VisitComplement(node),
        Node.NodeType.Repeat => VisitRepeat(node),
        Node.NodeType.For => VisitFor(node),
        Node.NodeType.Label => VisitLabel(node),
        Node.NodeType.LineStyleStmt => VisitLineStyleStmt(node),
        Node.NodeType.GrosorStmt => VisitGrosorStmt(node),
        Node.NodeType.FillStmt => VisitFillStmt(node),
        Node.NodeType.LayerStmt => VisitLayerStmt(node),
        Node.NodeType.HideStmt => VisitHideStmt(node),
        Node.NodeType.ShowStmt => VisitShowStmt(node),
        Node.NodeType.SnapStmt => VisitSnapStmt(node),
        Node.NodeType.Points => VisitPoints(node),
        Node.NodeType.Randoms => VisitRandoms(node),
        Node.NodeType.Samples => VisitSamples(node),
        Node.NodeType.Empty_Seq => VisitEmptySeq(node),
        Node.NodeType.Enclosed_Infinite_Seq => VisitEnclosedInfiniteSeq(node),
        Node.NodeType.Infinite_Seq => VisitInfiniteSeq(node),
        Node.NodeType.Finite_Seq => VisitFiniteSeq(node),
        Node.NodeType.PI => VisitPI(node),
        Node.NodeType.E => VisitE(node),
        Node.NodeType.Indefined => VisitIndefined(node),
        Node.NodeType.Undefined => VisitUndefined(node),
        _ => throw new NotImplementedException($"Unknown node type: {node.Type}")
    };

    // Already migrated methods
    public EvaluationResult VisitCircle(Node node)
    {
        Point center = new(0, 0);
        Circle c = new(center, 1);
        c.RandomCircle(_figures.ExistingCircles, _figures.ExistingPoints);
        _figures.TryAddExistingCircle(c);
        StoreVariable(node.NodeExpression!.ToString()!, c);
        return new StringResult("circle created");
    }

    public EvaluationResult VisitPoint(Node node)
    {
        Point p = new(0, 0);
        p.RandomPoint(_figures.ExistingPoints);
        _figures.TryAddExistingPoint(p);
        StoreVariable(node.NodeExpression!.ToString()!, p);
        return new StringResult("point created");
    }

    public EvaluationResult VisitLine(Node node)
    {
        Line l = new(new Point(0, 0), new Point(1, 1));
        l.RandomLine(_figures.ExistingLines, _figures.ExistingPoints);
        _figures.TryAddExistingLine(l);
        StoreVariable(node.NodeExpression!.ToString()!, l);
        return new StringResult("line created");
    }

    public EvaluationResult VisitSegment(Node node)
    {
        Segment s = new(new Point(0, 0), new Point(1, 1));
        s.RandomSegment(_figures.ExistingSegments, _figures.ExistingPoints);
        _figures.TryAddExistingSegment(s);
        StoreVariable(node.NodeExpression!.ToString()!, s);
        return new StringResult("segment created");
    }

    public EvaluationResult VisitRay(Node node)
    {
        Ray r = new(new Point(0, 0), new Point(1, 1));
        r.RandomRay(_figures.ExistingRays, _figures.ExistingPoints);
        _figures.TryAddExistingRay(r);
        StoreVariable(node.NodeExpression!.ToString()!, r);
        return new StringResult("ray created");
    }

    public EvaluationResult VisitNumber(Node node) => new NumberResult(double.Parse(node.NodeExpression!.ToString()!));
    public EvaluationResult VisitVarName(Node node) => new StringResult(node.NodeExpression!.ToString()!);
    public EvaluationResult VisitFucName(Node node) => new StringResult(node.NodeExpression!.ToString()!);
    public EvaluationResult VisitDeclaredFucName(Node node) => new StringResult(node.NodeExpression!.ToString()!);
    public EvaluationResult VisitParName(Node node) => new StringResult(node.NodeExpression!.ToString()!);
    public EvaluationResult VisitText(Node node) => new StringResult(node.NodeExpression!.ToString()!);
    public EvaluationResult VisitLowHyphen(Node node) => new StringResult(node.NodeExpression!.ToString()!);
    public EvaluationResult VisitPI(Node node) => new NumberResult(Math.PI);
    public EvaluationResult VisitE(Node node) => new NumberResult(Math.E);
    public EvaluationResult VisitPhi(Node node) => new NumberResult(1.618033988749895);
    public EvaluationResult VisitSqrt2(Node node) => new NumberResult(Math.Sqrt(2));

    public EvaluationResult VisitTan(Node node) => VisitTrigFunc(node, "tan");
    public EvaluationResult VisitAtan(Node node) => VisitTrigFunc(node, "atan");
    public EvaluationResult VisitAbs(Node node) => VisitTrigFunc(node, "abs");
    public EvaluationResult VisitFloor(Node node) => VisitTrigFunc(node, "floor");
    public EvaluationResult VisitCeil(Node node) => VisitTrigFunc(node, "ceil");

    private EvaluationResult VisitTrigFunc(Node node, string funcName)
    {
        EvaluationResult argResult = Visit(node.Branches[0]);
        if (argResult is ErrorResult) return argResult;
        object arg = UnwrapRaw(argResult)!;
        if (!(arg is double) && !(arg is long))
        {
            AddError("numerical values");
            return new VoidResult();
        }
        return new NumberResult(_context.Trig_functions[funcName](Convert.ToDouble(arg)));
    }

    public EvaluationResult VisitSeed(Node node)
    {
        EvaluationResult argResult = Visit(node.Branches[0]);
        if (argResult is ErrorResult) return argResult;
        object arg = UnwrapRaw(argResult)!;
        if (!(arg is double) && !(arg is long))
        {
            AddError("numerical values");
            return new VoidResult();
        }
        RandomProvider.Seed(Convert.ToInt32(arg));
        return new VoidResult();
    }

    public EvaluationResult VisitPrint(Node node)
    {
        EvaluationResult argResult = Visit(node.Branches[0]);
        if (argResult is ErrorResult) return argResult;
        object raw = UnwrapRaw(argResult)!;
        string text = raw switch
        {
            double d => d.ToString(System.Globalization.CultureInfo.InvariantCulture),
            long l => l.ToString(),
            _ => raw.ToString()!
        };
        _context.PrintOutput.Add(text);
        return new VoidResult();
    }

    public EvaluationResult VisitColorRgb(Node node)
    {
        string hex = EvalColorHex(node.Branches, 3, null);
        _scene.PushColor(hex);
        return new StringResult($"Color changed to {hex}");
    }

    public EvaluationResult VisitColorRgba(Node node)
    {
        string hex = EvalColorHex(node.Branches, 4, node.Branches[3]);
        _scene.PushColor(hex);
        return new StringResult($"Color changed to {hex}");
    }

    public EvaluationResult VisitColorHsl(Node node)
    {
        double[] vals = new double[3];
        for (int i = 0; i < 3; i++)
        {
            EvaluationResult r = Visit(node.Branches[i]);
            if (r is ErrorResult) return r;
            vals[i] = Convert.ToDouble(UnwrapRaw(r));
        }
        string hex = HslConverter.ToHex(vals[0], vals[1], vals[2]);
        _scene.PushColor(hex);
        return new StringResult($"Color changed to {hex}");
    }

    public EvaluationResult VisitLighten(Node node)
    {
        EvaluationResult r = Visit(node.Branches[0]);
        if (r is ErrorResult) return r;
        double amount = Convert.ToDouble(UnwrapRaw(r));
        string current = _scene.UtilizedColors.Peek();
        string result = HslConverter.Lighten(current, amount);
        _scene.PushColor(result);
        return new StringResult($"Color lightened to {result}");
    }

    public EvaluationResult VisitDarken(Node node)
    {
        EvaluationResult r = Visit(node.Branches[0]);
        if (r is ErrorResult) return r;
        double amount = Convert.ToDouble(UnwrapRaw(r));
        string current = _scene.UtilizedColors.Peek();
        string result = HslConverter.Darken(current, amount);
        _scene.PushColor(result);
        return new StringResult($"Color darkened to {result}");
    }

    public EvaluationResult VisitMixColors(Node node)
    {
        EvaluationResult otherResult = Visit(node.Branches[0]);
        if (otherResult is ErrorResult) return otherResult;
        string otherHex = ColorTable.Resolve(RawString(otherResult));
        EvaluationResult ratioResult = Visit(node.Branches[1]);
        if (ratioResult is ErrorResult) return ratioResult;
        double ratio = Convert.ToDouble(UnwrapRaw(ratioResult));
        string current = _scene.UtilizedColors.Peek();
        string result = HslConverter.Mix(current, otherHex, ratio);
        _scene.PushColor(result);
        return new StringResult($"Color mixed to {result}");
    }

    public EvaluationResult VisitComplement(Node node)
    {
        string current = _scene.UtilizedColors.Peek();
        string result = HslConverter.Complement(current);
        _scene.PushColor(result);
        return new StringResult($"Color complement is {result}");
    }

    private string EvalColorHex(List<Node> channels, int count, Node? alphaNode)
    {
        int[] vals = new int[count];
        for (int i = 0; i < count; i++)
        {
            EvaluationResult r = Visit(channels[i]);
            if (r is ErrorResult) return "#000000";
            object v = UnwrapRaw(r)!;
            double d = Convert.ToDouble(v);
            vals[i] = Math.Clamp((int)Math.Round(d), 0, 255);
        }
        if (alphaNode != null)
        {
            double a = Convert.ToDouble(UnwrapRaw(Visit(alphaNode))!);
            int ai = Math.Clamp((int)Math.Round(a * 255), 0, 255);
            return $"#{vals[0]:X2}{vals[1]:X2}{vals[2]:X2}{ai:X2}";
        }
        return $"#{vals[0]:X2}{vals[1]:X2}{vals[2]:X2}";
    }

    public EvaluationResult VisitRepeat(Node node)
    {
        EvaluationResult countResult = Visit(node.Branches[0]);
        if (countResult is ErrorResult) return countResult;
        object countRaw = UnwrapRaw(countResult)!;
        if (!(countRaw is long) && !(countRaw is double))
        {
            AddError("numerical value for repeat count");
            return new VoidResult();
        }
        int count = Convert.ToInt32(countRaw);
        Node body = node.Branches[1];
        EvaluationResult last = new VoidResult();
        for (int i = 0; i < count; i++)
        {
            foreach (var stmt in body.Branches)
            {
                last = Visit(stmt);
                if (last is ErrorResult) return last;
            }
        }
        return last;
    }

    public EvaluationResult VisitFor(Node node)
    {
        string varName = node.NodeExpression!.ToString()!;
        EvaluationResult seqResult = Visit(node.Branches[0]);
        if (seqResult is ErrorResult) return seqResult;
        Node body = node.Branches[1];
        EvaluationResult last = new VoidResult();
        IEnumerable<object>? items = null;
        object seqObj = UnwrapRaw(seqResult)!;
        switch (seqObj)
        {
            case Finite_Sequence<Point> fps:
                items = fps.Sequence!.Cast<object>();
                break;
            case InfinitePointSequence ips:
                items = ips.Sequence!.Take(10000).Cast<object>();
                break;
            case GenericSequence<Point> gsp:
                items = gsp.Sequence!.Cast<object>();
                break;
            case Finite_Sequence<object> fso:
                items = fso.Sequence!;
                break;
        }
        if (items is null)
        {
            AddError("a valid sequence for for-loop");
            return new VoidResult();
        }
        Scope loopScope = CurrentScope.Child();
        SetCurrentScope(loopScope);
        foreach (var item in items)
        {
            _currentScope.Variables[varName] = item;
            foreach (var stmt in body.Branches)
            {
                last = Visit(stmt);
                if (last is ErrorResult) { SetCurrentScope(loopScope.Parent!); return last; }
            }
        }
        Scope parent = CurrentScope.Parent!;
        SetCurrentScope(parent);
        return last;
    }

    public EvaluationResult VisitLabel(Node node)
    {
        EvaluationResult posResult = Visit(node.Branches[0]);
        if (posResult is ErrorResult) return posResult;
        EvaluationResult textResult = Visit(node.Branches[1]);
        if (textResult is ErrorResult) return textResult;
        EvaluationResult sizeResult = Visit(node.Branches[2]);
        if (sizeResult is ErrorResult) return sizeResult;
        object pos = UnwrapRaw(posResult)!;
        object text = UnwrapRaw(textResult)!;
        object size = UnwrapRaw(sizeResult)!;
        if (pos is not Point || text is not string || !(size is long || size is double))
        {
            AddError("a valid position point, text and font size");
            return new VoidResult();
        }
        double fontSize = Convert.ToDouble(size);
        string color = _scene.CurrentColor;
        _scene.AddLabel(new LabelObject((Point)pos, ((string)text).Trim('"'), fontSize, color));
        return new VoidResult();
    }

    public EvaluationResult VisitLineStyleStmt(Node node)
    {
        string styleName = node.NodeExpression!.ToString()!;
        _scene.CurrentLineStyle = styleName switch
        {
            "dashed" => LineStyle.Dashed,
            "dotted" => LineStyle.Dotted,
            "dashdot" => LineStyle.DashDot,
            _ => LineStyle.Solid,
        };
        return new VoidResult();
    }

    public EvaluationResult VisitGrosorStmt(Node node)
    {
        EvaluationResult widthResult = Visit(node.Branches[0]);
        if (widthResult is ErrorResult) return widthResult;
        object w = UnwrapRaw(widthResult)!;
        if (!(w is long) && !(w is double))
        {
            AddError("numerical value for grosor");
            return new VoidResult();
        }
        _scene.CurrentStrokeWidth = Convert.ToDouble(w);
        return new VoidResult();
    }

    public EvaluationResult VisitFillStmt(Node node)
    {
        string fillName = node.NodeExpression!.ToString()!;
        if (fillName == "unfill")
        {
            _scene.CurrentFillType = FillType.None;
            _scene.CurrentGradientColor1 = "";
            _scene.CurrentGradientColor2 = "";
        }
        else if (fillName == "fill")
        {
            _scene.CurrentFillType = FillType.Solid;
            _scene.CurrentGradientColor1 = "";
            _scene.CurrentGradientColor2 = "";
        }
        else if (fillName == "linear" && node.Branches != null && node.Branches.Count == 2)
        {
            _scene.CurrentFillType = FillType.LinearGradient;
            _scene.CurrentGradientColor1 = ResolveColor(node.Branches[0]);
            _scene.CurrentGradientColor2 = ResolveColor(node.Branches[1]);
        }
        else if (fillName == "radial" && node.Branches != null && node.Branches.Count == 2)
        {
            _scene.CurrentFillType = FillType.RadialGradient;
            _scene.CurrentGradientColor1 = ResolveColor(node.Branches[0]);
            _scene.CurrentGradientColor2 = ResolveColor(node.Branches[1]);
        }
        return new VoidResult();
    }

    private string ResolveColor(Node colorNode)
    {
        EvaluationResult result = Visit(colorNode);
        string raw = RawString(result);
        return ColorTable.Resolve(raw);
    }

    public EvaluationResult VisitLayerStmt(Node node)
    {
        double layer = Convert.ToDouble(node.NodeExpression);
        _scene.CurrentLayer = (int)layer;
        return new VoidResult();
    }

    public EvaluationResult VisitHideStmt(Node node)
    {
        string labelName = node.NodeExpression!.ToString()!;
        _scene.HiddenLabels.Add(labelName);
        return new VoidResult();
    }

    public EvaluationResult VisitShowStmt(Node node)
    {
        string labelName = node.NodeExpression!.ToString()!;
        _scene.HiddenLabels.Remove(labelName);
        return new VoidResult();
    }

    public EvaluationResult VisitSnapStmt(Node node)
    {
        double snapVal = Convert.ToDouble(node.NodeExpression);
        _scene.SnapValue = snapVal;
        return new VoidResult();
    }

    public EvaluationResult VisitIndefined(Node node) => new StringResult("undefined");
    public EvaluationResult VisitUndefined(Node node) => new StringResult("undefined");

    public EvaluationResult VisitInstructions(Node node)
    {
        // Instructions is a container node; each branch is individually evaluated by the caller.
        // Individual branches are not evaluated here.
        return new VoidResult();
    }

    public EvaluationResult VisitLetExp(Node node)
    {
        Scope expScope = CurrentScope.Child();
        if (_currentScope.InFunction)
            expScope.InFunction = true;
        SetCurrentScope(expScope);
        foreach (var instruction in node.Branches[0].Branches)
            Visit(instruction);
        EvaluationResult value = Visit(node.Branches[1].Branches[0]);
        Scope parent = CurrentScope.Parent!;
        SetCurrentScope(parent);
        return value;
    }

    public EvaluationResult VisitDeclaredFuc(Node node)
    {
        string dfuncName = node.Branches[0].NodeExpression!.ToString()!;
        Node funcParameters = node.Branches[1];
        bool exist = false;

        foreach (var function in _context.Available_Functions)
        {
            if (function.Name == dfuncName)
            {
                exist = true;
                break;
            }
        }

        int index = -1;
        if (exist)
        {
            Scope funcScope = CurrentScope.Child();
            funcScope.InFunction = true;
            SetCurrentScope(funcScope);

            for (int i = 0; i < _context.Available_Functions.Count; i++)
            {
                if (_context.Available_Functions[i].Name == dfuncName)
                {
                    if (_context.Available_Functions[i].Functions_Arguments.Count == funcParameters.Branches.Count)
                    {
                        int paramNumber = 0;
                        foreach (var pName in _context.Available_Functions[i].Functions_Arguments.Keys)
                        {
                            _context.Available_Functions[i].Functions_Arguments[pName] = funcParameters.Branches[paramNumber];
                            if (_currentScope.Variables.ContainsKey(pName))
                                _currentScope.Variables[pName] = UnwrapRaw(Visit(funcParameters.Branches[paramNumber]));
                            else
                            {
                                object parValue = UnwrapRaw(Visit(funcParameters.Branches[paramNumber]))!;
                                _currentScope.Variables.Add(pName, parValue);
                            }
                            paramNumber++;
                        }
                        index = i;
                        _context.Available_Functions[index].NumberofCalls++;
                        if (_context.Available_Functions[index].NumberofCalls > 100)
                        {
                            _semanticErrors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid,
                                $"call,full stack for function {_context.Available_Functions[i].Name}", new Location(_file, _line, "column")));
                            return new StringResult("");
                        }
                        EvaluationResult value = Visit(_context.Available_Functions[index].Code);
                        Scope parent = CurrentScope.Parent!;
                        SetCurrentScope(parent);
                        return value;
                    }
                    else
                    {
                        _semanticErrors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected,
                            $"{_context.Available_Functions[i].Functions_Arguments.Count} parameters but received {funcParameters.Branches.Count}", new Location(_file, _line, "column")));
                    }
                }
            }
        }
        else
        {
            foreach (var function in CurrentScope.TemporalFunctions)
            {
                if (function.Key == dfuncName)
                {
                    exist = true;
                    break;
                }
            }
            if (exist)
            {
                Scope funcScope = CurrentScope.Child();
                funcScope.InFunction = true;
                SetCurrentScope(funcScope);
                var func = CurrentScope.TemporalFunctions[dfuncName];
                if (func.Functions_Arguments.Count == funcParameters.Branches.Count)
                {
                    int paramNumber = 0;
                    foreach (var pName in func.Functions_Arguments.Keys)
                    {
                        func.Functions_Arguments[pName] = funcParameters.Branches[paramNumber];
                        if (_currentScope.Variables.ContainsKey(pName))
                            _currentScope.Variables[pName] = UnwrapRaw(Visit(funcParameters.Branches[paramNumber]));
                        else
                        {
                            object parValue = UnwrapRaw(Visit(funcParameters.Branches[paramNumber]))!;
                            _currentScope.Variables.Add(pName, parValue);
                        }
                        paramNumber++;
                    }
                    func.NumberofCalls++;
                    if (func.NumberofCalls > 100)
                    {
                        _semanticErrors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid,
                            $"call,full stack for function {func.Name}", new Location(_file, _line, "column")));
                        return new StringResult("");
                    }
                    EvaluationResult value = Visit(func.Code);
                    Scope parent = CurrentScope.Parent!;
                    SetCurrentScope(parent);
                    return value;
                }
                else
                {
                    _semanticErrors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected,
                        $"{func.Functions_Arguments.Count} parameters but received {funcParameters.Branches.Count}", new Location(_file, _line, "column")));
                }
            }
            else
            {
                _semanticErrors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid,
                    "name,function has not been declared", new Location(_file, _line, "column")));
            }
        }
        return new VoidResult();
    }

    public EvaluationResult VisitFuction(Node node)
    {
        string name = node.Branches[0].NodeExpression!.ToString()!;
        Dictionary<string, object> arg = new();
        string parName;
        foreach (var item in node.Branches[1].Branches)
        {
            parName = (string)item.NodeExpression!;
            arg.Add(parName, "");
        }
        var func = new Fuction(node.Branches[0].NodeExpression!.ToString()!, node.Branches[2], arg);
        bool exist = false;

        if (CurrentScope.Parent == null)
        {
            foreach (var function in _context.Available_Functions)
            {
                if (function.Name == name)
                {
                    exist = true;
                    break;
                }
            }
            if (exist)
            {
                _semanticErrors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid,
                    "function name, already exist a preexistent function with the same name", new Location(_file, _line, "column")));
                return new VoidResult();
            }
            _context.Available_Functions.Add(func);
            return new StringResult($"{node.Branches[0].NodeExpression!.ToString()!} Function created and saved");
        }
        else
        {
            foreach (var function in CurrentScope.TemporalFunctions)
            {
                if (function.Key == name)
                {
                    exist = true;
                    break;
                }
            }
            if (exist)
            {
                _semanticErrors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid,
                    "function name, already exist a preexistent function with the same name", new Location(_file, _line, "column")));
                return new VoidResult();
            }
            CurrentScope.TemporalFunctions.Add(name, func);
            return new StringResult($"{node.Branches[0].NodeExpression!.ToString()!} Function created and saved");
        }
    }

    // Multiple assignment from a sequence: a, b, _ = {seq};
    // '_' discards one element; the last target receives ALL remaining elements
    // as a new finite sequence ("{}" when exhausted). Unified replacement of the
    // ~600 duplicated legacy lines (one copy per sequence type).
    public EvaluationResult VisitGlobalSeq(Node node)
    {
        EvaluationResult valueResult = Visit(node.Branches[1]);
        if (valueResult is ErrorResult) return valueResult;
        object value = UnwrapRaw(valueResult)!;
        if (value is not AbsSequence seq)
        {
            AddError("sequence");
            return new VoidResult();
        }

        List<Node> targets = node.Branches[0].Branches;
        for (int i = 0; i < targets.Count; i++)
        {
            Node target = targets[i];
            bool isLast = i == targets.Count - 1;

            if (target.Type == Node.NodeType.Low_Hyphen)
            {
                ConsumeNext(seq);
                continue;
            }

            string name = RawString(Visit(target));

            if (!isLast)
            {
                object element = ConsumeNext(seq)!;
                StoreVariable(name, IsExhausted(element) ? "undefined" : element);
            }
            else
            {
                List<object> rest = new();
                object element = ConsumeNext(seq)!;
                while (!IsExhausted(element))
                {
                    rest.Add(element);
                    element = ConsumeNext(seq)!;
                }
                StoreVariable(name, rest.Count > 0 ? new Finite_Sequence<object>(rest) : "{}");
            }
        }
        return new StringResult("end");
    }

    // ReturnValue is declared on GenericSequence<T>; sequences reaching GlobalSeq
    // vary their T, so use the boxed override on AbsSequence to avoid DLR dispatch.
    private static object? ConsumeNext(AbsSequence seq) => seq.ReturnValueBoxed();

    private static bool IsExhausted(object? v) =>
        v is null
        || (v is long l && l == long.MinValue)
        || (v is double d && d == long.MinValue)
        || (v is Point p && p.x == 0 && p.y == 0);

    // Parser never produces Concat nodes (sequence concatenation flows through Sum).
    public EvaluationResult VisitConcat(Node node) => new VoidResult();
    public EvaluationResult VisitConditional(Node node)
    {
        EvaluationResult condResult = Visit(node.Branches[0]);
        if (condResult is ErrorResult) return condResult;
        object? condition = UnwrapRaw(condResult);
        if (condition is null)
        {
            AddError("valid value");
            return new VoidResult();
        }
        if (CheckTrueORFalse.Check(condition))
            return Visit(node.Branches[1]);
        return Visit(node.Branches[2]);
    }
    // Parser never produces Else/If/Parameters nodes; kept as transparent no-ops.
    public EvaluationResult VisitIf(Node node) => VisitConditional(node);
    public EvaluationResult VisitElse(Node node) => new VoidResult();
    public EvaluationResult VisitAssigment(Node node) => Visit(node.Branches[0]);
    public EvaluationResult VisitParameters(Node node)
    {
        foreach (var branch in node.Branches)
            Visit(branch);
        return new VoidResult();
    }
    public EvaluationResult VisitNegation(Node node)
    {
        EvaluationResult valResult = Visit(node.Branches[0]);
        if (valResult is ErrorResult) return valResult;
        object val = UnwrapRaw(valResult)!;
        if (CheckTrueORFalse.Check(val))
            return new NumberResult(0);
        return new NumberResult(1);
    }

    public EvaluationResult VisitMinor(Node node)
    {
        EvaluationResult leftResult = Visit(node.Branches[0]);
        if (leftResult is ErrorResult) return leftResult;
        EvaluationResult rightResult = Visit(node.Branches[1]);
        if (rightResult is ErrorResult) return rightResult;

        object left = UnwrapRaw(leftResult)!;
        object right = UnwrapRaw(rightResult)!;

        if ((left.GetType() != right.GetType()) || (!(left is double) && !(left is long) && !(left is Measure)))
        {
            AddError("numeric or measure values");
            return new VoidResult();
        }

        var min = new Minor();
        min.Evaluate(left, right);
        return WrapResult(min.Value);
    }

    public EvaluationResult VisitMajor(Node node)
    {
        EvaluationResult leftResult = Visit(node.Branches[0]);
        if (leftResult is ErrorResult) return leftResult;
        EvaluationResult rightResult = Visit(node.Branches[1]);
        if (rightResult is ErrorResult) return rightResult;

        object left = UnwrapRaw(leftResult)!;
        object right = UnwrapRaw(rightResult)!;

        if ((left.GetType() != right.GetType()) || (!(left is double) && !(left is long) && !(left is Measure)))
        {
            AddError("numeric or measure values");
            return new VoidResult();
        }

        var maj = new Major();
        maj.Evaluate(left, right);
        return WrapResult(maj.Value);
    }

    public EvaluationResult VisitEqualMajor(Node node)
    {
        EvaluationResult leftResult = Visit(node.Branches[0]);
        if (leftResult is ErrorResult) return leftResult;
        EvaluationResult rightResult = Visit(node.Branches[1]);
        if (rightResult is ErrorResult) return rightResult;

        object left = UnwrapRaw(leftResult)!;
        object right = UnwrapRaw(rightResult)!;

        if ((left.GetType() != right.GetType()) || (!(left is double) && !(left is long) && !(left is Measure)))
        {
            AddError("numeric or measure values");
            return new VoidResult();
        }

        var emaj = new Equal_Major();
        emaj.Evaluate(left, right);
        return WrapResult(emaj.Value);
    }

    public EvaluationResult VisitEqualMinor(Node node)
    {
        EvaluationResult leftResult = Visit(node.Branches[0]);
        if (leftResult is ErrorResult) return leftResult;
        EvaluationResult rightResult = Visit(node.Branches[1]);
        if (rightResult is ErrorResult) return rightResult;

        object left = UnwrapRaw(leftResult)!;
        object right = UnwrapRaw(rightResult)!;

        if ((left.GetType() != right.GetType()) || (!(left is double) && !(left is long) && !(left is Measure)))
        {
            AddError("numeric or measure values");
            return new VoidResult();
        }

        var emin = new Equal_Minor();
        emin.Evaluate(left, right);
        return WrapResult(emin.Value);
    }

    public EvaluationResult VisitOr(Node node)
    {
        EvaluationResult leftResult = Visit(node.Branches[0]);
        if (leftResult is ErrorResult) return leftResult;
        EvaluationResult rightResult = Visit(node.Branches[1]);
        if (rightResult is ErrorResult) return rightResult;

        object left = UnwrapRaw(leftResult)!;
        object right = UnwrapRaw(rightResult)!;

        if (left is null || right is null)
        {
            AddError("valid values to operate");
            return new VoidResult();
        }

        var or = new Or();
        or.Evaluate(left, right);
        return WrapResult(or.Value);
    }

    public EvaluationResult VisitAnd(Node node)
    {
        EvaluationResult leftResult = Visit(node.Branches[0]);
        if (leftResult is ErrorResult) return leftResult;
        EvaluationResult rightResult = Visit(node.Branches[1]);
        if (rightResult is ErrorResult) return rightResult;

        object left = UnwrapRaw(leftResult)!;
        object right = UnwrapRaw(rightResult)!;

        if (left is null || right is null)
        {
            AddError("valid values to operate");
            return new VoidResult();
        }

        var and = new And();
        and.Evaluate(left, right);
        return WrapResult(and.Value);
    }

    public EvaluationResult VisitEqual(Node node)
    {
        EvaluationResult leftResult = Visit(node.Branches[0]);
        if (leftResult is ErrorResult) return leftResult;
        EvaluationResult rightResult = Visit(node.Branches[1]);
        if (rightResult is ErrorResult) return rightResult;

        object left = UnwrapRaw(leftResult)!;
        object right = UnwrapRaw(rightResult)!;

        if (left is null || right is null)
        {
            AddError("valid values to operate");
            return new VoidResult();
        }

        var eq = new Equal();
        eq.Evaluate(left, right);
        return WrapResult(eq.Value);
    }

    public EvaluationResult VisitDiferent(Node node)
    {
        EvaluationResult leftResult = Visit(node.Branches[0]);
        if (leftResult is ErrorResult) return leftResult;
        EvaluationResult rightResult = Visit(node.Branches[1]);
        if (rightResult is ErrorResult) return rightResult;

        object left = UnwrapRaw(leftResult)!;
        object right = UnwrapRaw(rightResult)!;

        if (left is null || right is null)
        {
            AddError("valid values to operate");
            return new VoidResult();
        }

        var dif = new Diferent();
        dif.Evaluate(left, right);
        return WrapResult(dif.Value);
    }

    public EvaluationResult VisitSum(Node node)
    {
        EvaluationResult leftResult = Visit(node.Branches[0]);
        if (leftResult is ErrorResult) return leftResult;
        EvaluationResult rightResult = Visit(node.Branches[1]);
        if (rightResult is ErrorResult) return rightResult;

        object left = UnwrapRaw(leftResult)!;
        object right = UnwrapRaw(rightResult)!;

        if (left is string s1 && s1 == "undefined" && right is AbsSequence)
            return new StringResult("undefined");

        if ((left is AbsSequence || left is Enclosed_Infinite_Sequence || left is Finite_Sequence<object> ||
             left is Finite_Sequence<Point> || left is Infinite_Sequence || left is InfinitePointSequence ||
             left is InfiniteDoubleSequence) && right is string s2 && s2 == "undefined")
        {
            var sum = new Sum();
            sum.Evaluate(left, right);
            return WrapResult(sum.Value);
        }

        if (left is AbsSequence && right is AbsSequence)
        {
            var sum = new Sum();
            sum.Evaluate(left, right);
            return WrapResult(sum.Value);
        }

        if (left.GetType() != right.GetType() ||
            (!(left is double) && !(left is long) && !(left is string) && !(left is Measure) &&
             !(left is Finite_Sequence<object>) && !(left is Finite_Sequence<Point>) &&
             !(left is Enclosed_Infinite_Sequence) && !(left is Infinite_Sequence) &&
             !(left is InfiniteDoubleSequence) && !(left is InfinitePointSequence)))
        {
            AddError("valid values to operate");
            return new VoidResult();
        }

        var sumOp = new Sum();
        sumOp.Evaluate(left, right);
        return WrapResult(sumOp.Value);
    }

    public EvaluationResult VisitSub(Node node)
    {
        EvaluationResult leftResult = Visit(node.Branches[0]);
        if (leftResult is ErrorResult) return leftResult;
        EvaluationResult rightResult = Visit(node.Branches[1]);
        if (rightResult is ErrorResult) return rightResult;

        object left = UnwrapRaw(leftResult)!;
        object right = UnwrapRaw(rightResult)!;

        if ((left.GetType() != right.GetType()) || (!(left is double) && !(left is long) && !(left is Measure)))
        {
            AddError("valid values to operate");
            return new VoidResult();
        }

        var sub = new Substraction();
        sub.Evaluate(left, right);
        return WrapResult(sub.Value);
    }

    public EvaluationResult VisitMul(Node node)
    {
        EvaluationResult leftResult = Visit(node.Branches[0]);
        if (leftResult is ErrorResult) return leftResult;
        EvaluationResult rightResult = Visit(node.Branches[1]);
        if (rightResult is ErrorResult) return rightResult;

        object left = UnwrapRaw(leftResult)!;
        object right = UnwrapRaw(rightResult)!;

        if (!(left is double && right is Measure) && !(left is long && right is Measure) &&
            !(left is Measure && right is double) && !(left is Measure && right is long) &&
            !(left is double && right is double) && !(left is long && right is long))
        {
            AddError("valid values to operate");
            return new VoidResult();
        }

        var mul = new Multiplication();
        mul.Evaluate(left, right);
        return WrapResult(mul.Value);
    }

    public EvaluationResult VisitDiv(Node node)
    {
        EvaluationResult leftResult = Visit(node.Branches[0]);
        if (leftResult is ErrorResult) return leftResult;
        EvaluationResult rightResult = Visit(node.Branches[1]);
        if (rightResult is ErrorResult) return rightResult;

        object left = UnwrapRaw(leftResult)!;
        object right = UnwrapRaw(rightResult)!;

        if ((left.GetType() != right.GetType()) || (!(left is double) && !(left is long) && !(left is Measure)))
        {
            AddError("valid values to operate");
            return new VoidResult();
        }

        if ((right is double rd && rd == 0) || (right is Measure rm && rm.Value == 0))
        {
            AddError("operation,can't divide by zero");
            return WrapResult(left);
        }

        try
        {
            var div = new Division();
            div.Evaluate(left, right);
            return WrapResult(div.Value);
        }
        catch (DivideByZeroException)
        {
            AddError("operation,can't divide by zero");
            return WrapResult(left);
        }
    }

    public EvaluationResult VisitModule(Node node)
    {
        EvaluationResult leftResult = Visit(node.Branches[0]);
        if (leftResult is ErrorResult) return leftResult;
        EvaluationResult rightResult = Visit(node.Branches[1]);
        if (rightResult is ErrorResult) return rightResult;

        object left = UnwrapRaw(leftResult)!;
        object right = UnwrapRaw(rightResult)!;

        if ((!(left is double) && !(left is long)) || (!(right is double) && !(right is long)))
        {
            AddError("numerical values");
            return new VoidResult();
        }

        var mod = new Module();
        mod.Evaluate(left, right);
        return WrapResult(mod.Value);
    }

    public EvaluationResult VisitPow(Node node)
    {
        EvaluationResult leftResult = Visit(node.Branches[0]);
        if (leftResult is ErrorResult) return leftResult;
        EvaluationResult rightResult = Visit(node.Branches[1]);
        if (rightResult is ErrorResult) return rightResult;

        object left = UnwrapRaw(leftResult)!;
        object right = UnwrapRaw(rightResult)!;

        if ((!(left is double) && !(left is long)) || (!(right is double) && !(right is long)))
        {
            AddError("numerical values");
            return new VoidResult();
        }

        var pow = new Power();
        pow.Evaluate(left, right);
        return WrapResult(pow.Value);
    }
    public EvaluationResult VisitVar(Node node)
    {
        string name = node.NodeExpression!.ToString()!;
        // Scope variables take precedence over global constants: a let-body or a
        // function parameter may shadow a global. Checking globals first made any
        // shadowing impossible (moot before T2 because let-in never parsed).
        if (_currentScope.Variables.ContainsKey(name))
            return WrapResult(_currentScope.Variables[name]);
        if (_context.GlobalConstant.ContainsKey(name))
            return WrapResult(_context.GlobalConstant[name]);
        _semanticErrors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid,
            "variable", new Location(_file, _line, "column")));
        return new VoidResult();
    }

    public EvaluationResult VisitGlobalVar(Node node)
    {
        string name = RawString(Visit(node.Branches[0]));
        object value = UnwrapRaw(Visit(node.Branches[1]))!;
        StoreVariable(name, value);
        return new StringResult("global constant has been added");
    }

    public EvaluationResult VisitDraw(Node node)
    {
        EvaluationResult valResult = Visit(node.Branches[0]);
        if (valResult is ErrorResult) return valResult;
        object value = UnwrapRaw(valResult)!;
        string tag = " ";
        if (node.Branches[1].Type != Node.NodeType.Indefined)
            tag = RawString(Visit(node.Branches[1]));
        var d = new DrawObject(value, tag, _scene.UtilizedColors.Peek(), _scene.CurrentLineStyle, _scene.CurrentStrokeWidth, _scene.CurrentFillType, _scene.CurrentGradientColor1, _scene.CurrentGradientColor2, _scene.CurrentLayer);
        if (!d.CheckValidType())
        {
            _semanticErrors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid,
                "type,this type of object can't be draw", new Location(_file, _line, "column")));
        }
        else
            _scene.Add(d);
        return new StringResult("Function to draw added");
    }

    public EvaluationResult VisitColor(Node node)
    {
        string color = node.NodeExpression!.ToString()!;
        string resolved = ColorTable.TryGetHex(color, out var hex) ? hex : color;
        _scene.PushColor(resolved);
        return new StringResult($"Color changed to {resolved}");
    }

    public EvaluationResult VisitRestore(Node node)
    {
        _scene.RestoreColor();
        return new StringResult($"Used color has been restore to {_scene.CurrentColor}");
    }

    public EvaluationResult VisitSin(Node node)
    {
        EvaluationResult argResult = Visit(node.Branches[0]);
        if (argResult is ErrorResult) return argResult;
        object arg = UnwrapRaw(argResult)!;
        if (!(arg is double) && !(arg is long))
        {
            AddError("numerical values");
            return new VoidResult();
        }
        return new NumberResult(_context.Trig_functions["sin"](Convert.ToDouble(arg)));
    }

    public EvaluationResult VisitCos(Node node)
    {
        EvaluationResult argResult = Visit(node.Branches[0]);
        if (argResult is ErrorResult) return argResult;
        object arg = UnwrapRaw(argResult)!;
        if (!(arg is double) && !(arg is long))
        {
            AddError("numerical values");
            return new VoidResult();
        }
        return new NumberResult(_context.Trig_functions["cos"](Convert.ToDouble(arg)));
    }

    public EvaluationResult VisitSqrt(Node node)
    {
        EvaluationResult argResult = Visit(node.Branches[0]);
        if (argResult is ErrorResult) return argResult;
        object arg = UnwrapRaw(argResult)!;
        if (!(arg is double) && !(arg is long))
        {
            AddError("numerical values");
            return new VoidResult();
        }
        return new NumberResult(_context.Trig_functions["sqrt"](Convert.ToDouble(arg)));
    }

    public EvaluationResult VisitLog(Node node)
    {
        EvaluationResult baseResult = Visit(node.Branches[0]);
        if (baseResult is ErrorResult) return baseResult;
        EvaluationResult argResult = Visit(node.Branches[1]);
        if (argResult is ErrorResult) return argResult;
        object baseOf = UnwrapRaw(baseResult)!;
        object arg = UnwrapRaw(argResult)!;
        if ((!(arg is double) && !(arg is long)) || (!(baseOf is double) && !(baseOf is long)))
        {
            AddError("numerical values");
            return new VoidResult();
        }
        return new NumberResult(_context.Log["log"](Convert.ToDouble(baseOf), Convert.ToDouble(arg)));
    }

    public EvaluationResult VisitCount(Node node)
    {
        EvaluationResult argResult = Visit(node.Branches[0]);
        if (argResult is ErrorResult) return argResult;
        object arg = UnwrapRaw(argResult)!;

        // Try common sequence types for count
        switch (arg)
        {
            case Finite_Sequence<Point> fsp:
            {
                long c = fsp.count;
                if (c < 0) return new StringResult("undefined");
                return new NumberResult(c);
            }
            case Finite_Sequence<object> fso:
            {
                long c = fso.count;
                if (c < 0) return new StringResult("undefined");
                return new NumberResult(c);
            }
            case GenericSequence<object> gs:
            {
                long c = gs.count;
                if (c < 0) return new StringResult("undefined");
                return new NumberResult(c);
            }
            case Infinite_Sequence inf:
            {
                long c = inf.count;
                if (c < 0) return new StringResult("undefined");
                return new NumberResult(c);
            }
            case InfinitePointSequence ips:
            {
                long c = ips.count;
                if (c < 0) return new StringResult("undefined");
                return new NumberResult(c);
            }
            case InfiniteDoubleSequence ids:
            {
                long c = ids.count;
                if (c < 0) return new StringResult("undefined");
                return new NumberResult(c);
            }
            case Enclosed_Infinite_Sequence eis:
            {
                long c = eis.count;
                if (c < 0) return new StringResult("undefined");
                return new NumberResult(c);
            }
        }
        _semanticErrors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid,
            "argument,can't count this type", new Location(_file, _line, "column")));
        return new VoidResult();
    }

    public EvaluationResult VisitArc(Node node)
    {
        var results = new EvaluationResult[node.Branches.Count];
        for (int i = 0; i < node.Branches.Count; i++)
        {
            results[i] = Visit(node.Branches[i]);
            if (results[i] is ErrorResult) return results[i];
        }
        object p1 = UnwrapRaw(results[0])!, p2 = UnwrapRaw(results[1])!,
               p3 = UnwrapRaw(results[2])!, m = UnwrapRaw(results[3])!;
        if (p1 is not Point || p2 is not Point || p3 is not Point || !IsDistance(m))
        {
            AddError("valid points and distance to declare an arc");
            return new VoidResult();
        }
        double angle = m is Measure meas ? meas.Value : Convert.ToDouble(m);
        return new FigureResult(new Arc((Point)p1, (Point)p2, (Point)p3, angle));
    }

    private static bool IsDistance(object value) =>
        value is Measure || value is double || value is long;

    // Parser never produces plain Measure nodes; only measure(p1,p2).
    public EvaluationResult VisitMeasure(Node node) => new VoidResult();

    public EvaluationResult VisitPointFuc(Node node)
    {
        EvaluationResult xResult = Visit(node.Branches[0]);
        if (xResult is ErrorResult) return xResult;
        EvaluationResult yResult = Visit(node.Branches[1]);
        if (yResult is ErrorResult) return yResult;
        object x = UnwrapRaw(xResult)!;
        object y = UnwrapRaw(yResult)!;
        if ((x is not double && x is not long) || (y is not double && y is not long))
        {
            AddError("valid coordinates for point");
            return new VoidResult();
        }
        Point p = new(Convert.ToDouble(x), Convert.ToDouble(y));
        _figures.TryAddExistingPoint(p);
        return new FigureResult(p);
    }

    public EvaluationResult VisitCircleFuc(Node node)
    {
        EvaluationResult centerResult = Visit(node.Branches[0]);
        if (centerResult is ErrorResult) return centerResult;
        EvaluationResult radioResult = Visit(node.Branches[1]);
        if (radioResult is ErrorResult) return radioResult;
        object center = UnwrapRaw(centerResult)!;
        object radio = UnwrapRaw(radioResult)!;
        if (center is not Point || !IsDistance(radio))
        {
            AddError("a valid center point and distance");
            return new VoidResult();
        }
        double radius = radio is Measure meas ? meas.Value : Convert.ToDouble(radio);
        Circle c = new((Point)center, radius);
        _figures.TryAddExistingCircle(c);
        return new FigureResult(c);
    }

    public EvaluationResult VisitPolygonFuc(Node node)
    {
        EvaluationResult centerResult = Visit(node.Branches[0]);
        if (centerResult is ErrorResult) return centerResult;
        EvaluationResult radiusResult = Visit(node.Branches[1]);
        if (radiusResult is ErrorResult) return radiusResult;
        EvaluationResult sidesResult = Visit(node.Branches[2]);
        if (sidesResult is ErrorResult) return sidesResult;
        object center = UnwrapRaw(centerResult)!;
        object radius = UnwrapRaw(radiusResult)!;
        object sides = UnwrapRaw(sidesResult)!;
        if (center is not Point || !IsDistance(radius) || !(sides is long || sides is double))
        {
            AddError("a valid center point, distance and number of sides");
            return new VoidResult();
        }
        double r = radius is Measure meas ? meas.Value : Convert.ToDouble(radius);
        int n = Convert.ToInt32(sides);
        Polygon poly = new((Point)center, r, n);
        return new FigureResult(poly);
    }

    public EvaluationResult VisitEllipseFuc(Node node)
    {
        EvaluationResult centerResult = Visit(node.Branches[0]);
        if (centerResult is ErrorResult) return centerResult;
        EvaluationResult rxResult = Visit(node.Branches[1]);
        if (rxResult is ErrorResult) return rxResult;
        EvaluationResult ryResult = Visit(node.Branches[2]);
        if (ryResult is ErrorResult) return ryResult;
        object center = UnwrapRaw(centerResult)!;
        object rx = UnwrapRaw(rxResult)!;
        object ry = UnwrapRaw(ryResult)!;
        if (center is not Point || !IsDistance(rx) || !IsDistance(ry))
        {
            AddError("a valid center point and two distances");
            return new VoidResult();
        }
        double rxi = rx is Measure mx ? mx.Value : Convert.ToDouble(rx);
        double ryi = ry is Measure my ? my.Value : Convert.ToDouble(ry);
        Ellipse e = new((Point)center, rxi, ryi);
        return new FigureResult(e);
    }

    public EvaluationResult VisitLineFuc(Node node)
    {
        EvaluationResult r1 = Visit(node.Branches[0]);
        if (r1 is ErrorResult) return r1;
        EvaluationResult r2 = Visit(node.Branches[1]);
        if (r2 is ErrorResult) return r2;
        object p1 = UnwrapRaw(r1)!;
        object p2 = UnwrapRaw(r2)!;
        if (p1 is not Point || p2 is not Point)
        {
            AddError("valid points to declare a line");
            return new VoidResult();
        }
        Line l = new((Point)p1, (Point)p2);
        _figures.TryAddExistingLine(l);
        return new FigureResult(l);
    }

    public EvaluationResult VisitSegmentFuc(Node node)
    {
        EvaluationResult r1 = Visit(node.Branches[0]);
        if (r1 is ErrorResult) return r1;
        EvaluationResult r2 = Visit(node.Branches[1]);
        if (r2 is ErrorResult) return r2;
        object p1 = UnwrapRaw(r1)!;
        object p2 = UnwrapRaw(r2)!;
        if (p1 is not Point || p2 is not Point)
        {
            AddError("valid points to declare a segment");
            return new VoidResult();
        }
        Segment s = new((Point)p1, (Point)p2);
        _figures.TryAddExistingSegment(s);
        return new FigureResult(s);
    }

    public EvaluationResult VisitRayFuc(Node node)
    {
        EvaluationResult r1 = Visit(node.Branches[0]);
        if (r1 is ErrorResult) return r1;
        EvaluationResult r2 = Visit(node.Branches[1]);
        if (r2 is ErrorResult) return r2;
        object p1 = UnwrapRaw(r1)!;
        object p2 = UnwrapRaw(r2)!;
        if (p1 is not Point || p2 is not Point)
        {
            AddError("valid points to declare a ray");
            return new VoidResult();
        }
        Ray ray = new((Point)p1, (Point)p2);
        _figures.TryAddExistingRay(ray);
        return new FigureResult(ray);
    }

    public EvaluationResult VisitMeasureFuc(Node node)
    {
        EvaluationResult r1 = Visit(node.Branches[0]);
        if (r1 is ErrorResult) return r1;
        EvaluationResult r2 = Visit(node.Branches[1]);
        if (r2 is ErrorResult) return r2;
        object p1 = UnwrapRaw(r1)!;
        object p2 = UnwrapRaw(r2)!;
        if (p1 is not Point || p2 is not Point)
        {
            AddError("valid points to declare a measure");
            return new VoidResult();
        }
        // Legacy returned raw Measure; we expose its numeric value because the
        // sealed result hierarchy has no Measure variant yet.
        return new NumberResult(new Measure((Point)p1, (Point)p2).Value);
    }
    public EvaluationResult VisitPointSeq(Node node)
    {
        List<Point> elements = new();
        Random r = RandomProvider.Instance;
        int amount = r.Next(1, 30);
        for (int i = 0; i < amount; i++)
        {
            Point temp = new(0, 0);
            temp.RandomPoint(_figures.ExistingPoints);
            elements.Add(temp);
            _figures.TryAddExistingPoint(temp);
        }
        Finite_Sequence<Point> pts = new(elements);
        pts.type = Finite_Sequence<Point>.SeqType.point;
        StoreVariable(node.NodeExpression!.ToString()!, pts);
        return new StringResult("sequence of points created");
    }

    public EvaluationResult VisitLineSeq(Node node)
    {
        List<Line> elements = new();
        Random r = RandomProvider.Instance;
        int amount = r.Next(1, 30);
        for (int i = 0; i < amount; i++)
        {
            Line l = new(new Point(0, 0), new Point(0, 1));
            l.RandomLine(_figures.ExistingLines, _figures.ExistingPoints);
            elements.Add(l);
            _figures.TryAddExistingLine(l);
        }
        Finite_Sequence<Line> pts = new(elements);
        pts.type = Finite_Sequence<Line>.SeqType.line;
        StoreVariable(node.NodeExpression!.ToString()!, pts);
        return new StringResult("sequence of lines created");
    }

    public EvaluationResult VisitIntersect(Node node)
    {
        EvaluationResult r1 = Visit(node.Branches[0]);
        if (r1 is ErrorResult) return r1;
        EvaluationResult r2 = Visit(node.Branches[1]);
        if (r2 is ErrorResult) return r2;
        object f1 = UnwrapRaw(r1)!;
        object f2 = UnwrapRaw(r2)!;

        if (f2 is not Figure || f1 is not Figure)
        {
            AddError("figure");
            return new VoidResult();
        }

        Finite_Sequence<Point> result = ((Figure)f1).Intersect((Figure)f2);
        if (result is null)
            return new StringResult("undefined");
        return new SequenceResult(result, result.count);
    }

    // Import wiring (T3): the pipeline layer injects a handler that resolves and
    // evaluates library files sharing this visitor/context. Domain cannot reach
    // Infrastructure directly. When no handler is set, imports report a semantic
    // error (same as legacy until the UI wires a source).
    public Func<string, EvaluationResult?>? ImportHandler { get; set; }

    public EvaluationResult VisitImport(Node node)
    {
        if (ImportHandler is null)
        {
            AddError("import requires UI/Infrastructure layer");
            return new VoidResult();
        }
        // The text token keeps its quotes: "name.geo"
        string name = node.NodeExpression!.ToString()!.Trim('"');
        if (name.EndsWith(".geo")) name = name[..^4];
        EvaluationResult? result = ImportHandler(name);
        return result ?? new VoidResult();
    }
    public EvaluationResult VisitEmptySeq(Node node)
    {
        Finite_Sequence<object> seq = new(new List<object>());
        return new SequenceResult(seq, seq.count);
    }

    public EvaluationResult VisitInfiniteSeq(Node node)
    {
        EvaluationResult valueResult = Visit(node.Branches[0]);
        if (valueResult is ErrorResult) return valueResult;
        object value = UnwrapRaw(valueResult)!;
        // Legacy required a long; the lexer only produces doubles, so integral
        // doubles are accepted (deliberate fix of a latent legacy bug).
        long start;
        if (value is long l) start = l;
        else if (value is double d && d % 1 == 0) start = Convert.ToInt64(d);
        else
        {
            AddError("argument");
            return new VoidResult();
        }
        Infinite_Sequence seq = new(start);
        return new SequenceResult(seq, seq.count);
    }

    public EvaluationResult VisitEnclosedInfiniteSeq(Node node)
    {
        EvaluationResult firstResult = Visit(node.Branches[0]);
        if (firstResult is ErrorResult) return firstResult;
        EvaluationResult finalResult = Visit(node.Branches[1]);
        if (finalResult is ErrorResult) return finalResult;
        object firstvalue = UnwrapRaw(firstResult)!;
        object finalvalue = UnwrapRaw(finalResult)!;
        if ((firstvalue is double f && f % 1 == 0) && (finalvalue is double c && c % 1 == 0))
        {
            Enclosed_Infinite_Sequence seq = new(Convert.ToInt64(f), Convert.ToInt64(c));
            return new SequenceResult(seq, seq.count);
        }
        AddError("boundries");
        return new VoidResult();
    }

    public EvaluationResult VisitFiniteSeq(Node node)
    {
        EvaluationResult firstResult = Visit(node.Branches[0]);
        if (firstResult is ErrorResult) return firstResult;
        object firstvalue = UnwrapRaw(firstResult)!;
        List<object> valuesofseq = new() { firstvalue };
        for (int index = 1; index < node.Branches.Count; index++)
        {
            EvaluationResult itemResult = Visit(node.Branches[index]);
            if (itemResult is ErrorResult) return itemResult;
            object value = UnwrapRaw(itemResult)!;
            if (firstvalue.GetType() != value.GetType())
            {
                AddError("sequence, all values must belong to the same type");
                return new StringResult("Invalid sequence");
            }
            valuesofseq.Add(value);
        }
        Finite_Sequence<object> seq = new(valuesofseq);
        seq.type = ClassifySequenceType(firstvalue);
        return new SequenceResult(seq, seq.count);
    }

    private static Finite_Sequence<object>.SeqType ClassifySequenceType(object first) => first switch
    {
        double => Finite_Sequence<object>.SeqType.number,
        string => Finite_Sequence<object>.SeqType.text,
        Point => Finite_Sequence<object>.SeqType.point,
        Line => Finite_Sequence<object>.SeqType.line,
        Segment => Finite_Sequence<object>.SeqType.segment,
        Ray => Finite_Sequence<object>.SeqType.ray,
        Circle => Finite_Sequence<object>.SeqType.circle,
        Arc => Finite_Sequence<object>.SeqType.arc,
        GenericSequence<object> => Finite_Sequence<object>.SeqType.sequence,
        _ => Finite_Sequence<object>.SeqType.other
    };

    public EvaluationResult VisitRandoms(Node node)
    {
        IEnumerable<double> rand = _context.Randoms["randoms"]();
        InfiniteDoubleSequence randoms = new(rand);
        return new SequenceResult(randoms, randoms.count);
    }

    public EvaluationResult VisitSamples(Node node)
    {
        IEnumerable<Point> sam = _context.Samples["samples"]();
        InfinitePointSequence samples = new(sam);
        return new SequenceResult(samples, samples.count);
    }

    public EvaluationResult VisitPoints(Node node)
    {
        EvaluationResult argResult = Visit(node.Branches[0]);
        if (argResult is ErrorResult) return argResult;
        object arg = UnwrapRaw(argResult)!;
        if (arg is Circle circle)
        {
            IEnumerable<Point> point = _context.Points["points"](circle);
            InfinitePointSequence samples = new(point);
            return new SequenceResult(samples, samples.count);
        }
        AddError("argument");
        return new VoidResult();
    }

    private static EvaluationResult WrapResult(object? result)
    {
        if (result is null) return new VoidResult();
        if (result is EvaluationResult er) return er;
        if (result is string s) return new StringResult(s);
        if (result is double d) return new NumberResult(d);
        if (result is long l) return new NumberResult(l);
        if (result is int i) return new NumberResult(i);
        if (result is Figure f) return new FigureResult(f);
        if (result is AbsSequence seq)
            return new SequenceResult(seq, seq.count);
        if (result is Measure m) return new NumberResult(m.Value);
        return new StringResult(result.ToString()!);
    }

    private static object? UnwrapRaw(EvaluationResult result) => result switch
    {
        NumberResult n => n.Value,
        StringResult s => s.Value,
        FigureResult f => f.Value,
        SequenceResult seq => seq.Value,
        _ => null
    };

    private static string RawString(EvaluationResult result) =>
        result is StringResult s ? s.Value : result.ToString()!;

    private void AddError(string expected)
    {
        _semanticErrors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Expected,
            expected, new Location(_file, _line, "column")));
    }

    private void StoreVariable(string name, object value)
    {
        if (_currentScope.Parent == null)
        {
            if (_context.GlobalConstant.ContainsKey(name))
            {
                _semanticErrors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid,
                    "operation,constants can't be modified", new Location(_file, _line, "column")));
            }
            else _context.GlobalConstant.Add(name, value);
        }
        else
        {
            if (_currentScope.Variables.ContainsKey(name) && !_currentScope.InFunction)
            {
                _semanticErrors.Add(new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid,
                    "operation,constants can't be modified", new Location(_file, _line, "column")));
            }
            else _currentScope.Variables[name] = value;
        }
    }
}
