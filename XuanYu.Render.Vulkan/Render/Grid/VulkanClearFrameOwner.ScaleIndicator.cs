using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    const uint ScaleIndicatorPushFloatCount = 20;
    public const uint ScaleIndicatorPushSize = ScaleIndicatorPushFloatCount * 4;
    Silk.NET.Vulkan.Pipeline _scaleIndicatorPipeline;
    PipelineLayout _scaleIndicatorPipelineLayout;

    public void SetScaleIndicatorPipeline(
        Silk.NET.Vulkan.Pipeline pipeline, PipelineLayout layout)
    {
        _scaleIndicatorPipeline = pipeline;
        _scaleIndicatorPipelineLayout = layout;
        if (_views.Length > 0 && !RecordCommandBuffers(_views))
            throw new InvalidOperationException("比例尺 Overlay 管线注入后 CommandBuffer 重录失败");
    }

    void DrawScaleIndicator(CommandBuffer cb)
    {
        if (_scaleIndicatorPipeline.Handle == 0 ||
            _scaleIndicatorPipelineLayout.Handle == 0) return;
        var projection = _renderProjection.ScaleIndicator;
        if (!projection.Visible) return;
        var dpi = Math.Max(0.5, _renderProjection.ViewportDpiScale);
        var viewportWidthDip = _extent.Width / dpi;
        var viewportHeightDip = _extent.Height / dpi;
        var label = projection.Label ?? "";
        Span<int> glyphs = stackalloc int[ScaleIndicatorGlyphLite.MaxGlyphs];
        var labelLength = ScaleIndicatorGlyphLite.EncodeLabel(label, glyphs);
        var rect = ViewportOverlayLayoutResolver.Resolve(new(
            viewportWidthDip, viewportHeightDip,
            ScaleIndicatorOverlayProjection.CardWidthDip,
            ScaleIndicatorOverlayProjection.CardHeightDip,
            16.0, 16.0, ViewportOverlayAnchor.BottomLeft));
        var scene = new float[ScaleIndicatorPushFloatCount];
        scene[0] = _extent.Width; scene[1] = _extent.Height;
        scene[2] = (float)dpi; scene[3] = 1.0f;
        scene[4] = (float)rect.X; scene[5] = (float)rect.Y;
        scene[6] = (float)rect.Width; scene[7] = (float)rect.Height;
        scene[8] = (float)ScaleIndicatorMetric.FixedBarWidthDip; scene[9] = labelLength;
        for (var i = 0; i < glyphs.Length; i++) scene[12 + i] = glyphs[i];
        fixed (float* constants = scene)
        {
            _vk.CmdPushConstants(cb, _scaleIndicatorPipelineLayout,
                ShaderStageFlags.VertexBit | ShaderStageFlags.FragmentBit,
                0, ScaleIndicatorPushSize, constants);
            _vk.CmdDraw(cb, RenderDrawPlan.FullscreenTriangleVertexCount, 1, 0, 0);
        }
    }
}
