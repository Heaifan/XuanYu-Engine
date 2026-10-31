using XuanYu.Core.Diagnostics;

namespace XuanYu.World.Tests.Diagnostics;

public sealed class XyeDiagnosticPathsTests
{
    [Fact]
    public void Probe_directories_are_unique_and_inside_the_canonical_project()
    {
        var first = XyeDiagnosticPaths.ResolveProbeDirectory();
        var second = XyeDiagnosticPaths.ResolveProbeDirectory();
        try
        {
            var diagnostics = Path.Combine(TestSourceRoot.Root, "artifacts", "diagnostics");
            Assert.StartsWith(diagnostics, first, StringComparison.OrdinalIgnoreCase);
            Assert.StartsWith(diagnostics, second, StringComparison.OrdinalIgnoreCase);
            Assert.NotEqual(first, second);
            Assert.True(Directory.Exists(first));
            Assert.True(Directory.Exists(second));
        }
        finally
        {
            if (Directory.Exists(first)) Directory.Delete(first, recursive: true);
            if (Directory.Exists(second)) Directory.Delete(second, recursive: true);
        }
    }

    [Fact]
    public void Probe_path_rejects_paths_outside_the_project()
    {
        var outside = Path.Combine(Directory.GetParent(TestSourceRoot.Root)!.FullName,
            "outside-diagnostic-probe");
        Assert.Throws<InvalidOperationException>(() =>
            XyeDiagnosticPaths.EnsureInsideProject(outside));
    }
}
