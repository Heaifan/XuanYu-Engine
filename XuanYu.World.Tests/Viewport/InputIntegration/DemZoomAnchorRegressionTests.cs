using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Editor.Input;
using XuanYu.World.Map;
using Xunit.Abstractions;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class DemZoomAnchorRegressionTests(ITestOutputHelper output)
{
    readonly ITestOutputHelper _output = output;
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);

    [Fact]
    public void TerrainZoom_DoesNotMutateOrbitPivot()
    {
        var camera = DefaultEditorCamera.Create(1);
        var pivot = new Vector3d(3, 2, 4);
        Assert.True(CameraNavigation.TryDolly(camera, pivot, 1, 2,
            new(2, 1, 0), out var result, out var reason), reason);
        Assert.Equal(pivot, result.ObservationCenter);
        Assert.InRange((camera.Forward - result.Camera.Forward).Length, 0, 1e-9);
        Assert.InRange((camera.Up - result.Camera.Up).Length, 0, 1e-9);
    }

    [Fact]
    public void TerrainZoom_FixedScreenAnchor_PreservesReferenceSurfacePoint()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(800, 600);
        const double x = 620, y = 180;
        var before = vm.RenderSnapshot.CameraState;
        var ray = WorldRayFactory.FromViewportPoint(ViewProjectionState.Create(before, Viewport), x, y);
        var distance = -ray.Origin.Z / ray.Direction.Z;
        var anchor = ray.Origin + ray.Direction * distance;
        vm.ViewportInput.Sink.Handle(Wheel(x, y, 1));
        var actual = ViewProjectionState.Create(vm.RenderSnapshot.CameraState, Viewport)
            .ProjectWorldPoint(anchor);
        Assert.InRange(System.Math.Abs(actual.X - x), 0, 1e-3);
        Assert.InRange(System.Math.Abs(actual.Y - y), 0, 1e-3);
    }

    [Fact]
    public void TerrainZoom_UsesTerrainHitWithoutReferencePlaneFallback()
    {
        var ray = new WorldRay(new(0, 0, 100), new(0, 0, -1));
        var result = GroundPickResolver.Resolve(ray, new FixedSurface(42), 0);
        Assert.True(result.IsValid);
        Assert.Equal(SurfaceBindingKind.Terrain, result.SurfaceBinding.Kind);
        Assert.Equal(42, result.ResolvedElevation);
    }

    [Fact]
    public void TerrainZoom_InOutRoundTrip_EmitsStepTrace()
    {
        var vm = new UiVm(null, () => true); vm.UpdateViewportFrame(800, 600);
        const double x = 620, y = 180;
        var before = vm.RenderSnapshot.CameraState;
        var ray = WorldRayFactory.FromViewportPoint(ViewProjectionState.Create(before, Viewport), x, y);
        var anchor = ray.Origin + ray.Direction * (-ray.Origin.Z / ray.Direction.Z);
        Trace(0, vm, anchor, x, y);
        for (var step = 1; step <= 20; step++)
        {
            vm.ViewportInput.Sink.Handle(Wheel(x, y, step <= 10 ? 1 : -1));
            Trace(step, vm, anchor, x, y);
        }
        var actual = ViewProjectionState.Create(vm.RenderSnapshot.CameraState, Viewport)
            .ProjectWorldPoint(anchor);
        var error = System.Math.Sqrt(System.Math.Pow(actual.X - x, 2) + System.Math.Pow(actual.Y - y, 2));
        Assert.InRange(error, 0, 1e-3);
        Assert.InRange((vm.RenderSnapshot.CameraState.Position - before.Position).Length, 0, 1e-4);
    }

    void Trace(int step, UiVm vm, Vector3d anchor, double x, double y)
    {
        var camera = vm.RenderSnapshot.CameraState;
        var state = ViewProjectionState.Create(camera, Viewport);
        var ray = WorldRayFactory.FromViewportPoint(state, x, y);
        var projected = state.ProjectWorldPoint(anchor);
        _output.WriteLine($"step {step}; Position={camera.Position}; Forward={camera.Forward}; Pivot={vm.ObservationCenter}; " +
            $"Cursor=({x},{y}); RayOrigin={ray.Origin}; RayDirection={ray.Direction}; SurfaceSource=ReferencePlane; " +
            $"TerrainHit=UNAVAILABLE; ReferencePlaneHit={anchor}; ResolvedZoomAnchor={anchor}; RenderOrigin=WorldSpace; " +
            $"ScreenError={System.Math.Sqrt(System.Math.Pow(projected.X - x, 2) + System.Math.Pow(projected.Y - y, 2)):0.########}");
    }

    static EditorPointerEvent Wheel(double x, double y, double delta) => new(
        EditorPointerEventKind.Wheel, new(x, y), EditorPointerButtons.None,
        EditorPointerModifiers.None, delta, 1, new("test"), 1);

    sealed class FixedSurface(double elevation) : IGroundSurface
    {
        public SurfaceBinding Binding => SurfaceBinding.Terrain("test-terrain");
        public int? Revision => 1;
        public bool TryGetElevation(MapPoint point, out double value) { value = elevation; return true; }
    }
}
