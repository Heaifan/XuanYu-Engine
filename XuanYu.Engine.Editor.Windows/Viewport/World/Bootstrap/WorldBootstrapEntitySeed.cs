using XuanYu.Engine.World;
using XuanYu.Engine.Project.Content;
using XuanYu.Engine.Bridge.ProjectEngine.World;

namespace FluidWarfare.Editor.Windows.Viewport.World.Bootstrap;

/// <summary>从项目内容文件生成 World 占位实体。纯数据逻辑。</summary>
public static class WorldBootstrapEntitySeed
{
    public static (WorldState World, int Count, IReadOnlyList<string> SourcePaths)
        Seed(IReadOnlyList<GameContentFileInfo> contentFiles, WorldState? existingWorld = null)
    {
        var world = existingWorld ?? new WorldState();
        var result = ProjectContentWorldSeeder.SeedUnitTemplatePlaceholders(world, contentFiles);
        return (world, result.CreatedEntityCount, result.SourcePaths);
    }
}
