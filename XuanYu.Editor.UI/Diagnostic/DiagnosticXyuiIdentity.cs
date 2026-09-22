namespace XuanYu.Editor.UI;

public sealed record DiagnosticXyuiIdentity(
    string Number,
    string ComponentName,
    string Source,
    string CatalogId)
{
    public bool IsMapped => !string.IsNullOrEmpty(Number);
    public string DisplayIndex => IsMapped ? $"{Number} · {ComponentName}" : "无";
    public static DiagnosticXyuiIdentity None(string source) =>
        new(string.Empty, string.Empty, source, string.Empty);
}
