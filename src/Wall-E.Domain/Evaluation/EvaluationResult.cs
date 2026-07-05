namespace Wall_E.Domain;

public abstract record EvaluationResult
{
    public static implicit operator EvaluationResult(string value) => new StringResult(value);
    public static implicit operator EvaluationResult(double value) => new NumberResult(value);
    public static implicit operator EvaluationResult(long value) => new NumberResult(value);
    public static implicit operator EvaluationResult(int value) => new NumberResult(value);
    public static implicit operator EvaluationResult(Point value) => new FigureResult(value);
    public static implicit operator EvaluationResult(Circle value) => new FigureResult(value);
    public static implicit operator EvaluationResult(Line value) => new FigureResult(value);
    public static implicit operator EvaluationResult(Segment value) => new FigureResult(value);
    public static implicit operator EvaluationResult(Ray value) => new FigureResult(value);
    public static implicit operator EvaluationResult(Arc value) => new FigureResult(value);
}

public sealed record NumberResult(double Value) : EvaluationResult;
public sealed record StringResult(string Value) : EvaluationResult;
public sealed record FigureResult(Figure Value) : EvaluationResult;
public sealed record SequenceResult(object Value, long Count) : EvaluationResult;
public sealed record ErrorResult(Error Value) : EvaluationResult;
public sealed record VoidResult : EvaluationResult;
