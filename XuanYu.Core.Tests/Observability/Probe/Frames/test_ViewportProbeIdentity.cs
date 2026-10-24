using XuanYu.Core.Diagnostics;

namespace XuanYu.Core.Tests.Observability.Probe.Frames;

public sealed class ViewportProbeIdentityTests
{
    [Fact]
    public void Identity_contains_head_dirty_probe_and_build()
    {
        var id = ViewportProbeIdentity.Format(
            "a3d5787b", true, "INSTRUMENT-VIEWPORT-ZOOM-GROUND-DEM-R1",
            "2026-10-03T15:00:00+08:00");
        Assert.Equal(
            "HEAD=a3d5787b;DIRTY=YES;PROBE=INSTRUMENT-VIEWPORT-ZOOM-GROUND-DEM-R1;BUILD=2026-10-03T15:00:00+08:00",
            id);
    }

    [Fact]
    public void Identity_formats_clean_source()
    {
        Assert.Equal("HEAD=abc;DIRTY=NO;PROBE=probe;BUILD=build",
            ViewportProbeIdentity.Format("abc", false, "probe", "build"));
    }
}
