using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void DrawStaticModel(CommandBuffer cb, float* scene, RenderEntityProjection entity)
    {
        var model = _renderProjection.StaticModelResources
            .FirstOrDefault(x => x.Key == entity.StaticModelKey);
        if (model is null) return;
        var gpu = _staticModels.Get(model);
        if (gpu is null) return;
        BindStaticModelBuffers(cb, gpu);
        foreach (var p in gpu.Primitives)
        {
            FillScenePushConstants(scene, _renderProjection, entity.Position, entity.Rotation,
                entity.Scale, (float)p.BaseColor.G, (float)p.BaseColor.B, -3.0f);
            scene[19] = (float)p.BaseColor.R;
            scene[23] = (float)p.BaseColor.A;
            PushSceneConstants(cb, scene);
            _vk.CmdDrawIndexed(cb, (uint)p.IndexCount, 1, (uint)p.FirstIndex, p.BaseVertex, 0);
        }
        if (entity.IsSelected) DrawStaticModelBounds(cb, scene, entity, model);
    }

    void BindStaticModelBuffers(CommandBuffer cb, VulkanStaticModelResource gpu)
    {
        var buffer = gpu.VertexBuffer.Buffer;
        ulong offset = 0;
        _vk.CmdBindVertexBuffers(cb, 0, 1, &buffer, &offset);
        _vk.CmdBindIndexBuffer(cb, gpu.IndexBuffer.Buffer, 0, IndexType.Uint32);
    }
}
