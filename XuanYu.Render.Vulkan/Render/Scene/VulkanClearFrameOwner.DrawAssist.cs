using Silk.NET.Vulkan;
using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Pipeline;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void DrawAssist(CommandBuffer cb, float* scene, RenderDrawPlan.FrameEntry draw)
    {
        // D4：地图地面/边界已改由 Draw.cs 按 MapGround/MapBounds 分项分发；
        // 此处只处理 EditorBackground（天空）。
        var mode = -10.0f;
        FillScenePushConstants(scene, _renderProjection, Vector3d.Zero,
            Vector3d.Zero, new Vector3d(1, 1, 1), 0.0f, gizmoModeOverride: mode);
        PushSceneConstants(cb, scene);
        _vk.CmdDraw(cb, (uint)draw.VertexCount, 1, 0, 0);
    }
}
