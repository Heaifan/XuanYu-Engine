using FluidWarfare.Project.Content;
using FluidWarfare.Project.Metadata;

namespace FluidWarfare.Editor.Windows.Viewport.World.Bootstrap;

/// <summary>World 引导输入。</summary>
public readonly record struct WorldBootstrapInput(
    GameProjectInfo Project,
    IReadOnlyList<GameContentFileInfo> ContentFiles);
