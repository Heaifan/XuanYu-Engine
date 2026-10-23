using XuanYu.Core.Diagnostics;
using XuanYu.Render.Abstractions.Diagnostics;

namespace XuanYu.World.Tests.Diagnostics;

[Collection("XYE workspace environment")]
public sealed class XyeWorkspacePathsTests
{
    [Fact]
    public void Probe_directory_generates_run_identity_and_stays_inside_workspace()
    {
        var workspace = Environment.GetEnvironmentVariable("XYE_WORKSPACE_ROOT")
            ?? throw new InvalidOperationException("Test runner must set XYE_WORKSPACE_ROOT.");
        var testRoot = Path.Combine(workspace, "Temp", "xye-workspace-path-test", Guid.NewGuid().ToString("N"));
        var defaultWorkspace = Path.Combine(TestSourceRoot.Root, ".xye-workspace");
        var defaultWorkspaceExisted = Directory.Exists(defaultWorkspace);
        Directory.CreateDirectory(testRoot);
        using var env = new WorkspaceEnvironment();
        try
        {
            Environment.SetEnvironmentVariable("XYE_PROBE_DIRECTORY", null);
            Environment.SetEnvironmentVariable("XYE_WORKSPACE_ROOT", null);
            Environment.SetEnvironmentVariable("XYE_DIAGNOSTICS_DIRECTORY", null);
            var defaultPath = XyeWorkspacePaths.ResolveProbeDirectory();
            Assert.True(defaultPath.Contains(Path.Combine(".xye-workspace", "Diagnostics"),
                StringComparison.OrdinalIgnoreCase));

            Environment.SetEnvironmentVariable("XYE_WORKSPACE_ROOT", testRoot);
            Environment.SetEnvironmentVariable("XYE_TASK_RUN_ID", null);
            var path = XyeWorkspacePaths.ResolveProbeDirectory();
            Assert.StartsWith(Path.Combine(testRoot, "Diagnostics"), path, StringComparison.OrdinalIgnoreCase);
            Assert.True(Directory.Exists(path));

            Environment.SetEnvironmentVariable("XYE_PROBE_DIRECTORY", Path.Combine(testRoot, "..", "outside"));
            Assert.Throws<InvalidOperationException>(() => XyeWorkspacePaths.ResolveProbeDirectory());
        }
        finally
        {
            if (Directory.Exists(testRoot)) Directory.Delete(testRoot, recursive: true);
            RemoveEmptyDefaultWorkspace(defaultWorkspace, defaultWorkspaceExisted);
        }
    }

    [Fact]
    public async Task Renderer_session_uses_project_scoped_defaults_without_task_environment()
    {
        using var env = new WorkspaceEnvironment();
        Environment.SetEnvironmentVariable("XYE_WORKSPACE_ROOT", null);
        Environment.SetEnvironmentVariable("XYE_TASK_RUN_ID", null);
        Environment.SetEnvironmentVariable("XYE_PROBE_DIRECTORY", null);
        Environment.SetEnvironmentVariable("XYE_DIAGNOSTICS_DIRECTORY", null);
        var defaultWorkspace = Path.Combine(TestSourceRoot.Root, ".xye-workspace");
        var defaultWorkspaceExisted = Directory.Exists(defaultWorkspace);
        var session = RendererEventSessionFactory.TryCreate();
        try
        {
            Assert.NotNull(session);
            var sessionDirectory = Directory.GetParent(Path.GetDirectoryName(session!.RuntimeFilePath)!)!.FullName;
            Assert.True(sessionDirectory.Contains(Path.Combine(".xye-workspace", "Diagnostics"),
                StringComparison.OrdinalIgnoreCase));
            await session.DisposeAsync();
            if (Directory.Exists(sessionDirectory)) Directory.Delete(sessionDirectory, true);
        }
        finally { RemoveEmptyDefaultWorkspace(defaultWorkspace, defaultWorkspaceExisted); }
    }

    static void RemoveEmptyDefaultWorkspace(string path, bool existedBefore)
    {
        if (existedBefore) return;
        var diagnostics = Path.Combine(path, "Diagnostics");
        if (Directory.Exists(diagnostics) && !Directory.EnumerateFileSystemEntries(diagnostics).Any())
            Directory.Delete(diagnostics);
        if (Directory.Exists(path) && !Directory.EnumerateFileSystemEntries(path).Any())
            Directory.Delete(path);
    }

    sealed class WorkspaceEnvironment : IDisposable
    {
        readonly string? _root = Environment.GetEnvironmentVariable("XYE_WORKSPACE_ROOT");
        readonly string? _runId = Environment.GetEnvironmentVariable("XYE_TASK_RUN_ID");
        readonly string? _probe = Environment.GetEnvironmentVariable("XYE_PROBE_DIRECTORY");
        readonly string? _diagnostics = Environment.GetEnvironmentVariable("XYE_DIAGNOSTICS_DIRECTORY");

        public void Dispose()
        {
            Environment.SetEnvironmentVariable("XYE_WORKSPACE_ROOT", _root);
            Environment.SetEnvironmentVariable("XYE_TASK_RUN_ID", _runId);
            Environment.SetEnvironmentVariable("XYE_PROBE_DIRECTORY", _probe);
            Environment.SetEnvironmentVariable("XYE_DIAGNOSTICS_DIRECTORY", _diagnostics);
        }
    }
}

[CollectionDefinition("XYE workspace environment", DisableParallelization = true)]
public sealed class XyeWorkspaceEnvironmentCollection { }
