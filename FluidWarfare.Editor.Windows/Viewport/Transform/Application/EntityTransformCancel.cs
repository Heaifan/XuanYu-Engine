using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Math;
using FluidWarfare.Render.Vulkan.Scene3D.Session;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Application;

/// <summary>
/// Cancel：恢复实体到初始 Transform。还原 Vulkan + RenderScene + Inspector，请求一帧。
/// InitialTransform 由 TransformInteractionResult.Cancel 提供。
/// </summary>
public sealed class EntityTransformCancel
{
    readonly ViewportRenderSceneStore _renderScene;
    readonly Scene3dEntityPositionWriter _vulkan;
    readonly InspectorTransformDisplay _inspector;
    readonly Scene3dFrameRequest _frame;

    public EntityTransformCancel(
        ViewportRenderSceneStore renderScene,
        Scene3dEntityPositionWriter vulkan,
        InspectorTransformDisplay inspector,
        Scene3dFrameRequest frame)
    {
        _renderScene = renderScene; _vulkan = vulkan;
        _inspector = inspector; _frame = frame;
    }

    public TransformApplyResult Apply(Vector3d initialPosition, EntityId entityId)
    {
        _vulkan.Write(entityId, initialPosition);
        _renderScene.UpdatePosition(entityId, initialPosition);
        _inspector.SetPosition(initialPosition);
        _frame.Request(VulkanScene3dFrameReason.TransformPreview);
        return TransformApplyResult.SuccessResult;
    }
}
