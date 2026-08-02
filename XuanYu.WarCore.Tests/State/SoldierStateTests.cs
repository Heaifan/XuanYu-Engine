using XuanYu.WarCore.State;

namespace XuanYu.WarCore.Tests.State;

// WARCORE-A-R1-D1：士兵状态边界与隔离契约测试。
public sealed class SoldierStateTests
{
    [Fact]
    public void Valid_state_keeps_all_values()
    {
        var state = new SoldierState(80, 70, 90, 10);

        Assert.Equal(80, state.BodyCondition);
        Assert.Equal(70, state.Stamina);
        Assert.Equal(90, state.Morale);
        Assert.Equal(10, state.Suppression);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    public void Boundary_values_are_accepted(int value)
    {
        var state = new SoldierState(value, value, value, value);

        Assert.Equal(value, state.BodyCondition);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void BodyCondition_out_of_range_is_rejected(int value)
    {
        var error = Assert.Throws<ArgumentOutOfRangeException>(
            () => new SoldierState(value, 70, 90, 10));

        Assert.Contains("0 到 100", error.Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Stamina_out_of_range_is_rejected(int value)
    {
        var error = Assert.Throws<ArgumentOutOfRangeException>(
            () => new SoldierState(80, value, 90, 10));

        Assert.Contains("0 到 100", error.Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Morale_out_of_range_is_rejected(int value)
    {
        var error = Assert.Throws<ArgumentOutOfRangeException>(
            () => new SoldierState(80, 70, value, 10));

        Assert.Contains("0 到 100", error.Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Suppression_out_of_range_is_rejected(int value)
    {
        var error = Assert.Throws<ArgumentOutOfRangeException>(
            () => new SoldierState(80, 70, 90, value));

        Assert.Contains("0 到 100", error.Message);
    }

    [Fact]
    public void Two_states_do_not_share_values()
    {
        var first = new SoldierState(80, 70, 90, 10);
        var second = new SoldierState(50, 40, 60, 80);

        Assert.NotEqual(first, second);
        Assert.NotEqual(first.Stamina, second.Stamina);
        Assert.NotEqual(first.Suppression, second.Suppression);
    }
}
