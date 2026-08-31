using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed class XYToolGroup : Border
{
    public static readonly StyledProperty<bool> IsCollapsedProperty = AvaloniaProperty.Register<XYToolGroup, bool>(nameof(IsCollapsed));
    public bool IsCollapsed { get => GetValue(IsCollapsedProperty); set => SetValue(IsCollapsedProperty, value); }
    public IReadOnlyList<Control> Items { get; }
    public XYIconButton CollapsedTrigger { get; } = new() { Content = new XYIcon { Icon = XyuiVectorIcon.Section, Size = XyuiIconSize.Small } };
    public XYToolGroup(params Control[] items) { Items = items; Classes.Add("xyui-tool-group"); Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) { base.OnPropertyChanged(e); if (e.Property == IsCollapsedProperty) Build(); }
    void Build() { if (IsCollapsed) Child = CollapsedTrigger; else { var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 2 }; foreach (var item in Items) panel.Children.Add(item); Child = panel; } Classes.Set("xyui-tool-group-collapsed", IsCollapsed); }
}
