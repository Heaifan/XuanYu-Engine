using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
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

public sealed class XYWorkspaceChangeRequest : EventArgs
{
    public XYWorkspaceItem Workspace { get; }
    public bool IsAccepted { get; private set; }
    public bool IsRejected { get; private set; }
    public XYWorkspaceChangeRequest(XYWorkspaceItem workspace) => Workspace = workspace;
    public void Accept() { IsRejected = false; IsAccepted = true; }
    public void Reject() { IsAccepted = false; IsRejected = true; }
}

public sealed partial class XYWorkspaceSwitcher : Border
{
    readonly Popup _popup = new() { Placement = PlacementMode.Bottom, IsLightDismissEnabled = true };
    readonly XYMenu _menu = new() { Classes = { "xyui-workspace-menu" } };
    IActivatableLifetime? _applicationLifetime;
    WindowBase? _hostWindow;
    public XYButton Trigger { get; } = new() { Height = 34, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Stretch, VerticalContentAlignment = VerticalAlignment.Center, Variant = XyuiButtonVariant.Secondary, Classes = { "xyui-workspace-trigger" } };
    public IReadOnlyList<XYWorkspaceItem> Workspaces { get; }
    public XYWorkspaceState State { get; }
    public XYMenu WorkspaceMenu => _menu;
    public Popup WorkspacePopup => _popup;
    public string CurrentWorkspace => Workspaces.FirstOrDefault(x => x.Id == State.CurrentWorkspaceId)?.Label ?? "未命名工作区";
    public event EventHandler<XYWorkspaceChangeRequest>? WorkspaceChangeRequested;
    public event EventHandler<string>? WorkspaceChanged;
    public event EventHandler? ManageRequested;

    public XYWorkspaceSwitcher(string current, params string[] workspaces) : this(CreateState(current, workspaces), CreateItems(current, workspaces)) { }
    public XYWorkspaceSwitcher(XYWorkspaceState state, params XYWorkspaceItem[] workspaces) : this(state, (IEnumerable<XYWorkspaceItem>)workspaces) { }
    public XYWorkspaceSwitcher(XYWorkspaceState state, IEnumerable<XYWorkspaceItem> workspaces)
    {
        State = state; Workspaces = workspaces.ToArray(); Classes.Add("xyui-workspace-switcher");
        Trigger.Click += (_, _) => Toggle(); _popup.Closed += (_, _) => ClosePopup(); _menu.Closed += (_, _) => ClosePopup(); _popup.Child = _menu; Child = Build(); Refresh();
    }
    static XYWorkspaceState CreateState(string current, string[] labels) { var items = CreateItems(current, labels); return new XYWorkspaceState(items.FirstOrDefault(x => x.Label == current)?.Id ?? current); }
    static XYWorkspaceItem[] CreateItems(string current, string[] labels) => (labels.Length == 0 ? [current] : labels).Select(x => new XYWorkspaceItem(Slug(x), x)).ToArray();
    static string Slug(string value) => new(value.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
    Control Build() { var panel = new StackPanel { Spacing = 2, HorizontalAlignment = HorizontalAlignment.Stretch }; panel.Children.Add(Trigger); panel.Children.Add(_popup); return panel; }
}
