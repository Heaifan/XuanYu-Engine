namespace XuanYu.Engine.Project.World.SaveLoad;

/// <summary>World 文档写入结果。</summary>
public sealed record WorldDocumentWriteResult(
    bool IsSuccess,
    string? ErrorMessage)
{
    public static WorldDocumentWriteResult Success() => new(true, null);
    public static WorldDocumentWriteResult Fail(string message) => new(false, message);
}
