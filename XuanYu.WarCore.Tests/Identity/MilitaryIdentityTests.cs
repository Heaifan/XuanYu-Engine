using XuanYu.WarCore.Identity;

namespace XuanYu.WarCore.Tests.Identity;

// WARCORE-A-R1-D1：身份生成与校验契约测试。
public sealed class MilitaryIdentityTests
{
    [Fact]
    public void NewSoldier_creates_legal_identity()
    {
        var identity = MilitaryIdentity.NewSoldier(UnitId.FromInt(1), "士兵 S-0001");

        Assert.True(identity.UnitId.IsValid);
        Assert.Equal(1, identity.UnitId.Value);
        Assert.Equal("士兵 S-0001", identity.DisplayName);
        Assert.Equal(UnitKind.Soldier, identity.Kind);
    }

    [Fact]
    public void UnitId_FromInt_rejects_zero()
    {
        var error = Assert.Throws<ArgumentOutOfRangeException>(() => UnitId.FromInt(0));

        Assert.Contains("必须大于 0", error.Message);
    }

    [Fact]
    public void UnitId_FromInt_rejects_negative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => UnitId.FromInt(-1));
    }

    [Fact]
    public void UnitId_None_is_invalid()
    {
        Assert.False(UnitId.None.IsValid);
    }

    [Fact]
    public void Identity_rejects_invalid_unit_id()
    {
        var error = Assert.Throws<ArgumentException>(
            () => new MilitaryIdentity(UnitId.None, "士兵 S-0001", UnitKind.Soldier));

        Assert.Contains("单位编号无效", error.Message);
    }

    [Fact]
    public void Identity_rejects_empty_display_name()
    {
        var error = Assert.Throws<ArgumentException>(
            () => MilitaryIdentity.NewSoldier(UnitId.FromInt(1), "  "));

        Assert.Contains("显示名称不能为空", error.Message);
    }

    [Fact]
    public void Two_identities_do_not_share_state()
    {
        var first = MilitaryIdentity.NewSoldier(UnitId.FromInt(1), "士兵 S-0001");
        var second = MilitaryIdentity.NewSoldier(UnitId.FromInt(2), "士兵 S-0002");

        Assert.NotEqual(first.UnitId, second.UnitId);
        Assert.NotEqual(first.DisplayName, second.DisplayName);
    }
}
