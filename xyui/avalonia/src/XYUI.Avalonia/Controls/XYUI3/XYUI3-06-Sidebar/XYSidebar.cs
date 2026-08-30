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
    readonly Grid _panel = new() { RowDefinitions = new RowDefinitions("Auto,Auto,Auto,*,Auto") };
    public XYSidebar() { Classes.Add("xyui-sidebar"); Child = _panel; }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) { base.OnPropertyChanged(change); if (change.Property == IsCollapsedProperty) Build(); }
    public void Build()
    {
        _panel.Children.Clear();
        if (IsCollapsed) { _panel.Children.Add(new XYNavigationRail(PrimaryItems)); return; }
        Add(new Border { Classes = { "xyui-sidebar-header" }, Child = new TextBlock { Text = "玄域", Classes = { "xyui-sidebar-title" } } }, 0);
        Add(new XYNavigationMenu(XYNavigationMenu.Group("", PrimaryItems.ToArray())), 1);
        if (ContextItems.Count > 0) Add(Context(), 2);
        Add(new XYSidebarFooter(), 4);
    }
    void Add(Control control, int row) { _panel.Children.Add(control); Grid.SetRow(control, row); }
    Control Context() => new StackPanel { Classes = { "xyui-sidebar-context" }, Children =
    { new TextBlock { Text = "地图内容", Classes = { "xyui-sidebar-context-label" } }, new Border { Child = new TextBlock { Text = "地图基础", Classes = { "xyui-sidebar-context-item" } } }, new TextBlock { Text = "地图环境", Classes = { "xyui-sidebar-context-item" } }, new TextBlock { Text = "数据集", Classes = { "xyui-sidebar-context-item" } }, new TextBlock { Text = "区域与道路", Classes = { "xyui-sidebar-context-item" } } } };
}

sealed class XYSidebarFooter : Border
{
    public XYSidebarFooter() { Classes.Add("xyui-sidebar-footer"); Child = new TextBlock { Text = "设置", Classes = { "xyui-sidebar-footer-label" } }; }
}
