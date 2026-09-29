using XuanYu.Editor.Input;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class RegionDrawContextSyncFix1Tests
{
    [Fact]
    public async Task Region_draw_from_terrain_uses_region_context_and_zero_node_contract()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.EnterTerrainContext();

        Assert.True(await vm.BeginContextDrawingAsync("区域面"),
            $"edit={vm.IsRegionEditMode}, surface={vm.IsRegionSurfaceAuthoringMode}, " +
            $"request={vm.CanRequestRegionDrawing}, start={vm.CanStartRegionDrawing}, " +
            $"dataset={vm.SelectedDataset?.Type}/{vm.SelectedDataset?.Status}");
        Assert.True(vm.IsRegionDrawingTool);
        Assert.False(vm.IsTerrainContext);
        Assert.Equal("区域绘制", vm.ContextToolbarButtonLabel);
        Assert.Equal("区域绘制中 · 0", vm.DrawingTransactionLabel);
        Assert.False(vm.CanOpenContextSelector);
        Assert.False(vm.CanUndoDrawingVertex);
        Assert.False(vm.CanCompleteDrawing);
        Assert.True(vm.CanCancelDrawing);
        Assert.Equal("区域绘制", vm.ActiveTool);
        vm.EnterTerrainContext();
        Assert.False(vm.IsTerrainContext);
    }

    [Fact]
    public async Task Region_draw_session_owns_region_pointer_and_releases_capture_on_cancel()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.BeginContextDrawingAsync("区域面"));
        var pointer = new EditorPointerEvent(EditorPointerEventKind.Pressed,
            new(20, 20), EditorPointerButtons.Left, EditorPointerModifiers.None,
            0, 11, new("viewport"), 1);
        Assert.Equal(ViewportInputDispatchKind.Captured, vm.ViewportInput.Dispatch(pointer).Kind);
        Assert.Equal(GestureOwner.Region, vm.ViewportInput.Router.State.Owner);
        Assert.True(vm.ViewportInput.Router.State.IsCaptured);

        Assert.True(vm.CancelRegionDrawing());
        Assert.Equal(ViewportGestureState.Idle, vm.ViewportInput.Router.State);
        Assert.False(vm.ViewportInput.Router.State.IsCaptured);
        Assert.False(vm.IsDrawingTransactionActive);
        Assert.False(vm.IsRegionDrawingDraftActive);
        Assert.Null(vm.RegionDrawingCursor);
    }

    [Fact]
    public async Task Region_draw_session_rejects_reentry_and_repeated_cancel()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.BeginContextDrawingAsync("区域面"));
        Assert.False(await vm.BeginContextDrawingAsync("区域面"));
        Assert.True(vm.CancelRegionDrawing());
        Assert.False(vm.CancelRegionDrawing());
        Assert.False(vm.IsDrawingTransactionActive);
        Assert.True(vm.CanOpenContextSelector);
    }
}
