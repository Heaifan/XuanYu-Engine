using XuanYu.Core.Space;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class FeatureEditSelectionResetTests
{
    [Fact]
    public void Selected_road_vertex_can_be_added_and_deleted()
    {
        var (vm, _, road, _) = Create();
        vm.ToggleEditorModeCommand.Execute(null);
        vm.SelectMapGeometry(road);
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, viewport);
        var screen = projection.ProjectWorldPoint(new(0, 0, vm.MapSession.CurrentMap.Surface.BaseHeightMeters));

        Assert.True(vm.TryBeginMapGeometryPointer(screen.X, screen.Y, viewport));
        Assert.True(vm.CanAddSelectedRoadVertex);
        vm.CancelMapGeometryPointer("test");
        vm.AddSelectedRoadVertexCommand.Execute(null);
        Assert.Equal(3, vm.MapSession.CurrentMap.Roads[0].Points.Length);
        vm.DeleteSelectedRoadVertexCommand.Execute(null);
        Assert.Equal(2, vm.MapSession.CurrentMap.Roads[0].Points.Length);
    }
}
