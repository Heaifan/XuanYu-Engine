namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session;

/// <summary>
/// Vertex Buffer + DeviceMemory 释放步骤。
/// 每个 Buffer/Memory 对独立释放，不合并语义。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    private void DisposeUnitBufferStep()
    {
        if (_unitBufOk)
        {
            if (_unitBuffer.Handle != 0) _vk!.DestroyBuffer(_device, _unitBuffer, null);
            if (_unitMemory.Handle != 0) _vk!.FreeMemory(_device, _unitMemory, null);
            _unitBufOk = false;
        }
    }

    private void DisposeGridBufferStep()
    {
        if (_gridBufOk)
        {
            if (_gridBuffer.Handle != 0) _vk!.DestroyBuffer(_device, _gridBuffer, null);
            if (_gridMemory.Handle != 0) _vk!.FreeMemory(_device, _gridMemory, null);
            _gridBufOk = false;
        }
    }

    private void DisposeCursorBufferStep()
    {
        if (_cursorBufOk)
        {
            if (_cursorBuffer.Handle != 0) _vk!.DestroyBuffer(_device, _cursorBuffer, null);
            if (_cursorMemory.Handle != 0) _vk!.FreeMemory(_device, _cursorMemory, null);
            _cursorBufOk = false;
        }
    }
}
