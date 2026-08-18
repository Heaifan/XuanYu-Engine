using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static class XYMonoPreviewFactory
{
    public static Control Create()
    {
        var grid = new Grid { Classes = { "xyui-mono-preview" } };
        grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(96)));
        grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(24)));
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        foreach (var row in new[] { ("X 坐标", "142.583"), ("Y 坐标", "-26.410"), ("Z 坐标", "0.000"), ("Frame", "16.67 ms") }) AddRow(grid, row.Item1, row.Item2);
        return grid;
    }

    static void AddRow(Grid grid, string label, string value)
    {
        var row = grid.RowDefinitions.Count; grid.RowDefinitions.Add(new RowDefinition(new GridLength(22)));
        var labelControl = new XYLabel { Text = label }; var valueControl = new XYMonoText { Text = value };
        Grid.SetRow(labelControl, row); Grid.SetColumn(labelControl, 0); Grid.SetRow(valueControl, row); Grid.SetColumn(valueControl, 2);
        grid.Children.Add(labelControl); grid.Children.Add(valueControl);
    }
}
