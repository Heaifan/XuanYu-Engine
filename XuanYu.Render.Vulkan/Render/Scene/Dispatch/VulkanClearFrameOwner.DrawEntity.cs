using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void BindProceduralVertexBuffer(CommandBuffer cb)
    {
        if (_proceduralVertexBuffer is null) return;
        var buffer = _proceduralVertexBuffer.Buffer;
        ulong offset = 0;
        _vk.CmdBindVertexBuffers(cb, 0, 1, &buffer, &offset);
    }

    void DrawEntity(CommandBuffer cb, float* scene, RenderDrawPlan.FrameEntry draw)
    {
        var entity = _renderProjection.Entities[draw.EntityIndex];
        if (entity.EntityType == RenderEntityType.StaticModel)
        {
            DrawStaticModel(cb, scene, entity);
            return;
        }
        var mode = draw.EntityType == RenderEntityType.Cube ? -1.0f : -2.0f;
        var selection = draw.Kind == RenderDrawKind.EntityOutline ? 2.0f : (entity.IsSelected ? 1.0f : 0.0f);
        FillScenePushConstants(scene, _renderProjection, entity.Position, entity.Rotation,
            entity.Scale, 0.0f, selection, mode);
        PushSceneConstants(cb, scene);
        _vk.CmdDraw(cb, (uint)draw.VertexCount, 1, 0, 0);
    }
}
