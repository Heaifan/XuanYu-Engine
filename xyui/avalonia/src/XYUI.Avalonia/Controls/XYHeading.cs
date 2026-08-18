using Avalonia;

namespace XYUI.Avalonia.Controls;

public enum XyuiHeadingVariant { PanelTitle, PageTitle }

public sealed class XYHeading : XyuiTextComponent
{
    public static readonly StyledProperty<XyuiHeadingVariant> VariantProperty =
        AvaloniaProperty.Register<XYHeading, XyuiHeadingVariant>(nameof(Variant), XyuiHeadingVariant.PanelTitle);

    public XYHeading() : base("xyui-heading") => ApplyVariant(Variant);
    public override string CanonicalId => "XYUI-1-04";
    public XyuiHeadingVariant Variant { get => GetValue(VariantProperty); set { SetValue(VariantProperty, value); ApplyVariant(value); } }

    void ApplyVariant(XyuiHeadingVariant value)
    {
        Classes.Remove("xyui-heading-panel"); Classes.Remove("xyui-heading-page");
        Classes.Add(value == XyuiHeadingVariant.PageTitle ? "xyui-heading-page" : "xyui-heading-panel");
    }
}
