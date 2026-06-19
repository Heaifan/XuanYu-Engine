using FluidWarfare.Core.Identity;
using FluidWarfare.Project.World.Transform;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Application;

/// <summary>
/// Cancel：恢复实体到初始 Transform。还原 RenderScene + Vulkan + Inspector。
/// 如果 RenderScene 恢复失败返回 Failure，由调用层决定后续处理。
/// </summary>
public sealed class EntityTransformCancel
{
    readonly ViewportRenderSceneStore _renderScene;
    readonly Scene3dEntityPositionWriter _vulkan;
    readonly InspectorTransformDisplay _inspector;

    public EntityTransformCancel(
        ViewportRenderSceneStore renderScene,
        Scene3dEntityPositionWriter vulkan,
        InspectorTransformDisplay inspector)
    {
        _renderScene = renderScene; _vulkan = vulkan; _inspector = inspector;
    }

    public TransformApplyResult Apply(SceneTransform initialTransform, EntityId entityId)
    {
        var pos = initialTransform.Position;
        if (!_renderScene.UpdatePosition(entityId, pos))
            return TransformApplyResult.Failure(TransformFailureReason.RenderSceneSyncFailed);
        _vulkan.Write(entityId, pos);
        _inspector.SetPosition(pos);
        return TransformApplyResult.SuccessResult;
    }
}
