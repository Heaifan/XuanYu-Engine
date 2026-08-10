using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void DrawRegionModel(CommandBuffer cb, float* scene, int index)
    {
        var model = _renderProjection.RegionModelResources.ElementAtOrDefault(index);
        if (model is null) return;
        var gpu = _staticModels.Get(model);
        if (gpu is null) return;
        BindStaticModelBuffers(cb, gpu);
        var transform = RenderStaticModelTransform.Identity;
        foreach (var primitive in gpu.Primitives)
        {
            FillScenePushConstants(scene, _renderProjection, transform.Position,
                transform.Rotation, transform.Scale,
                (float)primitive.BaseColor.G, (float)primitive.BaseColor.B, -3.0f);
            scene[19] = (float)primitive.BaseColor.R;
            scene[23] = (float)primitive.BaseColor.A;
            PushSceneConstants(cb, scene);
            _vk.CmdDrawIndexed(cb, (uint)primitive.IndexCount, 1,
                (uint)primitive.FirstIndex, primitive.BaseVertex, 0);
        }
    }
}
