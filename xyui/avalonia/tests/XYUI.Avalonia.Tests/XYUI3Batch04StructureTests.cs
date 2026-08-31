using Avalonia.Controls;
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
        XyuiBatchTestHost.Prepare(); var toolbar = Assert.IsType<XYToolbar>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.15")); Assert.Contains(toolbar.Items, x => x is XYToolbarTool); var groups = Assert.IsType<XYToolbar>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.16")); var group = Assert.IsType<XYToolGroup>(groups.Items[0]); Assert.All(group.Items, x => Assert.IsType<XYToolbarTool>(x)); group.IsCollapsed = true; Assert.Same(group.CollapsedTrigger, group.Child);
    });
}
