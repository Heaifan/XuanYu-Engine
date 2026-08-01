namespace XuanYu.Editor.Assets;

public readonly record struct AssetId
{
    public const string Prefix = "asset_";

    private AssetId(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public bool IsValid => Value.StartsWith(Prefix, StringComparison.Ordinal)
        && Value.Length == Prefix.Length + 32
        && Value[Prefix.Length..].All(Uri.IsHexDigit);

    public static AssetId New() => new(Prefix + Guid.NewGuid().ToString("N"));

    public static bool TryParse(string? value, out AssetId assetId)
    {
        assetId = default;
        if (string.IsNullOrWhiteSpace(value)) return false;
        var candidate = new AssetId(value.Trim());
        if (!candidate.IsValid) return false;
        assetId = candidate;
        return true;
    }

    public override string ToString() => Value ?? "";
}
