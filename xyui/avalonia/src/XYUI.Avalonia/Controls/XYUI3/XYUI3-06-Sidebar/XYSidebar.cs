using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed class XYSidebar : Border
{
    public static readonly StyledProperty<bool> IsCollapsedProperty = AvaloniaProperty.Register<XYSidebar, bool>(nameof(IsCollapsed));
    public IReadOnlyList<XYNavigationItem> PrimaryItems { get; set; } = [];
    public IReadOnlyList<XYNavigationItem> ContextItems { get; set; } = [];
    public bool IsCollapsed { get => GetValue(IsCollapsedProperty); set => SetValue(IsCollapsedProperty, value); }
    readonly StackPanel _panel = new();
    public XYSidebar() { Classes.Add("xyui-sidebar"); Child = _panel; }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) { base.OnPropertyChanged(change); if (change.Property == IsCollapsedProperty) Build(); }
    public void Build()
    {
        _panel.Children.Clear();
        if (IsCollapsed) { _panel.Children.Add(new XYNavigationRail(PrimaryItems)); return; }
        _panel.Children.Add(new Border { Classes = { "xyui-sidebar-header" }, Child = new TextBlock { Text = "玄域", Classes = { "xyui-sidebar-title" } } });
        _panel.Children.Add(new XYNavigationMenu(XYNavigationMenu.Group("", PrimaryItems.ToArray())));
        if (ContextItems.Count > 0) _panel.Children.Add(new XYNavigationMenu(XYNavigationMenu.Group("地图内容", ContextItems.ToArray())));
        _panel.Children.Add(new XYSeparator { Variant = XyuiSeparatorVariant.Section });
        _panel.Children.Add(new XYNavigationItem { Id = "settings", Label = "设置", Icon = XyuiVectorIcon.Section });
    }
}
