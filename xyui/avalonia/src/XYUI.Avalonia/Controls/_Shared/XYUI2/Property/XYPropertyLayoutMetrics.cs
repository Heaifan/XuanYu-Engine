using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

internal static class XYPropertyLayoutMetrics
{
    internal const double LabelColumnWidth = 160;
    internal const double ColumnGap = 8;
    internal const double RowHeight = 34;
    internal const double WideBreakpoint = 520;
    internal const double CompactBreakpoint = 300;

    internal static bool IsCompact(double width) => width > 0 && width < CompactBreakpoint;

    internal static void ConfigureRow(Grid row, Control label, Control value, double width)
    {
        row.ColumnDefinitions.Clear(); row.RowDefinitions.Clear();
        if (IsCompact(width))
        {
            row.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            row.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); row.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            Grid.SetColumn(label, 0); Grid.SetRow(label, 0); Grid.SetColumn(value, 0); Grid.SetRow(value, 1);
            return;
        }
        row.ColumnDefinitions.Add(new ColumnDefinition(LabelColumnWidth, GridUnitType.Pixel));
        row.ColumnDefinitions.Add(new ColumnDefinition(ColumnGap, GridUnitType.Pixel));
        row.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        Grid.SetColumn(label, 0); Grid.SetRow(label, 0); Grid.SetColumn(value, 2); Grid.SetRow(value, 0);
    }
}
