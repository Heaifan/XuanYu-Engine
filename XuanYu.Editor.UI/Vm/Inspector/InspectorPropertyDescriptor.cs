namespace XuanYu.Editor.UI;

public sealed record InspectorPropertyDescriptor(
    string Key,
    InspectorObjectKind ObjectKind,
    string Category,
    string Section,
    string DisplayName,
    string Alias = "",
    bool IsEditable = false);
