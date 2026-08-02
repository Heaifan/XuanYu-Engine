namespace XuanYu.WarCore.Identity;

/// <summary>
/// 单位编号：WarCore 领域的军事编号（如 S-0001），
/// 独立于 World 的 EntityId，两者通过注册表关联。
/// </summary>
public readonly record struct UnitId
{
    private UnitId(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public bool IsValid => Value > 0;

    public static UnitId None { get; } = new(0);

    public static UnitId FromInt(int value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "单位编号必须大于 0。");
        }

        return new UnitId(value);
    }

    public override string ToString()
    {
        return IsValid ? $"UnitId({Value})" : "UnitId(None)";
    }
}
