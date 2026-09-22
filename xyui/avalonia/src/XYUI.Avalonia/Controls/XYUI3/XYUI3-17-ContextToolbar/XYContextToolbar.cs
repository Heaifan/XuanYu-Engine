using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed class XYContextToolbar : Border
{
    readonly StackPanel _panel = new() { Orientation = Orientation.Horizontal };
    public AvaloniaList<XYContextGroup> Items { get; } = [];
    public static readonly StyledProperty<double> SpacingProperty = AvaloniaProperty.Register<XYContextToolbar, double>(nameof(Spacing), 8);
    public static readonly StyledProperty<bool> CompactProperty = AvaloniaProperty.Register<XYContextToolbar, bool>(nameof(Compact));
    public double Spacing { get => GetValue(SpacingProperty); set => SetValue(SpacingProperty, value); }
    public bool Compact { get => GetValue(CompactProperty); set => SetValue(CompactProperty, value); }
    public XYContextToolbar(params XYContextGroup[] groups)
    {
        Classes.Add("xyui-context-toolbar"); Child = _panel; Items.CollectionChanged += (_, _) => Refresh();
        foreach (var group in groups) Items.Add(group); Refresh();
    }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    { base.OnPropertyChanged(e); if (e.Property == SpacingProperty) _panel.Spacing = Spacing; }
    void Refresh() { _panel.Spacing = Spacing; _panel.Children.Clear(); foreach (var item in Items) _panel.Children.Add(item); }
}
