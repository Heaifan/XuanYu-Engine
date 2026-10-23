namespace XuanYu.Render.Vulkan.Render.Terrain.Offscreen;

public sealed unsafe partial class VulkanOffscreenTerrainRenderer
{
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_device is not null) _vk.DeviceWaitIdle(_device.LogicalDevice);
        _terrainPipeline?.Dispose(); _terrainPipeline = null;
        _frame?.DisposeTerrain();
        _frame?.Dispose(); _frame = null;
        _target?.Dispose(); _target = null;
        _device?.Dispose(); _device = null;
        _instance?.Dispose(); _instance = null;
        _vk.Dispose();
    }
}
