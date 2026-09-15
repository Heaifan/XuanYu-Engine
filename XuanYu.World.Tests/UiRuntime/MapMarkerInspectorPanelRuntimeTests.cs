using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using XYUI.Avalonia.Controls;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class MapMarkerInspectorPanelRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public MapMarkerInspectorPanelRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Selected_marker_uses_single_focus_property_content_without_legacy_panel()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, () => true, seedInitialScene: false);
            var marker = new MapMarker(MapMarkerId.New(), vm.MapSession.ActiveRegionLayerId, "运行时标记", new(3, 4));
            Assert.True(vm.MapSession.CreateMarker(marker).IsSuccess);
            vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
            vm.ToggleEditorMode();
            vm.SelectMapGeometry(new(MapGeometryFeatureKind.Marker, marker.MarkerId.ToString()));
            var panel = new InspectorPanel { DataContext = vm };
            host.Show(panel, 420, 620); Dispatcher.UIThread.RunJobs(); panel.UpdateLayout();
            vm.SelectInspectorCategoryCommand.Execute("基础"); Dispatcher.UIThread.RunJobs(); panel.UpdateLayout();
            return (Presenters: UiRuntimeTestHost.Descendants<InspectorReadOnlyValuePresenter>(panel).Count(),
                LegacyPanels: UiRuntimeTestHost.Descendants<MarkerInspectorPanel>(panel).Count(),
                Selectable: UiRuntimeTestHost.Descendants<XYSelectableText>(panel).Count(),
                Categories: vm.InspectorCategories.ToArray());
        });

        Assert.True(state.Presenters > 0);
        Assert.Equal(0, state.LegacyPanels);
        Assert.Equal(0, state.Selectable);
        Assert.Equal("最近", state.Categories[0]);
    }
}
