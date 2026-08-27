using Avalonia;
using XYUI.Avalonia.Spatial;
using XYUI.Avalonia.Vector;
using TextAlignment = global::Avalonia.Media.TextAlignment;
using TextWrapping = global::Avalonia.Media.TextWrapping;

namespace XYUI.Avalonia.Controls;

public enum XyuiBadgeVariant { Default, Accent }

public sealed class XYBadge : XyuiVectorTextSurface
{
    public const double BadgeHeight = 22;
    public const double PointerWidth = 11;
    public const double PointerTipInset = 2;
    public static readonly StyledProperty<XyuiBadgeVariant> VariantProperty =
        AvaloniaProperty.Register<XYBadge, XyuiBadgeVariant>(nameof(Variant), XyuiBadgeVariant.Default);

    public XYBadge() : base("xyui-badge", XyuiVectorIcon.Tag,
        XyuiVectorMarkPlacement.Background, new XyuiBadgeTagPath())
    {
        HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Left;
        VectorMark.Classes.Add("xyui-badge-background-shape"); Height = BadgeHeight;
        VectorMark.IsHitTestVisible = false;
        TextPresenter.TextAlignment = TextAlignment.Center; TextPresenter.VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center;
        TextPresenter.TextWrapping = TextWrapping.NoWrap;
        TextPresenter.Margin = new Thickness(PointerWidth + XyuiSpatialTokens.Space2, 0,
            XyuiSpatialTokens.Space2, 0); ApplyVariant(Variant);
    }
    public override string CanonicalId => "XYUI-1-09";
    public XyuiBadgeVariant Variant { get => GetValue(VariantProperty); set { SetValue(VariantProperty, value); ApplyVariant(value); } }

    protected override Size MeasureOverride(Size availableSize)
    {
        var measured = base.MeasureOverride(new Size(double.PositiveInfinity, BadgeHeight));
        return new Size(measured.Width, BadgeHeight);
    }

    void ApplyVariant(XyuiBadgeVariant value)
    {
        Classes.Remove("xyui-badge-accent"); VectorMark.Classes.Set("xyui-badge-mark-default", value == XyuiBadgeVariant.Default);
        VectorMark.Classes.Set("xyui-badge-mark-accent", value == XyuiBadgeVariant.Accent);
        TextPresenter.Classes.Set("xyui-badge-text-accent", value == XyuiBadgeVariant.Accent);
        if (value == XyuiBadgeVariant.Accent) Classes.Add("xyui-badge-accent");
    }
}
