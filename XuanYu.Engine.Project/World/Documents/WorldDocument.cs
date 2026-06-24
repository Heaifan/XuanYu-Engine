namespace XuanYu.Engine.Project.World.Documents;

/// <summary>World 文档根节点。表示一个完整的 World 文件。</summary>
public sealed record WorldDocument(
    int SchemaVersion,
    string WorldId,
    string DisplayName,
    IReadOnlyList<WorldEntityDocument> Entities,
    WorldMetadataDocument Metadata);
