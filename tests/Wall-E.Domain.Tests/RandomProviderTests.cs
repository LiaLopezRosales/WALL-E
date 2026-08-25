using Xunit;

namespace Wall_E.Domain.Tests;

public class RandomProviderTests
{
    [Fact]
    public void Seed_produces_deterministic_sequence()
    {
        RandomProvider.Seed(42);
        int a = RandomProvider.Instance.Next(1000);
        RandomProvider.Seed(42);
        int b = RandomProvider.Instance.Next(1000);
        Assert.Equal(a, b);
    }

    [Fact]
    public void Different_seeds_produce_different_values()
    {
        RandomProvider.Seed(1);
        int a = RandomProvider.Instance.Next(100000);
        RandomProvider.Seed(2);
        int b = RandomProvider.Instance.Next(100000);
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Instance_is_not_null()
    {
        Assert.NotNull(RandomProvider.Instance);
    }
}
