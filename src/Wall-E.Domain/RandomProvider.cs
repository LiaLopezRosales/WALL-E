namespace Wall_E.Domain;
using System;

/// <summary>Thread-safe singleton source of Random instances for deterministic or default RNG.</summary>
public static class RandomProvider
{
    private static readonly ThreadLocal<Random> _random = new(() => new Random());

    /// <summary>Gets the thread-local Random instance.</summary>
    public static Random Instance => _random.Value!;

    /// <summary>Reseeds the current thread's Random instance for reproducible sequences.</summary>
    public static void Seed(int seed)
    {
        _random.Value = new Random(seed);
    }
}
