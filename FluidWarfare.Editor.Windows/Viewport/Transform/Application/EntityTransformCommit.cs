using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Math;
using FluidWarfare.Render.Vulkan.Scene3D.Session;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Application;

/// <summary>
/// Commit：原子式提交最终 Transform。写入 WorldState + RenderScene + Vulkan + Inspector + Dirty。
/// 只提交一次，不做多次增量更新。
/// </summary>
public sealed class EntityTransformCommit
{
    readonly WorldTransformWriter _world;
    readonly ViewportRenderSceneStore _renderScene;
    readonly Scene3dEntityPositionWriter _vulkan;
    readonly InspectorTransformDisplay _inspector;
    readonly Scene3dFrameRequest _frame;

    public EntityTransformCommit(
        WorldTransformWriter world,
        ViewportRenderSceneStore renderScene,
        Scene3dEntityPositionWriter vulkan,
        InspectorTransformDisplay inspector,
        Scene3dFrameRequest frame)
    {
        _world = world; _renderScene = renderScene;
        _vulkan = vulkan; _inspector = inspector; _frame = frame;
    }

    public TransformApplyResult Apply(Vector3d position, EntityId entityId)
    {
        if (!_world.TrySetPosition(entityId, position))
            return TransformApplyResult.SuccessResult; // NoOp: 位置未变化

        _vulkan.Write(entityId, position);
        _renderScene.UpdatePosition(entityId, position);
        _inspector.SetPosition(position);
        _frame.Request(VulkanScene3dFrameReason.EntityTransformChanged);
        return TransformApplyResult.SuccessResult;
    }
}
