using System.Security.Cryptography;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.World.Tests.Xyc;

public sealed class XycTestDemPolicyTests
{
    [Fact]
    public void Registered_dem_fixture_has_expected_size_and_hash()
    {
        var dem = Path.Combine(TestSourceRoot.Root, "artifacts", "diagnostics", "wp2-p4-r3", "n24e121.hgt");
        Assert.True(File.Exists(dem), $"Required DEM fixture is missing: {dem}");
        Assert.Equal(25_934_402, new FileInfo(dem).Length);
        using var stream = File.OpenRead(dem);
        Assert.Equal(XycTestDemPolicy.SourceSha256, Convert.ToHexString(SHA256.HashData(stream)));
    }

    [Fact]
    public void Allows_only_registered_real_dem_with_expected_hash()
    {
        var root = TestSourceRoot.Root;
        var appBase = Path.Combine(root, "XuanYu.Editor.App", "bin", "Debug", "net10.0");
        var dem = Path.Combine(root, "artifacts", "diagnostics", "wp2-p4-r3", "n24e121.hgt");
        Assert.Equal(dem, XycTestDemPolicy.Resolve(appBase, dem));
    }

    [Fact]
    public void Rejects_paths_outside_the_fixed_diagnostic_dem()
    {
        var root = TestSourceRoot.Root;
        var appBase = Path.Combine(root, "XuanYu.Editor.App", "bin", "Debug", "net10.0");
        Assert.Throws<UnauthorizedAccessException>(() =>
            XycTestDemPolicy.Resolve(appBase, Path.Combine(root, "README.md")));
    }
}
