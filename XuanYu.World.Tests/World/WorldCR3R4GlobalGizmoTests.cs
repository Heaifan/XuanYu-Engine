using XuanYu.Core.Math;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

public sealed class WorldCR3R4GlobalGizmoTests
{
    [Fact]
    public void Scale_gizmo_stays_global_after_entity_rotation()
    {
        var vm = new UiVm(null, () => true);
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(i => i.Key == "EntityId(1)");
        Assert.True(vm.TryCommitInspectorTransformValue("旋转", "Z", "180"));

        vm.SelectToolCommand.Execute("缩放");
        var projection = vm.RenderProjection.Projection;

        Assert.True(vm.RenderSnapshot.ShowScaleGizmo);
        Assert.True(projection.ScaleGizmoVisible);
        Assert.Equal(Vector3d.Zero, projection.GizmoRotation);
    }

    [Fact]
    public void No_visible_global_local_switch_exists_and_scale_does_not_auto_enter_local()
    {
        var vm = new UiVm(null, () => true);
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(i => i.Key == "EntityId(1)");
        vm.SelectToolCommand.Execute("缩放");

        Assert.DoesNotContain(vm.LogItems, x => x.Message.Contains("Local", StringComparison.Ordinal));
        Assert.Equal(Vector3d.Zero, vm.RenderProjection.Projection.GizmoRotation);
    }
}
