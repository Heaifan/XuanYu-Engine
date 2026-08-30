using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYMenuBarItem
{
    Grid BuildVisual()
    {
        var grid = new Grid { RowDefinitions = new RowDefinitions("32,3") };
        grid.Children.Add(new TextBlock { Text = Label, Classes = { "xyui-menu-bar-label" }, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
        var indicator = new Border { Classes = { "xyui-menu-bar-indicator" }, Width = Math.Max(28, Label.Length * 14), HorizontalAlignment = HorizontalAlignment.Center };
        grid.Children.Add(indicator); Grid.SetRow(indicator, 1); return grid;
    }
}
