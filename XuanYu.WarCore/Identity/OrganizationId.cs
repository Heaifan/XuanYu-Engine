namespace XuanYu.WarCore.Identity;

/// <summary>
/// 组织编号：0 表示默认未编组，正数表示已分配编制。
/// </summary>
public readonly record struct OrganizationId
{
    private OrganizationId(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public bool IsValid => Value >= 0;

    public static OrganizationId Unassigned { get; } = new(0);

    public static OrganizationId FromInt(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "组织编号不能为负数。");
        }

        return new OrganizationId(value);
    }

    public override string ToString()
    {
        return IsValid ? $"OrganizationId({Value})" : "OrganizationId(Invalid)";
    }
}
