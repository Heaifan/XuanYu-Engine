using Silk.NET.Vulkan;
using XuanYu.Core.Diagnostics;
using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void DrawMapSurface(CommandBuffer cb, float* scene)
    {
        var frameSlot = _activeFrameContext?.FrameIndex ?? 0;
        var vertexBuffer = _mapSurfaceFrameVertexBuffers.ElementAtOrDefault(frameSlot);
        if (vertexBuffer is null || _mapSurfaceIndexBuffer is null || !_mapSurfacePatchReady)
        {
            GroundProbeChain.DrawCommand(ViewportProbe.CurrentFrameId, false, true,
                vertexBuffer is not null, _mapSurfaceIndexBuffer is not null, 0, 0,
                "NONE", "NONE", "BUFFER_NOT_READY");
            return;
        }
        TrackFrameResource(vertexBuffer);
        TrackFrameResource(_mapSurfaceIndexBuffer);
        var vb = vertexBuffer.Buffer;
        var ib = _mapSurfaceIndexBuffer.Buffer;
        ulong offset = 0;
        _vk.CmdBindVertexBuffers(cb, 0, 1, &vb, &offset);
        _vk.CmdBindIndexBuffer(cb, ib, 0, IndexType.Uint32);
        FillScenePushConstants(scene, _renderProjection, Vector3d.Zero, Vector3d.Zero,
            new Vector3d(1, 1, 1), 0.0f, gizmoModeOverride: -14.0f);
        PushSceneConstants(cb, scene);
        _vk.CmdDrawIndexed(cb, _mapSurfaceIndexCount, 1, 0, 0, 0);
        GroundProbeChain.DrawCommand(ViewportProbe.CurrentFrameId, true, true, true, true, 4,
            (int)_mapSurfaceIndexCount, $"0x{vb.Handle:X}", $"0x{ib.Handle:X}", "NONE");
        BindProceduralVertexBuffer(cb);
    }

    void DrawMapBounds(CommandBuffer cb, float* scene)
    {
        if (_mapBoundsVertexBuffer is null || _mapBoundsVertexCount == 0) return;
        TrackFrameResource(_mapBoundsVertexBuffer);
        var vb = _mapBoundsVertexBuffer.Buffer;
        ulong offset = 0;
        _vk.CmdBindVertexBuffers(cb, 0, 1, &vb, &offset);
        FillScenePushConstants(scene, _renderProjection, Vector3d.Zero, Vector3d.Zero,
            new Vector3d(1, 1, 1), 0.0f, gizmoModeOverride: -15.0f);
        PushSceneConstants(cb, scene);
        _vk.CmdDraw(cb, _mapBoundsVertexCount, 1, 0, 0);
        BindProceduralVertexBuffer(cb);
    }
}
