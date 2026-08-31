using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public enum XYStepState { Completed, Current, Pending, Warning, Error }
public sealed class XYStepNode : Border
{
    public static readonly StyledProperty<XYStepState> StateProperty = AvaloniaProperty.Register<XYStepNode, XYStepState>(nameof(State), XYStepState.Pending);
    public static readonly StyledProperty<bool> CanNavigateProperty = AvaloniaProperty.Register<XYStepNode, bool>(nameof(CanNavigate), true);
    readonly Grid _layout = new(); readonly TextBlock _label = new(); bool _vertical;
    public Border Marker { get; } = new(); public string Label { get; }
    public XYStepState State { get => GetValue(StateProperty); set => SetValue(StateProperty, value); }
    public bool CanNavigate { get => GetValue(CanNavigateProperty); set => SetValue(CanNavigateProperty, value); }
    public event EventHandler? NavigationRequested;
    public XYStepNode(string label, XYStepState state)
    {
        Label = label; State = state; Classes.Add("xyui-step-node"); _label.Text = label; _label.VerticalAlignment = VerticalAlignment.Center;
        _layout.HorizontalAlignment = HorizontalAlignment.Stretch; _layout.VerticalAlignment = VerticalAlignment.Stretch; Child = _layout;
        PointerPressed += (_, _) => { if (CanNavigate) NavigationRequested?.Invoke(this, EventArgs.Empty); }; SetVertical(false); ApplyState();
    }
    internal void SetVertical(bool vertical)
    {
        _vertical = vertical;
        _layout.Children.Clear(); _layout.RowDefinitions.Clear(); _layout.ColumnDefinitions.Clear();
        if (vertical)
        {
            _layout.ColumnDefinitions.Add(new ColumnDefinition(58, GridUnitType.Pixel)); _layout.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
            Marker.HorizontalAlignment = HorizontalAlignment.Left; Grid.SetColumn(Marker, 0); Grid.SetColumn(_label, 1);
        }
        else
        {
            _layout.RowDefinitions.Add(new RowDefinition(68, GridUnitType.Pixel)); _layout.RowDefinitions.Add(new RowDefinition(44, GridUnitType.Pixel));
            Marker.Margin = new Thickness(0, 2, 0, 0); Marker.HorizontalAlignment = HorizontalAlignment.Center; _label.HorizontalAlignment = HorizontalAlignment.Center; Grid.SetRow(Marker, 0); Grid.SetRow(_label, 1);
        }
        _layout.Children.Add(Marker); _layout.Children.Add(_label);
    }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) { base.OnPropertyChanged(e); if (e.Property == StateProperty || e.Property == CanNavigateProperty) ApplyState(); }
    void ApplyState()
    {
        Classes.Set("xyui-step-disabled", !CanNavigate); var size = State == XYStepState.Current ? 34 : State == XYStepState.Pending ? 30 : 32; Marker.Width = size; Marker.Height = size; Marker.CornerRadius = new CornerRadius(size / 2d); if (_vertical) Marker.Margin = new Thickness(42 - size / 2d, 0, 0, 0);
        Marker.Classes.Set("xyui-step-completed", State == XYStepState.Completed); Marker.Classes.Set("xyui-step-current", State == XYStepState.Current); Marker.Classes.Set("xyui-step-pending", State == XYStepState.Pending); Marker.Classes.Set("xyui-step-warning", State == XYStepState.Warning); Marker.Classes.Set("xyui-step-error", State == XYStepState.Error);
        XyuiVectorIcon? glyph = State == XYStepState.Completed ? XyuiVectorIcon.Check : State == XYStepState.Warning ? XyuiVectorIcon.Warning : State == XYStepState.Error ? XyuiVectorIcon.Error : null;
        Marker.Child = glyph is { } icon ? new XYIcon { Icon = icon, Stroke = Brushes.White, Size = XyuiIconSize.Tiny, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } : State == XYStepState.Current ? new Border { Width = 10, Height = 10, CornerRadius = new CornerRadius(5), Classes = { "xyui-step-inner-dot" } } : null;
    }
}
