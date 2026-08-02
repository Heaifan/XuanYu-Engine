using XuanYu.Editor.SceneDocument;

namespace XuanYu.Editor.Assets;

// D4-R1：Complete 在后续场景文件保存成功后调用，删除备份并收尾。
public sealed partial class SceneAssetHostingTransaction
{
    public SceneDocumentResult<bool> Complete()
    {
        if (State != SceneAssetHostingState.Activated)
            return Fail(SceneAssetHostingError.InvalidTransactionState, "Complete 只能在 Activated 状态执行。");
        if (Directory.Exists(_plan.StagingRootPath))
        {
            LastError = "Staging 仍存在，不得 Complete。";
            State = SceneAssetHostingState.Failed;
            return Fail(SceneAssetHostingError.CompleteFailed, LastError);
        }

        try
        {
            if (Directory.Exists(_plan.BackupRootPath))
                Directory.Delete(_plan.BackupRootPath, true);
            State = SceneAssetHostingState.Completed;
            return SceneDocumentResult<bool>.Ok(true);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            LastError = $"删除备份失败：{ex.Message}";
            State = SceneAssetHostingState.Failed;
            return Fail(SceneAssetHostingError.CompleteFailed, LastError);
        }
    }
}
