namespace XuanYu.Core.Diagnostics;

public static class XyeWorkspacePaths
{
    public static string ResolveProbeDirectory()
    {
        var root = GetRootDirectory();
        if (!Directory.Exists(root))
            throw new InvalidOperationException($"XYE workspace root is unavailable: {root}");

        var runId = Environment.GetEnvironmentVariable("XYE_TASK_RUN_ID");
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
        var rootValue = Environment.GetEnvironmentVariable("XYE_WORKSPACE_ROOT");
        if (string.IsNullOrWhiteSpace(rootValue))
            throw new InvalidOperationException("XYE_WORKSPACE_ROOT is required for XYE diagnostics.");
        return Path.GetFullPath(rootValue.Trim()).TrimEnd(Path.DirectorySeparatorChar) +
            Path.DirectorySeparatorChar;
    }
}
