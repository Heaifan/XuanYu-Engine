using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3Batch04StructureTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3Batch04StructureTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void Pagination_uses_real_input_and_icon_actions() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var host = Assert.IsType<StackPanel>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.13")); var p = Assert.IsType<XYPagination>(host.Children[0]);
        Assert.Equal(3, p.CurrentPage); Assert.Equal(24, p.TotalPages); Assert.IsType<XYNumberField>(p.JumpInput); Assert.IsType<XYIcon>(p.PreviousButton.Content); p.GoTo(4); Assert.Equal(4, p.CurrentPage);
    });

    [Fact] public void Pagination_rejects_invalid_jump_and_clamps_total_pages() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var p = new XYPagination { TotalPages = 24, CurrentPage = 20 }; var invalid = 0; p.InvalidPageRequested += (_, _) => invalid++; p.GoTo(99); Assert.Equal(1, invalid); p.GoTo(0); Assert.Equal(2, invalid); p.TotalPages = 5; Assert.Equal(5, p.CurrentPage);
    });

    [Fact] public void Steps_node_state_is_mutable_and_connector_is_owned_by_steps() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var node = new XYStepNode("数据配置", XYStepState.Current); node.State = XYStepState.Completed; Assert.Equal(XYStepState.Completed, node.State); var steps = new XYSteps(node, new XYStepNode("验证", XYStepState.Pending)); Assert.Contains(steps.GetVisualDescendants().OfType<Border>(), x => x.Classes.Contains("xyui-step-connector"));
    });

    [Fact] public void Pagination_footer_reuses_pagination_and_select() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var footer = Assert.IsType<StackPanel>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.13")); var variant = Assert.IsType<XYPaginationFooter>(footer.Children[2]);
        Assert.IsType<XYPagination>(variant.Pagination); Assert.IsType<XYSelect>(variant.PageSize);
    });

    [Fact] public void Steps_share_state_data_between_orientations() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var panel = Assert.IsType<StackPanel>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.14")); var horizontal = Assert.IsType<XYSteps>(panel.Children[0]); var vertical = Assert.IsType<XYSteps>(panel.Children[1]);
        Assert.Equal(horizontal.Items[2].Label, vertical.Items[2].Label); Assert.Equal(XYStepState.Current, horizontal.Items[2].State); Assert.Equal(XYStepsOrientation.Vertical, vertical.Orientation);
    });

    [Fact] public void Toolbar_and_tool_group_reuse_real_tools() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var toolbar = Assert.IsType<XYToolbar>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.15")); var tool = Assert.IsType<XYToolbarTool>(toolbar.Items[1]); tool.Button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Assert.Equal(tool.ToolId, toolbar.ActiveToolId); var groups = Assert.IsType<XYToolbar>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.16")); var group = Assert.IsType<XYToolGroup>(groups.Items[0]); Assert.All(group.Items, x => Assert.IsType<XYToolbarTool>(x)); group.IsCollapsed = true; Assert.Same(group.CollapsedTrigger, group.Child); group.CollapsedTrigger.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Assert.False(group.IsCollapsed);
    });
}
