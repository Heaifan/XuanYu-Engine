using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.VisualTree;
using XYUI.Avalonia.Vector;
using Avalonia.Metadata;
using System.Collections.ObjectModel;

namespace XYUI.Avalonia.Controls;

public enum XYCommandRole { Normal, Primary, Danger }
public enum XYCommandBarVariant { Standard, Contextual }

public sealed class XYCommandItem : XYButton
{
    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<XYCommandItem, bool>(nameof(IsSelected));
    string _commandId = ""; XYCommandRole _role; XyuiVectorIcon? _icon; string _label = "";
    public string CommandId { get => _commandId; set => _commandId = value; }
    public XYCommandRole Role { get => _role; set { _role = value; Variant = value switch { XYCommandRole.Primary => XyuiButtonVariant.Primary, XYCommandRole.Danger => XyuiButtonVariant.Danger, _ => XyuiButtonVariant.Secondary }; SyncVisual(); } }
    public new XyuiVectorIcon? Icon { get => _icon; set { _icon = value; SyncVisual(); } }
    public string Label { get => _label; set { _label = value; SyncVisual(); } }
    public string? Shortcut { get; set; }
    public bool IsDestructive { get => Role == XYCommandRole.Danger; set { if (value) Role = XYCommandRole.Danger; } }
    public bool IsSelected { get => GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }
    public event EventHandler? ExecuteRequested;
    public XYCommandItem() : this("") { }
    public XYCommandItem(string label, string? commandId = null, XYCommandRole role = XYCommandRole.Normal, XyuiVectorIcon? icon = null)
    {
        _label = label; _commandId = commandId ?? label; _role = role; _icon = icon; Variant = role switch { XYCommandRole.Primary => XyuiButtonVariant.Primary, XYCommandRole.Danger => XyuiButtonVariant.Danger, _ => XyuiButtonVariant.Secondary }; VerticalAlignment = VerticalAlignment.Center; HorizontalContentAlignment = HorizontalAlignment.Left; VerticalContentAlignment = VerticalAlignment.Center; Classes.Add("xyui-command-item"); SyncVisual(); Click += (_, _) => { if (IsEnabled) ExecuteRequested?.Invoke(this, EventArgs.Empty); };
    }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsSelectedProperty) Classes.Set("xyui-command-selected", e.GetNewValue<bool>());
    }
    void SyncVisual() { base.Icon = null; Content = _icon is null ? new TextBlock { Text = Label, Classes = { "xyui-command-label" } } : new StackPanel { Orientation = Orientation.Horizontal, Spacing = 5, Children = { new XYIcon { Icon = _icon.Value, Size = XyuiIconSize.Small }, new TextBlock { Text = Label, Classes = { "xyui-command-label" } } } }; Classes.Set("xyui-command-primary", Role == XYCommandRole.Primary); Classes.Set("xyui-command-danger", Role == XYCommandRole.Danger); Classes.Set("xyui-command-normal", Role == XYCommandRole.Normal); }
}

public sealed class XYCommandBar : Border
{
    readonly Popup _popup = new() { Placement = PlacementMode.Bottom, IsLightDismissEnabled = true };
    IActivatableLifetime? _applicationLifetime;
    WindowBase? _hostWindow;
    [Content] public ObservableCollection<XYCommandItem> Items { get; } = [];
    public IReadOnlyList<XYCommandItem> PrimaryCommands => Items;
    public ObservableCollection<XYCommandItem> SecondaryCommands { get; } = [];
    public ObservableCollection<XYCommandItem> OverflowCommands { get; } = [];
    public XYMenu MoreMenu { get; } = new();
    public Popup MorePopup => _popup;
    public XYIconButton MoreButton { get; }
    public XYCommandBarVariant Variant { get; set; }
    public string ContextIdentity { get; set; } = "";
    public XYCommandItem? SelectedItem { get; private set; }
    public event EventHandler<XYCommandItem>? CommandExecuted;
    public event EventHandler<XYCommandItem>? CommandRequested;
    public XYCommandBar() : this(XYCommandBarVariant.Standard, "", Array.Empty<XYCommandItem>()) { }
    public XYCommandBar(params XYCommandItem[] items) : this(XYCommandBarVariant.Standard, "", items) { }
    public XYCommandBar(XYCommandBarVariant variant, string contextIdentity, params XYCommandItem[] items)
    {
        Variant = variant; ContextIdentity = contextIdentity; foreach (var item in items) Items.Add(item); Items.CollectionChanged += (_, _) => Rebuild(); SecondaryCommands.CollectionChanged += (_, _) => Rebuild(); OverflowCommands.CollectionChanged += (_, _) => RefreshMore(); Height = 34; HorizontalAlignment = HorizontalAlignment.Stretch; Classes.Add("xyui-command-bar"); MoreButton = new XYIconButton { VerticalAlignment = VerticalAlignment.Center, Content = new XYIcon { Icon = XyuiVectorIcon.MoreHorizontal, Size = XyuiIconSize.Small, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }, Classes = { "xyui-command-more" } };
        foreach (var item in Items) Attach(item); MoreButton.Click += (_, _) => ToggleMore(); MoreButton.KeyDown += OnMoreKeyDown; _popup.Closed += (_, _) => CloseMore(); MoreMenu.Closed += (_, _) => CloseMore(); _popup.Child = MoreMenu; Child = Build(); RefreshMore();
    }
    Control Build()
    {
        foreach (var item in Items.Concat(SecondaryCommands)) if (item.GetVisualParent() is Panel panel) panel.Children.Remove(item);
        if (MoreButton.GetVisualParent() is Panel morePanel) morePanel.Children.Remove(MoreButton);
        Child = null;
        var grid = new Grid { Height = 28, VerticalAlignment = VerticalAlignment.Center, ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto") }; var commands = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 2, Height = 28, VerticalAlignment = VerticalAlignment.Center };
        if (Variant == XYCommandBarVariant.Contextual) { commands.Children.Add(new TextBlock { Text = "已选择 ·", VerticalAlignment = VerticalAlignment.Center, Classes = { "xyui-command-context" } }); commands.Children.Add(new TextBlock { Text = ContextIdentity, VerticalAlignment = VerticalAlignment.Center, Classes = { "xyui-command-context-name" } }); commands.Children.Add(new Border { Width = 1, Height = 20, Classes = { "xyui-command-divider" } }); }
        foreach (var item in Items.Concat(SecondaryCommands)) { if (item.Role == XYCommandRole.Danger) commands.Children.Add(new Border { Width = 1, Height = 20, Classes = { "xyui-command-divider" } }); item.Height = 28; commands.Children.Add(item); }
        grid.Children.Add(commands); Grid.SetColumn(MoreButton, 2); MoreButton.Width = 28; MoreButton.Height = 28; grid.Children.Add(MoreButton); return new Border { Classes = { "xyui-command-bar-surface" }, Child = grid };
    }
    void ToggleMore() { if (_popup.IsOpen) CloseMore(); else { _popup.PlacementTarget = MoreButton; _popup.IsOpen = true; MoreMenu.ApplyOverlayStyling(); MoreMenu.Open(); } }
    public void CloseMore() { if (_popup.IsOpen) _popup.IsOpen = false; }
    public void RefreshMore() { if (OverflowCommands.Count == 0) { MoreButton.IsVisible = MoreMenu.Items.Any(); return; } var items = MoreMenu.Items.ToList(); foreach (var command in OverflowCommands) if (!items.OfType<XYMenuItem>().Any(x => x.Label == command.Label)) { var item = new XYMenuItem { Label = command.Label, IsEnabled = command.IsEnabled }; item.SelectionRequested += (_, _) => ExecuteOverflow(command); items.Add(item); } MoreMenu.Child = null; MoreMenu.Items = items.ToArray(); MoreButton.IsVisible = MoreMenu.Items.Any(); }
    void ExecuteOverflow(XYCommandItem item) { if (item.IsEnabled) { SelectedItem = item; CommandRequested?.Invoke(this, item); CommandExecuted?.Invoke(this, item); } }
    void Rebuild() { foreach (var item in Items) Attach(item); Child = Build(); RefreshMore(); }
    public void UpdateContext(string identity, params XYCommandItem[] commands) { ContextIdentity = identity; foreach (var item in Items) Detach(item); Items.Clear(); foreach (var item in commands) Items.Add(item); SelectedItem = null; }
    void Attach(XYCommandItem item) => item.ExecuteRequested += OnItemExecuted;
    void Detach(XYCommandItem item) => item.ExecuteRequested -= OnItemExecuted;
    void OnItemExecuted(object? sender, EventArgs e) { if (sender is not XYCommandItem item) return; foreach (var candidate in Items.Concat(SecondaryCommands)) candidate.IsSelected = ReferenceEquals(candidate, item); SelectedItem = item; CommandRequested?.Invoke(this, item); CommandExecuted?.Invoke(this, item); }
    void OnMoreKeyDown(object? sender, KeyEventArgs e) { if (e.Key == Key.Escape) { CloseMore(); e.Handled = true; } }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) { base.OnAttachedToVisualTree(e); _applicationLifetime = Application.Current?.ApplicationLifetime as IActivatableLifetime; if (_applicationLifetime is not null) _applicationLifetime.Deactivated += OnDeactivated; _hostWindow = e.RootVisual as WindowBase; if (_hostWindow is not null) _hostWindow.Deactivated += OnDeactivated; }
    void OnDeactivated(object? sender, EventArgs e) => CloseMore();
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) { if (_applicationLifetime is not null) _applicationLifetime.Deactivated -= OnDeactivated; if (_hostWindow is not null) _hostWindow.Deactivated -= OnDeactivated; _applicationLifetime = null; _hostWindow = null; CloseMore(); base.OnDetachedFromVisualTree(e); }
}
