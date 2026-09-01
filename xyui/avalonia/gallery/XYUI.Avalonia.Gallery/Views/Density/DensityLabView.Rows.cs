using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Density;

namespace XYUI.Avalonia.Gallery.Views.Density;

public partial class DensityLabView
{
    static Control CreateRow(string title, string code, string type, string status, string desc, XyuiDensity density)
    {
        var border = new Border { Classes = { "xyui-surface-panel" }, CornerRadius = new(4), Padding = new(8, 6) };
        if (density == XyuiDensity.Compact)
        {
            var grid = new Grid { ColumnDefinitions = new("Auto,Auto,*,Auto,Auto") };
            grid.Children.Add(new XYLabel { Text = title, Margin = new(0, 0, 8, 0), VerticalAlignment = VerticalAlignment.Center });
            var codeText = new XYCodeText { Text = code, Margin = new(0, 0, 8, 0), VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(codeText, 1); grid.Children.Add(codeText);
            var badge = new XYStatusBadge { Text = $"{type} · {status}", Margin = new(0, 0, 8, 0), VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(badge, 2); grid.Children.Add(badge);
            var btnEdit = new XYIconButton { Content = "✎", Margin = new(0, 0, 4, 0) };
            ToolTip.SetTip(btnEdit, "快速编辑");
            Grid.SetColumn(btnEdit, 3); grid.Children.Add(btnEdit);
            var btnLoc = new XYIconButton { Content = "⊙" };
            ToolTip.SetTip(btnLoc, "定位视口");
            Grid.SetColumn(btnLoc, 4); grid.Children.Add(btnLoc);
            border.Child = grid;
        }
        else if (density == XyuiDensity.Default)
        {
            var stack = new StackPanel { Spacing = 4 };
            var top = new Grid { ColumnDefinitions = new("*,Auto") };
            var titleBox = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6 };
            titleBox.Children.Add(new XYLabel { Text = title });
            titleBox.Children.Add(new XYCodeText { Text = code });
            titleBox.Children.Add(new XYStatusBadge { Text = status });
            top.Children.Add(titleBox);
            var acts = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4 };
            Grid.SetColumn(acts, 1); acts.Children.Add(new XYButton { Content = "定位", Variant = XyuiButtonVariant.Secondary });
            acts.Children.Add(new XYButton { Content = "编辑", Variant = XyuiButtonVariant.Secondary });
            top.Children.Add(acts); stack.Children.Add(top);
            stack.Children.Add(new XYCaption { Text = desc });
            border.Child = stack;
        }
        else
        {
            var stack = new StackPanel { Spacing = 6, Margin = new(4) };
            var top = new Grid { ColumnDefinitions = new("*,Auto") };
            var titleBox = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
            titleBox.Children.Add(new XYHeading { Text = title, Variant = XyuiHeadingVariant.PanelTitle });
            titleBox.Children.Add(new XYBadge { Text = type });
            titleBox.Children.Add(new XYStatusBadge { Text = status });
            top.Children.Add(titleBox);
            var acts = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6 };
            Grid.SetColumn(acts, 1); acts.Children.Add(new XYButton { Content = "定位视口", Variant = XyuiButtonVariant.Secondary });
            acts.Children.Add(new XYButton { Content = "属性检查", Variant = XyuiButtonVariant.Primary });
            top.Children.Add(acts); stack.Children.Add(top);
            stack.Children.Add(new XYCodeText { Text = $"标识符：{code}  |  类别：{type}  |  状态：{status}" });
            stack.Children.Add(new XYText { Text = desc });
            border.Child = stack;
        }
        return border;
    }
}
