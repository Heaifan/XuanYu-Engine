using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public enum XYNavigationDrawerVariant { FullSidebar, Context }
public sealed class XYNavigationDrawerState
{
    public bool IsOpen { get; private set; } public event EventHandler? Changed;
    public void Open() { if (!IsOpen) { IsOpen = true; Changed?.Invoke(this, EventArgs.Empty); } }
    public void Close() { if (IsOpen) { IsOpen = false; Changed?.Invoke(this, EventArgs.Empty); } }
}

public sealed class XYNavigationDrawer : Border
{
    readonly Popup _popup = new() { Placement = PlacementMode.Left, IsLightDismissEnabled = true, Child = new Control() };
    readonly Border _backdrop = new() { Classes = { "xyui-navigation-drawer-backdrop" }, Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
    readonly Grid _overlay = new() { Classes = { "xyui-navigation-drawer-overlay" }, IsVisible = false, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
    readonly Control _content;
    bool _wasOpen;

    public XYNavigationState NavigationState { get; } public XYNavigationDrawerState DrawerState { get; }
    public XYNavigationDrawerVariant Variant { get; } public Popup DrawerPopup => _popup; public Border Backdrop => _backdrop;
    internal Control DrawerSurface => _content; internal Control? Host => ResolveHost();
    public XYButton OpenTrigger { get; } = new() { Content = "☰ 导航", Variant = XyuiButtonVariant.Secondary, Classes = { "xyui-navigation-drawer-trigger" }, HorizontalAlignment = HorizontalAlignment.Left };
    public bool IsOpen => DrawerState.IsOpen; public double DrawerWidth { get; set; } = 280; public event EventHandler? Closed;

    public XYNavigationDrawer(XYNavigationState navigationState, XYNavigationDrawerVariant variant = XYNavigationDrawerVariant.FullSidebar, XYNavigationDrawerState? drawerState = null)
    {
        NavigationState = navigationState; DrawerState = drawerState ?? new XYNavigationDrawerState(); Variant = variant; Classes.Add("xyui-navigation-drawer");
        OpenTrigger.Click += (_, _) => Open();
        _backdrop.PointerPressed += (_, e) => { Close(); e.Handled = true; };
        _content = BuildContent();
        _overlay.Children.Add(_backdrop);
        _overlay.Children.Add(_content);
        _overlay.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Bubble | RoutingStrategies.Tunnel);
        DrawerState.Changed += (_, _) => Sync();
        AttachedToVisualTree += (_, _) => Sync();
        DetachedFromVisualTree += (_, _) => Close();
        Child = new Grid { Children = { OpenTrigger, _popup } };
        Sync();
    }

    Control BuildContent()
    {
        var surface = new Border { Classes = { "xyui-navigation-drawer-surface" }, Width = DrawerWidth, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Stretch, Focusable = true };
        var stack = new Grid { RowDefinitions = new RowDefinitions("Auto,*,Auto") };
        var header = new StackPanel { Margin = new Thickness(16, 16, 16, 12), Spacing = 4, Children = { new TextBlock { Text = "玄域引擎 XuanYu", Classes = { "xyui-navigation-drawer-kicker" } }, new TextBlock { Text = "导航抽屉", Classes = { "xyui-navigation-drawer-title" } } } };
        stack.Children.Add(header);
        Control middle = Variant == XYNavigationDrawerVariant.FullSidebar ? new XYNavigationMenu(NavigationState) { Classes = { "xyui-navigation-drawer-sidebar" } } : new StackPanel { Spacing = 8, Children = { new XYSearchField { Classes = { "xyui-navigation-drawer-search" } }, new XYNavigationMenu(NavigationState) } };
        Grid.SetRow(middle, 1); stack.Children.Add(middle);
        var footerBtn = new XYButton { Content = "收起抽屉", Variant = XyuiButtonVariant.Secondary, Classes = { "xyui-navigation-drawer-footer" }, HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new Thickness(12) };
        footerBtn.Click += (_, _) => Close(); Grid.SetRow(footerBtn, 2); stack.Children.Add(footerBtn);
        surface.Child = stack; return surface;
    }

    Control? ResolveHost() => this.GetVisualAncestors().OfType<Control>().FirstOrDefault(x => x.Classes.Contains("xyui-navigation-drawer-host"));
    void EnsureAttached(Control host)
    {
        if (host.Bounds.Width <= 0 || host.Bounds.Height <= 0) return;
        _overlay.Width = host.Bounds.Width; _overlay.Height = host.Bounds.Height; if (_overlay.GetVisualParent() != null) return;
        if (host is Panel panel) { if (!panel.Children.Contains(_overlay)) panel.Children.Add(_overlay); _overlay.ZIndex = 1000; return; }
        if (host is Decorator decorator)
        {
            if (decorator.Child is Grid l && l.Classes.Contains("xyui-navigation-drawer-host-layer"))
            { if (!l.Children.Contains(_overlay)) l.Children.Add(_overlay); }
            else
            {
                var existing = decorator.Child; decorator.Child = null; var layer = new Grid { Classes = { "xyui-navigation-drawer-host-layer" } };
                if (existing != null) layer.Children.Add(existing); layer.Children.Add(_overlay); decorator.Child = layer;
            }
            _overlay.ZIndex = 1000;
        }
    }

    void Sync()
    {
        var host = ResolveHost();
        _popup.PlacementTarget = host;
        if (host == null || host.Bounds.Width <= 0 || host.Bounds.Height <= 0) { _popup.PlacementTarget = null; if (DrawerState.IsOpen) DrawerState.Close(); _overlay.IsVisible = false; return; }
        _overlay.IsVisible = DrawerState.IsOpen;
        if (DrawerState.IsOpen)
        {
            EnsureAttached(host);
            _wasOpen = true; _content.Focus();
        }
        else if (_wasOpen) { _wasOpen = false; Closed?.Invoke(this, EventArgs.Empty); OpenTrigger.Focus(); }
    }

    public void Open() { var host = ResolveHost(); if (host == null || host.Bounds.Width <= 0 || host.Bounds.Height <= 0) return; EnsureAttached(host); DrawerState.Open(); }
    public void Close() => DrawerState.Close();
    public void SelectDestination(string id) => NavigationState.RequestNavigation(id);
    void OnKeyDown(object? sender, KeyEventArgs e) { if (e.Key == Key.Escape && IsOpen) { Close(); e.Handled = true; } }
}
