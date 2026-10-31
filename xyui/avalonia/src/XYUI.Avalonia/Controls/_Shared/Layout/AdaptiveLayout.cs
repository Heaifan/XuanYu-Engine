using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

/// <summary>Container-first layout primitive that reflows children into 1..N columns.</summary>
public class AdaptiveLayout : Panel
{
    public static readonly StyledProperty<double> MinItemWidthProperty =
        AvaloniaProperty.Register<AdaptiveLayout, double>(nameof(MinItemWidth), 280);
    public static readonly StyledProperty<int> MaxColumnsProperty =
        AvaloniaProperty.Register<AdaptiveLayout, int>(nameof(MaxColumns), 3);

    double _gap;
    internal int CurrentColumnCount { get; private set; }

    public double MinItemWidth { get => GetValue(MinItemWidthProperty); set => SetValue(MinItemWidthProperty, value); }
    public int MaxColumns { get => GetValue(MaxColumnsProperty); set => SetValue(MaxColumnsProperty, value); }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == MinItemWidthProperty || change.Property == MaxColumnsProperty) InvalidateMeasure();
    }

    internal void SetResolvedGap(double gap)
    {
        var value = Math.Max(0, gap);
        if (Math.Abs(_gap - value) < 0.01) return;
        _gap = value;
        InvalidateMeasure();
    }

    internal int CalculateColumnCount(double availableWidth)
    {
        var min = Math.Max(1, MinItemWidth);
        var max = Math.Max(1, MaxColumns);
        if (double.IsInfinity(availableWidth)) return max;
        var count = (int)Math.Floor((Math.Max(0, availableWidth) + _gap) / (min + _gap));
        return Math.Clamp(count, 1, max);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var width = double.IsInfinity(availableSize.Width)
            ? Math.Max(1, MaxColumns) * Math.Max(1, MinItemWidth) + Math.Max(0, MaxColumns - 1) * _gap
            : Math.Max(0, availableSize.Width);
        var columns = CalculateColumnCount(width);
        var itemWidth = ItemWidth(width, columns);
        foreach (var child in Children) child.Measure(new Size(itemWidth, availableSize.Height));
        CurrentColumnCount = columns;
        return new Size(width, RowHeightSum(columns));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var columns = CalculateColumnCount(finalSize.Width);
        var itemWidth = ItemWidth(finalSize.Width, columns);
        var rowHeights = new double[(Children.Count + columns - 1) / columns];
        for (var i = 0; i < Children.Count; i++) rowHeights[i / columns] = Math.Max(rowHeights[i / columns], Children[i].DesiredSize.Height);
        var y = 0d;
        for (var i = 0; i < Children.Count; i++)
        {
            var row = i / columns;
            var column = i % columns;
            Children[i].Arrange(new Rect(column * (itemWidth + _gap), y, itemWidth, rowHeights[row]));
            if (column == columns - 1) y += rowHeights[row] + _gap;
        }
        CurrentColumnCount = columns;
        return finalSize;
    }

    double ItemWidth(double width, int columns) => Math.Max(0, (width - Math.Max(0, columns - 1) * _gap) / columns);

    double RowHeightSum(int columns)
    {
        var rows = (Children.Count + columns - 1) / columns;
        var heights = new double[rows];
        for (var i = 0; i < Children.Count; i++) heights[i / columns] = Math.Max(heights[i / columns], Children[i].DesiredSize.Height);
        return heights.Sum() + Math.Max(0, rows - 1) * _gap;
    }
}
