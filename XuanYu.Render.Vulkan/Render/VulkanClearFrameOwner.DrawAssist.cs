using Silk.NET.Vulkan;
using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Pipeline;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void DrawAssist(CommandBuffer cb, float* scene, RenderDrawPlan.FrameEntry draw)
    {
        if (draw.Kind == RenderDrawKind.MapBounds)
        {
            DrawMapBounds(cb, scene);
            return;
        }

        var mode = draw.Kind switch
        {
            RenderDrawKind.EditorBackground => -10.0f,
            RenderDrawKind.WorldOrigin => -12.0f,
            RenderDrawKind.WorldAxes => -13.0f,
            _ => -10.0f
        };
        FillScenePushConstants(scene, _renderProjection, Vector3d.Zero,
            Vector3d.Zero, new Vector3d(1, 1, 1), 0.0f, gizmoModeOverride: mode);
        PushSceneConstants(cb, scene);
        _vk.CmdDraw(cb, (uint)draw.VertexCount, 1, 0, 0);
    }
}
