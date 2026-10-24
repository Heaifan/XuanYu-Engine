using XuanYu.Core.Identity;

namespace XuanYu.Core.Tests.Foundation.Identity;

public sealed class EntityIdTests
{
    [Fact]
    public void DefaultAndNoneAreInvalid()
    {
        var id = default(EntityId);

        Assert.False(id.IsValid);
        Assert.False(EntityId.None.IsValid);
        Assert.Equal(0, id.Value);
        Assert.Equal(id, EntityId.None);
        Assert.Equal("EntityId(None)", id.ToString());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(int.MaxValue)]
    public void PositiveIntegerCreatesValidId(int value)
    {
        var id = EntityId.FromInt(value);

        Assert.True(id.IsValid);
        Assert.Equal(value, id.Value);
        Assert.Equal($"EntityId({value})", id.ToString());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void NonPositiveIntegerIsRejected(int value)
    {
        var error = Assert.Throws<ArgumentOutOfRangeException>(
            () => EntityId.FromInt(value));

        Assert.Equal("value", error.ParamName);
    }

    [Fact]
    public void SameValueHasValueEquality()
    {
        var first = EntityId.FromInt(7);
        var second = EntityId.FromInt(7);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.True(first == second);
    }

    [Fact]
    public void DifferentValuesRemainDistinct()
    {
        var first = EntityId.FromInt(7);
        var second = EntityId.FromInt(8);

        Assert.NotEqual(first, second);
        Assert.NotEqual(EntityId.None, first);
    }
}
