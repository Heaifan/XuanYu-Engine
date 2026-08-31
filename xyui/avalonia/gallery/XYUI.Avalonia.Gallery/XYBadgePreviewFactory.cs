using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Spatial;

namespace XYUI.Avalonia.Gallery;

public static class XYBadgePreviewFactory
{
    public static Control Create() => new StackPanel
    {
        Orientation = Orientation.Horizontal,
        HorizontalAlignment = HorizontalAlignment.Left,
        Spacing = XyuiSpatialTokens.Space2,
        Children =
        {
            new XYBadge { Text = "草稿", Variant = XyuiBadgeVariant.Default },
            new XYBadge { Text = "已选中", Variant = XyuiBadgeVariant.Accent },
            new XYBadge { Text = "待审核", Variant = XyuiBadgeVariant.Default }
        }
    };
}
