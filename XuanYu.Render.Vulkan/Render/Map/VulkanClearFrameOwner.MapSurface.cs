using Silk.NET.Vulkan;
using XuanYu.Core.Map;
using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.StaticModels;
using XuanYu.Core.Diagnostics;
namespace XuanYu.Render.Vulkan.Render;
public sealed unsafe partial class VulkanClearFrameOwner
{
    VulkanStaticModelBuffer?[] _mapSurfaceFrameVertexBuffers = [];
    VulkanStaticModelBuffer? _mapSurfaceIndexBuffer, _mapBoundsVertexBuffer;
    uint _mapSurfaceIndexCount, _mapBoundsVertexCount;
    MapRenderSnapshot _mapSurfaceMap;
    long _lastConsumedMapGeneration = long.MinValue, _lastConsumedMapSequence = long.MinValue;
    MapSurfaceResourceKey _mapSurfaceResourceKey;
    bool _hasMapSurfaceResourceKey;
    public void SetMapSurface(MapRenderSnapshot map)
    {
        GroundProbeChain.Snapshot(ViewportProbe.CurrentFrameId, map, _renderProjection.TerrainResources.Count != 0);
        var update = MapSurfaceResourceUpdatePolicy.Decide(
            map, _lastConsumedMapGeneration, _lastConsumedMapSequence,
            _hasMapSurfaceResourceKey ? _mapSurfaceResourceKey : null);
        if (update.Kind == MapSurfaceResourceUpdateKind.RejectStale)
        {
            Log($"地图资源更新决策：处理={MapSurfaceResourceUpdateText.Of(update.Kind)}；接收代次={map.SessionGeneration}；已消费代次={_lastConsumedMapGeneration}；接收序号={map.SourceChangeSequence}；已消费序号={_lastConsumedMapSequence}");
            return;
        }
        if (update.Kind == MapSurfaceResourceUpdateKind.NoRebuild)
        {
            _mapSurfaceMap = map; _lastConsumedMapGeneration = map.SessionGeneration;
            _lastConsumedMapSequence = map.SourceChangeSequence;
            return;
        }
        Log($"地图资源更新决策：处理={MapSurfaceResourceUpdateText.Of(update.Kind)}；序号={map.SourceChangeSequence}；资源键已变化=是；尺寸={map.WidthMeters:0.####}×{map.DepthMeters:0.####}；基础高度={map.BaseHeightMeters}");
        _mapSurfaceMap = map;
        ClearMapSurface();
        _mapSurfaceResourceKey = update.Key; _hasMapSurfaceResourceKey = true;
        _lastConsumedMapGeneration = map.SessionGeneration; _lastConsumedMapSequence = map.SourceChangeSequence;
        if (!map.HasMap) return;
        if (!CreateMapBuffers(MapSurfaceGeometryBuilder.Build(map), MapBoundsGeometryBuilder.Build(map), map))
            ClearMapSurface();
    }
    bool CreateMapBuffers(MapSurfaceGeometry geometry, MapTerrainVertex[] bounds, MapRenderSnapshot map)
    {
        _mapSurfaceFrameVertexBuffers = new VulkanStaticModelBuffer?[_frameSlotCount];
        for (var slot = 0; slot < _mapSurfaceFrameVertexBuffers.Length; slot++)
        {
            _mapSurfaceFrameVertexBuffers[slot] = VulkanStaticModelBuffer.Create(_vk, _deviceOwner,
                geometry.Vertices, BufferUsageFlags.VertexBufferBit, out var vbErr);
            if (_mapSurfaceFrameVertexBuffers[slot] is not null) continue;
            ClearMapSurface();
            return CreateFailed("地图地面顶点缓冲创建失败", vbErr);
        }
        _mapSurfaceIndexBuffer = VulkanStaticModelBuffer.Create(_vk, _deviceOwner,
            geometry.Indices, BufferUsageFlags.IndexBufferBit, out var ibErr);
        if (_mapSurfaceIndexBuffer is null) return CreateFailed("地图地面索引缓冲创建失败", ibErr);
        _mapBoundsVertexBuffer = VulkanStaticModelBuffer.Create(_vk, _deviceOwner,
            bounds, BufferUsageFlags.VertexBufferBit, out var bErr);
        if (_mapBoundsVertexBuffer is null) return CreateFailed("地图边界线缓冲创建失败", bErr);
        _mapSurfaceIndexCount = (uint)geometry.Indices.Length;
        _mapBoundsVertexCount = (uint)bounds.Length;
        Log($"地图渲染资源重建完成：地面顶点={geometry.Vertices.Length}；索引={_mapSurfaceIndexCount}；边界顶点={_mapBoundsVertexCount}；尺寸={map.WidthMeters:0.####}×{map.DepthMeters:0.####}；基础高度={map.BaseHeightMeters}；序号={map.SourceChangeSequence}");
        return true;
    }

    internal bool ReconfigureMapSurfaceFrames(int frameCount)
    {
        if (_mapSurfaceIndexBuffer is null) return true;
        foreach (var buffer in _mapSurfaceFrameVertexBuffers)
            if (buffer is not null) RetireGpuResource(buffer);
        _mapSurfaceFrameVertexBuffers = [];
        var geometry = MapSurfaceGeometryBuilder.Build(_mapSurfaceMap);
        var next = new VulkanStaticModelBuffer?[frameCount];
        for (var slot = 0; slot < next.Length; slot++)
        {
            next[slot] = VulkanStaticModelBuffer.Create(_vk, _deviceOwner, geometry.Vertices,
                BufferUsageFlags.VertexBufferBit, out var error);
            if (next[slot] is not null) continue;
            foreach (var buffer in next) if (buffer is not null) RetireGpuResource(buffer);
            _mapSurfaceFrameVertexBuffers = [];
            return CreateFailed("地图地面帧槽缓冲重建失败", error);
        }
        _mapSurfaceFrameVertexBuffers = next;
        _mapSurfacePatchReady = false;
        return true;
    }
}
