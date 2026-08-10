using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void DrawVectorOverlay(CommandBuffer cb, float* scene, int index)
    {
        var model = _renderProjection.VectorOverlayResources.ElementAtOrDefault(index);
        if (model is null) return;
        var gpu = _vectorOverlays.Get(model);
        if (gpu is null) return;
        var vertex = gpu.VertexBuffer.Buffer; ulong offset = 0;
        _vk.CmdBindVertexBuffers(cb, 0, 1, &vertex, &offset);
        _vk.CmdBindIndexBuffer(cb, gpu.IndexBuffer.Buffer, 0, IndexType.Uint32);
        foreach (var primitive in gpu.Primitives)
        {
            var size = primitive.Kind == RenderVectorOverlayPrimitiveKind.Marker
                ? primitive.RadiusDip : primitive.WidthDip;
            FillScenePushConstants(scene, _renderProjection, default, default, new(1, 1, 1),
                (float)(size * _renderProjection.ViewportDpiScale), (float)primitive.Kind, -20.0f);
            scene[19] = (float)primitive.Color.R;
            scene[23] = (float)primitive.Color.A;
            scene[24] = (float)primitive.Color.R;
            scene[25] = (float)primitive.Color.G;
            scene[26] = (float)primitive.Color.B;
            PushSceneConstants(cb, scene);
            _vk.CmdDrawIndexed(cb, (uint)primitive.IndexCount, 1,
                (uint)primitive.FirstIndex, primitive.BaseVertex, 0);
        }
        BindProceduralVertexBuffer(cb);
    }
}
