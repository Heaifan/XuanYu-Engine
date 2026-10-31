using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYContextMenu : Border
{
    bool _building;
    readonly Grid _host = new();
    readonly StackPanel _surface = new();
    public static readonly StyledProperty<string> ContextTypeProperty = AvaloniaProperty.Register<XYContextMenu, string>(nameof(ContextType), "ENTITY");
    public static readonly StyledProperty<string> ContextNameProperty = AvaloniaProperty.Register<XYContextMenu, string>(nameof(ContextName), "");
    XYMenu _menu = new() { IsEmbedded = true };
    public string ContextType { get => GetValue(ContextTypeProperty); set => SetValue(ContextTypeProperty, value); }
    public string ContextName { get => GetValue(ContextNameProperty); set => SetValue(ContextNameProperty, value); }
    public XYMenu Menu { get => _menu; set { _menu.Closed -= OnMenuClosed; _menu = value; _menu.IsEmbedded = true; _menu.Closed += OnMenuClosed; Build(); } }
    public XYContextMenu() { Classes.Add("xyui-context-menu"); Child = _host; _host.Children.Add(_surface); _menu.Closed += OnMenuClosed; Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) { base.OnPropertyChanged(change); if (!_building && change.Property != ChildProperty) Build(); }
    void Build()
    {
        _building = true; _surface.Children.Clear(); _surface.Children.Add(Header()); _surface.Children.Add(new XYSeparator { Variant = XyuiSeparatorVariant.Header }); _surface.Children.Add(Menu); _building = false;
    }
}
