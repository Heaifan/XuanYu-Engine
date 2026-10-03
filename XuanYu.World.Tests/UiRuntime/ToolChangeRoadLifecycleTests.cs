using XuanYu.Core.Space;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.Input;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class ToolChangeRoadLifecycleTests
{
    [Fact]
    public async Task Tool_change_closes_road_owner_draft_and_transaction()
    {
        var root = Path.Combine(Path.GetTempPath(), $"road-tool-change-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            var vm = new UiVm(null, () => true, seedInitialScene: false);
            Assert.True(await vm.SaveMapManifestAsync(Path.Combine(root, "map.json")));
            vm.DatasetCreateType = MapDatasetTypes.Road;
            Assert.True(await vm.CreateDatasetAsync());
            vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
            vm.ToggleEditorMode();
            vm.SelectRegionAuthoringMode("道路");
            vm.SelectToolCommand.Execute("道路绘制");
            Assert.True(vm.IsRoadDrawingTool);
            Assert.True(vm.IsDrawingTransactionActive);

            var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
            var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, viewport);
            var hit = Enumerable.Range(0, 17).SelectMany(x => Enumerable.Range(0, 13)
                .Select(y => (X: x * 50d, Y: y * 50d)))
                .First(p => MapSurfacePicker.TryPick(vm.MapSession.CurrentMap,
                    projection, p.X, p.Y, out _));
            Assert.True(vm.RoadDrawingPointerPressed(hit.X, hit.Y, viewport));
            Assert.True(vm.IsRoadDrawingDraftActive);
            var pointer = new EditorPointerEvent(EditorPointerEventKind.Pressed,
                new(hit.X, hit.Y), EditorPointerButtons.Left, EditorPointerModifiers.None,
                0, 15, new("viewport"), 1);
            Assert.Equal(ViewportInputDispatchKind.Captured,
                vm.ViewportInput.Dispatch(pointer).Kind);
            Assert.Equal(GestureOwner.Road, vm.ViewportInput.Router.State.Owner);

            vm.SelectToolCommand.Execute("选择");

            Assert.False(vm.IsRoadDrawingTool);
            Assert.False(vm.IsRoadDrawingDraftActive);
            Assert.False(vm.IsDrawingTransactionActive);
            Assert.Equal(ViewportGestureState.Idle, vm.ViewportInput.Router.State);
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }
}
