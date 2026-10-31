using Avalonia.Controls;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYNavigationMenu : Border
{
    IReadOnlyList<XYNavigationGroup> _groups = [];
    readonly StackPanel _panel = new() { Spacing = 4 };
    XYNavigationState _state = new([]);
    public IReadOnlyList<XYNavigationGroup> Groups { get => _groups; set { _groups = value; UseGroups(); } }
    public IReadOnlyList<XYNavigationItem> Items => _panel.Children.OfType<XYNavigationItem>().ToArray();
    public XYNavigationState NavigationState { get => _state; set { Unsubscribe(); _state = value; Subscribe(); Build(); } }
    public string? CurrentDestinationId => NavigationState.CurrentDestinationId;
    public string? SelectedId { get => CurrentDestinationId; set => NavigationState.Select(value); }
    public event EventHandler<XYNavigationRequest>? NavigationRequested;
    public event EventHandler<XYNavigationItem>? SelectionChanged;
    public XYNavigationMenu() { Classes.Add("xyui-navigation-menu"); Child = _panel; Subscribe(); }
    public XYNavigationMenu(params XYNavigationGroup[] groups) : this() => Groups = groups;
    public XYNavigationMenu(XYNavigationState state) : this() => NavigationState = state;
    void UseGroups() { Unsubscribe(); _state = new(_groups.SelectMany(x => x.Items).Select(ToEntry)); Subscribe(); Build(); }
    static XYNavigationEntry ToEntry(XYNavigationItem item) => new(item.Id, item.Label, item.Icon, item.Badge, item.Status, item.IsEnabled);
    void Subscribe() { _state.Changed += OnStateChanged; _state.NavigationRequested += OnStateNavigationRequested; }
    void Unsubscribe() { _state.Changed -= OnStateChanged; _state.NavigationRequested -= OnStateNavigationRequested; }
    void Build()
    {
        _panel.Children.Clear();
        var groups = _groups.Count > 0 ? _groups : [new XYNavigationGroup("", NavigationState.Entries.Select(Create).ToArray())];
        foreach (var group in groups)
        {
            if (_panel.Children.Count > 0) _panel.Children.Add(new XYSeparator { Variant = XyuiSeparatorVariant.Section, Classes = { "xyui-navigation-separator" } });
            if (!string.IsNullOrEmpty(group.Label)) _panel.Children.Add(new TextBlock { Text = group.Label, Classes = { "xyui-navigation-group" } });
            foreach (var item in group.Items) Attach(item);
        }
    }
    XYNavigationItem Create(XYNavigationEntry entry) => new() { Id = entry.Id, Label = entry.Label, Icon = entry.Icon, Badge = entry.Badge, Status = entry.Status, IsEnabled = entry.IsEnabled };
    void Attach(XYNavigationItem item)
    {
        if (item.GetVisualParent() is not null) item = Clone(item);
        item.IsSelected = item.Id == CurrentDestinationId; item.Selected -= OnSelected; item.Selected += OnSelected; _panel.Children.Add(item);
    }
    static XYNavigationItem Clone(XYNavigationItem item) => new() { Id = item.Id, Label = item.Label, Icon = item.Icon, Badge = item.Badge, Status = item.Status, IsEnabled = item.IsEnabled, IsSelected = item.IsSelected };
    void OnStateChanged(object? sender, EventArgs e) { foreach (var item in Items) item.IsSelected = item.Id == CurrentDestinationId; }
    void OnStateNavigationRequested(object? sender, XYNavigationRequest request) => NavigationRequested?.Invoke(this, request);
    void OnSelected(object? sender, EventArgs e)
    {
        if (sender is not XYNavigationItem item || !NavigationState.RequestNavigation(item.Id)) { OnStateChanged(this, EventArgs.Empty); return; }
        SelectionChanged?.Invoke(this, item);
    }
    public bool SelectDestination(string id) => NavigationState.RequestNavigation(id);
    public static XYNavigationGroup Group(string label, params XYNavigationItem[] items) => new(label, items);
}

public sealed record XYNavigationGroup(string Label, IReadOnlyList<XYNavigationItem> Items);
