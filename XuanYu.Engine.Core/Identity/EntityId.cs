namespace XuanYu.Engine.Core.Identity;

public readonly record struct EntityId
{
    private EntityId(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public bool IsValid => Value > 0;

    public static EntityId None { get; } = new(0);

    public static EntityId FromInt(int value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "实体编号必须大于 0。");
        }

        return new EntityId(value);
    }

    public override string ToString()
    {
        return IsValid ? $"EntityId({Value})" : "EntityId(None)";
    }
}
