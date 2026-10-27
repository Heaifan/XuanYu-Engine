using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Pipeline;
using XuanYu.Core.Diagnostics;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    Silk.NET.Vulkan.Pipeline _skyPipeline;
    PipelineLayout _skyPipelineLayout;
    Silk.NET.Vulkan.Pipeline _gridPipeline;
    PipelineLayout _gridPipelineLayout;

    public void SetSkyPipeline(Silk.NET.Vulkan.Pipeline pipeline, PipelineLayout layout)
    {
        _skyPipeline = pipeline;
        _skyPipelineLayout = layout;
        if (_views.Length > 0 && !RecordCommandBuffers(_views))
            throw new InvalidOperationException("Pipeline 注入后 CommandBuffer 重录失败");
    }

    void RecordDraw(CommandBuffer cb)
    {
        if (ViewportProbe.CurrentFrameId == 0) ViewportProbe.BeginFrame();
        LogR7ViewportFrame();
        GroundProbeChain.FrameSeen(ViewportProbe.CurrentFrameId, _hasRenderProjection);
        var viewport = new[] { new Viewport { X = 0, Y = 0, Width = _extent.Width,
            Height = _extent.Height, MinDepth = 0, MaxDepth = 1 } };
        var scissor = new[] { new Rect2D { Offset = new Offset2D { X = 0, Y = 0 }, Extent = _extent } };
        var scene = new float[VulkanScenePushConstants.FloatCount];
        fixed (Viewport* pVp = viewport)
        fixed (Rect2D* pSc = scissor)
        fixed (float* pScene = scene)
        {
            _vk.CmdSetViewport(cb, 0, 1, pVp);
            _vk.CmdSetScissor(cb, 0, 1, pSc);
            BindProceduralVertexBuffer(cb);
            if (!_hasRenderProjection)
            {
                GroundProbeChain.FrameSummary(ViewportProbe.CurrentFrameId);
                return;
            }
            TerrainStats = default;
            _terrainCache?.RetainOnly(_renderProjection.TerrainResources);
            _staticModels.RetainOnly(_renderProjection.Entities.Select(e => e.StaticModelKey));
            _vectorOverlays.RetainOnly(_renderProjection.VectorOverlayResources.Select(r => r.Key));
            var drawPlan = RenderDrawPlan.GetFrameDrawPlan(_renderProjection);
            _mapLabelTextures?.RetainOnly(drawPlan
                .Where(draw => draw.Kind == RenderDrawKind.MapVectorOverlay)
                .SelectMany(draw => _renderProjection.VectorOverlayResources.ElementAtOrDefault(draw.EntityIndex)?.LabelBitmaps
                    ?? Array.Empty<RenderLabelBitmap>())
                .Select(bitmap => bitmap.CacheKey));
            foreach (var draw in drawPlan)
            {
                if (!CanRecordDraw(draw.Kind))
                {
                    if (draw.Kind == RenderDrawKind.MapGround)
                        GroundProbeChain.DrawCommand(ViewportProbe.CurrentFrameId, false,
                            false, false, false, 0, 0, "NONE", "NONE", "PIPELINE_NOT_READY");
                    continue;
                }
                BindFramePipeline(cb, draw.Kind);
                DispatchDraw(cb, pScene, draw);
            }
            GroundProbeChain.FrameSummary(ViewportProbe.CurrentFrameId);
            ViewportProbe.CameraSnapshot("T3_FRAME_END", CurrentViewProjectionState().Camera, default,
                CurrentViewProjectionState());
        }
    }
}
