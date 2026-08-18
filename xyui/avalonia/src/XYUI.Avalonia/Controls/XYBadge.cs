using Avalonia;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public enum XyuiBadgeVariant { Default, Accent }

public sealed class XYBadge : XyuiVectorTextSurface
{
    public static readonly StyledProperty<XyuiBadgeVariant> VariantProperty =
        AvaloniaProperty.Register<XYBadge, XyuiBadgeVariant>(nameof(Variant), XyuiBadgeVariant.Default);

    public XYBadge() : base("xyui-badge", XyuiVectorIcon.Tag, XyuiVectorMarkPlacement.Background) => ApplyVariant(Variant);
    public override string CanonicalId => "XYUI-1-09";
    public XyuiBadgeVariant Variant { get => GetValue(VariantProperty); set { SetValue(VariantProperty, value); ApplyVariant(value); } }
    void ApplyVariant(XyuiBadgeVariant value)
    {
        Classes.Remove("xyui-badge-accent"); VectorMark.Classes.Set("xyui-badge-mark-default", value == XyuiBadgeVariant.Default);
        VectorMark.Classes.Set("xyui-badge-mark-accent", value == XyuiBadgeVariant.Accent);
        if (value == XyuiBadgeVariant.Accent) Classes.Add("xyui-badge-accent");
    }
}
