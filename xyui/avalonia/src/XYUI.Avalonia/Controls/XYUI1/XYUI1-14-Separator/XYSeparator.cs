using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public enum XyuiSeparatorVariant { Default, Header, Panel, Section, ListRow, VerticalSplit }

public sealed class XYSeparator : Border
{
    public static readonly StyledProperty<XyuiSeparatorVariant> VariantProperty = AvaloniaProperty.Register<XYSeparator, XyuiSeparatorVariant>(nameof(Variant), XyuiSeparatorVariant.Default);
    public XYSeparator() { Classes.Add("xyui-separator"); ApplyVariant(Variant); }
    public string CanonicalId => "XYUI-1-14";
    public XyuiSeparatorVariant Variant { get => GetValue(VariantProperty); set { SetValue(VariantProperty, value); ApplyVariant(value); } }
    void ApplyVariant(XyuiSeparatorVariant value) { foreach (var item in Enum.GetNames<XyuiSeparatorVariant>()) Classes.Remove($"xyui-separator-{item.ToLowerInvariant()}"); Classes.Add($"xyui-separator-{value.ToString().ToLowerInvariant()}"); }
}
