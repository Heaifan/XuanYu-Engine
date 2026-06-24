using XuanYu.Engine.Core.Identity;

namespace FluidWarfare.Tests.Core.Identity;

public sealed class EntityIdTests
{
    [Fact]
    public void None_ShouldBeInvalid()
    {
        Assert.False(EntityId.None.IsValid);
    }

    [Fact]
    public void None_ShouldHaveZeroValue()
    {
        Assert.Equal(0, EntityId.None.Value);
    }

    [Fact]
    public void FromInt_WithPositiveValue_ShouldCreateValidId()
    {
        var id = EntityId.FromInt(1);

        Assert.True(id.IsValid);
        Assert.Equal(1, id.Value);
    }

    [Fact]
    public void FromInt_WithZero_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => EntityId.FromInt(0));
    }

    [Fact]
    public void FromInt_WithNegativeValue_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => EntityId.FromInt(-1));
    }

    [Fact]
    public void SameValue_ShouldBeEqual()
    {
        Assert.Equal(EntityId.FromInt(42), EntityId.FromInt(42));
    }

    [Fact]
    public void DifferentValue_ShouldNotBeEqual()
    {
        Assert.NotEqual(EntityId.FromInt(1), EntityId.FromInt(2));
    }

    [Fact]
    public void ToString_ForNone_ShouldBeStable()
    {
        Assert.Equal("EntityId(None)", EntityId.None.ToString());
    }

    [Fact]
    public void ToString_ForValidId_ShouldBeStable()
    {
        Assert.Equal("EntityId(42)", EntityId.FromInt(42).ToString());
    }
}
