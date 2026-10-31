using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI4GalleryCatalog
{
    static Control InsertionIndicatorPreview() => new StackPanel
    {
        Spacing = 8,
        Children = { InsertRow("Before · 顶层", XyuiInsertionMode.Before, 0), InsertRow("Into · 子级", XyuiInsertionMode.Into, 1), InsertRow("After · 深层", XyuiInsertionMode.After, 2) }
    };

    static Border InsertRow(string text, XyuiInsertionMode mode, int level)
    {
        var grid = new Grid();
        grid.Children.Add(new Border { Background = Brushes.Transparent, BorderBrush = Brushes.SlateGray, BorderThickness = new Thickness(1) });
        grid.Children.Add(new TextBlock { Text = text, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10 + level * 16, 0) });
        grid.Children.Add(new XYInsertionIndicator { IsActive = true, Mode = mode, IndentLevel = level, TargetRect = new Rect(1, 1, 298, 40), LineBrush = Brushes.SteelBlue, GapBorderBrush = Brushes.SlateGray });
        return new Border { Width = 300, Height = 42, Child = grid };
    }

    static Control InsertionIndicatorLiveExample()
    {
        var indicator = new XYInsertionIndicator { IsActive = true, Mode = XyuiInsertionMode.Before, IndentLevel = 1, TargetRect = new Rect(1, 1, 318, 52), LineBrush = Brushes.SteelBlue };
        var state = new TextBlock { Text = "插入关系：Before · 层级 1", Classes = { "xyui-text-caption" } };
        var button = new XYButton { Content = "切换插入位置", Variant = XyuiButtonVariant.Secondary };
        button.Click += (_, _) => { indicator.Mode = indicator.Mode == XyuiInsertionMode.Before ? XyuiInsertionMode.Into : indicator.Mode == XyuiInsertionMode.Into ? XyuiInsertionMode.After : XyuiInsertionMode.Before; state.Text = $"插入关系：{indicator.Mode} · 层级 {indicator.IndentLevel}"; };
        return new StackPanel { Spacing = 8, Children = { new Border { Width = 320, Height = 54, Child = indicator }, state, button } };
    }
}
