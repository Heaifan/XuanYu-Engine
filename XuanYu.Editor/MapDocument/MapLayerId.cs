namespace XuanYu.Editor.MapDocument;

// MAP-A-R2-D1：图层稳定唯一标识。与 MapId 同族格式（32 个十六进制字符，无前缀）。
// 名称可修改，ID 不得变化；不得依赖列表序号/UI 索引。
public readonly record struct MapLayerId
{
    public const int HexLength = 32;

    private MapLayerId(string value) => Value = value;

    public string Value { get; }

    public bool IsValid => Value is not null
        && Value.Length == HexLength
        && Value.All(Uri.IsHexDigit);

    public static MapLayerId New() => new(Guid.NewGuid().ToString("N"));

    public static bool TryParse(string? value, out MapLayerId layerId)
    {
        layerId = default;
        if (string.IsNullOrWhiteSpace(value)) return false;
        var candidate = new MapLayerId(value.Trim());
        if (!candidate.IsValid) return false;
        layerId = candidate;
        return true;
    }

    public override string ToString() => Value ?? "";
}
