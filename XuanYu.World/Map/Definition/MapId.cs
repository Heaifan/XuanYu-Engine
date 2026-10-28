namespace XuanYu.World.Map;

// MAP-A-R2-D1/D1-F1：地图稳定唯一标识（领域权威层）。D1 合同冻结格式：32 位十六进制，无前缀。
public readonly record struct MapId
{
    public const int HexLength = 32;

    private MapId(string value) => Value = value;

    public string Value { get; }

    public bool IsValid => Value is not null
        && Value.Length == HexLength
        && Value.All(Uri.IsHexDigit);

    public static MapId New() => new(Guid.NewGuid().ToString("N"));

    public static bool TryParse(string? value, out MapId mapId)
    {
        mapId = default;
        if (string.IsNullOrWhiteSpace(value)) return false;
        var candidate = new MapId(value.Trim());
        if (!candidate.IsValid) return false;
        mapId = candidate;
        return true;
    }

    public override string ToString() => Value ?? "";
}
