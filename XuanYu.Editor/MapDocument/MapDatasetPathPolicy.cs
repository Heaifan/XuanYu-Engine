namespace XuanYu.Editor.MapDocument;

public static class MapDatasetPathPolicy
{
    public static bool IsSafeSource(string? source)
    {
        if (string.IsNullOrWhiteSpace(source) || source.Contains('\\') || source.Contains(':'))
            return false;
        if (Path.IsPathRooted(source) || !source.StartsWith("data/", StringComparison.Ordinal))
            return false;
        var segments = source.Split('/');
        return segments.All(segment => segment.Length > 0 && segment != "." && segment != "..");
    }

    public static bool TryResolve(string mapRoot, string? source, out string fullPath)
    {
        fullPath = "";
        if (!IsSafeSource(source) || string.IsNullOrWhiteSpace(mapRoot)) return false;
        var root = Path.GetFullPath(mapRoot.TrimEnd(Path.DirectorySeparatorChar) +
            Path.DirectorySeparatorChar);
        var candidate = Path.GetFullPath(Path.Combine(root, source!.Replace('/', Path.DirectorySeparatorChar)));
        if (!candidate.StartsWith(root, StringComparison.OrdinalIgnoreCase)) return false;
        fullPath = candidate;
        return true;
    }
}
