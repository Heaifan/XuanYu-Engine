using XuanYu.Editor.UI.XycControl;

namespace XuanYu.World.Tests.Xyc;

public sealed class XycTestDemPolicyTests
{
    [Fact]
    public void Allows_only_registered_real_dem_with_expected_hash()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
        var appBase = Path.Combine(root, "XuanYu.Editor.App", "bin", "Debug", "net10.0");
        var dem = Path.Combine(root, "artifacts", "diagnostics", "wp2-p4-r3", "n24e121.hgt");
        Assert.Equal(dem, XycTestDemPolicy.Resolve(appBase, dem));
    }

    [Fact]
    public void Rejects_paths_outside_the_fixed_diagnostic_dem()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
        var appBase = Path.Combine(root, "XuanYu.Editor.App", "bin", "Debug", "net10.0");
        Assert.Throws<UnauthorizedAccessException>(() =>
            XycTestDemPolicy.Resolve(appBase, Path.Combine(root, "README.md")));
    }
}
