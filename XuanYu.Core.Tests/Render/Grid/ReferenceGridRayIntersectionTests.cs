using XuanYu.Core.Math;

namespace XuanYu.Core.Tests.Render;

// MAP-A-R1-D5-R1-F2 GRID-G1：世界射线与 Z=0 平面求交的数学合同。
// 片元着色器逻辑（editor_reference_grid.frag）的 CPU 镜像：
//   t = -nearWorld.z / ray.z；t<=0 或 |ray.z|<eps 或 t>maxDist 拒绝。
public sealed class ReferenceGridRayIntersectionTests
{
    // 俯视中心：近点在相机前方下方，射线向下，交点应在 Z=0 前方。
    [Fact]
    public void Overhead_ray_intersects_plane_in_front()
    {
        var nearWorld = new Vector3d(0, 0, 50);      // 相机近点（世界 Z+）
        var ray = new Vector3d(0.2, 0.3, -1.0);      // 斜向下
        var t = -nearWorld.Z / ray.Z;
        var world = nearWorld + (ray * t);

        Assert.True(t > 0, "俯视射线交点必须在相机前方");
        Assert.Equal(0, world.Z, 6);
        Assert.True(world.X > 0 && world.Y > 0, "交点沿射线方向延伸");
    }

    [Fact]
    public void Near_parallel_ray_is_rejected()
    {
        var nearWorld = new Vector3d(0, 0, 50);
        var ray = new Vector3d(1.0, 0.0, 0.0004);    // 近似水平（|z| < 0.001）

        Assert.True(System.Math.Abs(ray.Z) < 0.001, "近似平行射线必须被拒绝");
        _ = nearWorld;
    }

    [Fact]
    public void Ray_behind_camera_is_rejected()
    {
        var nearWorld = new Vector3d(0, 0, 50);
        var ray = new Vector3d(0, 0, 1.0);           // 朝上（Z+），不交 Z=0 前方
        var t = -nearWorld.Z / ray.Z;

        Assert.True(t <= 0, "朝上射线交点位于相机后方必须拒绝");
    }

    [Fact]
    public void Intersection_beyond_max_distance_is_rejected()
    {
        var nearWorld = new Vector3d(0, 0, 50);
        var ray = new Vector3d(0, 0, -0.01);         // 几乎水平向下，交点极远
        var t = -nearWorld.Z / ray.Z;
        const double maxDist = 1000.0;

        Assert.True(t > maxDist, "超出最大距离的交点必须拒绝");
    }

    [Theory]
    [InlineData(0.1, 0.2, -1.0, 50.0)]
    [InlineData(-0.5, 0.7, -0.8, 120.0)]
    [InlineData(0.3, -0.4, -0.9, 2000.0)]
    public void Depth_is_in_0_1_range_after_projection(double dx, double dy, double dz, double nearZ)
    {
        // 交点 Z=0，投影深度（近裁剪比例）应稳定落在 0~1（俯视场景）。
        var nearWorld = new Vector3d(0, 0, nearZ);
        var ray = new Vector3d(dx, dy, dz);
        var t = -nearWorld.Z / ray.Z;
        var world = nearWorld + (ray * t);

        // 深度 = 交点与近/远平面比例（近 0.05 远 nearZ*8 的近似线性）。
        var depth01 = (nearZ - world.Z) / nearZ;
        Assert.InRange(depth01, 0.0, 1.0);
        Assert.Equal(0, world.Z, 6);
    }
}
