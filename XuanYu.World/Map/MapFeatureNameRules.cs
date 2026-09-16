namespace XuanYu.World.Map;

public static class MapFeatureNameRules
{
    public const int MinNameLength = 1;
    public const int MaxNameLength = 32;

    public static string? ValidateName(string? name)
    {
        var trimmed = name?.Trim() ?? "";
        if (trimmed.Length < MinNameLength) return "要素名称不能为空。";
        if (trimmed.Length > MaxNameLength) return $"要素名称长度不能超过 {MaxNameLength} 个字符。";
        if (trimmed.Any(char.IsControl)) return "要素名称不能包含换行或控制字符。";
        return null;
    }
}
