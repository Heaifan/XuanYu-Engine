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
    public void Selected_marker_mounts_the_two_dimensional_inspector_panel()
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
            var markerPanel = panel.FindControl<MarkerInspectorPanel>("MarkerInspectorHost")!;
            var vector = markerPanel.FindControl<XYVectorProperty>("PositionProperty")!;
            var apply = UiRuntimeTestHost.Descendants<XYButton>(markerPanel).Single(button => button.Content?.ToString() == "应用位置");
            vector.X = 9; vector.Y = 10; apply.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            return (Visible: markerPanel.IsEffectivelyVisible, Dimension: vector.Dimension,
                X: vector.X, Y: vector.Y, OtherPanels: UiRuntimeTestHost.Descendants<MarkerInspectorPanel>(panel).Count(),
                AppliedX: vm.MarkerInspectorPositionX, AppliedY: vm.MarkerInspectorPositionY);
        });

        Assert.True(state.Visible);
        Assert.Equal(XYVectorDimension.Vector2, state.Dimension);
        Assert.Equal(9, state.X);
        Assert.Equal(10, state.Y);
        Assert.Equal(1, state.OtherPanels);
        Assert.Equal(9, state.AppliedX);
        Assert.Equal(10, state.AppliedY);
    }
}
