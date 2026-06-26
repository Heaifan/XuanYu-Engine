using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.Transform.Translation.Axis;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Drag;
using XuanYu.Engine.Render.Camera;
using XuanYu.Engine.Render.Camera.Navigation;
using XuanYu.Engine.Render.Vulkan.Camera;

namespace XuanYu.Engine.Tests.Editor.Transform.Translation.Axis;

public sealed class AxisDragAnchorBuilderTests
{
    static PresentedCameraSnapshot MakeCamera(SceneOrbitCameraState? state = null)
    {
        var orbit = state ?? SceneOrbitCameraMotion.CreateDefault();
        var pose = SceneCameraPose.FromOrbitState(orbit, 1);
        var cam = new VulkanCameraInfo(
            pose.PositionX, pose.PositionY, pose.PositionZ,
            pose.TargetX, pose.TargetY, pose.TargetZ,
            pose.UpX, pose.UpY, pose.UpZ,
            pose.FieldOfViewDegrees, pose.NearPlane, pose.FarPlane);
        var vp = VulkanCameraMatrices.ComputeVulkanMVP(cam, 1280f / 720f);
        VulkanMatrixInvert.TryInvert(vp, out var inv, out _);
        return new PresentedCameraSnapshot
        {
            CameraPose = pose,
            ViewProjection = vp,
            InverseViewProjection = inv!,
            ViewportWidth = 1280,
            ViewportHeight = 720,
            FrameIndex = 1,
            CameraRevision = 1
        };
    }

    [Fact]
    public void Build_ZAxis_UsesScreenProjection_WhenMetricIsAvailable()
    {
        var result = AxisDragAnchorBuilder.Build(Vector3d.UnitZ, 640, 360, Vector3d.Zero, MakeCamera(), Vector3d.Zero);
        Assert.True(result.Success);
        Assert.Equal(AxisTranslationMode.ScreenProjection, result.Anchor.Mode);
    }

    [Fact]
    public void Build_XAxis_UsesScreenProjection()
    {
        var result = AxisDragAnchorBuilder.Build(Vector3d.UnitX, 640, 360, Vector3d.Zero, MakeCamera(), Vector3d.Zero);
        Assert.True(result.Success);
        Assert.Equal(AxisTranslationMode.ScreenProjection, result.Anchor.Mode);
    }

    [Fact]
    public void Build_ZAxis_WithLowPitch_UsesContinuousScreenProjection()
    {
        var camera = MakeCamera(SceneOrbitCameraMotion.CreateDefault() with { Pitch = 18f });

        var result = AxisDragAnchorBuilder.Build(Vector3d.UnitZ, 640, 360, Vector3d.Zero, camera, Vector3d.Zero);

        Assert.True(result.Success);
        Assert.Equal(AxisTranslationMode.ScreenProjection, result.Anchor.Mode);
    }

    [Fact]
    public void Solve_ZAxis_ScreenProjection_CanMoveUpThenBackDown()
    {
        var result = AxisDragAnchorBuilder.Build(Vector3d.UnitZ, 640, 360, Vector3d.Zero, MakeCamera(), Vector3d.Zero);
        Assert.True(result.Success);
        Assert.Equal(AxisTranslationMode.ScreenProjection, result.Anchor.Mode);

        var dir = result.Anchor.ScreenDirection;
        var up = AxisTranslationSolver.Solve(result.Anchor, 640 + dir.X * 30, 360 + dir.Y * 30);
        var down = AxisTranslationSolver.Solve(result.Anchor, 640 - dir.X * 30, 360 - dir.Y * 30);

        Assert.True(up.Z > 0);
        Assert.True(down.Z < 0);
    }
}
