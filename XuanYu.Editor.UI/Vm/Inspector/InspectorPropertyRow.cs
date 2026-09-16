namespace XuanYu.Editor.UI;

public sealed record InspectorPropertyRow(
    InspectorPropertyDescriptor Descriptor,
    string Value,
    bool IsEditable = false,
    InspectorEditTarget EditTarget = default)
{
    public string Key => Descriptor.Key;
    public string DisplayName => Descriptor.DisplayName;
    public string Path => $"{Descriptor.Category} / {Descriptor.Section}";
}
