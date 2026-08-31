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
    readonly Canvas _cells = new(); readonly Canvas _track = new(); readonly Grid _root = new(); readonly List<Border> _connectors = new(); bool _vertical;
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
        _vertical = vertical; _cells.Children.Clear(); _track.Children.Clear(); _connectors.Clear(); _root.Height = vertical ? 326 : 112;
        if (vertical) BuildVertical(); else BuildHorizontal(); UpdateConnectors();
    }
    void BuildHorizontal()
    {
        for (var i = 0; i < Items.Count; i++) { Items[i].SetVertical(false); Items[i].HorizontalAlignment = HorizontalAlignment.Stretch; _cells.Children.Add(Items[i]); }
        for (var i = 0; i + 1 < Items.Count; i++) _connectors.Add(AddConnector());
    }
    void BuildVertical()
    {
        for (var i = 0; i < Items.Count; i++) { Items[i].SetVertical(true); Items[i].HorizontalAlignment = HorizontalAlignment.Stretch; _cells.Children.Add(Items[i]); }
        for (var i = 0; i + 1 < Items.Count; i++) _connectors.Add(AddConnector());
    }
    Border AddConnector()
    {
        var line = new Border { Height = 2, CornerRadius = new CornerRadius(1), Background = Brushes.Gray, IsHitTestVisible = false }; line.Classes.Add("xyui-step-connector"); _track.Children.Add(line); return line;
    }
    void UpdateConnectors()
    {
        var centers = _vertical ? new[] { 40d, 110d, 182d, 251d, 309d } : new[] { 44d, 230d, 412d, 590d, 716d };
        var width = Bounds.Width > 0 ? Bounds.Width : _vertical ? 300 : 760;
        for (var i = 0; i < Items.Count && i < centers.Length; i++)
        {
            var slot = _vertical ? width : HorizontalSlotWidth(i, width);
            Canvas.SetLeft(Items[i], _vertical ? 0 : centers[i] * width / 760d - slot / 2); Canvas.SetTop(Items[i], _vertical ? centers[i] - 35 : 0); Items[i].Width = slot; Items[i].Height = _vertical ? 70 : 112;
        }
        for (var i = 0; i < _connectors.Count && i + 1 < centers.Length; i++)
        {
            var line = _connectors[i]; line.Classes.Set("xyui-step-connector-completed", Items[i].State == XYStepState.Completed); line.Classes.Set("xyui-step-connector-pending", Items[i].State != XYStepState.Completed);
            var a = centers[i]; var b = centers[i + 1]; var ar = MarkerRadius(Items[i]); var br = MarkerRadius(Items[i + 1]);
            if (_vertical) { Canvas.SetLeft(line, 41); Canvas.SetTop(line, a + ar); line.Height = Math.Max(0, b - br - a - ar); line.Width = 2; }
            else { var scale = width / 760d; Canvas.SetLeft(line, (a + ar) * scale); Canvas.SetTop(line, 35); line.Width = Math.Max(0, (b - br - a - ar) * scale); }
        }
    }
    static double MarkerRadius(XYStepNode node) => node.State == XYStepState.Current ? 17 : node.State == XYStepState.Pending ? 15 : 16;
    static double HorizontalSlotWidth(int index, double width) => (new[] { 137d, 184, 180, 152, 107 })[Math.Min(index, 4)] * width / 760d;
}
