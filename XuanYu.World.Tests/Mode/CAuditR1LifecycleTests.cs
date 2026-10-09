using XuanYu.Core.Space;
using XuanYu.Editor.Input;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Mode;

public sealed class CAuditR1LifecycleTests
{
    [Fact]
    public void Workspace_change_ends_region_preview_and_preserves_committed_selection()
    {
        var vm = RegionDrawingTestVm.Create();
        var region = new MapRegion(MapRegionId.New(), vm.MapSession.ActiveRegionLayerId,
            "保留区域", MapRegionKind.Generic, [new(0, 0), new(100, 0), new(100, 100)]);
        Assert.True(vm.MapSession.CreateRegion(region).IsSuccess);
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Region, region.RegionId.ToString()));
        vm.SelectToolCommand.Execute("区域绘制");
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var screen = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, viewport)
            .ProjectWorldPoint(new(0, 0, vm.MapSession.CurrentMap.Surface.BaseHeightMeters));
        Assert.True(vm.RegionDrawingPointerMoved(screen.X, screen.Y, viewport));
        Assert.NotNull(vm.MapGeometryPreview);

        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.MapEditor);

        Assert.Null(vm.MapGeometryPreview);
        Assert.False(vm.IsRegionDrawingDraftActive);
        Assert.Single(vm.MapSession.CurrentMap.Regions);
        Assert.True(vm.IsMapGeometrySelected);
    }

    [Fact]
    public void Mode_change_releases_camera_capture_and_keeps_map_selection_and_content()
    {
        var vm = RegionDrawingTestVm.Create();
        var region = new MapRegion(MapRegionId.New(), vm.MapSession.ActiveRegionLayerId,
            "持久对象", MapRegionKind.Generic, [new(0, 0), new(100, 0), new(100, 100)]);
        Assert.True(vm.MapSession.CreateRegion(region).IsSuccess);
        vm.SelectMapGeometry(new(MapGeometryFeatureKind.Region, region.RegionId.ToString()));
        vm.UpdateViewportFrame(800, 600);
        vm.ViewportInput.Dispatch(new EditorPointerEvent(EditorPointerEventKind.Pressed, new(300, 300),
            EditorPointerButtons.Middle, EditorPointerModifiers.None, 0, 4, new("test"), 1));
        Assert.True(vm.ViewportInput.Router.State.IsActive);

        Assert.True(vm.ToggleEditorMode());

        Assert.Equal(ViewportGestureState.Idle, vm.ViewportInput.Router.State);
        Assert.False(vm.IsCameraNavigationActive);
        Assert.Single(vm.MapSession.CurrentMap.Regions);
        Assert.True(vm.IsMapGeometrySelected);
    }
}
