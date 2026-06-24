namespace XuanYu.Engine.Project.World.Documents;

/// <summary>World 文件中一个实体的记录。</summary>
public sealed record WorldEntityDocument(
    string EntityId,
    string DisplayName,
    IReadOnlyList<WorldComponentDocument> Components);
