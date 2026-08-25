namespace Wall_E.Domain;

/// <summary>Base record for all values produced by the evaluator.</summary>
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

/// <summary>A numeric evaluation result.</summary>
public sealed record NumberResult(double Value) : EvaluationResult;

/// <summary>A string evaluation result.</summary>
public sealed record StringResult(string Value) : EvaluationResult;

/// <summary>A geometric figure evaluation result.</summary>
public sealed record FigureResult(Figure Value) : EvaluationResult;

/// <summary>A sequence evaluation result with its element count.</summary>
public sealed record SequenceResult(object Value, long Count) : EvaluationResult;

/// <summary>An error evaluation result.</summary>
public sealed record ErrorResult(Error Value) : EvaluationResult;

/// <summary>A void evaluation result (no value).</summary>
public sealed record VoidResult : EvaluationResult;
