using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3Round3HeadlessContractTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture fx;
    public XYUI3Round3HeadlessContractTests(XyuiHeadlessFixture fixture) => fx = fixture;
    [Fact] public void Pagination_owns_page_truth_and_boundaries() => fx.Run(() =>
    {
        var control = new XYPagination { PageCount = 12, CurrentPage = 6 }; var changed = 0; var invalid = 0;
        control.PageChanged += (_, _) => changed++; control.InvalidPageRequested += (_, _) => invalid++; control.GoTo(7); control.GoTo(99);
        Assert.Equal(7, control.CurrentPage); Assert.Equal(1, changed); Assert.Equal(1, invalid); control.CurrentPage = 99; Assert.Equal(12, control.CurrentPage); Assert.False(control.LastButton.IsEnabled);
    });

    [Fact] public void Steps_forwards_state_and_supports_disabled_visual() => fx.Run(() =>
    {
        var node = new XYStepNode("完成", XYStepState.Completed) { IsClickable = false }; var steps = new XYSteps { Items = { node } }; var changed = 0;
        node.StateChanged += (_, _) => changed++; node.State = XYStepState.Error;
        Assert.Equal(1, changed); Assert.False(node.IsClickable); Assert.Contains("xyui-step-disabled", node.Classes); Assert.Equal(XYStepsOrientation.Horizontal, steps.Orientation);
    });

    [Fact] public void Toolbar_group_and_dropdown_use_real_runtime_controls() => fx.Run(() =>
    {
        var toggle = new XYToolbarTool { Label = "筛选", IsToggle = true }; var dropdown = new XYToolbarTool { Label = "更多", DropdownMenu = new XYMenu(new XYMenuItem { Label = "全部" }) }; var toolbar = new XYToolbar { Items = { toggle, dropdown } }; var group = new XYToolGroup(new XYToolbarTool { Label = "分组" }) { OwnsSeparator = false };
        toggle.Invoke(); Assert.True(toggle.IsSelected); toggle.Invoke(); Assert.False(toggle.IsSelected); dropdown.Invoke(); Assert.True(dropdown.DropdownPopup.IsOpen); group.IsCollapsed = true; Assert.Same(group.CollapsedTrigger, group.Child); Assert.NotNull(toolbar.Child);
    });

    [Fact] public void CommandBar_and_palette_share_declarative_command_contract() => fx.Run(() =>
    {
        var command = new XYCommandItem("删除", "delete", XYCommandRole.Danger) { Shortcut = "Del" }; var bar = new XYCommandBar { Items = { command } }; var requested = 0; bar.CommandRequested += (_, _) => requested++; command.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        var paletteCommand = new XYPaletteCommand("道路"); var palette = new XYCommandPalette(paletteCommand); palette.SearchBox.Text = "道路";
        Assert.Equal(1, requested); Assert.Equal("delete", command.CommandId); Assert.Single(palette.FilteredCommands); palette.Open(); Assert.True(palette.IsOpen); palette.Close(); Assert.False(palette.IsOpen);
    });

    [Fact] public void Tree_status_and_disabled_visuals_are_present() => fx.Run(() =>
    {
        var node = new XYTreeNode { Label = "道路", Badge = "!", Status = XyuiStatusState.Warning, IsEnabled = false }; var badge = node.GetVisualDescendants().OfType<XYStatusBadge>().Single();
        Assert.Equal(XyuiStatusState.Warning, badge.State); Assert.Contains("xyui-tree-disabled", node.Classes); Assert.False(node.IsEnabled);
    });
}
