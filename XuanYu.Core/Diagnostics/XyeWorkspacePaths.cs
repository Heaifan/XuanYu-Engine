namespace XuanYu.Core.Diagnostics;

public static class XyeWorkspacePaths
{
    public static string ResolveProbeDirectory(string? requestedRunId = null)
    {
        var root = GetRootDirectory();
        Directory.CreateDirectory(root);

        var runId = requestedRunId ?? Environment.GetEnvironmentVariable("XYE_TASK_RUN_ID");
        if (string.IsNullOrWhiteSpace(runId))
            runId = $"local-{DateTime.UtcNow:yyyyMMddHHmmss}-{Environment.ProcessId}-{Guid.NewGuid():N}";
        if (string.IsNullOrWhiteSpace(runId) || runId.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
            runId.Contains("..", StringComparison.Ordinal))
            throw new InvalidOperationException("XYE_TASK_RUN_ID is required for XYE diagnostics.");

        var requested = Environment.GetEnvironmentVariable("XYE_PROBE_DIRECTORY");
        var directory = string.IsNullOrWhiteSpace(requested)
            ? Path.Combine(root, "Diagnostics", runId)
            : Path.GetFullPath(requested);
        var fullDirectory = EnsureInsideWorkspace(directory);

        try { Directory.CreateDirectory(fullDirectory); }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            throw new InvalidOperationException($"XYE probe directory is unavailable: {fullDirectory}", error);
        }
        return fullDirectory;
    }

    public static string EnsureInsideWorkspace(string path)
    {
        var root = GetRootDirectory();
        var fullPath = Path.GetFullPath(path);
        if (!fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"XYE workspace path must stay under XYE_WORKSPACE_ROOT: {fullPath}");
        return fullPath;
    }

    static string GetRootDirectory()
    {
        var projectRoot = FindProjectRoot(AppContext.BaseDirectory) ??
            Path.GetFullPath(AppContext.BaseDirectory);
        var rootValue = Environment.GetEnvironmentVariable("XYE_WORKSPACE_ROOT");
        var root = string.IsNullOrWhiteSpace(rootValue)
            ? Path.Combine(projectRoot, ".xye-workspace")
            : Path.GetFullPath(rootValue.Trim());
        if (!IsWithin(projectRoot, root))
            throw new InvalidOperationException("XYE_WORKSPACE_ROOT must stay inside the project.");
        return root.TrimEnd(Path.DirectorySeparatorChar) +
            Path.DirectorySeparatorChar;
    }

    static string? FindProjectRoot(string start)
    {
        for (var current = new DirectoryInfo(Path.GetFullPath(start)); current is not null; current = current.Parent)
            if (File.Exists(Path.Combine(current.FullName, ".git")) || Directory.Exists(Path.Combine(current.FullName, ".git")))
                return current.FullName;
        return null;
    }

    static bool IsWithin(string root, string path)
    {
        var prefix = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var full = Path.GetFullPath(path);
        return full.Equals(prefix.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase) ||
            full.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }
}
