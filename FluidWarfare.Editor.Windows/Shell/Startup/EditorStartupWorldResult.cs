using FluidWarfare.Core.Identity;
using FluidWarfare.Engine.World;
using FluidWarfare.Render.Scene;

namespace FluidWarfare.Editor.Windows.Shell.Startup;

/// <summary>World 引导结果封装，包含世界状态、RenderScene、首位实体 ID、日志等。</summary>
public sealed record EditorStartupWorldResult(
    WorldState? World,
    RenderScene RenderScene,
    EntityId FirstEntityId,
    IReadOnlyList<string> SeedSourcePaths,
    IReadOnlyList<string> LogMessages,
    IReadOnlyList<string> LogWarnings)
{
    public bool HasEntities => World is not null;

    public static readonly EditorStartupWorldResult Empty = new(
        null, RenderScene.Empty, default, [],
        ["项目中没有可生成 World 占位实体的单位模板文件。"],
        ["项目中没有可生成 World 占位实体的单位模板文件。"]);

    public static EditorStartupWorldResult Created(
        WorldState world, RenderScene renderScene,
        EntityId firstId, IReadOnlyList<string> sources) =>
        new(world, renderScene, firstId, sources,
            ["最小 World 已创建。",
            ..sources.Select(s => $"已从项目内容生成 World 占位实体：{s}。"),
            $"RenderScene 已生成，渲染对象数量：{renderScene.Objects.Count}。"],
            []);
}
