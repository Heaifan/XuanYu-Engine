namespace FluidWarfare.Bridge.ProjectEngine.World;

/// <summary>
/// 保存项目内容生成 World 占位实体的结果。
/// </summary>
public sealed record ProjectContentWorldSeedResult(
    int CreatedEntityCount,
    IReadOnlyList<string> SourcePaths);
