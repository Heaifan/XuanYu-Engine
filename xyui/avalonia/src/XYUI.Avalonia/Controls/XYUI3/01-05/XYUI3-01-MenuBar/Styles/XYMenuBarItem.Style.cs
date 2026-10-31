using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYMenuBarItem
{
    Grid BuildVisual()
    {
        var grid = new Grid { RowDefinitions = new RowDefinitions("32,3") };
        var stack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 4,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        if (Icon is XyuiVectorIcon icon)
            stack.Children.Add(new XYIcon { Icon = icon, Size = IconSize, Classes = { "xyui-menu-icon", $"xyui-menu-icon-{IconSize.ToString().ToLowerInvariant()}" }, VerticalAlignment = VerticalAlignment.Center });
        stack.Children.Add(new TextBlock
        {
            Text = Label,
            Classes = { "xyui-menu-bar-label" },
            VerticalAlignment = VerticalAlignment.Center
        });
        if (ShowChevron)
        {
            stack.Children.Add(new XYIcon
            {
                Icon = XyuiVectorIcon.ChevronDown,
                Size = XyuiIconSize.Tiny,
                Classes = { "xyui-menu-chevron" },
                VerticalAlignment = VerticalAlignment.Center
            });
        }
        grid.Children.Add(stack);
        var indicator = new Border
        {
            Classes = { "xyui-menu-bar-indicator" },
            Width = Math.Max(28, (Label?.Length ?? 2) * 14 + (Icon is null ? 0 : 18)),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        grid.Children.Add(indicator);
        Grid.SetRow(indicator, 1);
        return grid;
    }
}
