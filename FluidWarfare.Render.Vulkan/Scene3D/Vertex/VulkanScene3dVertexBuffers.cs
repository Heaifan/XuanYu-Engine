using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>
/// Vertex Buffer 创建编排器。
/// 对外暴露 Create（Grid/Unit）和 CreateCursor 入口。
/// 实际 Buffer 创建 + 上传委托到 CreateOneBuffer。
/// </summary>
public static unsafe partial class VulkanScene3dVertexBuffers
{
    public static bool Create(Vk vk, Silk.NET.Vulkan.PhysicalDevice pd,
        Silk.NET.Vulkan.Device dev,
        ReadOnlySpan<VulkanScene3dVertex> gridVertices,
        ReadOnlySpan<VulkanScene3dVertex> unitVertices,
        out Silk.NET.Vulkan.Buffer gridBuffer, out DeviceMemory gridMemory,
        out Silk.NET.Vulkan.Buffer unitBuffer, out DeviceMemory unitMemory,
        out int gridVertexCount, out int unitVertexCount,
        out string errorMessage)
    {
        gridBuffer = default; gridMemory = default;
        unitBuffer = default; unitMemory = default;
        gridVertexCount = gridVertices.Length;
        unitVertexCount = unitVertices.Length;
        errorMessage = string.Empty;

        if (gridVertexCount == 0) { errorMessage = "Scene3D VertexBuffer：Grid vertex count 为 0。"; return false; }
        if (unitVertexCount == 0) { errorMessage = "Scene3D VertexBuffer：Unit vertex count 为 0。"; return false; }

        var gridData = VulkanScene3dVertices.ToInterleaved(gridVertices);
        var unitData = VulkanScene3dVertices.ToInterleaved(unitVertices);

        if (!CreateOneBuffer(vk, pd, dev, gridData, out gridBuffer, out gridMemory, out var gridErr))
        { errorMessage = $"Scene3D Grid VertexBuffer：{gridErr}"; return false; }

        if (!CreateOneBuffer(vk, pd, dev, unitData, out unitBuffer, out unitMemory, out var unitErr))
        { vk.DestroyBuffer(dev, gridBuffer, null); vk.FreeMemory(dev, gridMemory, null); gridBuffer = default; gridMemory = default; errorMessage = $"Scene3D Unit VertexBuffer：{unitErr}"; return false; }

        return true;
    }

    public static bool CreateCursor(Vk vk, Silk.NET.Vulkan.PhysicalDevice pd,
        Silk.NET.Vulkan.Device dev,
        ReadOnlySpan<VulkanScene3dVertex> cursorVertices,
        out Silk.NET.Vulkan.Buffer cursorBuffer, out DeviceMemory cursorMemory,
        out int cursorVertexCount,
        out string errorMessage)
    {
        cursorBuffer = default; cursorMemory = default; cursorVertexCount = 0; errorMessage = string.Empty;
        if (cursorVertices.Length == 0) { errorMessage = "Ground Cursor VertexBuffer：顶点数据为空。"; return false; }

        var data = VulkanScene3dVertices.ToInterleaved(cursorVertices);
        if (!CreateOneBuffer(vk, pd, dev, data, out cursorBuffer, out cursorMemory, out var err))
        { errorMessage = $"Ground Cursor VertexBuffer：{err}"; return false; }

        cursorVertexCount = cursorVertices.Length;
        return true;
    }
}
