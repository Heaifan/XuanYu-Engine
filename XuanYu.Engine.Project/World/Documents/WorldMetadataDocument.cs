namespace XuanYu.Engine.Project.World.Documents;

/// <summary>World 文件元数据。</summary>
public sealed record WorldMetadataDocument(
    string CreatedBy,
    string Note)
{
    /// <summary>默认元数据，供保存时使用。</summary>
    public static readonly WorldMetadataDocument Default = new(
        "XuanYu Engine", "Milestone 9.0A World 保存 / 加载");
}
