namespace XuanYu.World;

public enum WorldEntityType
{
    LegacyMinimalTriangle,
    Cube
}

public static class WorldEntityTypes
{
    public const string LegacyMinimalTriangle = nameof(WorldEntityType.LegacyMinimalTriangle);
    public const string Cube = nameof(WorldEntityType.Cube);

    public static bool TryParse(string value, out WorldEntityType type)
    {
        if (value == "MinimalSceneEntity")
        {
            type = WorldEntityType.LegacyMinimalTriangle;
            return true;
        }
        return Enum.TryParse(value, ignoreCase: false, out type);
    }
}
