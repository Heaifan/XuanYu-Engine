using Silk.NET.Vulkan;
using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.Render.Vulkan.Render;

// MAP-A-R2-D3：有限 Flat 地面（4 顶点 6 索引常量几何）+ 四条边界（24 顶点细条）。
// 地图尺寸只进入顶点坐标，不随米数增加顶点；map resize 只重建本资源。
public sealed unsafe partial class VulkanClearFrameOwner
{
    VulkanStaticModelBuffer? _mapSurfaceVertexBuffer;
    VulkanStaticModelBuffer? _mapSurfaceIndexBuffer;
    VulkanStaticModelBuffer? _mapBoundsVertexBuffer;
    uint _mapSurfaceIndexCount;
    uint _mapBoundsVertexCount;
    MapRenderSnapshot _mapSnapshot;

    public void SetMapSurface(MapRenderSnapshot map)
    {
        if (map.Equals(_mapSnapshot)) return;
        ClearMapSurface();
        if (!map.HasMap) return;
        var geometry = MapSurfaceGeometryBuilder.Build(map);
        var bounds = MapBoundsGeometryBuilder.Build(map);
        _mapSurfaceVertexBuffer = VulkanStaticModelBuffer.Create(_vk, _deviceOwner,
            geometry.Vertices, BufferUsageFlags.VertexBufferBit, out var vbErr);
        if (_mapSurfaceVertexBuffer is null)
        {
            Log($"地图地面顶点缓冲创建失败：{vbErr}");
            ClearMapSurface();
            return;
        }

        _mapSurfaceIndexBuffer = VulkanStaticModelBuffer.Create(_vk, _deviceOwner,
            geometry.Indices, BufferUsageFlags.IndexBufferBit, out var ibErr);
        if (_mapSurfaceIndexBuffer is null)
        {
            Log($"地图地面索引缓冲创建失败：{ibErr}");
            ClearMapSurface();
            return;
        }

        _mapBoundsVertexBuffer = VulkanStaticModelBuffer.Create(_vk, _deviceOwner,
            bounds, BufferUsageFlags.VertexBufferBit, out var bErr);
        if (_mapBoundsVertexBuffer is null)
        {
            Log($"地图边界线缓冲创建失败：{bErr}");
            ClearMapSurface();
            return;
        }

        _mapSurfaceIndexCount = (uint)geometry.Indices.Length;
        _mapBoundsVertexCount = (uint)bounds.Length;
        _mapSnapshot = map;
        Log($"地图渲染资源已创建：{map.WidthMeters:0}×{map.DepthMeters:0} 米（{_mapSurfaceIndexCount} 索引）");
    }

    public void ClearMapSurface()
    {
        _mapSurfaceVertexBuffer?.Dispose();
        _mapSurfaceVertexBuffer = null;
        _mapSurfaceIndexBuffer?.Dispose();
        _mapSurfaceIndexBuffer = null;
        _mapBoundsVertexBuffer?.Dispose();
        _mapBoundsVertexBuffer = null;
        _mapSurfaceIndexCount = 0;
        _mapBoundsVertexCount = 0;
        _mapSnapshot = default;
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
