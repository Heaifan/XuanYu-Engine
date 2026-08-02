namespace XuanYu.Editor.SceneDocument;

// D4：场景资产记录（D0 合同字段）。只描述托管来源，不含顶点/索引/GPU 数据。
public sealed record SceneDocumentAsset(
    string AssetId,
    string Kind,
    string RelativePath,
    string DisplayName,
    int ImporterVersion)
{
    public const string ModelGltfKind = "ModelGltf";

    public static SceneDocumentAsset ModelGltf(
        string assetId,
        string relativePath,
        string displayName,
        int importerVersion) =>
        new(assetId, ModelGltfKind, relativePath, displayName, importerVersion);
}
