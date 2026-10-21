using XuanYu.Core.Diagnostics;

namespace XuanYu.Render.Abstractions.Diagnostics;

public static class RendererEventSessionFactory
{
    public static RendererEventSession? TryCreate()
    {
        var runId = Environment.GetEnvironmentVariable("XYE_TASK_RUN_ID");
        if (string.IsNullOrWhiteSpace(runId))
            runId = $"local-{DateTime.UtcNow:yyyyMMddHHmmss}-{Environment.ProcessId}-{Guid.NewGuid():N}";
        try
        {
            var directory = Environment.GetEnvironmentVariable("XYE_DIAGNOSTICS_DIRECTORY");
            directory = string.IsNullOrWhiteSpace(directory) ? XyeWorkspacePaths.ResolveProbeDirectory(runId) : directory;
            directory = XyeWorkspacePaths.EnsureInsideWorkspace(directory);
            var probe = Environment.GetEnvironmentVariable("XYE_RENDERER_PROBE") == "1";
            return RendererEventSession.Create(new RendererEventSessionOptions(directory, runId,
                RendererBuildIdentity.Current(), probe));
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or
            InvalidOperationException or ArgumentException) { return null; }
    }
}
