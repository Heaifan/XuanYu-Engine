using XuanYu.Editor.SceneDocument;

namespace XuanYu.Editor.Assets;

// D4-I1：托管规划生成。只计算路径与规划，不写磁盘。
public static class SceneAssetHostingPlanner
{
    public static SceneDocumentResult<SceneAssetHostingPlan> Create(
        string sceneFilePath,
        IReadOnlyList<SceneStaticModelBinding> bindings)
    {
        if (string.IsNullOrWhiteSpace(sceneFilePath))
            return Fail(SceneAssetHostingError.InvalidScenePath, "场景路径不能为空。", sceneFilePath);
        var sceneFull = Path.GetFullPath(sceneFilePath);
        if (!string.Equals(Path.GetExtension(sceneFull), ".xyscene", StringComparison.OrdinalIgnoreCase))
            return Fail(SceneAssetHostingError.InvalidScenePath, "场景文件扩展名必须是 .xyscene。", sceneFull);
        var directory = Path.GetDirectoryName(sceneFull);
        var name = Path.GetFileNameWithoutExtension(sceneFull);
        if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
            return Fail(SceneAssetHostingError.InvalidScenePath, "场景所在目录不存在。", sceneFull);
        if (string.IsNullOrWhiteSpace(name))
            return Fail(SceneAssetHostingError.InvalidScenePath, "场景文件名不能为空。", sceneFull);

        var assetRoot = Path.Combine(directory, name + SceneAssetPathPolicy.AssetFolderExtension);
        var stagingRoot = Path.Combine(directory, $".{name}.xyassets.staging-{Guid.NewGuid():N}");
        var backupRoot = Path.Combine(directory, $".{name}.xyassets.backup-{Guid.NewGuid():N}");

        var dedup = new Dictionary<AssetId, string>();
        var assets = new List<HostedSceneAsset>();
        foreach (var binding in bindings)
        {
            if (string.IsNullOrEmpty(binding.AssetId.Value) || !binding.AssetId.IsValid)
                return Fail(SceneAssetHostingError.InvalidAssetId, "资产 ID 非法。", binding.AssetId.ToString());
            if (dedup.TryGetValue(binding.AssetId, out var existing))
            {
                if (!SamePath(existing, binding.SourcePath))
                    return Fail(SceneAssetHostingError.AssetSourceConflict,
                        $"同一资产 ID 对应不同来源：{existing} 与 {binding.SourcePath}。", binding.AssetId.ToString());
                continue;
            }

            var source = ValidateSource(binding, out var error, out var detail);
            if (source is null) return Fail(error, detail, binding.AssetId.ToString());
            var relative = SceneAssetPathPolicy.ModelSourceRelativePath(binding.AssetId);
            if (!SceneAssetPathPolicy.IsSafeRelativePath(relative))
                return Fail(SceneAssetHostingError.UnsafeManagedRelativePath,
                    "托管相对路径不安全。", relative);
            if (!SceneAssetPathPolicy.TryResolveManagedPath(assetRoot, relative, out var finalPath))
                return Fail(SceneAssetHostingError.UnsafeManagedRelativePath,
                    "托管目标路径逃出资产根目录。", relative);

            var staged = Path.Combine(stagingRoot, relative.Replace('/', Path.DirectorySeparatorChar));
            dedup[binding.AssetId] = binding.SourcePath;
            assets.Add(new HostedSceneAsset(binding.AssetId, binding.SourcePath, relative, staged, finalPath));
        }

        var ordered = assets.OrderBy(a => a.AssetId.Value, StringComparer.Ordinal).ToArray();
        return SceneDocumentResult<SceneAssetHostingPlan>.Ok(
            new SceneAssetHostingPlan(sceneFull, assetRoot, stagingRoot, backupRoot, ordered));
    }

    static string? ValidateSource(SceneStaticModelBinding binding, out string error, out string detail)
    {
        error = ""; detail = "";
        if (string.IsNullOrWhiteSpace(binding.SourcePath))
        { error = SceneAssetHostingError.InvalidSourcePath; detail = "来源路径为空。"; return null; }
        if (!Path.IsPathFullyQualified(binding.SourcePath))
        { error = SceneAssetHostingError.InvalidSourcePath; detail = "来源必须是绝对路径。"; return null; }
        var full = Path.GetFullPath(binding.SourcePath);
        if (Directory.Exists(full))
        { error = SceneAssetHostingError.InvalidSourcePath; detail = "来源是目录而非文件。"; return null; }
        if (!File.Exists(full))
        { error = SceneAssetHostingError.SourceFileMissing; detail = "来源文件不存在。"; return null; }
        if (!string.Equals(Path.GetExtension(full), ".glb", StringComparison.OrdinalIgnoreCase))
        { error = SceneAssetHostingError.UnsupportedSourceExtension; detail = "来源扩展名必须是 .glb。"; return null; }
        try
        {
            using var stream = File.OpenRead(full);
            if (stream.Length <= 0)
            { error = SceneAssetHostingError.InvalidSourcePath; detail = "来源文件为空。"; return null; }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        { error = SceneAssetHostingError.InvalidSourcePath; detail = $"来源不可读取：{ex.Message}"; return null; }
        return full;
    }

    static bool SamePath(string a, string b) =>
        string.Equals(Path.GetFullPath(a), Path.GetFullPath(b), StringComparison.OrdinalIgnoreCase);

    static SceneDocumentResult<SceneAssetHostingPlan> Fail(string code, string message, string detail) =>
        SceneDocumentResult<SceneAssetHostingPlan>.Fail(code, message, "Plan", detail);
}
