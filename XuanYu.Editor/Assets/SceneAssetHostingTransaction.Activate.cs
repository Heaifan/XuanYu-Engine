using XuanYu.Editor.SceneDocument;

namespace XuanYu.Editor.Assets;

// D4-R1：Activate 将 staging 激活为正式 .xyassets，同时保留旧目录为备份。
public sealed partial class SceneAssetHostingTransaction
{
    public SceneDocumentResult<bool> Activate()
    {
        if (State != SceneAssetHostingState.Prepared)
            return Fail(SceneAssetHostingError.InvalidTransactionState, "Activate 只能在 Prepared 状态执行。");
        if (!Directory.Exists(_plan.StagingRootPath))
        {
            LastError = "Staging 不存在，无法激活。";
            State = SceneAssetHostingState.Failed;
            return Fail(SceneAssetHostingError.ActivateFailed, LastError);
        }

        try
        {
            if (!Directory.Exists(_plan.AssetRootPath))
            {
                Directory.Move(_plan.StagingRootPath, _plan.AssetRootPath);
            }
            else
            {
                if (Directory.Exists(_plan.BackupRootPath))
                {
                    LastError = "备份目录已存在，无法激活。";
                    State = SceneAssetHostingState.Failed;
                    return Fail(SceneAssetHostingError.ActivateFailed, LastError);
                }
                Directory.Move(_plan.AssetRootPath, _plan.BackupRootPath);
                try
                {
                    Directory.Move(_plan.StagingRootPath, _plan.AssetRootPath);
                }
                catch
                {
                    TryMoveBack(_plan.BackupRootPath, _plan.AssetRootPath);
                    CleanupStaging();
                    LastError = "激活失败，已尝试恢复旧目录。";
                    State = SceneAssetHostingState.Failed;
                    return Fail(SceneAssetHostingError.ActivateFailed, LastError);
                }
            }
            State = SceneAssetHostingState.Activated;
            return SceneDocumentResult<bool>.Ok(true);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            LastError = $"激活失败：{ex.Message}";
            State = SceneAssetHostingState.Failed;
            return Fail(SceneAssetHostingError.ActivateFailed, LastError);
        }
    }

    static void TryMoveBack(string from, string to)
    {
        try
        {
            if (Directory.Exists(from) && !Directory.Exists(to))
                Directory.Move(from, to);
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}
