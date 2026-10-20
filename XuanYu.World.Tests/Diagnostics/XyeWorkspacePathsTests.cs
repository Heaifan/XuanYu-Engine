using XuanYu.Core.Diagnostics;

namespace XuanYu.World.Tests.Diagnostics;

[Collection("XYE workspace environment")]
public sealed class XyeWorkspacePathsTests
{
    [Fact]
    public void Probe_directory_requires_configured_run_context_and_stays_inside_root()
    {
        var workspace = Environment.GetEnvironmentVariable("XYE_WORKSPACE_ROOT")
            ?? throw new InvalidOperationException("Test runner must set XYE_WORKSPACE_ROOT.");
        var testRoot = Path.Combine(workspace, "Temp", "xye-workspace-path-test", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(testRoot);
        using var env = new WorkspaceEnvironment();
        try
        {
            Environment.SetEnvironmentVariable("XYE_PROBE_DIRECTORY", null);
            Environment.SetEnvironmentVariable("XYE_WORKSPACE_ROOT", null);
            Assert.Throws<InvalidOperationException>(() => XyeWorkspacePaths.ResolveProbeDirectory());

            Environment.SetEnvironmentVariable("XYE_WORKSPACE_ROOT", testRoot);
            Environment.SetEnvironmentVariable("XYE_TASK_RUN_ID", null);
            Assert.Throws<InvalidOperationException>(() => XyeWorkspacePaths.ResolveProbeDirectory());

            Environment.SetEnvironmentVariable("XYE_TASK_RUN_ID", "run-1");
            var path = XyeWorkspacePaths.ResolveProbeDirectory();
            Assert.Equal(Path.Combine(testRoot, "Diagnostics", "run-1"), path);
            Assert.True(Directory.Exists(path));

            Environment.SetEnvironmentVariable("XYE_PROBE_DIRECTORY", Path.Combine(testRoot, "..", "outside"));
            Assert.Throws<InvalidOperationException>(() => XyeWorkspacePaths.ResolveProbeDirectory());
        }
        finally { if (Directory.Exists(testRoot)) Directory.Delete(testRoot, recursive: true); }
    }

    sealed class WorkspaceEnvironment : IDisposable
    {
        readonly string? _root = Environment.GetEnvironmentVariable("XYE_WORKSPACE_ROOT");
        readonly string? _runId = Environment.GetEnvironmentVariable("XYE_TASK_RUN_ID");
        readonly string? _probe = Environment.GetEnvironmentVariable("XYE_PROBE_DIRECTORY");

        public void Dispose()
        {
            Environment.SetEnvironmentVariable("XYE_WORKSPACE_ROOT", _root);
            Environment.SetEnvironmentVariable("XYE_TASK_RUN_ID", _runId);
            Environment.SetEnvironmentVariable("XYE_PROBE_DIRECTORY", _probe);
        }
    }
}

[CollectionDefinition("XYE workspace environment", DisableParallelization = true)]
public sealed class XyeWorkspaceEnvironmentCollection { }
