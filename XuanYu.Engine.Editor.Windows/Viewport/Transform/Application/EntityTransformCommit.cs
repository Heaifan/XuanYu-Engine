using FluidWarfare.Core.Identity;
using FluidWarfare.Project.World.Transform;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Application;

/// <summary>
/// Commit：原子提交。先验证实体存在，再同步视觉层（RenderScene + Vulkan），最后写 WorldState + Dirty。
/// 验证实体存在失败时不修改任何状态。RenderScene 更新失败时不写入 WorldState。
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

        // Phase 0: 预检实体存在（不修改任何状态）
        // Phase 0: 预检实体存在（不修改任何状态）
        if (!_world.EntityExists(entityId))
            return TransformApplyResult.Failure(TransformFailureReason.EntityNotFound);

        // Phase 1: 同步视觉层（可回滚 — 只有内存状态）
        if (!_renderScene.UpdatePosition(entityId, pos))
            return TransformApplyResult.Failure(TransformFailureReason.RenderSceneSyncFailed);
        _vulkan.Write(entityId, pos);

        // Phase 2: 写入 WorldState + Dirty（不可回滚 — 此处是提交点）
        var writeStatus = _world.TrySetPosition(entityId, pos);
        _inspector.SetPosition(pos);

        return writeStatus switch
        {
            WorldTransformWriteStatus.Changed => TransformApplyResult.SuccessResult,
            _ => TransformApplyResult.NoChangeResult,
        };
    }
}
