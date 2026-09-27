using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Camera;

public sealed class EmptySceneCameraContractTests
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);

    [Fact]
    public void EmptySceneUsesValidDefaultCamera()
    {
        var vm = StartEmptyScene();
        var camera = vm.RenderSnapshot.CameraState;
        Assert.Equal(DefaultEditorCamera.Position, camera.Position);
        Assert.Equal(DefaultEditorCamera.Target, vm.ObservationCenter);
        Assert.Equal(ProjectionMode.Perspective, camera.Mode);
        Assert.True(camera.NearPlane > 0 && camera.FarPlane > camera.NearPlane);
    }

    [Fact]
    public void EmptySceneWorldPlaneVisible()
    {
        var state = Projection(StartEmptyScene());
        Assert.True(state.TryProjectWorldPoint(Vector3d.Zero, out var point));
        Assert.InRange(point.X, 0, Viewport.LogicalWidth);
        Assert.InRange(point.Y, 0, Viewport.LogicalHeight);
    }

    [Fact]
    public void EmptySceneCenterRayHitsWorldPlane()
    {
        var ray = WorldRayFactory.FromViewportPoint(Projection(StartEmptyScene()), 400, 300);
        var distance = -ray.Origin.Z / ray.Direction.Z;
        Assert.True(double.IsFinite(distance) && distance > 0);
        Assert.Equal(0, (ray.Origin + ray.Direction * distance).Z, precision: 8);
    }

    [Fact]
    public void EmptySceneCameraDoesNotRequireTerrainBounds()
    {
        var vm = StartEmptyScene();
        Assert.Empty(vm.RenderProjection.Projection.TerrainResources);
        Assert.True(ViewProjectionState.TryCreate(vm.RenderSnapshot.CameraState, Viewport,
            out _));
    }

    [Fact]
    public void EmptySceneCameraDoesNotReuseTerrainFrame()
    {
        var vm = StartEmptyScene();
        vm.DollyCamera(-100);
        vm.NewBlankScene();
        Assert.Equal(DefaultEditorCamera.Position, vm.RenderSnapshot.CameraState.Position);
        Assert.Equal(DefaultEditorCamera.Target, vm.ObservationCenter);
    }

    [Fact]
    public void PerspectiveDefaultCameraValid() => AssertValid(StartEmptyScene());

    [Fact]
    public void OrthographicDefaultCameraValid()
    {
        var vm = StartEmptyScene();
        vm.RunCommand.Execute("视角-顶视图");
        Assert.Equal(ProjectionMode.Orthographic, vm.RenderSnapshot.CameraState.Mode);
        Assert.True(Projection(vm).TryProjectWorldPoint(Vector3d.Zero, out _));
    }

    [Fact]
    public void ReverseZCameraStartupRegression() => AssertValid(StartEmptyScene());

    static UiVm StartEmptyScene()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.UpdateViewportFrame((int)Viewport.LogicalWidth, (int)Viewport.LogicalHeight);
        return vm;
    }

    static ViewProjectionState Projection(UiVm vm) =>
        ViewProjectionState.Create(vm.RenderSnapshot.CameraState, Viewport);

    static void AssertValid(UiVm vm)
    {
        var camera = vm.RenderSnapshot.CameraState;
        Assert.True(camera.Position.DistanceTo(vm.ObservationCenter) > 0);
        Assert.True(ViewProjectionState.TryCreate(camera, Viewport, out _));
    }
}
