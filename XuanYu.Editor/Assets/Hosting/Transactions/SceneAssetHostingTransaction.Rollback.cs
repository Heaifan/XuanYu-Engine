using XuanYu.Editor.SceneDocument;

namespace XuanYu.Editor.Assets;

// D4-I1：Rollback 恢复旧目录。旧数据安全优先于清理整洁。
public sealed partial class SceneAssetHostingTransaction
{
    public SceneDocumentResult<bool> Rollback()
    {
        if (State is SceneAssetHostingState.Completed or SceneAssetHostingState.RolledBack)
            return Fail(SceneAssetHostingError.InvalidTransactionState, "当前状态不允许回滚。");
        if (State == SceneAssetHostingState.Failed)
            return Fail(SceneAssetHostingError.InvalidTransactionState, "失败状态不允许回滚。");

        try
        {
            if (State == SceneAssetHostingState.Prepared)
            {
                CleanupStaging();
                State = SceneAssetHostingState.RolledBack;
                return SceneDocumentResult<bool>.Ok(true);
            }

            if (!Directory.Exists(_plan.BackupRootPath))
            {
                if (Directory.Exists(_plan.AssetRootPath))
                    Directory.Delete(_plan.AssetRootPath, true);
                CleanupStaging();
                State = SceneAssetHostingState.RolledBack;
                return SceneDocumentResult<bool>.Ok(true);
            }

            if (Directory.Exists(_plan.AssetRootPath))
                Directory.Delete(_plan.AssetRootPath, true);
            Directory.Move(_plan.BackupRootPath, _plan.AssetRootPath);
            CleanupStaging();
            State = SceneAssetHostingState.RolledBack;
            return SceneDocumentResult<bool>.Ok(true);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            LastError = $"回滚失败：{ex.Message}。备份目录保留：{_plan.BackupRootPath}";
            State = SceneAssetHostingState.Failed;
            return Fail(SceneAssetHostingError.RollbackFailed, LastError);
        }
    }
}
