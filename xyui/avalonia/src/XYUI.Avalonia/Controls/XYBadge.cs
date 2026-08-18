using Avalonia;
using XYUI.Avalonia.Vector;
using TextAlignment = global::Avalonia.Media.TextAlignment;

namespace XYUI.Avalonia.Controls;

public enum XyuiBadgeVariant { Default, Accent }

public sealed class XYBadge : XyuiVectorTextSurface
{
    public static readonly StyledProperty<XyuiBadgeVariant> VariantProperty =
        AvaloniaProperty.Register<XYBadge, XyuiBadgeVariant>(nameof(Variant), XyuiBadgeVariant.Default);

    public XYBadge() : base("xyui-badge", XyuiVectorIcon.Tag, XyuiVectorMarkPlacement.Background)
    {
        VectorMark.Classes.Add("xyui-badge-background-shape"); Height = 22;
        TextPresenter.TextAlignment = TextAlignment.Center; TextPresenter.VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center;
        TextPresenter.Margin = new Thickness(11, 0, 2, 0); ApplyVariant(Variant);
    }
    public override string CanonicalId => "XYUI-1-09";
    public XyuiBadgeVariant Variant { get => GetValue(VariantProperty); set { SetValue(VariantProperty, value); ApplyVariant(value); } }
    void ApplyVariant(XyuiBadgeVariant value)
    {
        Classes.Remove("xyui-badge-accent"); VectorMark.Classes.Set("xyui-badge-mark-default", value == XyuiBadgeVariant.Default);
        VectorMark.Classes.Set("xyui-badge-mark-accent", value == XyuiBadgeVariant.Accent);
        if (value == XyuiBadgeVariant.Accent) Classes.Add("xyui-badge-accent");
    }
}
