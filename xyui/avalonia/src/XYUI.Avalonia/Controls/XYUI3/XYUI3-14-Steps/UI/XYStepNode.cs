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
    readonly StackPanel _panel = new(); readonly TextBlock _label = new();
    public Border Marker { get; } = new(); public string Label { get; }
    public XYStepState State { get => GetValue(StateProperty); set => SetValue(StateProperty, value); }
    public bool CanNavigate { get => GetValue(CanNavigateProperty); set => SetValue(CanNavigateProperty, value); }
    public event EventHandler? NavigationRequested;
    public XYStepNode(string label, XYStepState state)
    {
        Label = label; State = state; Classes.Add("xyui-step-node"); _label.Text = label; _label.VerticalAlignment = VerticalAlignment.Center;
        _panel.HorizontalAlignment = HorizontalAlignment.Center; _panel.VerticalAlignment = VerticalAlignment.Center; _panel.Spacing = 8;
        _panel.Children.Add(Marker); _panel.Children.Add(_label); Child = _panel; PointerPressed += (_, _) => { if (CanNavigate) NavigationRequested?.Invoke(this, EventArgs.Empty); }; ApplyState();
    }
    internal void SetVertical(bool vertical)
    {
        _panel.Orientation = vertical ? Orientation.Horizontal : Orientation.Vertical;
        _panel.HorizontalAlignment = vertical ? HorizontalAlignment.Stretch : HorizontalAlignment.Center;
        Marker.HorizontalAlignment = vertical ? HorizontalAlignment.Left : HorizontalAlignment.Center;
        _label.HorizontalAlignment = vertical ? HorizontalAlignment.Left : HorizontalAlignment.Center;
    }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) { base.OnPropertyChanged(e); if (e.Property == StateProperty || e.Property == CanNavigateProperty) ApplyState(); }
    void ApplyState()
    {
        Classes.Set("xyui-step-disabled", !CanNavigate); Marker.Width = State == XYStepState.Current ? 26 : 24; Marker.Height = Marker.Width;
        Marker.Classes.Set("xyui-step-completed", State == XYStepState.Completed); Marker.Classes.Set("xyui-step-current", State == XYStepState.Current); Marker.Classes.Set("xyui-step-pending", State == XYStepState.Pending); Marker.Classes.Set("xyui-step-warning", State == XYStepState.Warning); Marker.Classes.Set("xyui-step-error", State == XYStepState.Error);
        XyuiVectorIcon? glyph = State == XYStepState.Completed ? XyuiVectorIcon.Check : State == XYStepState.Warning ? XyuiVectorIcon.Warning : State == XYStepState.Error ? XyuiVectorIcon.Error : null;
        Marker.Child = glyph is { } icon ? new XYIcon { Icon = icon, Stroke = Brushes.White, Size = XyuiIconSize.Tiny, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } : State == XYStepState.Current ? new Border { Width = 8, Height = 8, CornerRadius = new CornerRadius(8), Classes = { "xyui-step-inner-dot" } } : null;
    }
}
