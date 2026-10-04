using XuanYu.Core.Space;
using XuanYu.Editor.Drawing;
using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Lifecycle;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class UnifiedMarkerDrawingRuntimeTests : IDisposable
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-marker-unified-{Guid.NewGuid():N}");

    [Fact]
    public async Task Marker_preview_precedes_point_draft_and_commit()
    {
        var vm = await CreateAsync();
        Assert.True(vm.IsUnifiedMarkerDrawingActive);
        Assert.Equal(DrawingPrimitiveKind.Point, vm.UnifiedDrawingSession!.PrimitiveKind);
        Assert.Empty(vm.MapSession.CurrentMap.Markers);

        Assert.True(vm.MarkerDrawingPointerMoved(400, 300, Viewport));
        Assert.Equal(0, vm.UnifiedDrawingSession.Draft.PointCount);
        Assert.NotNull(vm.MarkerDrawingPreview);
        Assert.NotNull(vm.MarkerDrawingSnapCandidate);
        Assert.Empty(vm.MapSession.CurrentMap.Markers);

        Assert.True(vm.MarkerDrawingPointerReleased(400, 300, Viewport));
        Assert.Single(vm.MapSession.CurrentMap.Markers);
        Assert.False(vm.IsUnifiedMarkerDrawingActive);
        Assert.True(vm.IsSelectTool);
        Assert.Contains("地图标记", vm.SelectedMapGeometryText);
    }

    [Fact]
    public async Task Router_routes_marker_gesture_and_releases_owner_after_commit()
    {
        var vm = await CreateAsync(); vm.UpdateViewportFrame(800, 600);
        var pressed = Event(EditorPointerEventKind.Pressed);
        var moved = Event(EditorPointerEventKind.Move);
        var released = Event(EditorPointerEventKind.Released);

        Assert.Equal(ViewportInputDispatchKind.Captured, vm.ViewportInput.Dispatch(pressed).Kind);
        Assert.Equal(GestureOwner.Marker, vm.ViewportInput.Lifecycle.Current!.Owner);
        vm.ViewportInput.Dispatch(moved); vm.ViewportInput.Dispatch(released);

        Assert.Equal(ViewportGestureLifecycleState.Idle, vm.ViewportInput.Lifecycle.State);
        Assert.Single(vm.MapSession.CurrentMap.Markers);
        Assert.False(vm.IsUnifiedMarkerDrawingActive);
    }

    [Fact]
    public async Task Cancel_and_invalid_point_do_not_commit_marker()
    {
        var vm = await CreateAsync();
        Assert.True(vm.MarkerDrawingPointerReleased(double.NaN, 1, Viewport));
        Assert.Empty(vm.MapSession.CurrentMap.Markers);
        Assert.True(vm.IsUnifiedMarkerDrawingActive);
        Assert.True(vm.CancelMarkerDrawing());
        Assert.False(vm.IsUnifiedMarkerDrawingActive);
        Assert.Null(vm.MarkerDrawingPreview);
    }

    async Task<UiVm> CreateAsync()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(Path.Combine(_root, "map.json")));
        vm.DatasetCreateType = MapDatasetTypes.Marker; Assert.True(await vm.CreateDatasetAsync());
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor); vm.ToggleEditorMode();
        vm.SelectRegionAuthoringMode("地图标记"); Assert.True(await vm.BeginMarkerPlacementAsync());
        return vm;
    }

    static EditorPointerEvent Event(EditorPointerEventKind kind) => new(kind, new(400, 300),
        EditorPointerButtons.Left, EditorPointerModifiers.None, 0, 1, new("test"), 1);

    public void Dispose() { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
}
