using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed record XYWorkspaceItem(string Id, string Label);
public sealed class XYWorkspaceState
{
    public string CurrentWorkspaceId { get; private set; }
    public XYWorkspaceState(string id) => CurrentWorkspaceId = id;
    public void Commit(string id) => CurrentWorkspaceId = id;
}

public sealed class XYWorkspaceSwitcher : Border
{
    readonly Popup _popup = new() { Placement = PlacementMode.Bottom, IsLightDismissEnabled = true, IsVisible = false };
    readonly StackPanel _items = new();
    public Button Trigger { get; } = new() { Height = 34, HorizontalContentAlignment = HorizontalAlignment.Left };
    public IReadOnlyList<XYWorkspaceItem> Workspaces { get; }
    public XYWorkspaceState State { get; }
    public string CurrentWorkspace => Workspaces.FirstOrDefault(x => x.Id == State.CurrentWorkspaceId)?.Label ?? "未命名工作区";
    public event EventHandler<string>? WorkspaceChanged;
    public XYWorkspaceSwitcher(string current, params string[] workspaces)
    {
        Workspaces = workspaces.Length == 0 ? [new(current, current)] : workspaces.Select(x => new XYWorkspaceItem(x, x)).ToArray(); State = new XYWorkspaceState(current); Classes.Add("xyui-workspace-switcher"); Trigger.Click += (_, _) => Toggle(); _popup.Closed += (_, _) => Close(); _popup.Child = new Border { Classes = { "xyui-workspace-popup" }, Child = _items }; Child = Build(); Refresh();
    }
    Control Build() { var panel = new StackPanel { Spacing = 2 }; panel.Children.Add(Trigger); panel.Children.Add(_popup); return panel; }
    void Refresh() { var trigger = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6, Children = { new TextBlock { Text = "工作区" }, new TextBlock { Text = CurrentWorkspace, FontWeight = global::Avalonia.Media.FontWeight.SemiBold }, new XYIcon { Icon = XyuiVectorIcon.ChevronDown, Size = XyuiIconSize.Tiny } } }; Trigger.Content = trigger; _items.Children.Clear(); foreach (var workspace in Workspaces) { var item = new Button { Height = 32, HorizontalContentAlignment = HorizontalAlignment.Left, Classes = { "xyui-workspace-item" } }; item.Content = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6, Children = { new TextBlock { Text = workspace.Label }, new XYIcon { Icon = XyuiVectorIcon.Check, Size = XyuiIconSize.Tiny, IsVisible = workspace.Id == State.CurrentWorkspaceId } } }; item.Click += (_, _) => SelectWorkspace(workspace.Id); _items.Children.Add(item); } }
    public void SelectWorkspace(string id) { var item = Workspaces.FirstOrDefault(x => x.Id == id || x.Label == id); if (item is null) return; State.Commit(item.Id); Refresh(); Close(); WorkspaceChanged?.Invoke(this, item.Label); }
    public void Open() { _popup.PlacementTarget = Trigger; _popup.Width = Bounds.Width > 0 ? Bounds.Width : 224; _popup.IsOpen = true; }
    void Toggle() { if (_popup.IsOpen) Close(); else Open(); }
    public void Close() { if (_popup.IsOpen) _popup.IsOpen = false; }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) { Close(); base.OnDetachedFromVisualTree(e); }
}
