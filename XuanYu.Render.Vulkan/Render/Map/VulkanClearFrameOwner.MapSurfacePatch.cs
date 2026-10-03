using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Core.Diagnostics;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    bool _mapSurfacePatchReady;

    void UpdateMapSurfacePatch()
    {
        _mapSurfacePatchReady = false;
        var state = CurrentViewProjectionState();
        if (_mapSurfaceVertexBuffer is null || !_mapSurfaceMap.HasMap)
        {
            GroundProbeChain.Patch(ViewportProbe.CurrentFrameId, state,
                _mapSurfaceMap, default, true, true,
                _mapSurfaceVertexBuffer is null ? "NO_VERTEX_BUFFER" : "NO_MAP", false,
                _mapSurfaceVertexBuffer is not null);
            return;
        }
        var patch = ReferencePlanePatchPlacement.From(
            _mapSurfaceMap, state, _mapSurfaceMap.BaseHeightMeters);
        var valid = ReferencePlaneFootprint.TryCreate(state, _mapSurfaceMap.BaseHeightMeters, out _);
        var geometry = MapSurfaceGeometryBuilder.BuildPatch(
            _mapSurfaceMap, patch, state.RenderOrigin);
        var rebuilt = _mapSurfaceVertexBuffer.TryUpdate(geometry.Vertices);
        _mapSurfacePatchReady = rebuilt;
        GroundProbeChain.Patch(ViewportProbe.CurrentFrameId, state, _mapSurfaceMap, patch,
            true, false, "NONE", valid && rebuilt, true);
        if (!rebuilt)
            Log("参考平面动态 patch 顶点缓冲更新失败");
    }
}
