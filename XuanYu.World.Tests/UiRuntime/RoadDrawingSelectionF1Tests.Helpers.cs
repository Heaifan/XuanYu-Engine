using XuanYu.Core.Space;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class RoadDrawingSelectionF1Tests
{
    async Task<UiVm> CompleteAsync()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(Path.Combine(_root, "map.json")));
        vm.DatasetCreateType = MapDatasetTypes.Road;
        Assert.True(await vm.CreateDatasetAsync());
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor); vm.ToggleEditorMode();
        vm.SelectRegionAuthoringMode("道路"); Assert.True(await vm.BeginRoadDrawingAsync());
        AddNodes(vm, new(-1, -1), new(1, 1)); Assert.True(vm.CompleteRoadDrawing());
        return vm;
    }

    static void AddNodes(UiVm vm, MapPoint first, MapPoint second)
    {
        Assert.True(vm.RoadDrawingPointerPressed(Screen(vm, first).X, Screen(vm, first).Y, Viewport));
        Assert.True(vm.RoadDrawingPointerPressed(Screen(vm, second).X, Screen(vm, second).Y, Viewport));
    }

    static (double X, double Y) Screen(UiVm vm, MapPoint point)
    {
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var world = MapCoordinateContract.MapToWorld(point, vm.MapSession.CurrentMap.Surface.BaseHeightMeters);
        var screen = projection.ProjectWorldPoint(world);
        return (screen.X, screen.Y);
    }
}
