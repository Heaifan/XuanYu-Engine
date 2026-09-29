using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI4GalleryCatalog
{
    static Control DropIndicatorPreview() => new StackPanel
    {
        Spacing = 8,
        Children = { Drop("可放置", XyuiDropState.Valid), Drop("禁止放置", XyuiDropState.Invalid), Drop("需要转换", XyuiDropState.Conditional) }
    };

    static Border Drop(string text, XyuiDropState state) => new()
    {
        Width = 260, Height = 42, Padding = new global::Avalonia.Thickness(8),
        Child = new Grid { Children = { new XYDropIndicator { IsActive = true, State = state, TargetRect = new global::Avalonia.Rect(1, 1, 258, 40), TargetBorderBrush = Brushes.SlateGray, ValidBrush = Brushes.SeaGreen, InvalidBrush = Brushes.IndianRed, ConditionalBrush = Brushes.DarkGoldenrod }, new TextBlock { Text = text, VerticalAlignment = VerticalAlignment.Center } } }
    };

    static Control DropIndicatorLiveExample()
    {
        var indicator = new XYDropIndicator { IsActive = true, State = XyuiDropState.Valid, TargetRect = new global::Avalonia.Rect(1, 1, 298, 52), TargetBorderBrush = Brushes.SlateGray, ValidBrush = Brushes.SeaGreen, InvalidBrush = Brushes.IndianRed, ConditionalBrush = Brushes.DarkGoldenrod };
        var state = new TextBlock { Text = "当前状态：Valid", Classes = { "xyui-text-caption" } };
        var button = new XYButton { Content = "切换 Drop 状态", Variant = XyuiButtonVariant.Secondary };
        button.Click += (_, _) => { indicator.State = indicator.State == XyuiDropState.Valid ? XyuiDropState.Invalid : indicator.State == XyuiDropState.Invalid ? XyuiDropState.Conditional : XyuiDropState.Valid; state.Text = $"当前状态：{indicator.State}"; };
        return new StackPanel { Spacing = 8, Children = { new Border { Width = 300, Height = 54, Child = indicator }, state, button } };
    }
}
