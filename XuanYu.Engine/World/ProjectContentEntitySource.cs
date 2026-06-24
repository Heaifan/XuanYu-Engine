namespace XuanYu.Engine.World;

/// <summary>
/// 保存 World 实体的项目内容来源路径与内容类型。
/// 不读取文件，不解析 JSON，不依赖 Project，不依赖 Editor。
/// </summary>
public sealed record ProjectContentEntitySource(
    string RelativePath,
    string ContentKind);
