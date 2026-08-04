namespace XuanYu.Editor.Assets;

// D4-I1：托管事务错误码。复用 SceneDocumentResult 的 ErrorCode 字符串约定，
// 不创建独立错误框架；不得把所有失败退化为 Unknown。
public static class SceneAssetHostingError
{
    public const string InvalidScenePath = "InvalidScenePath";
    public const string InvalidAssetId = "InvalidAssetId";
    public const string InvalidSourcePath = "InvalidSourcePath";
    public const string SourceFileMissing = "SourceFileMissing";
    public const string UnsupportedSourceExtension = "UnsupportedSourceExtension";
    public const string AssetSourceConflict = "AssetSourceConflict";
    public const string UnsafeManagedRelativePath = "UnsafeManagedRelativePath";
    public const string StagingCreateFailed = "StagingCreateFailed";
    public const string SourceCopyFailed = "SourceCopyFailed";
    public const string CopyVerificationFailed = "CopyVerificationFailed";
    public const string ActivateFailed = "ActivateFailed";
    public const string CompleteFailed = "CompleteFailed";
    public const string RollbackFailed = "RollbackFailed";
    public const string InvalidTransactionState = "InvalidTransactionState";
}
