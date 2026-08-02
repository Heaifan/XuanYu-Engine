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
            RenderDrawKind.EditorGrid => -11.0f,
            RenderDrawKind.WorldOrigin => -12.0f,
            RenderDrawKind.WorldAxes => -13.0f,
            _ => -10.0f
        };
        // D5-R1：参考网格重心跟随相机，并按地图矩形裁切（entityScale.xy=地图半宽/半深）。
        if (draw.Kind == RenderDrawKind.EditorGrid)
        {
            var cam = _renderProjection.Camera.Position;
            var map = _renderProjection.Map;
            var halfW = map.HasMap ? map.WidthMeters / 2.0 : 0.0;
            var halfD = map.HasMap ? map.DepthMeters / 2.0 : 0.0;
            FillScenePushConstants(scene, _renderProjection, cam,
                Vector3d.Zero, new Vector3d(halfW, halfD, 1), 0.0f, gizmoModeOverride: mode);
            PushSceneConstants(cb, scene);
            _vk.CmdDraw(cb, (uint)draw.VertexCount, 1, 0, 0);
            return;
        }
        FillScenePushConstants(scene, _renderProjection, Vector3d.Zero,
            Vector3d.Zero, new Vector3d(1, 1, 1), 0.0f, gizmoModeOverride: mode);
        PushSceneConstants(cb, scene);
        _vk.CmdDraw(cb, (uint)draw.VertexCount, 1, 0, 0);
    }
}
