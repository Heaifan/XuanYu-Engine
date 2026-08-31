using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed class XYToolbar : Border
{
    public static readonly StyledProperty<bool> IsCompactProperty = AvaloniaProperty.Register<XYToolbar, bool>(nameof(IsCompact), true);
    public bool IsCompact { get => GetValue(IsCompactProperty); set => SetValue(IsCompactProperty, value); }
    public IReadOnlyList<Control> Items { get; }
    public XYToolbar(params Control[] items) { Items = items; Classes.Add("xyui-toolbar"); Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) { base.OnPropertyChanged(e); if (e.Property == IsCompactProperty) Build(); }
    void Build() { var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 2, VerticalAlignment = VerticalAlignment.Center }; foreach (var item in Items) panel.Children.Add(item); Child = panel; }
}
