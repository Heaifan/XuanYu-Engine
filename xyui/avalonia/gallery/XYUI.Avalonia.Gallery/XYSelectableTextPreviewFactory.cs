using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Spatial;

namespace XYUI.Avalonia.Gallery;

public static class XYSelectableTextPreviewFactory
{
    public static Control Create() => new StackPanel
    {
        Orientation = Orientation.Vertical,
        HorizontalAlignment = HorizontalAlignment.Left,
        Spacing = XyuiSpatialTokens.Space2,
        Children =
        {
            new XYSelectableText { Text = "可选择并复制的说明文本", Variant = XyuiSelectableTextVariant.Default },
            new XYSelectableText { Text = "region-7ad21c", Variant = XyuiSelectableTextVariant.Technical }
        }
    };
}
