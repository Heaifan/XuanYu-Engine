namespace XuanYu.Render.Vulkan.Render;

internal sealed unsafe partial class VulkanMapSurfaceOwner
{
    bool CreateFailed(string what, string error)
    {
        _log?.Invoke($"{what}失败：{error}");
        return false;
    }

    internal void ClearMapSurface()
    {
        foreach (var buffer in _mapSurfaceFrameVertexBuffers)
            if (buffer is not null) _retireGpuResource(buffer);
        _mapSurfaceFrameVertexBuffers = [];
        if (_mapSurfaceIndexBuffer is not null) _retireGpuResource(_mapSurfaceIndexBuffer);
        if (_mapBoundsVertexBuffer is not null) _retireGpuResource(_mapBoundsVertexBuffer);
        _mapSurfaceIndexBuffer = null; _mapBoundsVertexBuffer = null;
        _mapSurfaceIndexCount = 0;
        _mapBoundsVertexCount = 0;
        _mapSurfacePatchReady = false;
        _hasMapSurfaceResourceKey = false;
    }

    public void Dispose() => ClearMapSurface();
}
