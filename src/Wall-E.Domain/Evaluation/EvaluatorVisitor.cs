namespace Wall_E.Domain;

public class EvaluatorVisitor : INodeVisitor<EvaluationResult>
{
    private readonly EvaluationContext _context;
    private readonly FigureRepository _figures;
    private readonly RenderScene _scene;
    private readonly Scope _rootScope;
    private Scope _currentScope;
    private readonly string _file;
    private string _line = "0";
    private readonly List<Error> _semanticErrors = new();

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
        Node.NodeType.Measure => VisitMeasure(node),
        Node.NodeType.Measure_Fuc => VisitMeasureFuc(node),
        Node.NodeType.Intersect => VisitIntersect(node),
        Node.NodeType.Count => VisitCount(node),
        Node.NodeType.Text => VisitText(node),
        Node.NodeType.Cos => VisitCos(node),
        Node.NodeType.Sin => VisitSin(node),
        Node.NodeType.Log => VisitLog(node),
        Node.NodeType.Sqrt => VisitSqrt(node),
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
        l.RandomLine(_figures.ExistingPoints, _figures.ExistingLines);
        _figures.TryAddExistingLine(l);
        StoreVariable(node.NodeExpression!.ToString()!, l);
        return new StringResult("line created");
    }

    public EvaluationResult VisitSegment(Node node)
    {
        Segment s = new(new Point(0, 0), new Point(1, 1));
        s.RandomSegment(_figures.ExistingPoints, _figures.ExistingSegments);
        _figures.TryAddExistingSegment(s);
        StoreVariable(node.NodeExpression!.ToString()!, s);
        return new StringResult("segment created");
    }

    public EvaluationResult VisitRay(Node node)
    {
        Ray r = new(new Point(0, 0), new Point(1, 1));
        r.RandomRay(_figures.ExistingPoints, _figures.ExistingRays);
        _figures.TryAddExistingRay(r);
        StoreVariable(node.NodeExpression!.ToString()!, r);
        return new StringResult("ray created");
    }

    // Implemented simple nodes
    public EvaluationResult VisitNumber(Node node) => new NumberResult(double.Parse(node.NodeExpression!.ToString()!));
    public EvaluationResult VisitVarName(Node node) => new StringResult(node.NodeExpression!.ToString()!);
    public EvaluationResult VisitFucName(Node node) => new StringResult(node.NodeExpression!.ToString()!);
    public EvaluationResult VisitDeclaredFucName(Node node) => new StringResult(node.NodeExpression!.ToString()!);
    public EvaluationResult VisitParName(Node node) => new StringResult(node.NodeExpression!.ToString()!);
    public EvaluationResult VisitText(Node node) => new StringResult(node.NodeExpression!.ToString()!);
    public EvaluationResult VisitLowHyphen(Node node) => new StringResult(node.NodeExpression!.ToString()!);
    public EvaluationResult VisitPI(Node node) => new NumberResult(Math.PI);
    public EvaluationResult VisitE(Node node) => new NumberResult(Math.E);
    public EvaluationResult VisitIndefined(Node node) => new StringResult("undefined");
    public EvaluationResult VisitUndefined(Node node) => new StringResult("undefined");

    // Unimplemented - throw by default
    public EvaluationResult VisitInstructions(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitGlobalVar(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitGlobalSeq(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitAssigment(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitLetExp(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitDraw(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitConditional(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitIf(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitElse(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitDeclaredFuc(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitNegation(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitVar(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitParameters(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitFuction(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitConcat(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitAnd(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitOr(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitMinor(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitMajor(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitEqualMinor(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitEqualMajor(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitEqual(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitDiferent(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitSum(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitSub(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitMul(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitDiv(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitModule(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitPow(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitArc(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitPointSeq(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitLineSeq(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitColor(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitRestore(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitImport(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitPointFuc(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitLineFuc(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitSegmentFuc(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitRayFuc(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitCircleFuc(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitMeasure(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitMeasureFuc(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitIntersect(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitCount(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitCos(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitSin(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitLog(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitSqrt(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitPoints(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitRandoms(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitSamples(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitEmptySeq(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitEnclosedInfiniteSeq(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitInfiniteSeq(Node node) => throw new NotImplementedException();
    public EvaluationResult VisitFiniteSeq(Node node) => throw new NotImplementedException();

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
