namespace XuanYu.World;

public enum WorldEntityType
{
    LegacyMinimalTriangle,
    Cube,
    StaticModel
}

public static class WorldEntityTypes
{
    public const string LegacyMinimalTriangle = nameof(WorldEntityType.LegacyMinimalTriangle);
    public const string Cube = nameof(WorldEntityType.Cube);
    public const string StaticModel = nameof(WorldEntityType.StaticModel);

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
