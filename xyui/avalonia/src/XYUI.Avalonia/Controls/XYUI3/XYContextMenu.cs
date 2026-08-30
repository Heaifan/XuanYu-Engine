using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public sealed class XYContextMenu : Border
{
    bool _building;
    readonly StackPanel _panel = new();
    public static readonly StyledProperty<string> ContextTypeProperty = AvaloniaProperty.Register<XYContextMenu, string>(nameof(ContextType), "ENTITY");
    public static readonly StyledProperty<string> ContextNameProperty = AvaloniaProperty.Register<XYContextMenu, string>(nameof(ContextName), "");
    XYMenu _menu = new() { IsEmbedded = true };
    public string ContextType { get => GetValue(ContextTypeProperty); set => SetValue(ContextTypeProperty, value); }
    public string ContextName { get => GetValue(ContextNameProperty); set => SetValue(ContextNameProperty, value); }
    public XYMenu Menu { get => _menu; set { _menu = value; _menu.IsEmbedded = true; Build(); } }
    public XYContextMenu() { Classes.Add("xyui-context-menu"); Child = _panel; Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) { base.OnPropertyChanged(change); if (!_building && change.Property != ChildProperty) Build(); }
    void Build()
    {
        _building = true; _panel.Children.Clear(); _panel.Children.Add(Header()); _panel.Children.Add(new XYSeparator { Variant = XyuiSeparatorVariant.Header }); _panel.Children.Add(Menu); _building = false;
    }
    Control Header() => new Border { Classes = { "xyui-context-header" }, Child = new StackPanel { Children = { new TextBlock { Text = ContextType, Classes = { "xyui-context-type" } }, new TextBlock { Text = ContextName, Classes = { "xyui-context-name" } } } } };
}
