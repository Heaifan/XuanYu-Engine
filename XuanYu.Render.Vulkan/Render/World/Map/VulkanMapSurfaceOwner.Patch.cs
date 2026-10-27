using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Core.Diagnostics;
using XuanYu.Core.Space;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.Render.Vulkan.Render;

internal sealed unsafe partial class VulkanMapSurfaceOwner
{
    internal void UpdatePatch(ViewProjectionState state, int frameSlot)
    {
        _mapSurfacePatchReady = false;
        var vertexBuffer = _mapSurfaceFrameVertexBuffers.ElementAtOrDefault(frameSlot);
        if (vertexBuffer is null || !_mapSurfaceMap.HasMap)
        {
            GroundProbeChain.Patch(ViewportProbe.CurrentFrameId, state,
                _mapSurfaceMap, default, true, true,
                vertexBuffer is null ? "NO_VERTEX_BUFFER" : "NO_MAP", false,
                vertexBuffer is not null);
            return;
        }
        var patch = ReferencePlanePatchPlacement.From(
            _mapSurfaceMap, state, _mapSurfaceMap.BaseHeightMeters);
        var valid = ReferencePlaneFootprint.TryCreate(state, _mapSurfaceMap.BaseHeightMeters, out _);
        var geometry = MapSurfaceGeometryBuilder.BuildPatch(
            _mapSurfaceMap, patch, state.RenderOrigin);
        var rebuilt = vertexBuffer.TryUpdate(geometry.Vertices);
        _mapSurfacePatchReady = rebuilt;
        GroundProbeChain.Patch(ViewportProbe.CurrentFrameId, state, _mapSurfaceMap, patch,
            true, false, "NONE", valid && rebuilt, true);
        if (!rebuilt)
            _log?.Invoke("参考平面动态 patch 顶点缓冲更新失败");
    }
}
