namespace XuanYu.Editor.Assets;

// D4-I1：托管规划。Assets 按 AssetId.Value 稳定排序；所有绝对路径已 GetFullPath；
// 所有 RelativePath 已通过 SceneAssetPathPolicy；规划阶段不写磁盘。
public sealed record SceneAssetHostingPlan(
    string SceneFilePath,
    string AssetRootPath,
    string StagingRootPath,
    string BackupRootPath,
    IReadOnlyList<HostedSceneAsset> Assets);
