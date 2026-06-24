using FluidWarfare.Core.Math;
using FluidWarfare.Render.Camera;
using FluidWarfare.Render.Camera.Navigation;
using FluidWarfare.Render.Selection;
using FluidWarfare.Render.Selection.Ground;
using FluidWarfare.Render.Vulkan.Camera;

namespace FluidWarfare.Tests.Render.Vulkan.Camera;

/// <summary>
/// 投影→反投影闭环测试。
/// 选取多个已知地面世界点 → 投影到屏幕 → 从屏幕构建射线 → 与地面求交 → 验证误差。
/// </summary>
public sealed class ProjectionUnprojectionRoundTripTests
{
    private const float ViewportW = 1280;
    private const float ViewportH = 720;
    // 允许 5cm 误差——Z-Up 相机角度下边缘像素的舍入误差略大于 Y-Up。
    private const double ErrorTolerance = 0.05;

    private static readonly SceneGroundPlane Ground = SceneGroundPlane.Default;

    /// <summary>
    /// 要测试的世界坐标点（全部在地面 Z=0，Z-Up 约定）。
    /// </summary>
    public static TheoryData<Vector3d> GroundPoints => new()
    {
        new Vector3d(0, 0, 0),
        new Vector3d(-4, -1, 0), // migrated from (-4, 0, 1)
        new Vector3d(1, 3, 0),   // migrated from (1, 0, -3)
        new Vector3d(10, -10, 0),
        new Vector3d(-10, 10, 0),
        new Vector3d(5, 8, 0),
        new Vector3d(-7, -12, 0),
    };

    /// <summary>
    /// Z-Up 相机：从上方侧视 XY 地面。等价于 Yaw=135, Pitch=45, Dist=40。
    /// </summary>
    private static readonly VulkanCameraInfo ZUpTestCamera = new(
        PositionX: 20, PositionY: 20, PositionZ: 28.28f,  // orbit default position (Z-Up)
        TargetX: 0, TargetY: 0, TargetZ: 0,
        UpX: 0, UpY: 0, UpZ: 1,  // Z-Up
        FieldOfViewDegrees: 55,
        NearPlane: 0.1f,
        FarPlane: 1000f);

    [Theory]
    [MemberData(nameof(GroundPoints))]
    public void ProjectAndUnproject_ShouldReturnOriginalPosition(Vector3d worldPoint)
    {
        // 1. 使用 Z-Up 测试相机
        var camera = ZUpTestCamera;

        // 2. 计算 MVP 和逆矩阵
        var aspect = ViewportW / ViewportH;
        var mvp = VulkanCameraMatrices.ComputeVulkanMVP(camera, aspect);
        VulkanMatrixInvert.TryInvert(mvp, out var invMvp, out var invErr);
        Assert.NotNull(invMvp);

        // 3. 创建模拟 Present 的快照
        var pose = SceneCameraPose.FromOrbitState(
            SceneOrbitCameraMotion.CreateDefault(), 1);
        var snapshot = new PresentedCameraSnapshot
        {
            CameraPose = pose,
            ViewProjection = mvp,
            InverseViewProjection = invMvp!,
            ViewportWidth = (int)ViewportW,
            ViewportHeight = (int)ViewportH,
            FrameIndex = 1,
            CameraRevision = 1
        };

        // 4. 将世界点投影到屏幕
        var (pixelX, pixelY) = WorldToPixel(mvp, worldPoint);

        // 5. 从屏幕点构建射线（使用新 API：Snapshot + nearWorld 起点）
        var buildStatus = VulkanSceneRayBuilder.TryBuild(
            pixelX, pixelY,
            snapshot,
            (uint)ViewportW, (uint)ViewportH,
            out var ray);

        Assert.Equal(SceneRayBuildStatus.Success, buildStatus);
        Assert.NotNull(ray);

        // 6. 射线与地面求交
        var hit = SceneRayGroundIntersection.Intersect(ray!, Ground);
        Assert.True(hit.IsHit, $"地面求交未命中，世界点 ({worldPoint.X}, {worldPoint.Y}, {worldPoint.Z})");
        Assert.NotNull(hit.WorldPosition);

        // 7. 验证误差
        var result = hit.WorldPosition.Value;
        var dx = Math.Abs(result.X - worldPoint.X);
        var dz = Math.Abs(result.Z - worldPoint.Z);
        var dy = Math.Abs(result.Y - worldPoint.Y);

        Assert.True(dx < ErrorTolerance,
            $"X 误差过大：{dx:F6}，原始 {worldPoint.X:F6}，反投 {result.X:F6}");
        Assert.True(dy < ErrorTolerance,
            $"Y 误差过大：{dy:F6}，原始 {worldPoint.Y:F6}，反投 {result.Y:F6}");
        Assert.True(dz < ErrorTolerance,
            $"Z 误差过大：{dz:F6}，原始 {worldPoint.Z:F6}，反投 {result.Z:F6}");
    }

    /// <summary>
    /// 将世界坐标点通过 MVP 投影到屏幕像素坐标。
    /// </summary>
    private static (int PixelX, int PixelY) WorldToPixel(float[] mvp, Vector3d worldPoint)
    {
        // 将世界点转换为齐次坐标并乘以 MVP
        var wx = (float)worldPoint.X;
        var wy = (float)worldPoint.Y;
        var wz = (float)worldPoint.Z;

        // clip = MVP × world (列优先矩阵)
        var clipX = mvp[0] * wx + mvp[4] * wy + mvp[8] * wz + mvp[12];
        var clipY = mvp[1] * wx + mvp[5] * wy + mvp[9] * wz + mvp[13];
        var clipZ = mvp[2] * wx + mvp[6] * wy + mvp[10] * wz + mvp[14];
        var clipW = mvp[3] * wx + mvp[7] * wy + mvp[11] * wz + mvp[15];

        // 透视除法 → NDC
        var ndcX = clipX / clipW;
        var ndcY = clipY / clipW;

        // NDC → 像素坐标
        // 投影矩阵已通过 -f 处理 Y 翻转，因此 NDC 中 +Y 向下，与像素坐标一致。
        // pixelX = (ndcX + 1) * 0.5 * width
        // pixelY = (ndcY + 1) * 0.5 * height
        var px = (int)Math.Round((ndcX + 1.0f) * 0.5f * ViewportW);
        var py = (int)Math.Round((ndcY + 1.0f) * 0.5f * ViewportH);

        // 裁剪到视口范围内
        px = Math.Clamp(px, 0, (int)ViewportW - 1);
        py = Math.Clamp(py, 0, (int)ViewportH - 1);

        return (px, py);
    }
}
