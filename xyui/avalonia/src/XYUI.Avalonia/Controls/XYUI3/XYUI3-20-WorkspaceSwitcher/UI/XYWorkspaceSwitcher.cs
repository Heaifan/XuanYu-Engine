using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed class XYWorkspaceSwitcher : Border
{
    readonly Popup _popup = new() { Placement = PlacementMode.Bottom, IsLightDismissEnabled = true, IsVisible = false };
    readonly StackPanel _items = new();
    public Button Trigger { get; } = new() { Height = 34, HorizontalContentAlignment = HorizontalAlignment.Left };
    public IReadOnlyList<string> Workspaces { get; }
    public string CurrentWorkspace { get; private set; }
    public event EventHandler<string>? WorkspaceChanged;
    public XYWorkspaceSwitcher(string current, params string[] workspaces)
    {
        CurrentWorkspace = current; Workspaces = workspaces.Length == 0 ? [current] : workspaces; Classes.Add("xyui-workspace-switcher"); Trigger.Click += (_, _) => Toggle(); _popup.Child = new Border { Classes = { "xyui-workspace-popup" }, Child = _items }; Child = Build(); Refresh();
    }
    Control Build() { var panel = new StackPanel { Spacing = 2 }; panel.Children.Add(Trigger); panel.Children.Add(_popup); return panel; }
    void Refresh() { Trigger.Content = $"工作区　{CurrentWorkspace}　⌄"; _items.Children.Clear(); foreach (var name in Workspaces) { var item = new Button { Content = name, Height = 32, HorizontalContentAlignment = HorizontalAlignment.Left, Classes = { "xyui-workspace-item" } }; item.Click += (_, _) => SelectWorkspace(name); _items.Children.Add(item); } }
    public void SelectWorkspace(string name) { if (!Workspaces.Contains(name)) return; CurrentWorkspace = name; Refresh(); Close(); WorkspaceChanged?.Invoke(this, name); }
    public void Open() { _popup.IsVisible = true; _popup.PlacementTarget = Trigger; _popup.Width = Bounds.Width > 0 ? Bounds.Width : 224; _popup.IsOpen = true; }
    void Toggle() { _popup.IsVisible = !_popup.IsVisible; _popup.PlacementTarget = Trigger; _popup.Width = Bounds.Width; _popup.IsOpen = _popup.IsVisible; }
    public void Close() { _popup.IsOpen = false; _popup.IsVisible = false; }
}
