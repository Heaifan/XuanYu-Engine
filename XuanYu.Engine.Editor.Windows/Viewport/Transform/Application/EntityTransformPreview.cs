using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Project.World.Transform;

namespace XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;

/// <summary>
/// Preview：验证 RenderScene 可更新 → 写 Vulkan → 更新 Inspector。
/// 不修改 WorldState/Dirty。不安排帧（由 Shell 在调用后统一调度）。
/// </summary>
public sealed class EntityTransformPreview
{
    readonly ViewportRenderSceneStore _renderScene;
    readonly Scene3dEntityPositionWriter _vulkan;
    readonly InspectorTransformDisplay _inspector;

    public EntityTransformPreview(
        ViewportRenderSceneStore renderScene,
        Scene3dEntityPositionWriter vulkan,
        InspectorTransformDisplay inspector)
    {
        _renderScene = renderScene; _vulkan = vulkan; _inspector = inspector;
    }

    public TransformApplyResult Apply(SceneTransform transform, EntityId entityId)
    {
        var pos = transform.Position;
        // 先验证 RenderScene 可更新，再写 Vulkan
        if (!_renderScene.UpdatePosition(entityId, pos))
            return TransformApplyResult.Failure(TransformFailureReason.RenderSceneSyncFailed);
        _vulkan.Write(entityId, pos);
        _inspector.SetPosition(pos);
        return TransformApplyResult.SuccessResult;
    }
}
