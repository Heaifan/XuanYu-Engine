using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public enum XYCommandRole { Normal, Primary, Danger }
public enum XYCommandBarVariant { Standard, Contextual }

public sealed class XYCommandItem : XYButton
{
    public string CommandId { get; }
    public XYCommandRole Role { get; }
    public XyuiVectorIcon? Icon { get; }
    public event EventHandler? ExecuteRequested;
    public XYCommandItem(string label, string? commandId = null, XYCommandRole role = XYCommandRole.Normal, XyuiVectorIcon? icon = null)
    {
        CommandId = commandId ?? label; Role = role; Icon = icon; Variant = role switch { XYCommandRole.Primary => XyuiButtonVariant.Primary, XYCommandRole.Danger => XyuiButtonVariant.Danger, _ => XyuiButtonVariant.Secondary }; Content = Visual(label); Classes.Add("xyui-command-item"); Classes.Add(role == XYCommandRole.Primary ? "xyui-command-primary" : role == XYCommandRole.Danger ? "xyui-command-danger" : "xyui-command-normal");
        Click += (_, _) => { if (IsEnabled) ExecuteRequested?.Invoke(this, EventArgs.Empty); };
    }
    Control Visual(string label) => Icon is { } icon ? new StackPanel { Orientation = Orientation.Horizontal, Spacing = 5, Children = { new XYIcon { Icon = icon, Size = XyuiIconSize.Small }, new TextBlock { Text = label } } } : new TextBlock { Text = label };
}

public sealed class XYCommandBar : Border
{
    readonly Popup _popup = new() { Placement = PlacementMode.Bottom, IsLightDismissEnabled = true };
    IActivatableLifetime? _applicationLifetime;
    WindowBase? _hostWindow;
    public IReadOnlyList<XYCommandItem> Items { get; private set; }
    public XYMenu MoreMenu { get; } = new();
    public Popup MorePopup => _popup;
    public XYIconButton MoreButton { get; }
    public XYCommandBarVariant Variant { get; private set; }
    public string ContextIdentity { get; private set; } = "";
    public event EventHandler<XYCommandItem>? CommandExecuted;
    public XYCommandBar(params XYCommandItem[] items) : this(XYCommandBarVariant.Standard, "", items) { }
    public XYCommandBar(XYCommandBarVariant variant, string contextIdentity, params XYCommandItem[] items)
    {
        Items = items; Variant = variant; ContextIdentity = contextIdentity; Height = 34; HorizontalAlignment = HorizontalAlignment.Stretch; Classes.Add("xyui-command-bar"); MoreButton = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.MoreHorizontal, Size = XyuiIconSize.Small }, Classes = { "xyui-command-more" } };
        foreach (var item in Items) Attach(item); MoreButton.Click += (_, _) => ToggleMore(); MoreButton.KeyDown += OnMoreKeyDown; _popup.Closed += (_, _) => CloseMore(); MoreMenu.Closed += (_, _) => CloseMore(); _popup.Child = MoreMenu; Child = Build(); RefreshMore();
    }
    Control Build()
    {
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto") }; var commands = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 2, Margin = new Thickness(4, 3, 0, 3) };
        if (Variant == XYCommandBarVariant.Contextual) { commands.Children.Add(new TextBlock { Text = "已选择 ·", VerticalAlignment = VerticalAlignment.Center, Classes = { "xyui-command-context" } }); commands.Children.Add(new TextBlock { Text = ContextIdentity, VerticalAlignment = VerticalAlignment.Center, Classes = { "xyui-command-context-name" } }); commands.Children.Add(new Border { Width = 1, Height = 20, Classes = { "xyui-command-divider" } }); }
        foreach (var item in Items) { if (item.Role == XYCommandRole.Danger) commands.Children.Add(new Border { Width = 1, Height = 20, Classes = { "xyui-command-divider" } }); item.Height = 28; commands.Children.Add(item); }
        grid.Children.Add(commands); Grid.SetColumn(MoreButton, 2); MoreButton.Width = 28; MoreButton.Height = 28; grid.Children.Add(MoreButton); return new Border { Classes = { "xyui-command-bar-surface" }, Child = grid };
    }
    void ToggleMore() { if (_popup.IsOpen) CloseMore(); else { _popup.PlacementTarget = MoreButton; _popup.IsOpen = true; } }
    public void CloseMore() { if (_popup.IsOpen) _popup.IsOpen = false; }
    public void RefreshMore() => MoreButton.IsVisible = MoreMenu.Items.Any();
    public void UpdateContext(string identity, params XYCommandItem[] commands) { ContextIdentity = identity; foreach (var item in Items) Detach(item); Items = commands; foreach (var item in Items) Attach(item); if (Variant == XYCommandBarVariant.Contextual) Child = Build(); }
    void Attach(XYCommandItem item) => item.ExecuteRequested += OnItemExecuted;
    void Detach(XYCommandItem item) => item.ExecuteRequested -= OnItemExecuted;
    void OnItemExecuted(object? sender, EventArgs e) { if (sender is XYCommandItem item) CommandExecuted?.Invoke(this, item); }
    void OnMoreKeyDown(object? sender, KeyEventArgs e) { if (e.Key == Key.Escape) { CloseMore(); e.Handled = true; } }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) { base.OnAttachedToVisualTree(e); _applicationLifetime = Application.Current?.ApplicationLifetime as IActivatableLifetime; if (_applicationLifetime is not null) _applicationLifetime.Deactivated += OnDeactivated; _hostWindow = e.RootVisual as WindowBase; if (_hostWindow is not null) _hostWindow.Deactivated += OnDeactivated; }
    void OnDeactivated(object? sender, EventArgs e) => CloseMore();
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) { if (_applicationLifetime is not null) _applicationLifetime.Deactivated -= OnDeactivated; if (_hostWindow is not null) _hostWindow.Deactivated -= OnDeactivated; _applicationLifetime = null; _hostWindow = null; CloseMore(); base.OnDetachedFromVisualTree(e); }
}
