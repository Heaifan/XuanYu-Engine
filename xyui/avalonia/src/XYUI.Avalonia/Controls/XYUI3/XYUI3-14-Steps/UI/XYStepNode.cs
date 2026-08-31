using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public enum XYStepState { Completed, Current, Pending, Warning, Error }
public sealed class XYStepNode : Border
{
    public static readonly StyledProperty<XYStepState> StateProperty = AvaloniaProperty.Register<XYStepNode, XYStepState>(nameof(State), XYStepState.Pending);
    public static readonly StyledProperty<bool> CanNavigateProperty = AvaloniaProperty.Register<XYStepNode, bool>(nameof(CanNavigate), true);
    public string Label { get; }
    public XYStepState State { get => GetValue(StateProperty); set => SetValue(StateProperty, value); }
    public bool CanNavigate { get => GetValue(CanNavigateProperty); set => SetValue(CanNavigateProperty, value); }
    public bool IsVertical { get; internal set; }
    public event EventHandler? NavigationRequested;
    public XYStepNode(string label, XYStepState state) { Label = label; State = state; Classes.Add("xyui-step-node"); Build(); PointerPressed += (_, _) => { if (CanNavigate) NavigationRequested?.Invoke(this, EventArgs.Empty); }; }
    internal void BuildForLayout() => Build();
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) { base.OnPropertyChanged(e); if (e.Property == StateProperty || e.Property == CanNavigateProperty) Build(); }
    void Build()
    {
        Classes.Set("xyui-step-disabled", !CanNavigate);
        var icon = State == XYStepState.Completed ? XyuiVectorIcon.StatusDot : State == XYStepState.Warning ? XyuiVectorIcon.Warning : State == XYStepState.Error ? XyuiVectorIcon.Error : XyuiVectorIcon.StatusDot;
        var dot = new Border { Width = State == XYStepState.Current ? 26 : 24, Height = State == XYStepState.Current ? 26 : 24, Child = new XYIcon { Icon = icon, Size = XyuiIconSize.Tiny, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } };
        dot.Classes.Set("xyui-step-completed", State == XYStepState.Completed); dot.Classes.Set("xyui-step-current", State == XYStepState.Current); dot.Classes.Set("xyui-step-pending", State == XYStepState.Pending); dot.Classes.Set("xyui-step-warning", State == XYStepState.Warning); dot.Classes.Set("xyui-step-error", State == XYStepState.Error);
        var panel = new StackPanel { Orientation = IsVertical ? Orientation.Horizontal : Orientation.Vertical, Spacing = 8, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }; panel.Children.Add(dot); panel.Children.Add(new TextBlock { Text = Label, VerticalAlignment = VerticalAlignment.Center }); Child = panel;
    }
}
