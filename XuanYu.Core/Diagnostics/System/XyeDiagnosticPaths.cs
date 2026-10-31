namespace XuanYu.Core.Diagnostics;

public static class XyeDiagnosticPaths
{
    public static string ResolveProbeDirectory(string? requestedRunId = null)
    {
        var root = Path.Combine(FindProjectRoot(AppContext.BaseDirectory),
            "artifacts", "diagnostics");
        var runId = requestedRunId ??
            $"probe-{DateTime.UtcNow:yyyyMMddHHmmss}-{Environment.ProcessId}-{Guid.NewGuid():N}";
        if (string.IsNullOrWhiteSpace(runId) ||
            runId.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
            runId.Contains("..", StringComparison.Ordinal))
            throw new InvalidOperationException("Diagnostic run identity is invalid.");

        var directory = EnsureInsideProject(Path.Combine(root, runId));
        try { Directory.CreateDirectory(directory); }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            throw new InvalidOperationException(
                $"Diagnostic directory is unavailable: {directory}", error);
        }
        return directory;
    }

    public static string EnsureInsideProject(string path)
    {
        var root = FindProjectRoot(AppContext.BaseDirectory);
        var fullPath = Path.GetFullPath(path);
        var prefix = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar) +
            Path.DirectorySeparatorChar;
        if (!fullPath.StartsWith(prefix, PathComparison))
            throw new InvalidOperationException(
                $"Diagnostic path must stay under the canonical project: {fullPath}");
        return fullPath;
    }

    static string FindProjectRoot(string start)
    {
        for (var current = new DirectoryInfo(Path.GetFullPath(start));
             current is not null; current = current.Parent)
            if (File.Exists(Path.Combine(current.FullName, ".git")) ||
                Directory.Exists(Path.Combine(current.FullName, ".git")))
                return current.FullName;
        throw new DirectoryNotFoundException("Could not locate the canonical project root.");
    }

    static StringComparison PathComparison => OperatingSystem.IsWindows()
        ? StringComparison.OrdinalIgnoreCase
        : StringComparison.Ordinal;
}
