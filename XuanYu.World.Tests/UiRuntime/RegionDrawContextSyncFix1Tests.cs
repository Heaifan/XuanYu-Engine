using XuanYu.Editor.Input;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Tests;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class RegionDrawContextSyncFix1Tests
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
        Assert.Equal("区域绘制", vm.ContextToolbarButtonLabel);
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

    [Fact]
    public async Task Region_draw_complete_releases_capture_and_returns_to_stable_region_context()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.BeginContextDrawingAsync("区域面"));
        var pointer = new EditorPointerEvent(EditorPointerEventKind.Pressed,
            new(20, 20), EditorPointerButtons.Left, EditorPointerModifiers.None,
            0, 12, new("viewport"), 1);
        vm.ViewportInput.Dispatch(pointer);
        vm.ViewportInput.Dispatch(pointer with { Kind = EditorPointerEventKind.Released });
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, viewport);
        foreach (var point in Enumerable.Range(0, 17).SelectMany(x => Enumerable.Range(0, 13)
                     .Select(y => (X: x * 50d, Y: y * 50d)))
                     .Where(p => MapSurfacePicker.TryPick(
                         vm.MapSession.CurrentMap, projection, p.X, p.Y, out _))
                     .Take(3))
            vm.RegionDrawingPointerPressed(point.X, point.Y, viewport);
        Assert.True(vm.CompleteRegionDrawing());

        Assert.False(vm.ViewportInput.Router.State.IsCaptured);
        Assert.False(vm.IsDrawingTransactionActive);
        Assert.Equal("区域", vm.ContextButtonLabel);
        Assert.False(vm.IsTerrainContext);
    }

    [Fact]
    public async Task Active_region_draw_rejects_context_menu_input()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.BeginContextDrawingAsync("区域面"));
        var context = vm.ViewportInput.Consumers.Single(x => x.Owner == GestureOwner.ContextMenu);
        var pointer = new EditorPointerEvent(EditorPointerEventKind.Pressed,
            new(10, 10), EditorPointerButtons.Right, EditorPointerModifiers.None,
            0, 1, new("test"), 1);

        Assert.False(context.CanBegin(pointer, ViewportGestureState.Idle));
    }

    [Fact]
    public async Task Cancel_at_zero_nodes_restores_tool_and_selector()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.BeginContextDrawingAsync("区域面"));

        Assert.True(vm.CancelRegionDrawing());
        Assert.False(vm.IsDrawingTransactionActive);
        Assert.False(vm.IsRegionDrawingTool);
        Assert.True(vm.CanOpenContextSelector);
        Assert.Equal(ViewportGestureState.Idle, vm.ViewportInput.Router.State);
    }

    [Fact]
    public async Task First_region_node_enables_undo_but_not_complete()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.BeginContextDrawingAsync("区域面"));
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, viewport);
        var hit = Enumerable.Range(0, 17).SelectMany(x => Enumerable.Range(0, 13)
            .Select(y => (X: x * 50d, Y: y * 50d)))
            .First(p => MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection, p.X, p.Y, out _));

        Assert.True(vm.RegionDrawingPointerPressed(hit.X, hit.Y, viewport));
        Assert.True(vm.CanUndoDrawingVertex);
        Assert.False(vm.CanCompleteDrawing);
    }

    [Fact]
    public void Context_toolbar_binds_selector_lock_to_transaction_state()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Top", "ContextToolBar.axaml");
        Assert.Contains("IsEnabled=\"{Binding CanOpenContextSelector}\"", File.ReadAllText(path));
    }
}
