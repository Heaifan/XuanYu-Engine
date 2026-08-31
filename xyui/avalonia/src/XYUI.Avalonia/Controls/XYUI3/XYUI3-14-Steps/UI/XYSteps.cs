using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public enum XYStepsOrientation { Horizontal, Vertical }
public sealed class XYSteps : Border
{
    public static readonly StyledProperty<XYStepsOrientation> OrientationProperty = AvaloniaProperty.Register<XYSteps, XYStepsOrientation>(nameof(Orientation), XYStepsOrientation.Horizontal);
    public static readonly StyledProperty<bool> IsAdaptiveProperty = AvaloniaProperty.Register<XYSteps, bool>(nameof(IsAdaptive));
    readonly Grid _cells = new(); readonly Canvas _track = new(); readonly Grid _root = new(); readonly List<Border> _connectors = new(); bool _vertical;
    public XYStepsOrientation Orientation { get => GetValue(OrientationProperty); set => SetValue(OrientationProperty, value); }
    public bool IsAdaptive { get => GetValue(IsAdaptiveProperty); set => SetValue(IsAdaptiveProperty, value); }
    public IReadOnlyList<XYStepNode> Items { get; }
    public XYSteps(params XYStepNode[] items)
    {
        Items = items; Classes.Add("xyui-steps"); _root.Children.Add(_track); _root.Children.Add(_cells); Child = _root; LayoutUpdated += (_, _) => UpdateConnectors(); Build();
    }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) { base.OnPropertyChanged(e); if (e.Property == OrientationProperty || e.Property == IsAdaptiveProperty) Build(); }
    void Build()
    {
        var vertical = Orientation == XYStepsOrientation.Vertical || (IsAdaptive && Bounds.Width > 0 && Bounds.Width < 520);
        if (_cells.Children.Count > 0 && vertical == _vertical) return;
        _vertical = vertical; _cells.Children.Clear(); _track.Children.Clear(); _connectors.Clear(); _cells.RowDefinitions.Clear(); _cells.ColumnDefinitions.Clear();
        if (vertical) BuildVertical(); else BuildHorizontal(); UpdateConnectors();
    }
    void BuildHorizontal()
    {
        for (var i = 0; i < Items.Count; i++) { _cells.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star)); Items[i].SetVertical(false); Grid.SetColumn(Items[i], i); Items[i].HorizontalAlignment = HorizontalAlignment.Center; _cells.Children.Add(Items[i]); }
        for (var i = 0; i + 1 < Items.Count; i++) _connectors.Add(AddConnector());
    }
    void BuildVertical()
    {
        _cells.ColumnDefinitions.Add(new ColumnDefinition(26, GridUnitType.Pixel)); _cells.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
        for (var i = 0; i < Items.Count; i++) { _cells.RowDefinitions.Add(new RowDefinition(70, GridUnitType.Pixel)); Items[i].SetVertical(true); Grid.SetRow(Items[i], i); Grid.SetColumnSpan(Items[i], 2); Items[i].HorizontalAlignment = HorizontalAlignment.Stretch; _cells.Children.Add(Items[i]); }
        for (var i = 0; i + 1 < Items.Count; i++) _connectors.Add(AddConnector());
    }
    Border AddConnector()
    {
        var line = new Border { Height = 2, Background = Brushes.Gray, IsHitTestVisible = false }; line.Classes.Add("xyui-step-connector"); _track.Children.Add(line); return line;
    }
    void UpdateConnectors()
    {
        for (var i = 0; i < _connectors.Count; i++)
        {
            var a = Items[i].Marker.TranslatePoint(new Point(0, 0), _track); var b = Items[i + 1].Marker.TranslatePoint(new Point(0, 0), _track); if (a is null || b is null) continue;
            var line = _connectors[i]; line.Classes.Set("xyui-step-connector-completed", Items[i].State == XYStepState.Completed); line.Classes.Set("xyui-step-connector-pending", Items[i].State != XYStepState.Completed);
            if (_vertical) { Canvas.SetLeft(line, a.Value.X + Items[i].Marker.Bounds.Width / 2 - 1); Canvas.SetTop(line, a.Value.Y + Items[i].Marker.Bounds.Height); line.Height = Math.Max(0, b.Value.Y - (a.Value.Y + Items[i].Marker.Bounds.Height)); line.Width = 2; }
            else { Canvas.SetLeft(line, a.Value.X + Items[i].Marker.Bounds.Width / 2); Canvas.SetTop(line, a.Value.Y + Items[i].Marker.Bounds.Height / 2 - 1); line.Width = Math.Max(0, b.Value.X - (a.Value.X + Items[i].Marker.Bounds.Width / 2)); }
        }
    }
}
