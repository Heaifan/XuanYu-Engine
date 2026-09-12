using Avalonia.Controls;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class AreaDR3InspectorPagerRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public AreaDR3InspectorPagerRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Map_pager_switches_pages_without_replacing_selection()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var result = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            var panel = new MapEditorPanel { DataContext = vm };
            host.Show(panel, 480, 640); panel.UpdateLayout();
            var pager = panel.FindControl<XYPager>("MapPager")!;
            var selected = vm.SelectedProjectItem;
            pager.Select("environment"); panel.UpdateLayout();
            return (pager.SelectedId, Selected: ReferenceEquals(selected, vm.SelectedProjectItem),
                Visible: panel.FindControl<XYPager>("MapPager")?.IsEffectivelyVisible == true);
        });
        Assert.Equal("environment", result.SelectedId);
        Assert.True(result.Selected);
        Assert.True(result.Visible);
    }

    [Fact]
    public void Layer_dock_toggle_preserves_layer_content()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            var dock = new EditorLayerDock { DataContext = vm };
            host.Show(dock, 480, 720); dock.UpdateLayout();
            var pane = dock.FindControl<XYCollapsiblePane>("Pane")!;
            var content = dock.FindControl<Grid>("LayerContent")!;
            pane.IsCollapsed = true; dock.UpdateLayout();
            var collapsed = (Pane: pane.IsCollapsed, Content: content.IsVisible);
            pane.IsCollapsed = false; dock.UpdateLayout();
            return (collapsed, Expanded: content.IsVisible, Rows: UiRuntimeTestHost.Descendants<XYTruncatedText>(dock).Count());
        });
        Assert.True(state.collapsed.Pane, $"pane={state.collapsed.Pane}, content={state.collapsed.Content}");
        Assert.False(state.collapsed.Content);
        Assert.True(state.Expanded);
        Assert.Equal(3, state.Rows);
    }
}
