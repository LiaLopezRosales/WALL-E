namespace Wall_E.Domain;
using System;

public static class RandomProvider
{
    private static readonly ThreadLocal<Random> _random = new(() => new Random());

    public static Random Instance => _random.Value!;

    public static void Seed(int seed)
    {
        _random.Value = new Random(seed);
    }
}
