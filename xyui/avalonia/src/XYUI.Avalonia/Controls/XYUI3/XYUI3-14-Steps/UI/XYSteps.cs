using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public enum XYStepsOrientation { Horizontal, Vertical }
public sealed class XYSteps : Border
{
    public static readonly StyledProperty<XYStepsOrientation> OrientationProperty = AvaloniaProperty.Register<XYSteps, XYStepsOrientation>(nameof(Orientation), XYStepsOrientation.Horizontal);
    public static readonly StyledProperty<bool> IsAdaptiveProperty = AvaloniaProperty.Register<XYSteps, bool>(nameof(IsAdaptive));
    public XYStepsOrientation Orientation { get => GetValue(OrientationProperty); set => SetValue(OrientationProperty, value); }
    public bool IsAdaptive { get => GetValue(IsAdaptiveProperty); set => SetValue(IsAdaptiveProperty, value); }
    public IReadOnlyList<XYStepNode> Items { get; }
    public XYSteps(params XYStepNode[] items) { Items = items; Classes.Add("xyui-steps"); SizeChanged += (_, _) => { if (IsAdaptive) Build(); }; Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) { base.OnPropertyChanged(e); if (e.Property == OrientationProperty || e.Property == IsAdaptiveProperty) Build(); }
    void Build()
    {
        if (Child is Panel old) old.Children.Clear();
        var vertical = Orientation == XYStepsOrientation.Vertical || (IsAdaptive && Bounds.Width > 0 && Bounds.Width < 520); var panel = new StackPanel { Orientation = vertical ? global::Avalonia.Layout.Orientation.Vertical : global::Avalonia.Layout.Orientation.Horizontal, Spacing = vertical ? 8 : 18 };
        foreach (var item in Items) { item.IsVertical = vertical; item.BuildForLayout(); panel.Children.Add(item); if (item != Items[^1]) { var done = item.State == XYStepState.Completed; panel.Children.Add(new Border { Classes = { "xyui-step-connector", done ? "xyui-step-connector-completed" : "xyui-step-connector-pending" }, Height = vertical ? 18 : 2, Width = vertical ? 2 : 48, VerticalAlignment = VerticalAlignment.Center }); } }
        Child = panel;
    }
}
