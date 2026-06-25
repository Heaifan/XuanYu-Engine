using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Editor.Windows.Shell.Diagnostics;
using XuanYu.Engine.Project.World.Transform;

namespace XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;

/// <summary>
/// Preview：验证 RenderScene 可更新 → 写 Vulkan。
/// 不更新 Inspector（拖动高频路径瘦身）。不修改 WorldState/Dirty。
/// 不安排帧（由 Shell 在调用后统一调度）。
/// </summary>
public sealed class EntityTransformPreview
{
    readonly ViewportRenderSceneStore _renderScene;
    readonly Scene3dEntityPositionWriter _vulkan;

    public EntityTransformPreview(
        ViewportRenderSceneStore renderScene,
        Scene3dEntityPositionWriter vulkan)
    {
        _renderScene = renderScene; _vulkan = vulkan;
    }

    public TransformApplyResult Apply(SceneTransform transform, EntityId entityId)
    {
        var pos = transform.Position;
        GizmoDragProbe.Log("RenderScene preview 写入");
        // 先验证 RenderScene 可更新，再写 Vulkan
        if (!_renderScene.UpdatePosition(entityId, pos))
            return TransformApplyResult.Failure(TransformFailureReason.RenderSceneSyncFailed);
        _vulkan.Write(entityId, pos);
        GizmoDragProbe.Log("RenderScene preview 写入完成");
        // Preview 不刷新 Inspector（拖动高频路径瘦身）
        // Inspector 在 Confirm / Commit 时由 EntityTransformCommit.Apply 更新
        return TransformApplyResult.SuccessResult;
    }
}
