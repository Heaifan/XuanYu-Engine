using XuanYu.Core.Math;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Camera;

public sealed class EditorOrbitObservationCenterTests
{
    [Fact]
    public void Default_orbit_uses_world_origin()
    {
        var vm = new UiVm(null, () => true);
        Assert.Equal(Vector3d.Zero, vm.ObservationCenter);
        Assert.True(vm.BeginCameraNavigation(1, 400, 300, false, 800, 600));
        Assert.Equal(Vector3d.Zero, vm.OrbitProbeEvents.Single().Pivot);
    }

    [Fact]
    public void Editor_orbit_uses_current_observation_center()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(800, 600);
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(item => item.Key == "EntityId(5)");
        var center = vm.ObservationCenter;

        Assert.True(vm.BeginCameraNavigation(1, 400, 300, false, 800, 600));

        Assert.Equal(center, vm.OrbitProbeEvents.Single(e => e.Phase == OrbitProbePhase.Begin).Pivot);
    }

    [Fact]
    public void Orbit_move_keeps_observation_center_and_never_resolves_surface()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(800, 600);
        Assert.True(vm.BeginCameraNavigation(1, 400, 300, false, 800, 600));
        Assert.True(vm.PreviewCameraNavigation(1, 430, 330));
        var center = vm.ObservationCenter;
        Assert.All(vm.OrbitProbeEvents, e => Assert.Equal(center, e.Pivot));
        Assert.All(vm.OrbitProbeEvents, e => Assert.Equal(XuanYu.Editor.UI.OrbitPivotSource.ObservationCenter, e.PivotSource));
        Assert.All(vm.OrbitProbeEvents.Where(e => e.Phase == OrbitProbePhase.Move),
            e => Assert.Equal(0, e.PivotResolveCount));
    }
}
