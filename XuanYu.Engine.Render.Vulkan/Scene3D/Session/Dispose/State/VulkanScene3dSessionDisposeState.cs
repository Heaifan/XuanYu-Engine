namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// Dispose 状态跟踪：资源创建标记的查询与重置。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    /// <summary>设备 Handle 有效，可用于释放设备级资源。</summary>
    private bool IsDeviceValid => _vk is not null && _device.Handle != 0;

    /// <summary>Vk 实例可用（用于 Device/Surface/Instance 释放）。</summary>
    private bool HasVk => _vk is not null;

    /// <summary>
    /// 清除所有资源创建标记。
    /// 在 Dispose 完成后调用，确保不产生二次释放。
    /// </summary>
    private void ClearAllResourceFlags()
    {
        _instOk = false;
        _surfOk = false;
        _devOk = false;
        _vertModOk = false;
        _fragModOk = false;
        _layoutOk = false;
        _gridPipeOk = false;
        _unitPipeOk = false;
        _gridBufOk = false;
        _unitBufOk = false;
        _cursorBufOk = false;
    }
}
