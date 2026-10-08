using XuanYu.Core.Math;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;
namespace XuanYu.World.Tests.Camera;
public sealed class OrbitPivotAuthorityTests
{
    [Fact]
    public void TerrainHit_IsPreferredOverReferencePlane()
    {
        var terrain = GroundPickResult.Valid(new(3, 4), SurfaceBinding.Terrain("dem"), 8);
        var plane = GroundPickResult.Valid(new(30, 40), SurfaceBinding.ReferencePlane, 0);

        var result = OrbitPivotAuthority.Resolve(null, terrain, plane, new(9, 9, 9));

        Assert.Equal(OrbitPivotSource.Terrain, result.Source);
        Assert.Equal(new Vector3d(3, 4, 8), result.Pivot);
    }
    [Fact]
    public void TerrainOutOfBounds_UsesExplicitReferencePlaneFallback()
    {
        var terrain = GroundPickResult.Invalid;
        var plane = GroundPickResult.Valid(new(30, 40), SurfaceBinding.ReferencePlane, 2);

        var result = OrbitPivotAuthority.Resolve(null, terrain, plane, new(9, 9, 9));

        Assert.Equal(OrbitPivotSource.ReferencePlane, result.Source);
        Assert.Equal(new Vector3d(30, 40, 2), result.Pivot);
    }
    [Fact]
    public void InvalidSurfaces_KeepPreviousValidPivot()
    {
        var previous = new Vector3d(9, 8, 7);

        var result = OrbitPivotAuthority.Resolve(null, GroundPickResult.Invalid,
            GroundPickResult.Invalid, previous);

        Assert.Equal(OrbitPivotSource.PreviousValidPivot, result.Source);
        Assert.Equal(previous, result.Pivot);
    }

    [Fact]
    public void SelectionPivot_IsHighestPriority()
    {
        var result = OrbitPivotAuthority.Resolve(new(1, 2, 3),
            GroundPickResult.Valid(new(4, 5), SurfaceBinding.Terrain("dem"), 6),
            GroundPickResult.Valid(new(7, 8), SurfaceBinding.ReferencePlane, 0),
            new(9, 9, 9));

        Assert.Equal(OrbitPivotSource.Selection, result.Source);
        Assert.Equal(new Vector3d(1, 2, 3), result.Pivot);
    }

    [Fact]
    public void OrbitBegin_ResolvesPivotExactlyOnce_AndMoveDoesNotRepick()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(800, 600);

        Assert.True(vm.BeginCameraNavigation(1, 400, 300, false, 800, 600));
        Assert.True(vm.PreviewCameraNavigation(1, 420, 320));
        Assert.True(vm.PreviewCameraNavigation(1, 460, 340));

        Assert.Equal(1, vm.OrbitProbeEvents.Count(e => e.Phase == OrbitProbePhase.Begin));
        Assert.Equal(0, vm.OrbitProbeEvents.Count(e => e.PivotResolveCount > 0 && e.Phase == OrbitProbePhase.Move));
        Assert.All(vm.OrbitProbeEvents, e => Assert.Equal(vm.OrbitProbeEvents[0].Pivot, e.Pivot));
    }

    [Fact]
    public void OrbitMove_DoesNotChangePivotSource()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(800, 600);
        Assert.True(vm.BeginCameraNavigation(1, 400, 300, false, 800, 600));
        var source = vm.OrbitProbeEvents.Single(e => e.Phase == OrbitProbePhase.Begin).PivotSource;

        vm.PreviewCameraNavigation(1, 450, 340);

        Assert.All(vm.OrbitProbeEvents.Where(e => e.Phase == OrbitProbePhase.Move),
            e => Assert.Equal(source, e.PivotSource));
    }

    [Fact]
    public void OrbitCancel_EndsFrozenPivotSession_AndNextGestureResolvesNewPivot()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(800, 600);
        Assert.True(vm.BeginCameraNavigation(1, 400, 300, false, 800, 600));
        Assert.True(vm.CancelCameraNavigation("Cancel"));
        Assert.False(vm.IsCameraNavigationActive);
        Assert.True(vm.BeginCameraNavigation(2, 400, 300, false, 800, 600));

        var begins = vm.OrbitProbeEvents.Where(e => e.Phase == OrbitProbePhase.Begin).ToArray();
        Assert.Equal(2, begins.Length);
        Assert.Equal(1, begins[0].OrbitSessionId);
        Assert.Equal(2, begins[1].OrbitSessionId);
        Assert.Equal(1, begins[0].PivotResolveCount);
        Assert.Equal(1, begins[1].PivotResolveCount);
    }
}
