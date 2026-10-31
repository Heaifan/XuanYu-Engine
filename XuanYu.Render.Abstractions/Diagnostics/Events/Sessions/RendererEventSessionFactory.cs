using XuanYu.Core.Diagnostics;

namespace XuanYu.Render.Abstractions.Diagnostics;

public static class RendererEventSessionFactory
{
    public static RendererEventSession? TryCreate()
    {
        var runId = $"probe-{DateTime.UtcNow:yyyyMMddHHmmss}-{Environment.ProcessId}-{Guid.NewGuid():N}";
        try
        {
            var directory = XyeDiagnosticPaths.ResolveProbeDirectory(runId);
            directory = XyeDiagnosticPaths.EnsureInsideProject(directory);
            var probe = Environment.GetEnvironmentVariable("XYE_RENDERER_PROBE") == "1";
            return RendererEventSession.Create(new RendererEventSessionOptions(directory, runId,
                RendererBuildIdentity.Current(), probe));
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or
            InvalidOperationException or ArgumentException) { return null; }
    }
}
