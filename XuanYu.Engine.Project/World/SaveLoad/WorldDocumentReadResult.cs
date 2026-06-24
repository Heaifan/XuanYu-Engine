using XuanYu.Engine.Project.World.Documents;

namespace XuanYu.Engine.Project.World.SaveLoad;

/// <summary>World 文档读取结果。</summary>
public sealed record WorldDocumentReadResult(
    bool IsSuccess,
    WorldDocument? Document,
    string? ErrorMessage)
{
    public static WorldDocumentReadResult Success(WorldDocument document) => new(true, document, null);
    public static WorldDocumentReadResult Fail(string message) => new(false, null, message);
}
