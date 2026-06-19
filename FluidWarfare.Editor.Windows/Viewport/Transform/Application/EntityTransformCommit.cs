using FluidWarfare.Core.Identity;
using FluidWarfare.Project.World.Transform;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Application;

/// <summary>
/// Commit：原子提交。先同步 RenderScene + Vulkan，再写 WorldState + Dirty。
/// 如果 RenderScene 更新失败，不写入 WorldState（不残留半提交状态）。
/// </summary>
public sealed class EntityTransformCommit
{
    readonly WorldTransformWriter _world;
    readonly ViewportRenderSceneStore _renderScene;
    readonly Scene3dEntityPositionWriter _vulkan;
    readonly InspectorTransformDisplay _inspector;

    public EntityTransformCommit(
        WorldTransformWriter world,
        ViewportRenderSceneStore renderScene,
        Scene3dEntityPositionWriter vulkan,
        InspectorTransformDisplay inspector)
    {
        _world = world; _renderScene = renderScene;
        _vulkan = vulkan; _inspector = inspector;
    }

    public TransformApplyResult Apply(SceneTransform transform, EntityId entityId)
    {
        var pos = transform.Position;

        // Phase 1: 更新视觉层（可回滚 — 只有内存状态）
        if (!_renderScene.UpdatePosition(entityId, pos))
            return TransformApplyResult.Failure(TransformFailureReason.RenderSceneSyncFailed);
        _vulkan.Write(entityId, pos);

        // Phase 2: 写入 WorldState + Dirty（不可回滚 — 此处是提交点）
        if (!_world.TrySetPosition(entityId, pos))
        {
            // 位置未变化但视觉已更新 → 仍是有效状态
            _inspector.SetPosition(pos);
            return TransformApplyResult.NoChangeResult;
        }

        _inspector.SetPosition(pos);
        return TransformApplyResult.SuccessResult;
    }
}
