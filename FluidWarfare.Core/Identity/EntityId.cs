namespace FluidWarfare.Core.Identity;

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
            throw new ArgumentOutOfRangeException(nameof(value), value, "Entity id must be greater than zero.");
        }

        return new EntityId(value);
    }

    public override string ToString()
    {
        return IsValid ? $"EntityId({Value})" : "EntityId(None)";
    }
}
