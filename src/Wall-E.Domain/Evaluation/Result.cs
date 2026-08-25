namespace Wall_E.Domain;

/// <summary>Functional Result monad that holds either a success value or an error.</summary>
public readonly struct Result<T, E>
{
    private readonly T? _value;
    private readonly E? _error;
    private readonly bool _isSuccess;

    public bool IsSuccess => _isSuccess;
    public bool IsError => !_isSuccess;
    public T Value => _isSuccess ? _value! : throw new InvalidOperationException("Cannot access Value of an Error result");
    public E Error => !_isSuccess ? _error! : throw new InvalidOperationException("Cannot access Error of a Success result");

    private Result(T value)
    {
        _value = value;
        _error = default;
        _isSuccess = true;
    }

    private Result(E error)
    {
        _value = default;
        _error = error;
        _isSuccess = false;
    }

    /// <summary>Creates a successful result wrapping the given value.</summary>
    public static Result<T, E> Ok(T value) => new(value);

    /// <summary>Creates a failed result wrapping the given error.</summary>
    public static Result<T, E> Fail(E error) => new(error);

    /// <summary>Transforms the success value using the given function, propagating errors unchanged.</summary>
    public Result<TNext, E> Map<TNext>(Func<T, TNext> map) =>
        _isSuccess ? Result<TNext, E>.Ok(map(_value!)) : Result<TNext, E>.Fail(_error!);

    /// <summary>Chains a computation that produces a new result from the success value.</summary>
    public Result<TNext, E> Bind<TNext>(Func<T, Result<TNext, E>> bind) =>
        _isSuccess ? bind(_value!) : Result<TNext, E>.Fail(_error!);

    /// <summary>Returns the success value or the given fallback if the result is an error.</summary>
    public T ValueOr(T fallback) => _isSuccess ? _value! : fallback;

    public void Deconstruct(out bool success, out T? value, out E? error)
    {
        success = _isSuccess;
        value = _value;
        error = _error;
    }
}
