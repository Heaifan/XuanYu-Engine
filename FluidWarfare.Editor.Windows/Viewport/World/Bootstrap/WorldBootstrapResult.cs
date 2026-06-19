using FluidWarfare.Core.Identity;
using FluidWarfare.Engine.World;
using FluidWarfare.Render.Scene;

namespace FluidWarfare.Editor.Windows.Viewport.World.Bootstrap;

/// <summary>World 引导结果。Shell 用此结果更新 Store、Selection、UI。</summary>
public sealed record WorldBootstrapResult(
    WorldState World,
    EntityId FirstEntityId,
    RenderScene RenderScene,
    IReadOnlyList<string> SeedSourcePaths,
    int CreatedEntityCount)
{
    public bool HasEntities => CreatedEntityCount > 0;
}
