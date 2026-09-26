using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Render.VectorOverlay;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void DrawMapLabels(CommandBuffer cb, float* scene, VulkanVectorOverlayResource overlay)
    {
        if (_mapLabelPipeline.Handle == 0 || _mapLabelTextures is null || overlay.Labels.Count == 0) return;
        _mapLabelTextures.RetainOnly(overlay.LabelBitmaps.Select(x => x.CacheKey));
        _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _mapLabelPipeline);
        foreach (var label in overlay.Labels)
        {
            var bitmap = overlay.LabelBitmaps.FirstOrDefault(x => x.CacheKey == label.CacheKey);
            if (bitmap is null) continue;
            var texture = _mapLabelTextures.GetOrCreate(bitmap);
            if (texture is null) continue;
            var descriptor = texture.DescriptorSet;
            _vk.CmdBindDescriptorSets(cb, PipelineBindPoint.Graphics, _mapLabelPipelineLayout,
                0, 1, &descriptor, 0, null);
            FillScenePushConstants(scene, _renderProjection, label.Anchor, default,
                new(bitmap.Width, bitmap.Height, 1), 0, gizmoModeOverride: -30);
            scene[23] = 1;
            scene[24] = (float)label.Color.R; scene[25] = (float)label.Color.G;
            scene[26] = (float)label.Color.B;
            PushLabelConstants(cb, scene);
            _vk.CmdDraw(cb, 6, 1, 0, 0);
        }
    }
}
