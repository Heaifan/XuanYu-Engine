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

    // ── DragPlane 退化路径测试 ───────────────────────────

    /// <summary>Camera 从正上方俯视（Pitch≈90°），Z 轴投影 < 6px → 退化到 DragPlane。</summary>
    [Fact]
    public void Build_ZAxis_WithTopDownCamera_FallsBackToDragPlane()
    {
        var camera = MakeCamera(new SceneOrbitCameraState
        {
            Yaw = 0,
            Pitch = 89f,
            Distance = 100,
            PivotX = 0, PivotY = 0, PivotZ = 0,
            FieldOfViewDegrees = 45,
            NearPlane = 0.1f,
            FarPlane = 1000f,
            ProjectionMode = SceneProjectionMode.Perspective
        });

        var result = AxisDragAnchorBuilder.Build(Vector3d.UnitZ, 640, 360, Vector3d.Zero, camera, Vector3d.Zero);
        Assert.True(result.Success);
        Assert.Equal(AxisTranslationMode.DragPlane, result.Anchor.Mode);

        // DragPlane 字段不出现 NaN / Infinity
        Assert.True(double.IsFinite(result.Anchor.StartIntersection.X));
        Assert.True(double.IsFinite(result.Anchor.StartIntersection.Y));
        Assert.True(double.IsFinite(result.Anchor.StartIntersection.Z));
        Assert.False(result.Anchor.DragPlaneNormal.IsZero);
        Assert.True(double.IsFinite(result.Anchor.CameraForward.X));
    }

    /// <summary>DragPlane 模式下移动方向可逆。</summary>
    [Fact]
    public void Solve_ZAxis_DragPlane_CanMoveUpThenBackDown()
    {
        var camera = MakeCamera(new SceneOrbitCameraState
        {
            Yaw = 0, Pitch = 89f, Distance = 100,
            PivotX = 0, PivotY = 0, PivotZ = 0,
            FieldOfViewDegrees = 45,
            NearPlane = 0.1f, FarPlane = 1000f,
            ProjectionMode = SceneProjectionMode.Perspective
        });

        var result = AxisDragAnchorBuilder.Build(Vector3d.UnitZ, 640, 360, Vector3d.Zero, camera, Vector3d.Zero);
        Assert.True(result.Success);
        Assert.Equal(AxisTranslationMode.DragPlane, result.Anchor.Mode);

        // 沿轴正方向移动
        var forward = AxisTranslationSolver.SolveDragPlane(result.Anchor,
            result.Anchor.StartIntersection + Vector3d.UnitZ * 5);
        // 回到原位置
        var backward = AxisTranslationSolver.SolveDragPlane(result.Anchor,
            result.Anchor.StartIntersection);

        Assert.True(forward.Z > 0, "沿 +Z 移动后 Z 应增加");
        Assert.True(Math.Abs(backward.Z) < 1e-6, "回到 StartIntersection 后 Z 应接近 0");
    }
}
