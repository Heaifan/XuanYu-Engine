using XuanYu.Core.Space;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class MapMarkerInspectorViewportTests : IDisposable
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-marker-viewport-{Guid.NewGuid():N}");

    [Fact]
    public async Task Viewport_drag_updates_the_selected_marker_inspector()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(Path.Combine(_root, "map.json")));
        vm.DatasetCreateType = MapDatasetTypes.Marker; Assert.True(await vm.CreateDatasetAsync());
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor); vm.ToggleEditorMode();
        vm.SelectRegionAuthoringMode("地图标记"); vm.SelectToolCommand.Execute("选择");
        var marker = new MapMarker(MapMarkerId.New(), vm.MapSession.ActiveRegionLayerId, "拖动标记", new(0, 0));
        Assert.True(vm.MapSession.CreateMarker(marker).IsSuccess);
        vm.SelectMapGeometry(new(XuanYu.Editor.MapEditing.MapGeometryFeatureKind.Marker, marker.MarkerId.ToString()));
        vm.IsGeometryEditingActive = true;
        var target = Screen(vm, new(6, 7));

        var source = Screen(vm, marker.Position);
        Assert.True(vm.TryBeginMapGeometryVertexPointer(source.X, source.Y, Viewport));
        Assert.True(vm.PreviewMapGeometryPointer(target.X, target.Y, Viewport));
        Assert.True(vm.CommitMapGeometryPointer(target.X, target.Y, Viewport));

        var position = vm.MapSession.CurrentMap.Markers.Single().Position;
        Assert.InRange(Math.Abs(position.X - 6), 0, 0.001);
        Assert.InRange(Math.Abs(position.Y - 7), 0, 0.001);
        Assert.InRange(Math.Abs(vm.MarkerInspectorPositionX - 6), 0, 0.001);
        Assert.InRange(Math.Abs(vm.MarkerInspectorPositionY - 7), 0, 0.001);
    }

    static (double X, double Y) Screen(UiVm vm, MapPoint point)
    {
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var world = MapCoordinateContract.MapToWorld(point, vm.MapSession.CurrentMap.Surface.BaseHeightMeters);
        var screen = projection.ProjectWorldPoint(world);
        return (screen.X, screen.Y);
    }

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, true);
    }
}
