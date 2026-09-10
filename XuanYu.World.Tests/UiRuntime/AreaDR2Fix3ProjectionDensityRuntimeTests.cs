using Avalonia.Controls;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class AreaDR2Fix3ProjectionDensityRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public AreaDR2Fix3ProjectionDensityRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Fresh_map_layer_dock_projects_all_real_default_layers()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var names = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            var right = new Right { DataContext = vm }; host.Show(right, 480, 720); right.UpdateLayout();
            var panel = UiRuntimeTestHost.Descendants<LayerPanel>(right).Single();
            return UiRuntimeTestHost.Descendants<XYTruncatedText>(panel).Select(x => x.Text ?? "").ToArray();
        });
        Assert.Equal(["区域 1", "边界", "地面"], names);
    }

    [Fact]
    public void Entity_header_is_not_measured_without_entity_owner()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var visibility = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            var panel = new InspectorPanel { DataContext = vm }; host.Show(panel, 480, 720); panel.UpdateLayout();
            var map = panel.FindControl<Grid>("EntityHeader")!.IsEffectivelyVisible;
            vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor); panel.UpdateLayout();
            return (Map: map, Region: panel.FindControl<Grid>("EntityHeader")!.IsEffectivelyVisible);
        });
        Assert.False(visibility.Map); Assert.False(visibility.Region);
    }

    [Theory]
    [InlineData(300)]
    [InlineData(480)]
    public void Expanded_layer_dock_allocates_space_for_several_rows(double width)
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            var right = new Right { DataContext = vm }; host.Show(right, width, 720); right.UpdateLayout();
            var dock = right.FindControl<EditorLayerDock>("LayerWorkspace")!;
            return (Height: dock.Bounds.Height, Rows: UiRuntimeTestHost.Descendants<XYTruncatedText>(dock).Count());
        });
        Assert.True(state.Height >= 192); Assert.Equal(3, state.Rows);
    }

    [Fact]
    public void Map_inspector_uses_engine_compact_typography_tokens()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var sizes = host.Run(() =>
        {
            var form = new MapEditorPanel { DataContext = new UiVm(null, seedInitialScene: false) };
            host.Show(form, 480, 640); form.UpdateLayout();
            return (Labels: UiRuntimeTestHost.Descendants<XYLabel>(form).Select(x => x.FontSize).Distinct().ToArray(),
                Body: UiRuntimeTestHost.Descendants<XYText>(form).Select(x => x.FontSize).Distinct().ToArray(),
                Sections: UiRuntimeTestHost.Descendants<XYSectionTitle>(form)
                    .SelectMany(x => UiRuntimeTestHost.Descendants<TextBlock>(x).Select(t => t.FontSize))
                    .Distinct().ToArray());
        });
        Assert.Contains(12, sizes.Labels); Assert.Contains(13, sizes.Body); Assert.Contains(14, sizes.Sections);
    }

    [Fact]
    public void Map_layer_eye_and_lock_actions_update_real_rows()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            var panel = new LayerPanel { DataContext = vm }; host.Show(panel, 480, 360); panel.UpdateLayout();
            var row = vm.LayerItems.First(x => x.IsSystem);
            row.IsVisible = false; row.IsLocked = true; panel.UpdateLayout();
            return (Visible: row.IsVisible, Locked: row.IsLocked);
        });
        Assert.False(state.Visible); Assert.True(state.Locked);
    }
}
