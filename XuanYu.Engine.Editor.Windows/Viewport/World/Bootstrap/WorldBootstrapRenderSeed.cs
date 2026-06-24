using XuanYu.Engine.World;
using XuanYu.Engine.Render.Scene;
using XuanYu.Engine.Render.World;

namespace XuanYu.Engine.Editor.Windows.Viewport.World.Bootstrap;

/// <summary>从 WorldState 生成 RenderScene。纯数据逻辑。</summary>
public static class WorldBootstrapRenderSeed
{
    public static RenderScene BuildRenderScene(WorldState world) =>
        WorldToRenderSceneBuilder.Build(world);
}
