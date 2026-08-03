using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;

namespace XuanYu.Core.Tests.Camera;

// F3-F3：Orbit 地平线合同——普通环绕保持世界 +Z Up、无 Roll、不累积倾斜。
public sealed class CameraNavigationRollTests
{
    static readonly Vector3d Center = Vector3d.Zero;
    static readonly CameraState Slant = new(new Vector3d(6, 6, 6), new Vector3d(-1, -1, -1).Normalize(),
        Vector3d.UnitZ, 60, 0.05, 200, 1);

    [Fact]
    public void Orbit_keeps_world_up_horizontal()
    {
        var result = CameraNavigation.Orbit(Slant, Center, 80, -30, 2);
        // 无 Roll 合同：Up 保持 +Z 主导且无 X 方向水平分量（不绕 Forward 旋转）。
        Assert.True(result.Camera.Up.Dot(Vector3d.UnitZ) > 0.5, "斜视 Orbit 后 Up 应保持世界 +Z 主导");
        Assert.True(System.Math.Abs(result.Camera.Up.X) < 0.2, "斜视 Orbit 后不得产生 Roll（Up 无水平横移）");
    }

    [Fact]
    public void Repeated_orbit_does_not_accumulate_roll()
    {
        var camera = Slant;
        Vector3d previousUp = camera.Up;
        for (var i = 0; i < 100; i++)
        {
            camera = CameraNavigation.Orbit(camera, Center, 7, -3, i + 2).Camera;
            // 地平线合同：Up 永不下翻；进入顶点奇异区后 Up 稳定（fallback 世界轴，不逐帧漂移）。
            Assert.True(camera.Up.Dot(Vector3d.UnitZ) > -0.2, $"第 {i} 次 Orbit 后 Up 下翻");
            Assert.True(camera.Up.Dot(previousUp) > 0.5, $"第 {i} 次 Orbit 后 Up 跳变");
            Assert.True(System.Math.Abs(camera.Forward.Dot(camera.Up)) < 1e-9);
            previousUp = camera.Up;
        }
    }

    [Fact]
    public void Orbit_after_top_view_uses_stable_up_without_flip()
    {
        var top = new CameraState(new Vector3d(0, 0, 8), new Vector3d(0, 0, -1),
            new Vector3d(0, 1, 0), 60, 0.05, 200, 1);
        var result = CameraNavigation.Orbit(top, Center, 40, -15, 2);
        // 顶视 Orbit 后仍接近顶视（Up 不翻转到 -Z，Forward 不指向 +Z）。
        Assert.True(result.Camera.Forward.Z < 0.0);
        Assert.True(result.Camera.Up.Dot(Vector3d.UnitZ) > -0.2, "顶点奇异区回退稳定 Up，不翻转");
        Assert.True(System.Math.Abs(result.Camera.Forward.Dot(result.Camera.Up)) < 1e-9);
    }

    [Fact]
    public void Orbit_after_bottom_view_keeps_valid_basis()
    {
        var bottom = new CameraState(new Vector3d(0, 0, -8), new Vector3d(0, 0, 1),
            new Vector3d(0, -1, 0), 60, 0.05, 200, 1);
        var result = CameraNavigation.Orbit(bottom, Center, -30, 10, 2);
        Assert.True(result.Camera.Forward.Z > 0.0);
        Assert.True(System.Math.Abs(result.Camera.Forward.Cross(result.Camera.Up).Length - 1.0) < 1e-6);
    }
}
