using Silk.NET.Vulkan;
using XuanYu.Core.Diagnostics;
using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

internal unsafe delegate void MapSurfacePushConstants(
    CommandBuffer commandBuffer, float* scene, float gizmoModeOverride);

internal sealed unsafe partial class VulkanMapSurfaceOwner
{
    internal void DrawMapSurface(CommandBuffer cb, float* scene, int frameSlot,
        MapSurfacePushConstants pushScene, Action<CommandBuffer> bindProcedural)
    {
        var vertexBuffer = _mapSurfaceFrameVertexBuffers.ElementAtOrDefault(frameSlot);
        if (vertexBuffer is null || _mapSurfaceIndexBuffer is null || !_mapSurfacePatchReady)
        {
            GroundProbeChain.DrawCommand(ViewportProbe.CurrentFrameId, false, true,
                vertexBuffer is not null, _mapSurfaceIndexBuffer is not null, 0, 0,
                "NONE", "NONE", "BUFFER_NOT_READY");
            return;
        }
        _trackFrameResource(vertexBuffer);
        _trackFrameResource(_mapSurfaceIndexBuffer);
        var vb = vertexBuffer.Buffer;
        var ib = _mapSurfaceIndexBuffer.Buffer;
        ulong offset = 0;
        _vk.CmdBindVertexBuffers(cb, 0, 1, &vb, &offset);
        _vk.CmdBindIndexBuffer(cb, ib, 0, IndexType.Uint32);
        pushScene(cb, scene, -14.0f);
        _vk.CmdDrawIndexed(cb, _mapSurfaceIndexCount, 1, 0, 0, 0);
        GroundProbeChain.DrawCommand(ViewportProbe.CurrentFrameId, true, true, true, true, 4,
            (int)_mapSurfaceIndexCount, $"0x{vb.Handle:X}", $"0x{ib.Handle:X}", "NONE");
        bindProcedural(cb);
    }

    internal void DrawMapBounds(CommandBuffer cb, float* scene,
        MapSurfacePushConstants pushScene, Action<CommandBuffer> bindProcedural)
    {
        if (_mapBoundsVertexBuffer is null || _mapBoundsVertexCount == 0) return;
        _trackFrameResource(_mapBoundsVertexBuffer);
        var vb = _mapBoundsVertexBuffer.Buffer;
        ulong offset = 0;
        _vk.CmdBindVertexBuffers(cb, 0, 1, &vb, &offset);
        pushScene(cb, scene, -15.0f);
        _vk.CmdDraw(cb, _mapBoundsVertexCount, 1, 0, 0);
        bindProcedural(cb);
    }
}
