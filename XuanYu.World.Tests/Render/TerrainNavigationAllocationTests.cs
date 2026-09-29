using System.Diagnostics;
using System.Numerics;
using Xunit.Abstractions;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;

namespace XuanYu.World.Tests.Render;

public sealed partial class TerrainNavigationAllocationTests(ITestOutputHelper output)
{
    [Fact]
    public void Empty_small_and_real_dem_navigation_reports_before_and_after()
    {
        var state = ViewState();
        foreach (var (name, count) in new[] { ("EMPTY", 0), ("SMALL", 1), ("REAL_DEM", 225) })
        {
            var chunks = Chunks(count);
            var before = MeasureLegacy(state, chunks);
            var after = MeasureCurrent(state, chunks);
            output.WriteLine($"{name}: BEFORE allocation={before.Allocated}, cpuMs={before.CpuMs:F2}, " +
                $"chunks={count}, visible={before.Visible / 60}; AFTER allocation={after.Allocated}, " +
                $"cpuMs={after.CpuMs:F2}, chunks={count}, visible={after.Visible / 60}");
            Assert.Equal(before.Visible, after.Visible);
            if (name == "REAL_DEM") Assert.InRange(after.Allocated, 0, 256_000);
        }
    }

}
