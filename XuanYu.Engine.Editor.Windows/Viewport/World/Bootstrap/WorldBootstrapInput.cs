using XuanYu.Engine.Project.Content;
using XuanYu.Engine.Project.Metadata;

namespace FluidWarfare.Editor.Windows.Viewport.World.Bootstrap;

/// <summary>World 引导输入。</summary>
public readonly record struct WorldBootstrapInput(
    GameProjectInfo Project,
    IReadOnlyList<GameContentFileInfo> ContentFiles);
