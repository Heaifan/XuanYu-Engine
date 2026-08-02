namespace XuanYu.WarCore.Identity;

/// <summary>
/// 阵营编号：0 表示默认未命名阵营，正数表示已分配阵营。
/// </summary>
public readonly record struct FactionId
{
    private FactionId(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public bool IsValid => Value >= 0;

    public static FactionId Unnamed { get; } = new(0);

    public static FactionId FromInt(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "阵营编号不能为负数。");
        }

        return new FactionId(value);
    }

    public override string ToString()
    {
        return IsValid ? $"FactionId({Value})" : "FactionId(Invalid)";
    }
}
