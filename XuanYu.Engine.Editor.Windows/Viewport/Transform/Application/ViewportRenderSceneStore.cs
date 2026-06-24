using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using FluidWarfare.Render.Scene;
using FluidWarfare.Render.Scene.Position;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Application;

/// <summary>RenderScene 当前位置的唯一写入所有者。读取仍可通过 Current 属性。</summary>
public sealed class ViewportRenderSceneStore
{
    public RenderScene Current { get; private set; } = RenderScene.Empty;

    public void Initialize(RenderScene scene) => Current = scene;

    /// <summary>更新实体位置。返回 true 表示成功。</summary>
    public bool UpdatePosition(EntityId entityId, Vector3d position)
    {
        var result = RenderSceneObjectPositionWriter.Update(Current, entityId, position);
        if (!result.IsSuccess || result.NewScene is null) return false;
        Current = result.NewScene;
        return true;
    }
}
