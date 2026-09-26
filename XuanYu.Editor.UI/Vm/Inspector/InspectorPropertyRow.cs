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
    public bool IsColorEditor => Key == "Region.Style.FillColor";
    public bool IsTextEditor => IsEditable && !IsColorEditor;
    public bool IsReadOnly => !IsEditable;
    public Avalonia.Media.Color EditorColor => InspectorColorValue.Parse(Value);
}
