using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3WorkspaceSwitcherTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    static XYWorkspaceItem[] Items => [new("map-edit", "地图编辑"), new("data-edit", "数据编辑"), new("war-lab", "战争实验"), new("debug-analysis", "调试分析")];
    public XYUI3WorkspaceSwitcherTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void Trigger_and_popup_share_width_and_trigger_stretches() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var switcher = new XYWorkspaceSwitcher(new XYWorkspaceState("map-edit"), Items); var window = XyuiBatchTestHost.Show(switcher); switcher.Open(); Dispatcher.UIThread.RunJobs(); Assert.IsType<XYButton>(switcher.Trigger); Assert.Equal(switcher.Trigger.Bounds.Width, switcher.WorkspacePopup.Width); Assert.Equal(224, switcher.Trigger.Bounds.Width); Assert.Equal(HorizontalAlignment.Stretch, switcher.Trigger.HorizontalAlignment); window.Close();
    });

    [Fact] public void Items_are_menu_rows_with_right_aligned_selected_check() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var switcher = new XYWorkspaceSwitcher(new XYWorkspaceState("map-edit"), Items); var window = XyuiBatchTestHost.Show(switcher); switcher.Open(); Dispatcher.UIThread.RunJobs(); var rows = switcher.WorkspaceMenu.Items.OfType<XYMenuItem>().Where(x => x.Classes.Contains("xyui-workspace-item")).ToArray(); Assert.Equal(4, rows.Length); Assert.All(rows, row => { Assert.Equal(32, row.Height); Assert.Equal(HorizontalAlignment.Stretch, row.HorizontalAlignment); }); var selected = rows.Single(x => x.Label == "地图编辑"); Assert.Contains("xyui-menu-selected", selected.Classes); var check = selected.GetVisualDescendants().Single(x => x.Classes.Contains("xyui-workspace-check")); Assert.True(check.IsVisible); Assert.True(check.Bounds.Right >= selected.Bounds.Right - 24); window.Close();
    });

    [Fact] public void Manage_item_exists_and_trigger_chevron_is_right_aligned() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var switcher = new XYWorkspaceSwitcher(new XYWorkspaceState("map-edit"), Items); var managed = false; switcher.ManageRequested += (_, _) => managed = true; var window = XyuiBatchTestHost.Show(switcher); switcher.Open(); var manage = switcher.WorkspaceMenu.Items.OfType<XYMenuItem>().Single(x => x.Label == "管理工作区..."); manage.Activate(); Assert.True(managed); var chevron = switcher.Trigger.GetVisualDescendants().Single(x => x is XYIcon); Assert.True(chevron.Bounds.Right >= switcher.Trigger.Bounds.Right - 18); window.Close();
    });

    [Fact] public void Shared_state_and_distinct_ids_are_supported() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var state = new XYWorkspaceState("map-edit"); var first = new XYWorkspaceSwitcher(state, Items); var second = new XYWorkspaceSwitcher(state, Items); first.SelectWorkspace("data-edit"); Assert.Same(state, second.State); Assert.Equal("data-edit", second.State.CurrentWorkspaceId); Assert.NotEqual(Items[0].Id, Items[0].Label); Assert.Equal("数据编辑", second.CurrentWorkspace);
    });

    [Fact] public void Change_request_must_accept_before_commit_and_reject_keeps_state() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var switcher = new XYWorkspaceSwitcher(new XYWorkspaceState("map-edit"), Items); switcher.WorkspaceChangeRequested += (_, _) => { }; switcher.SelectWorkspace("data-edit"); Assert.Equal("map-edit", switcher.State.CurrentWorkspaceId); switcher.WorkspaceChangeRequested += (_, request) => request.Accept(); switcher.SelectWorkspace("data-edit"); Assert.Equal("data-edit", switcher.State.CurrentWorkspaceId); switcher.WorkspaceChangeRequested += (_, request) => request.Reject(); switcher.SelectWorkspace("war-lab"); Assert.Equal("data-edit", switcher.State.CurrentWorkspaceId);
    });

    [Fact] public void Escape_and_detach_close_popup() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var switcher = new XYWorkspaceSwitcher(new XYWorkspaceState("map-edit"), Items); var window = XyuiBatchTestHost.Show(switcher); switcher.Open(); switcher.WorkspaceMenu.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Escape }); Assert.False(switcher.WorkspacePopup.IsOpen); switcher.Open(); window.Close(); Assert.False(switcher.WorkspacePopup.IsOpen);
    });
}
