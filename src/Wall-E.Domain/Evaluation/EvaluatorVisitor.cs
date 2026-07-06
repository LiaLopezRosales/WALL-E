using System.Threading;
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

    // Fallback to adapted Evaluator for all remaining types
    public EvaluationResult VisitInstructions(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitGlobalVar(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitGlobalSeq(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitAssigment(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitLetExp(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitDraw(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitConditional(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitIf(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitElse(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitDeclaredFuc(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitNegation(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitVar(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitParameters(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitFuction(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitConcat(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitAnd(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitOr(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitMinor(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitMajor(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitEqualMinor(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitEqualMajor(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitEqual(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitDiferent(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitSum(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitSub(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitMul(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitDiv(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitModule(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitPow(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitArc(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitPointSeq(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitLineSeq(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitColor(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitRestore(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitImport(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitPointFuc(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitLineFuc(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitSegmentFuc(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitRayFuc(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitCircleFuc(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitMeasure(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitMeasureFuc(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitIntersect(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitCount(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitCos(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitSin(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitLog(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitSqrt(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitPoints(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitRandoms(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitSamples(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitEmptySeq(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitEnclosedInfiniteSeq(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitInfiniteSeq(Node node) => EvaluateFallback(node);
    public EvaluationResult VisitFiniteSeq(Node node) => EvaluateFallback(node);

    private EvaluationResult EvaluateFallback(Node node)
    {
        var evaluator = new Evaluator(_context, _figures, _scene, _file);
        evaluator.line = _line;
        evaluator.CurrentScope = _currentScope;
        evaluator.CancellationToken = CancellationToken;
        object result = evaluator.GeneralEvaluation(node);
        _semanticErrors.AddRange(evaluator.Semantic_Errors);
        _currentScope = evaluator.CurrentScope;
        _context = evaluator._context;
        _figures = evaluator._figures;
        _scene = evaluator._scene;
        return WrapResult(result);
    }

    private static EvaluationResult WrapResult(object? result)
    {
        if (result is null) return new VoidResult();
        if (result is string s) return new StringResult(s);
        if (result is double d) return new NumberResult(d);
        if (result is long l) return new NumberResult(l);
        if (result is int i) return new NumberResult(i);
        if (result is Figure f) return new FigureResult(f);
        if (result is EvaluationResult er) return er;
        return new StringResult(result.ToString()!);
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
