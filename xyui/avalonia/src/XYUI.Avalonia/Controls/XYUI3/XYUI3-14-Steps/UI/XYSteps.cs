using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public enum XYStepsOrientation { Horizontal, Vertical }
public sealed class XYSteps : Border
{
    public static readonly StyledProperty<XYStepsOrientation> OrientationProperty = AvaloniaProperty.Register<XYSteps, XYStepsOrientation>(nameof(Orientation), XYStepsOrientation.Horizontal);
    public XYStepsOrientation Orientation { get => GetValue(OrientationProperty); set => SetValue(OrientationProperty, value); }
    public IReadOnlyList<XYStepNode> Items { get; }
    public XYSteps(params XYStepNode[] items) { Items = items; Classes.Add("xyui-steps"); Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) { base.OnPropertyChanged(e); if (e.Property == OrientationProperty) Build(); }
    void Build()
    {
        if (Child is Panel old) old.Children.Clear();
        var panel = new StackPanel { Orientation = Orientation == XYStepsOrientation.Horizontal ? global::Avalonia.Layout.Orientation.Horizontal : global::Avalonia.Layout.Orientation.Vertical, Spacing = Orientation == XYStepsOrientation.Horizontal ? 18 : 8 };
        foreach (var item in Items) { panel.Children.Add(item); if (item != Items[^1]) panel.Children.Add(new Border { Classes = { "xyui-step-connector" }, Height = Orientation == XYStepsOrientation.Horizontal ? 2 : 18, Width = Orientation == XYStepsOrientation.Horizontal ? 48 : 2, VerticalAlignment = VerticalAlignment.Center }); }
        Child = panel;
    }
}
