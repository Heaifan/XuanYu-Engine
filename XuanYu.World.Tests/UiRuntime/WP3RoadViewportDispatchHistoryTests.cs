using XuanYu.Core.Space;
using XuanYu.Editor.Input;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.VectorOverlay;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class WP3RoadViewportDispatchHistoryTests : IDisposable
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xye-wp3-road-dispatch-{Guid.NewGuid():N}");

    [Fact]
    public async Task Production_router_completes_road_after_multiclick_and_map_undo_redo_restores_it()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(Path.Combine(_root, "map.json")));
        vm.DatasetCreateType = MapDatasetTypes.Road;
        Assert.True(await vm.CreateDatasetAsync());
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        vm.ToggleEditorMode();
        vm.SelectRegionAuthoringMode("道路");
        vm.SelectToolCommand.Execute("道路绘制");
        vm.UpdateViewportFrame(800, 600);

        var first = FindHit(vm, 350, 300);
        var second = FindHit(vm, 450, 300);
        DispatchClick(vm, first.X, first.Y, 1);
        Assert.True(vm.IsRoadDrawingDraftActive);
        Assert.Equal(1, vm.RoadDrawingDraftPointCount);
        Assert.Equal(0, vm.RoadContentCount);

        DispatchClick(vm, second.X, second.Y, 2);
        Assert.Equal(2, vm.RoadDrawingDraftPointCount);
        Assert.Equal(0, vm.RoadContentCount);
        Assert.True(vm.CompleteRoadDrawing());
        Assert.False(vm.IsRoadDrawingDraftActive);
        Assert.Equal(1, vm.RoadContentCount);
        AssertRoadOverlay(vm, true);

        vm.MapUndo();
        Assert.Equal(0, vm.RoadContentCount);
        AssertRoadOverlay(vm, false);
        vm.MapRedo();
        Assert.Equal(1, vm.RoadContentCount);
        AssertRoadOverlay(vm, true);
    }

    static void AssertRoadOverlay(UiVm vm, bool expected)
    {
        var resources = vm.RenderProjection.Projection!.VectorOverlayResources;
        if (!expected) { Assert.Empty(resources); return; }
        var resource = Assert.Single(resources);
        Assert.True(VulkanVectorOverlayValidator.Validate(resource, out var error), error);
        var stroke = Assert.Single(resource.Primitives, item => item.Kind == RenderVectorOverlayPrimitiveKind.Stroke);
        var points = vm.MapSession.CurrentMap.Roads.Single().Points;
        var a = MapCoordinateContract.MapToWorld(points[0], vm.MapSession.CurrentMap.Surface.BaseHeightMeters);
        var b = MapCoordinateContract.MapToWorld(points[1], vm.MapSession.CurrentMap.Surface.BaseHeightMeters);
        Assert.All(resource.Indices.Skip(stroke.FirstIndex).Take(stroke.IndexCount)
            .Select(index => resource.Vertices[(int)index]), vertex =>
        { Assert.Equal(a, vertex.Position); Assert.Equal(b, vertex.Secondary); });
    }

    static (double X, double Y) FindHit(UiVm vm, int startX, int startY)
    {
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        return Enumerable.Range(startX, 101).SelectMany(x => Enumerable.Range(startY - 50, 101)
                .Select(y => (X: (double)x, Y: (double)y)))
            .First(p => MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection, p.X, p.Y, out _));
    }

    static void DispatchClick(UiVm vm, double x, double y, long id)
    {
        var source = new ViewportPointerSource("wp3-road-test");
        var pressed = vm.ViewportInput.Dispatch(new EditorPointerEvent(EditorPointerEventKind.Pressed, new(x, y),
            EditorPointerButtons.Left, EditorPointerModifiers.None, 0, id, source, 1));
        Assert.Equal(ViewportInputDispatchKind.Captured, pressed.Kind);
        Assert.Equal(GestureOwner.Road, vm.ViewportInput.Router.State.Owner);
        vm.ViewportInput.Dispatch(new EditorPointerEvent(EditorPointerEventKind.Released, new(x, y),
            EditorPointerButtons.None, EditorPointerModifiers.None, 0, id, source, 1));
        Assert.Equal(ViewportGestureState.Idle, vm.ViewportInput.Router.State);
    }

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, true);
    }
}
