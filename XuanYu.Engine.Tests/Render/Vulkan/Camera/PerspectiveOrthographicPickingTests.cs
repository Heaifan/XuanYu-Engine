using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Math;
using FluidWarfare.Render.Camera;
using FluidWarfare.Render.Camera.Navigation;
using FluidWarfare.Render.Scene;
using FluidWarfare.Render.Selection;
using FluidWarfare.Render.Selection.Ground;
using FluidWarfare.Render.Selection.Pointer;
using FluidWarfare.Render.Vulkan.Camera;

namespace FluidWarfare.Tests.Render.Vulkan.Camera;

/// <summary>
/// 投影-反投影-Picking 闭环测试。
/// 验证"画出来的物体，点击它的像素必须命中"这一核心不变量。
/// 覆盖透视、正交和六个标准轴向视图。
/// </summary>
public sealed class PerspectiveOrthographicPickingTests
{
    private const float ViewportW = 1280;
    private const float ViewportH = 720;

    // 三个样例实体（Z-Up 坐标）
    private static readonly EntityId Id1 = EntityId.FromInt(1);
    private static readonly EntityId Id2 = EntityId.FromInt(2);
    private static readonly EntityId Id3 = EntityId.FromInt(3);

    private static readonly Vector3d Pos1 = new(-4, -1, 0); // sample_unit
    private static readonly Vector3d Pos2 = new(0, 0, 0);   // sample_unit_2
    private static readonly Vector3d Pos3 = new(1, 3, 0);   // sample_unit_3

    private static readonly SceneGroundPlane Ground = SceneGroundPlane.Default;

    /// <summary>创建包含三个样例实体的场景。</summary>
    private static RenderScene CreateSampleScene()
    {
        const double half = 0.625;
        var b1 = new SceneAxisAlignedBounds(new Vector3d(-4, -1, half), new Vector3d(half, half, half));
        var b2 = new SceneAxisAlignedBounds(new Vector3d(0, 0, half), new Vector3d(half, half, half));
        var b3 = new SceneAxisAlignedBounds(new Vector3d(1, 3, half), new Vector3d(half, half, half));

        return new RenderScene([
            new RenderObjectInfo(Id1, "sample_unit", Pos1, RenderObjectVisualKind.UnitMarker, "a.json", b1),
            new RenderObjectInfo(Id2, "sample_unit_2", Pos2, RenderObjectVisualKind.UnitMarker, "b.json", b2),
            new RenderObjectInfo(Id3, "sample_unit_3", Pos3, RenderObjectVisualKind.UnitMarker, "c.json", b3),
        ]);
    }

    /// <summary>从轨道相机状态创建模拟已呈现快照。</summary>
    private static PresentedCameraSnapshot CreateSnapshot(SceneOrbitCameraState state, float aspect)
    {
        var pose = SceneCameraPose.FromOrbitState(state, 1);
        var camInfo = new VulkanCameraInfo(
            pose.PositionX, pose.PositionY, pose.PositionZ,
            pose.TargetX, pose.TargetY, pose.TargetZ,
            pose.UpX, pose.UpY, pose.UpZ,
            pose.FieldOfViewDegrees,
            pose.NearPlane, pose.FarPlane);

        float[] mvp;
        if (state.ProjectionMode == SceneProjectionMode.Orthographic)
        {
            mvp = VulkanCameraMatrices.ComputeVulkanOrthoMVP(camInfo, aspect, state.OrthographicHeight);
        }
        else
        {
            mvp = VulkanCameraMatrices.ComputeVulkanMVP(camInfo, aspect);
        }

        VulkanMatrixInvert.TryInvert(mvp, out var inv, out _);

        return new PresentedCameraSnapshot
        {
            CameraPose = pose,
            ViewProjection = mvp,
            InverseViewProjection = inv ?? [],
            ViewportWidth = (int)ViewportW,
            ViewportHeight = (int)ViewportH,
            FrameIndex = 1,
            CameraRevision = 1,
            ProjectionMode = state.ProjectionMode
        };
    }

    /// <summary>将世界坐标投影到像素坐标。</summary>
    private static (int Px, int Py) WorldToPixel(float[] mvp, Vector3d p)
    {
        var wx = (float)p.X;
        var wy = (float)p.Y;
        var wz = (float)p.Z;

        var clipX = mvp[0] * wx + mvp[4] * wy + mvp[8] * wz + mvp[12];
        var clipY = mvp[1] * wx + mvp[5] * wy + mvp[9] * wz + mvp[13];
        var clipW = mvp[3] * wx + mvp[7] * wy + mvp[11] * wz + mvp[15];

        var ndcX = clipX / clipW;
        var ndcY = clipY / clipW;

        var px = (int)Math.Round((ndcX + 1.0f) * 0.5f * ViewportW);
        var py = (int)Math.Round((ndcY + 1.0f) * 0.5f * ViewportH);

        px = Math.Clamp(px, 0, (int)ViewportW - 1);
        py = Math.Clamp(py, 0, (int)ViewportH - 1);

        return (px, py);
    }

    /// <summary>核心测试方法：投影 → 射线 → Picking → 断言命中。</summary>
    private static void AssertWorldPointPicksEntity(Vector3d worldPoint, EntityId expectedId, PresentedCameraSnapshot snapshot)
    {
        var mvp = snapshot.ViewProjection;
        var (px, py) = WorldToPixel(mvp, worldPoint);

        var status = VulkanSceneRayBuilder.TryBuild(px, py, snapshot, (uint)ViewportW, (uint)ViewportH, out var ray);
        Assert.Equal(SceneRayBuildStatus.Success, status);
        Assert.NotNull(ray);

        var scene = CreateSampleScene();
        var result = ScenePointerPicker.Pick(ray!, scene, Ground);

        Assert.Equal(ScenePointerPickKind.Entity, result.Kind);
        Assert.NotNull(result.EntityId);
        Assert.Equal(expectedId, result.EntityId);
    }

    // ─── 透视测试 ──────────────────────────────────────────────

    [Fact]
    public void PerspectiveCenterPixel_HitsCorrectEntity()
    {
        // Default camera looks at origin, unit_2 is at origin
        var state = SceneOrbitCameraMotion.CreateDefault();
        var snapshot = CreateSnapshot(state, ViewportW / ViewportH);
        AssertWorldPointPicksEntity(Pos2, Id2, snapshot);
    }

    [Fact]
    public void PerspectiveOffCenter_HitsCorrectEntity()
    {
        var state = SceneOrbitCameraMotion.CreateDefault();
        var snapshot = CreateSnapshot(state, ViewportW / ViewportH);

        // sample_unit at (-4, -1, 0) — should be visible
        AssertWorldPointPicksEntity(Pos1, Id1, snapshot);
    }

    [Fact]
    public void PerspectiveThirdEntity_HitsCorrectEntity()
    {
        var state = SceneOrbitCameraMotion.CreateDefault();
        var snapshot = CreateSnapshot(state, ViewportW / ViewportH);

        // sample_unit_3 at (1, 3, 0)
        AssertWorldPointPicksEntity(Pos3, Id3, snapshot);
    }

    // ─── 正交测试 ──────────────────────────────────────────────

    [Fact]
    public void OrthographicCenterPixel_HitsCorrectEntity()
    {
        var state = SceneOrbitCameraMotion.CreateDefault() with
        {
            ProjectionMode = SceneProjectionMode.Orthographic
        };
        var snapshot = CreateSnapshot(state, ViewportW / ViewportH);
        AssertWorldPointPicksEntity(Pos2, Id2, snapshot);
    }

    [Fact]
    public void OrthographicOffCenter_HitsCorrectEntity()
    {
        var state = SceneOrbitCameraMotion.CreateDefault() with
        {
            ProjectionMode = SceneProjectionMode.Orthographic
        };
        var snapshot = CreateSnapshot(state, ViewportW / ViewportH);
        AssertWorldPointPicksEntity(Pos1, Id1, snapshot);
    }

    [Fact]
    public void OrthographicThirdEntity_HitsCorrectEntity()
    {
        var state = SceneOrbitCameraMotion.CreateDefault() with
        {
            ProjectionMode = SceneProjectionMode.Orthographic
        };
        var snapshot = CreateSnapshot(state, ViewportW / ViewportH);
        AssertWorldPointPicksEntity(Pos3, Id3, snapshot);
    }

    // ─── 标准视图测试 ──────────────────────────────────────────

    [Fact]
    public void PositiveXView_PicksEntity()
    {
        var state = SceneNavigationCameraMotion.SnapToView(
            SceneOrbitCameraMotion.CreateDefault(), SceneNavigationView.PositiveX);
        var snapshot = CreateSnapshot(state, ViewportW / ViewportH);
        AssertWorldPointPicksEntity(Pos2, Id2, snapshot);
    }

    [Fact]
    public void NegativeXView_PicksEntity()
    {
        var state = SceneNavigationCameraMotion.SnapToView(
            SceneOrbitCameraMotion.CreateDefault(), SceneNavigationView.NegativeX);
        var snapshot = CreateSnapshot(state, ViewportW / ViewportH);
        AssertWorldPointPicksEntity(Pos2, Id2, snapshot);
    }

    [Fact]
    public void PositiveYView_PicksEntity()
    {
        var state = SceneNavigationCameraMotion.SnapToView(
            SceneOrbitCameraMotion.CreateDefault(), SceneNavigationView.PositiveY);
        var snapshot = CreateSnapshot(state, ViewportW / ViewportH);
        AssertWorldPointPicksEntity(Pos2, Id2, snapshot);
    }

    [Fact]
    public void NegativeYView_PicksEntity()
    {
        var state = SceneNavigationCameraMotion.SnapToView(
            SceneOrbitCameraMotion.CreateDefault(), SceneNavigationView.NegativeY);
        var snapshot = CreateSnapshot(state, ViewportW / ViewportH);
        AssertWorldPointPicksEntity(Pos2, Id2, snapshot);
    }

    [Fact]
    public void PositiveZView_PicksEntity()
    {
        var state = SceneNavigationCameraMotion.SnapToView(
            SceneOrbitCameraMotion.CreateDefault(), SceneNavigationView.PositiveZ);
        var snapshot = CreateSnapshot(state, ViewportW / ViewportH);
        AssertWorldPointPicksEntity(Pos2, Id2, snapshot);
    }

    [Fact]
    public void NegativeZView_PicksEntity()
    {
        var state = SceneNavigationCameraMotion.SnapToView(
            SceneOrbitCameraMotion.CreateDefault(), SceneNavigationView.NegativeZ);
        var snapshot = CreateSnapshot(state, ViewportW / ViewportH);
        AssertWorldPointPicksEntity(Pos2, Id2, snapshot);
    }

    // ─── 射线起点测试 ──────────────────────────────────────────

    [Fact]
    public void PerspectiveRay_OriginIsNearWorld()
    {
        // Verify that the ray origin is the near-plane point, not camera position
        var state = SceneOrbitCameraMotion.CreateDefault();
        var snapshot = CreateSnapshot(state, ViewportW / ViewportH);

        var (px, py) = WorldToPixel(snapshot.ViewProjection, Pos2);
        var status = VulkanSceneRayBuilder.TryBuild(px, py, snapshot, (uint)ViewportW, (uint)ViewportH, out var ray);

        Assert.Equal(SceneRayBuildStatus.Success, status);
        Assert.NotNull(ray);

        // Ray origin should NOT be at camera position
        var camPos = new Vector3d(
            snapshot.CameraPose.PositionX,
            snapshot.CameraPose.PositionY,
            snapshot.CameraPose.PositionZ);

        var distFromCamera = (ray!.Origin - camPos).Length;
        Assert.True(distFromCamera > 0.01,
            $"Ray origin should differ from camera position (dist={distFromCamera})");
    }

    [Fact]
    public void OrthographicRays_AreParallel()
    {
        var state = SceneOrbitCameraMotion.CreateDefault() with
        {
            ProjectionMode = SceneProjectionMode.Orthographic
        };
        var snapshot = CreateSnapshot(state, ViewportW / ViewportH);

        // Build rays for two different pixels
        var status1 = VulkanSceneRayBuilder.TryBuild(100, 100, snapshot, (uint)ViewportW, (uint)ViewportH, out var ray1);
        var status2 = VulkanSceneRayBuilder.TryBuild(200, 150, snapshot, (uint)ViewportW, (uint)ViewportH, out var ray2);

        Assert.Equal(SceneRayBuildStatus.Success, status1);
        Assert.Equal(SceneRayBuildStatus.Success, status2);
        Assert.NotNull(ray1);
        Assert.NotNull(ray2);

        // In orthographic, directions should be nearly identical
        var dot = ray1!.Direction.X * ray2!.Direction.X +
                  ray1.Direction.Y * ray2.Direction.Y +
                  ray1.Direction.Z * ray2.Direction.Z;
        Assert.True(dot > 0.999, $"Orthographic rays should be parallel (dot={dot})");
    }

    [Fact]
    public void OrthographicRayOrigins_AreDifferent()
    {
        var state = SceneOrbitCameraMotion.CreateDefault() with
        {
            ProjectionMode = SceneProjectionMode.Orthographic
        };
        var snapshot = CreateSnapshot(state, ViewportW / ViewportH);

        var status1 = VulkanSceneRayBuilder.TryBuild(100, 100, snapshot, (uint)ViewportW, (uint)ViewportH, out var ray1);
        var status2 = VulkanSceneRayBuilder.TryBuild(200, 200, snapshot, (uint)ViewportW, (uint)ViewportH, out var ray2);

        Assert.Equal(SceneRayBuildStatus.Success, status1);
        Assert.Equal(SceneRayBuildStatus.Success, status2);

        // In orthographic, different pixels = different origins
        var dist = (ray1!.Origin - ray2!.Origin).Length;
        Assert.True(dist > 0.01, $"Orthographic ray origins should differ (dist={dist})");
    }

    // ─── 技术失败语义测试 ──────────────────────────────────────

    [Fact]
    public void SnapshotExtentMismatch_ReturnsCorrectStatus()
    {
        var state = SceneOrbitCameraMotion.CreateDefault();
        var snapshot = CreateSnapshot(state, ViewportW / ViewportH);

        // Pass current viewport size that differs from snapshot
        var status = VulkanSceneRayBuilder.TryBuild(
            100, 100, snapshot, 640, 480, out _);

        Assert.Equal(SceneRayBuildStatus.SnapshotExtentMismatch, status);
    }

    [Fact]
    public void EmptySnapshot_ReturnsUnavailable()
    {
        var status = VulkanSceneRayBuilder.TryBuild(
            100, 100, PresentedCameraSnapshot.Empty, 1280, 720, out _);

        Assert.Equal(SceneRayBuildStatus.SnapshotUnavailable, status);
    }

    [Fact]
    public void PixelOutOfBounds_ReturnsCorrectStatus()
    {
        var state = SceneOrbitCameraMotion.CreateDefault();
        var snapshot = CreateSnapshot(state, ViewportW / ViewportH);

        var status = VulkanSceneRayBuilder.TryBuild(
            -1, -1, snapshot, (uint)ViewportW, (uint)ViewportH, out _);

        Assert.Equal(SceneRayBuildStatus.PixelOutOfBounds, status);
    }
}
