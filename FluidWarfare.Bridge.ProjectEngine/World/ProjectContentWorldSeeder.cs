using FluidWarfare.Core.Math;
using FluidWarfare.Engine.World;
using FluidWarfare.Project.Content;

namespace FluidWarfare.Bridge.ProjectEngine.World;

/// <summary>
/// 把项目内容文件入口转换为 Engine World 的占位实体。
/// 不读取文件内容，不解析 JSON，不写日志，不依赖 Editor。
/// </summary>
public static class ProjectContentWorldSeeder
{
    private const string TargetContentKind = "unitTemplate";

    /// <summary>
    /// 根据 unitTemplate 内容文件入口在 WorldState 中创建占位实体。
    /// </summary>
    /// <param name="worldState">目标 World 状态。</param>
    /// <param name="contentFiles">项目内容文件入口列表。</param>
    /// <returns>创建结果，包含创建数量和来源路径。</returns>
    public static ProjectContentWorldSeedResult SeedUnitTemplatePlaceholders(
        WorldState worldState,
        IReadOnlyList<GameContentFileInfo> contentFiles)
    {
        if (worldState is null)
        {
            throw new ArgumentNullException(nameof(worldState));
        }

        if (contentFiles is null || contentFiles.Count == 0)
        {
            return new ProjectContentWorldSeedResult(0, []);
        }

        var targetFiles = contentFiles
            .Where(f => f.ContentKind == TargetContentKind)
            .OrderBy(f => f.RelativePath, StringComparer.Ordinal)
            .ToList();

        var sourcePaths = new List<string>();
        var index = 0;

        foreach (var file in targetFiles)
        {
            var displayName = Path.GetFileNameWithoutExtension(file.FileName);
            var position = new Vector3d(index * 10.0, 0.0, 0.0);
            var source = new ProjectContentEntitySource(file.RelativePath, file.ContentKind);

            worldState.CreateEntity(displayName, position, source);

            sourcePaths.Add(file.RelativePath);
            index++;
        }

        return new ProjectContentWorldSeedResult(sourcePaths.Count, sourcePaths);
    }
}
