using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYWorkspaceSwitcher
{
    void Refresh()
    {
        var triggerGrid = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"), VerticalAlignment = VerticalAlignment.Center };
        triggerGrid.Children.Add(new TextBlock { Text = "工作区", Classes = { "xyui-workspace-secondary" }, VerticalAlignment = VerticalAlignment.Center });
        var current = new TextBlock { Text = CurrentWorkspace, Classes = { "xyui-workspace-current" }, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(12, 0, 8, 0) }; Grid.SetColumn(current, 1); triggerGrid.Children.Add(current);
        var chevron = new XYIcon { Icon = XyuiVectorIcon.ChevronDown, Size = XyuiIconSize.Tiny, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center }; Grid.SetColumn(chevron, 2); triggerGrid.Children.Add(chevron); Trigger.Content = triggerGrid;
        var items = Workspaces.Select(WorkspaceItem).Cast<Control>().Append(XYMenu.Separator()).Append(ManageItem()).ToArray(); _menu.Items = items; _menu.Width = Width > 0 ? Width : 224; _menu.ApplyOverlayStyling();
    }
    XYMenuItem WorkspaceItem(XYWorkspaceItem workspace)
    {
        var item = new XYMenuItem { Label = workspace.Label, IsSelected = workspace.Id == State.CurrentWorkspaceId, Classes = { "xyui-workspace-item" } };
        item.SelectionRequested += (_, _) => SelectWorkspace(workspace.Id); return item;
    }
    XYMenuItem ManageItem()
    {
        var item = new XYMenuItem { Label = "管理工作区...", Classes = { "xyui-workspace-manage" } };
        item.SelectionRequested += (_, _) => { ClosePopup(); ManageRequested?.Invoke(this, EventArgs.Empty); }; return item;
    }
    public void SelectWorkspace(string id)
    {
        var item = Workspaces.FirstOrDefault(x => x.Id == id || x.Label == id); if (item is null) return;
        if (WorkspaceChangeRequested is null) { CommitWorkspace(item.Id); return; }
        var request = new XYWorkspaceChangeRequest(item); WorkspaceChangeRequested.Invoke(this, request); if (request.IsAccepted) CommitWorkspace(item.Id);
    }
    public void CommitWorkspace(string id)
    {
        var item = Workspaces.FirstOrDefault(x => x.Id == id); if (item is null) return; State.Commit(item.Id); Refresh(); ClosePopup(); WorkspaceChanged?.Invoke(this, item.Id);
    }
    public void Open()
    {
        _popup.PlacementTarget = Trigger; _popup.Width = Trigger.Bounds.Width > 0 ? Trigger.Bounds.Width : Width > 0 ? Width : 224; _menu.Width = _popup.Width; _popup.IsOpen = true; _menu.Open(); foreach (var item in _menu.Items.OfType<XYMenuItem>()) item.IsSelected = Workspaces.Any(x => x.Label == item.Label && x.Id == State.CurrentWorkspaceId);
    }
    void Toggle() { if (_popup.IsOpen) ClosePopup(); else Open(); }
    public void ClosePopup() { if (_popup.IsOpen) _popup.IsOpen = false; _menu.Close(); }
    protected override void OnKeyDown(KeyEventArgs e) { if (e.Key == Key.Escape) { ClosePopup(); e.Handled = true; return; } base.OnKeyDown(e); }
}
