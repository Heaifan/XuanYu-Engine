using Silk.NET.Vulkan;
using XuanYu.Core.Map;
using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.Render.Vulkan.Render;

// MAP-A-R2-D3：有限 Flat 地面（4 顶点 6 索引）+ 四条边界（24 顶点细条）；资源判等用 ResourceKey（Rename 不重建）。
public sealed unsafe partial class VulkanClearFrameOwner
{
    VulkanStaticModelBuffer? _mapSurfaceVertexBuffer, _mapSurfaceIndexBuffer, _mapBoundsVertexBuffer;
    uint _mapSurfaceIndexCount, _mapBoundsVertexCount;
    long _lastConsumedMapSequence = long.MinValue;
    MapSurfaceResourceKey _mapSurfaceResourceKey;
    bool _hasMapSurfaceResourceKey;

    public void SetMapSurface(MapRenderSnapshot map)
    {
        var update = MapSurfaceResourceUpdatePolicy.Decide(
            map, _lastConsumedMapSequence, _hasMapSurfaceResourceKey ? _mapSurfaceResourceKey : null);
        if (update.Kind == MapSurfaceResourceUpdateKind.RejectStale)
        {
            Log($"地图资源更新决策：处理={MapSurfaceResourceUpdateText.Of(update.Kind)}；接收序号={map.SourceChangeSequence}；已消费序号={_lastConsumedMapSequence}");
            return;
        }
        if (update.Kind == MapSurfaceResourceUpdateKind.NoRebuild)
        {
            Log($"地图资源更新决策：处理={MapSurfaceResourceUpdateText.Of(update.Kind)}；序号={map.SourceChangeSequence}；资源键已变化=否");
            _lastConsumedMapSequence = map.SourceChangeSequence;
            return;
        }
        Log($"地图资源更新决策：处理={MapSurfaceResourceUpdateText.Of(update.Kind)}；序号={map.SourceChangeSequence}；资源键已变化=是；尺寸={map.WidthMeters:0.####}×{map.DepthMeters:0.####}；基础高度={map.BaseHeightMeters}");
        ClearMapSurface();
        _mapSurfaceResourceKey = update.Key; _hasMapSurfaceResourceKey = true;
        _lastConsumedMapSequence = map.SourceChangeSequence;
        if (!map.HasMap) return;
        if (!CreateMapBuffers(MapSurfaceGeometryBuilder.Build(map), MapBoundsGeometryBuilder.Build(map), map))
            ClearMapSurface();
    }
    bool CreateMapBuffers(MapSurfaceGeometry geometry, MapTerrainVertex[] bounds, MapRenderSnapshot map)
    {
        _mapSurfaceVertexBuffer = VulkanStaticModelBuffer.Create(_vk, _deviceOwner,
            geometry.Vertices, BufferUsageFlags.VertexBufferBit, out var vbErr);
        if (_mapSurfaceVertexBuffer is null) return CreateFailed("地图地面顶点缓冲创建失败", vbErr);
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
    bool CreateFailed(string what, string error)
    {
        Log($"{what}失败：{error}");
        return false;
    }
    public void ClearMapSurface()
    {
        _mapSurfaceVertexBuffer?.Dispose(); _mapSurfaceVertexBuffer = null;
        _mapSurfaceIndexBuffer?.Dispose(); _mapSurfaceIndexBuffer = null;
        _mapBoundsVertexBuffer?.Dispose(); _mapBoundsVertexBuffer = null;
        _mapSurfaceIndexCount = 0;
        _mapBoundsVertexCount = 0;
        _hasMapSurfaceResourceKey = false;
    }
    void DrawMapSurface(CommandBuffer cb, float* scene)
    {
        if (_mapSurfaceVertexBuffer is null || _mapSurfaceIndexBuffer is null) return;
        var vb = _mapSurfaceVertexBuffer.Buffer;
        var ib = _mapSurfaceIndexBuffer.Buffer;
        ulong offset = 0;
        _vk.CmdBindVertexBuffers(cb, 0, 1, &vb, &offset);
        _vk.CmdBindIndexBuffer(cb, ib, 0, IndexType.Uint32);
        FillScenePushConstants(scene, _renderProjection, Vector3d.Zero,
            Vector3d.Zero, new Vector3d(1, 1, 1), 0.0f, gizmoModeOverride: -14.0f);
        PushSceneConstants(cb, scene);
        _vk.CmdDrawIndexed(cb, _mapSurfaceIndexCount, 1, 0, 0, 0);
        BindProceduralVertexBuffer(cb);
    }
    void DrawMapBounds(CommandBuffer cb, float* scene)
    {
        if (_mapBoundsVertexBuffer is null) return;
        var vb = _mapBoundsVertexBuffer.Buffer;
        ulong offset = 0;
        _vk.CmdBindVertexBuffers(cb, 0, 1, &vb, &offset);
        FillScenePushConstants(scene, _renderProjection, Vector3d.Zero,
            Vector3d.Zero, new Vector3d(1, 1, 1), 0.0f, gizmoModeOverride: -15.0f);
        PushSceneConstants(cb, scene);
        _vk.CmdDraw(cb, _mapBoundsVertexCount, 1, 0, 0);
        BindProceduralVertexBuffer(cb);
    }
}
