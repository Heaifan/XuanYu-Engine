namespace XuanYu.World.Map;

public readonly record struct MapRoadId
{
    public const int HexLength = 32;
    private MapRoadId(string value) => Value = value;
    public string Value { get; }
    public bool IsValid => Value is not null && Value.Length == HexLength && Value.All(Uri.IsHexDigit);
    public static MapRoadId New() => new(Guid.NewGuid().ToString("N"));
    public static bool TryParse(string? value, out MapRoadId id)
    {
        id = default;
        if (string.IsNullOrWhiteSpace(value)) return false;
        var candidate = new MapRoadId(value.Trim());
        if (!candidate.IsValid) return false;
        id = candidate; return true;
    }
    public override string ToString() => Value ?? "";
}
