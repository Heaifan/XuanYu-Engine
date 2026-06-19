using FluidWarfare.Core.Identity;
using FluidWarfare.Engine.World;
using FluidWarfare.Render.Scene;

namespace FluidWarfare.Editor.Windows.Viewport.World.Bootstrap;

/// <summary>
/// World 引导路由。创建 WorldState → 播种实体 → 生成 RenderScene → 返回结果。
/// 不持有 Shell/UI 控件引用。
/// </summary>
public sealed class WorldBootstrapRoute
{
    public WorldBootstrapResult Build(WorldBootstrapInput input)
    {
        // Phase 1: 创建 WorldState 并播种实体
        var (world, count, sourcePaths) = WorldBootstrapEntitySeed.Seed(input.ContentFiles);

        // Phase 2: 确定第一个实体 ID
        var entities = world.ListEntities();
        var firstId = entities.Count > 0 ? entities[0].EntityId : default;

        // Phase 3: 生成 RenderScene
        var renderScene = WorldBootstrapRenderSeed.BuildRenderScene(world);

        return new WorldBootstrapResult(world, firstId, renderScene, sourcePaths, count);
    }
}
