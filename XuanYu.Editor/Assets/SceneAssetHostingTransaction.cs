using XuanYu.Editor.SceneDocument;

namespace XuanYu.Editor.Assets;

// D4-R1：托管资源事务。Prepare 只写 staging；Activate 激活正式 .xyassets 并保留备份；
// Complete 清理备份；Rollback 恢复旧目录。状态机见 partial 文件。
public sealed partial class SceneAssetHostingTransaction
{
    readonly SceneAssetHostingPlan _plan;

    SceneAssetHostingTransaction(SceneAssetHostingPlan plan) => _plan = plan;

    public SceneAssetHostingState State { get; private set; } = SceneAssetHostingState.Prepared;
    public string LastError { get; private set; } = "";
    public SceneAssetHostingPlan Plan => _plan;

    public static SceneAssetHostingTransaction Prepare(SceneAssetHostingPlan plan)
    {
        var transaction = new SceneAssetHostingTransaction(plan);
        var result = transaction.PrepareInternal();
        if (!result.Succeeded)
        {
            transaction.State = SceneAssetHostingState.Failed;
            transaction.LastError = result.Message;
        }
        return transaction;
    }

    SceneDocumentResult<bool> PrepareInternal()
    {
        if (State != SceneAssetHostingState.Prepared)
            return Fail(SceneAssetHostingError.InvalidTransactionState, "Prepare 只能在 Prepared 状态执行。");
        if (Directory.Exists(_plan.StagingRootPath))
            return Fail(SceneAssetHostingError.StagingCreateFailed, "Staging 目录已存在。");
        try
        {
            Directory.CreateDirectory(_plan.StagingRootPath);
            foreach (var asset in _plan.Assets)
            {
                var directory = Path.GetDirectoryName(asset.StagedPath)!;
                Directory.CreateDirectory(directory);
                if (!IsInside(asset.StagedPath, _plan.StagingRootPath))
                    return Fail(SceneAssetHostingError.StagingCreateFailed, "Staged 路径逃出 staging 根。");
                var source = Path.GetFullPath(asset.SourcePath);
                File.Copy(source, asset.StagedPath, overwrite: false);
                var staged = new FileInfo(asset.StagedPath);
                var original = new FileInfo(source);
                if (!staged.Exists || staged.Length != original.Length)
                {
                    CleanupStaging();
                    return Fail(SceneAssetHostingError.CopyVerificationFailed,
                        $"复制校验失败：{asset.AssetId}，长度 {staged.Length} != {original.Length}。");
                }
            }
            return SceneDocumentResult<bool>.Ok(true);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            CleanupStaging();
            return Fail(SceneAssetHostingError.SourceCopyFailed, $"复制失败：{ex.Message}");
        }
    }

    void CleanupStaging()
    {
        try { if (Directory.Exists(_plan.StagingRootPath)) Directory.Delete(_plan.StagingRootPath, true); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }

    static bool IsInside(string path, string root)
    {
        var fullPath = Path.GetFullPath(path);
        var fullRoot = Path.GetFullPath(root);
        return fullPath.StartsWith(fullRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
    }

    static SceneDocumentResult<bool> Fail(string code, string message) =>
        SceneDocumentResult<bool>.Fail(code, message, "Transaction");
}
