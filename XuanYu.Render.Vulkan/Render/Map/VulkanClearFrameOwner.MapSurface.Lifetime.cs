namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    bool CreateFailed(string what, string error)
    {
        Log($"{what}失败：{error}");
        return false;
    }

    public void ClearMapSurface()
    {
        foreach (var buffer in _mapSurfaceFrameVertexBuffers)
            if (buffer is not null) RetireGpuResource(buffer);
        _mapSurfaceFrameVertexBuffers = [];
        if (_mapSurfaceIndexBuffer is not null) RetireGpuResource(_mapSurfaceIndexBuffer);
        if (_mapBoundsVertexBuffer is not null) RetireGpuResource(_mapBoundsVertexBuffer);
        _mapSurfaceIndexBuffer = null; _mapBoundsVertexBuffer = null;
        _mapSurfaceIndexCount = 0;
        _mapBoundsVertexCount = 0;
        _mapSurfacePatchReady = false;
        _hasMapSurfaceResourceKey = false;
    }
}
