namespace XuanYu.Editor.Assets;

// D4-R1：托管资产项。SourcePath 是 D3 导入时记录的规范化绝对路径（运行时来源）；
// RelativePath 固定为 models/<AssetId>/source.glb；StagedPath/FinalPath 由事务填充。
public readonly record struct HostedSceneAsset(
    AssetId AssetId,
    string SourcePath,
    string RelativePath,
    string StagedPath,
    string FinalPath);
