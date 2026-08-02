namespace XuanYu.WarCore.Identity;

/// <summary>
/// 军事身份：单位编号、显示名称与单位类型。
/// 构造时校验编号有效与显示名称非空；非法输入得到明确错误。
/// </summary>
public sealed record MilitaryIdentity
{
    public MilitaryIdentity(UnitId unitId, string displayName, UnitKind kind)
    {
        if (!unitId.IsValid)
        {
            throw new ArgumentException("单位编号无效。", nameof(unitId));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("显示名称不能为空。", nameof(displayName));
        }

        UnitId = unitId;
        DisplayName = displayName;
        Kind = kind;
    }

    public UnitId UnitId { get; }

    public string DisplayName { get; }

    public UnitKind Kind { get; }

    public static MilitaryIdentity NewSoldier(UnitId unitId, string displayName)
    {
        return new MilitaryIdentity(unitId, displayName, UnitKind.Soldier);
    }
}
