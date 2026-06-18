using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Math;
using FluidWarfare.Render.Vulkan.Scene3D.Session;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Application;

/// <summary>
/// Preview：将预览位置同步到 Vulkan + RenderScene + Inspector，请求一帧。
/// 不修改 WorldState / Dirty / 日志。
/// </summary>
public sealed class EntityTransformPreview
{
    readonly ViewportRenderSceneStore _renderScene;
    readonly Scene3dEntityPositionWriter _vulkan;
    readonly InspectorTransformDisplay _inspector;
    readonly Scene3dFrameRequest _frame;

    public EntityTransformPreview(
        ViewportRenderSceneStore renderScene,
        Scene3dEntityPositionWriter vulkan,
        InspectorTransformDisplay inspector,
        Scene3dFrameRequest frame)
    {
        _renderScene = renderScene; _vulkan = vulkan;
        _inspector = inspector; _frame = frame;
    }

    public TransformApplyResult Apply(Vector3d position, EntityId entityId)
    {
        _vulkan.Write(entityId, position);
        if (!_renderScene.UpdatePosition(entityId, position))
            return TransformApplyResult.Failure("RenderScene 同步失败");
        _inspector.SetPosition(position);
        _frame.Request(VulkanScene3dFrameReason.TransformPreview);
        return TransformApplyResult.SuccessResult;
    }
}
