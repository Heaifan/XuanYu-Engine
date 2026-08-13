namespace XuanYu.World.Map;

public readonly record struct MapMarkerId
{
    public string Value { get; }
    private MapMarkerId(string value) => Value = value;
    public static MapMarkerId New() => new(Guid.NewGuid().ToString("N"));
    public static bool TryParse(string? value, out MapMarkerId id)
    {
        id = default;
        if (string.IsNullOrWhiteSpace(value)) return false;
        var candidate = value.Trim();
        if (candidate.Length != 32 || !candidate.All(Uri.IsHexDigit)) return false;
        id = new(candidate); return true;
    }
    public bool IsValid => TryParse(Value, out _);
    public override string ToString() => Value;
}
