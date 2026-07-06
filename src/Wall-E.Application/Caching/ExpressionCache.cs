using System.Collections.Concurrent;
using Wall_E.Domain;

namespace Wall_E.Application.Caching;

public class ExpressionCache
{
    private readonly ConcurrentDictionary<string, EvaluationResult> _cache = new();

    public bool TryGet(string source, out EvaluationResult result)
    {
        return _cache.TryGetValue(source, out result);
    }

    public void Set(string source, EvaluationResult result)
    {
        _cache[source] = result;
    }

    public void Invalidate(string source)
    {
        _cache.TryRemove(source, out _);
    }

    public void Clear()
    {
        _cache.Clear();
    }

    public int Count => _cache.Count;
}
