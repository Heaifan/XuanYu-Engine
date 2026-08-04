namespace XuanYu.Editor.Assets;

public static class SceneAssetPathPolicy
{
    public const string AssetFolderExtension = ".xyassets";

    public static string AssetRootForScene(string scenePath)
    {
        var directory = Path.GetDirectoryName(scenePath) ?? "";
        var name = Path.GetFileNameWithoutExtension(scenePath);
        return Path.Combine(directory, name + AssetFolderExtension);
    }

    public static string ModelSourceRelativePath(AssetId assetId) =>
        Path.Combine("models", assetId.Value, "source.glb").Replace('\\', '/');

    public static bool IsSafeRelativePath(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return false;
        if (Path.IsPathFullyQualified(relativePath)) return false;
        if (relativePath.Contains('\\')) return false;
        var parts = relativePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return false;
        return parts.All(p => p != "." && p != ".." && !p.Contains(':'));
    }

    public static bool TryResolveManagedPath(
        string assetRoot,
        string relativePath,
        out string fullPath)
    {
        fullPath = "";
        if (!IsSafeRelativePath(relativePath)) return false;
        var root = Path.GetFullPath(assetRoot);
        var candidate = Path.GetFullPath(Path.Combine(root, relativePath));
        if (!candidate.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            return false;
        fullPath = candidate;
        return true;
    }
}
