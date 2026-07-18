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
}
