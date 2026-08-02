using Silk.NET.Vulkan;
using XuanYu.Core.Map;
using XuanYu.Core.Math;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.Render.Vulkan.Render;

// MAP-A-R1-D4：有限地表与边界线渲染。
// CPU 网格（MapTerrainMeshBuilder，唯一采样源）→ 顶点/索引缓冲 → indexed draw。
public sealed unsafe partial class VulkanClearFrameOwner
{
    VulkanStaticModelBuffer? _mapTerrainVertexBuffer;
    VulkanStaticModelBuffer? _mapTerrainIndexBuffer;
    VulkanStaticModelBuffer? _mapBoundsVertexBuffer;
    uint _mapTerrainIndexCount;
    uint _mapBoundsVertexCount;
    MapRenderSnapshot _mapSnapshot;

    public void SetMapTerrain(MapRenderSnapshot map)
    {
        if (map.Equals(_mapSnapshot)) return;
        ClearMapTerrain();
        if (!map.HasMap) return;
        var mesh = MapTerrainMeshBuilder.Build(map);
        _mapTerrainVertexBuffer = VulkanStaticModelBuffer.Create(_vk, _deviceOwner,
            mesh.Vertices, BufferUsageFlags.VertexBufferBit, out var vbErr);
        if (_mapTerrainVertexBuffer is null)
        {
            Log($"地图地形顶点缓冲创建失败：{vbErr}");
            ClearMapTerrain();
            return;
        }

        _mapTerrainIndexBuffer = VulkanStaticModelBuffer.Create(_vk, _deviceOwner,
            mesh.Indices, BufferUsageFlags.IndexBufferBit, out var ibErr);
        if (_mapTerrainIndexBuffer is null)
        {
            Log($"地图地形索引缓冲创建失败：{ibErr}");
            ClearMapTerrain();
            return;
        }

        var bounds = MapBoundsMeshBuilder.BuildBounds(map);
        _mapBoundsVertexBuffer = VulkanStaticModelBuffer.Create(_vk, _deviceOwner,
            bounds, BufferUsageFlags.VertexBufferBit, out var bErr);
        if (_mapBoundsVertexBuffer is null)
        {
            Log($"地图边界线缓冲创建失败：{bErr}");
            ClearMapTerrain();
            return;
        }

        _mapTerrainIndexCount = (uint)mesh.Indices.Length;
        _mapBoundsVertexCount = (uint)bounds.Length;
        _mapSnapshot = map;
        Log($"地图渲染资源已创建：{map.Name} {map.WidthMeters}x{map.DepthMeters} 米");
    }

    public void ClearMapTerrain()
    {
        _mapTerrainVertexBuffer?.Dispose();
        _mapTerrainVertexBuffer = null;
        _mapTerrainIndexBuffer?.Dispose();
        _mapTerrainIndexBuffer = null;
        _mapBoundsVertexBuffer?.Dispose();
        _mapBoundsVertexBuffer = null;
        _mapTerrainIndexCount = 0;
        _mapBoundsVertexCount = 0;
        _mapSnapshot = default;
    }

    void DrawMapTerrain(CommandBuffer cb, float* scene)
    {
        if (_mapTerrainVertexBuffer is null || _mapTerrainIndexBuffer is null) return;
        var vb = _mapTerrainVertexBuffer.Buffer;
        var ib = _mapTerrainIndexBuffer.Buffer;
        ulong offset = 0;
        _vk.CmdBindVertexBuffers(cb, 0, 1, &vb, &offset);
        _vk.CmdBindIndexBuffer(cb, ib, 0, IndexType.Uint32);
        FillScenePushConstants(scene, _renderProjection, Vector3d.Zero,
            Vector3d.Zero, new Vector3d(1, 1, 1), 0.0f, gizmoModeOverride: -14.0f);
        PushSceneConstants(cb, scene);
        _vk.CmdDrawIndexed(cb, _mapTerrainIndexCount, 1, 0, 0, 0);
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
