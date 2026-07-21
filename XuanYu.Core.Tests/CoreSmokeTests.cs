using XuanYu.Core.Diagnostics;
using XuanYu.Core.Math;

namespace XuanYu.Core.Tests;

public sealed class CoreSmokeTests
{
    [Fact]
    public void CoreSelfTest_passes()
    {
        var report = CoreSelfTest.Run();

        Assert.True(report.IsPassed);
    }

    [Fact]
    public void Vector3d_distance_remains_deterministic()
    {
        var a = new Vector3d(1.0, 2.0, 3.0);
        var b = new Vector3d(4.0, 6.0, 3.0);

        Assert.Equal(5.0, a.DistanceTo(b), precision: 12);
    }

    [Fact]
    public void World_basis_is_right_handed_and_z_up()
    {
        Assert.Equal(Vector3d.UnitZ, Vector3d.UnitX.Cross(Vector3d.UnitY));
    }

    [Fact]
    public void Positive_yaw_rotates_local_basis_on_xy_plane_around_z_up()
    {
        var yaw = YawRotation.FromDegrees(90);

        Assert.NotEqual(Vector3d.UnitX, yaw.RotatedUnitXOnXYPlane);
        Assert.True(yaw.RotatedUnitXOnXYPlane.DistanceTo(Vector3d.UnitY) < 0.000001);
        Assert.True(yaw.RotatedUnitYOnXYPlane.DistanceTo(-Vector3d.UnitX) < 0.000001);
    }
}
