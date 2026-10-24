using XuanYu.Core.Diagnostics;

namespace XuanYu.Core.Tests.Observability.Runtime;

public sealed class XyeWorkspacePathsTests
{
    static string Workspace() =>
        Path.Combine(Path.GetTempPath(), "xye-audit-workspace-root");

    [Fact]
    public void AllowsChildDirectoriesWithoutWritingFiles()
    {
        var root = Workspace();
        var child = Path.Combine(root, "Diagnostics", "capture");

        Assert.True(XyeWorkspacePaths.IsChildOf(root, child));
    }

    [Fact]
    public void RejectsRootAndSamePrefixSibling()
    {
        var root = Workspace();

        Assert.False(XyeWorkspacePaths.IsChildOf(root, root));
        Assert.False(XyeWorkspacePaths.IsChildOf(root, root + "-other"));
    }

    [Fact]
    public void RejectsTraversalOutsideWorkspace()
    {
        var root = Workspace();
        var outside = Path.Combine(root, "..", "sibling");

        Assert.False(XyeWorkspacePaths.IsChildOf(root, outside));
    }

    [Fact]
    public void UsesPlatformAppropriateCaseComparison()
    {
        var root = Workspace();
        var upper = Path.Combine(root.ToUpperInvariant(), "Diagnostics");
        var expected = OperatingSystem.IsWindows();

        Assert.Equal(expected, XyeWorkspacePaths.IsChildOf(root, upper));
    }
}
