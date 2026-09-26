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
            var pipeline = primitive.Kind == RenderVectorOverlayPrimitiveKind.Stroke
                ? _vectorStrokePipeline : _vectorOverlayPipeline;
            if (pipeline.Handle == 0) continue;
            _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, pipeline);
            var size = primitive.Kind == RenderVectorOverlayPrimitiveKind.Marker
                ? primitive.RadiusDip : primitive.WidthDip;
            var strokeRadius = primitive.Kind == RenderVectorOverlayPrimitiveKind.Stroke
                ? size * 0.5 : size;
            FillScenePushConstants(scene, _renderProjection, default, default, new(1, 1, 1),
                (float)(strokeRadius * _renderProjection.ViewportDpiScale), (float)primitive.Kind, -20.0f);
            scene[19] = (float)primitive.Color.R;
            scene[23] = (float)primitive.Color.A;
            scene[24] = (float)primitive.Color.R;
            scene[25] = (float)primitive.Color.G;
            scene[26] = (float)primitive.Color.B;
            PushSceneConstants(cb, scene);
            _vk.CmdDrawIndexed(cb, (uint)primitive.IndexCount, 1,
                (uint)primitive.FirstIndex, primitive.BaseVertex, 0);
        }
        DrawMapLabels(cb, scene, gpu);
        BindProceduralVertexBuffer(cb);
    }
}
