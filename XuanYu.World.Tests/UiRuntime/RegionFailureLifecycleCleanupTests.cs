using System.Collections.Immutable;
using System.Reflection;
using XuanYu.Core.Space;
using XuanYu.Editor.Input;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class RegionFailureLifecycleCleanupTests
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);

    [Fact]
    public async Task Identity_failure_terminates_region_transient_state_and_notifies()
    {
        var vm = await CreateVm();
        var points = FindHits(vm, 3);
        vm.SelectToolCommand.Execute("区域绘制");
        vm.ViewportInput.Router.Dispatch(new EditorPointerEvent(EditorPointerEventKind.Pressed, new(points[0].X, points[0].Y),
            EditorPointerButtons.Left, EditorPointerModifiers.None, 0, 7, new("test"), 1));
        foreach (var point in points.Skip(1)) vm.RegionDrawingPointerPressed(point.X, point.Y, Viewport);
        var owner = (MapManifestOwner)typeof(UiVm).GetField("_mapManifestOwner",
            BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(vm)!;
        owner.Modify(vm.CurrentMapManifest with { Id = MapId.New().Value });

        Assert.False(vm.CompleteRegionDrawing());
        AssertRegionTransientStateCleared(vm);
        Assert.Equal(UiNotificationLevel.Error, vm.NotificationLevel);
        Assert.Empty(vm.MapSession.CurrentMap.Regions);
    }

    [Fact]
    public async Task Validator_failure_clears_transient_state_but_preserves_region_selection()
    {
        var vm = await CreateVm();
        var first = FindHits(vm, 3);
        vm.SelectToolCommand.Execute("区域绘制");
        foreach (var point in first) vm.RegionDrawingPointerPressed(point.X, point.Y, Viewport);
        Assert.True(vm.CompleteRegionDrawing());
        var existing = vm.MapSession.CurrentMap.Regions.Single();
        Assert.True(vm.MapSession.SelectRegion(existing.RegionId).IsSuccess);

        vm.SelectToolCommand.Execute("区域绘制");
        foreach (var point in FindHits(vm, 3)) vm.RegionDrawingPointerPressed(point.X, point.Y, Viewport);
        var layer = vm.MapSession.CurrentMap.Layers.Select(item => item.LayerId == existing.LayerId
            ? item with { IsLocked = true } : item).ToImmutableArray();
        Assert.True(vm.MapSession.ApplyRuntimeLayerProjection(vm.MapSession.CurrentMap with { Layers = layer }).IsSuccess);

        Assert.True(vm.CompleteRegionDrawing());
        AssertRegionTransientStateCleared(vm);
        Assert.Equal(MapSelectionKind.Region, vm.MapSession.Selection.Kind);
        Assert.Equal(existing.RegionId, vm.MapSession.Selection.RegionId);
        Assert.Single(vm.MapSession.CurrentMap.Regions);
    }

    static void AssertRegionTransientStateCleared(UiVm vm)
    {
        Assert.False(vm.IsRegionDrawingDraftActive);
        Assert.False(vm.IsRegionDrawingSnapActive);
        Assert.False(vm.IsDrawingTransactionActive);
        Assert.False(vm.IsRegionDrawingTool);
        Assert.Equal(ViewportGestureState.Idle, vm.ViewportInput.Router.State);
        Assert.False(vm.ViewportInput.Router.State.IsCaptured);
    }

    static async Task<UiVm> CreateVm()
    {
        var root = Path.Combine(Path.GetTempPath(), $"xuanyu-region-failure-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(Path.Combine(root, "map.json")));
        vm.DatasetCreateType = "region";
        Assert.True(await vm.CreateDatasetAsync());
        vm.ToggleEditorMode();
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        return vm;
    }

    static IReadOnlyList<(double X, double Y)> FindHits(UiVm vm, int count)
    {
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        return Enumerable.Range(0, 17).SelectMany(x => Enumerable.Range(0, 13)
                .Select(y => (X: x * 50d, Y: y * 50d)))
            .Where(point => MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection,
                point.X, point.Y, out _)).Take(count).ToArray();
    }
}
