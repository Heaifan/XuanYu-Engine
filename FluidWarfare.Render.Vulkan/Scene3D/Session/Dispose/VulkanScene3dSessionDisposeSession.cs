using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// DisposeSessionResources 释放顺序（13 步）。
/// 由 Dispose() 调用，销毁所有 Vulkan 资源。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    /// <summary>
    /// 释放顺序：
    ///   1.  Swapchain 资源
    ///   2.  Unit Pipeline
    ///   3.  Grid Pipeline
    ///   4.  Pipeline Layout
    ///   5.  Fragment Shader
    ///   6.  Vertex Shader
    ///   7.  Unit Buffer + Memory
    ///   8.  Grid Buffer + Memory
    ///   9.  Cursor Buffer + Memory
    ///  10.  Device
    ///  11.  Surface（函数指针）
    ///  12.  Debug Messenger
    ///  13.  Instance
    /// </summary>
    private void DisposeSessionResources()
    {
        // Step 1
        _swapchainRes?.Dispose();
        _swapchainRes = null;

        // Pipeline/Shader/Buffer 需要有效 Device Handle
        if (_device.Handle == 0) goto ReleaseDeviceAndAbove;

        // Step 2
        if (_unitPipeOk)
            _vk!.DestroyPipeline(_device, _unitPipeline, null);
        // Step 3
        if (_gridPipeOk)
            _vk!.DestroyPipeline(_device, _gridPipeline, null);
        // Step 4
        if (_layoutOk)
            _vk!.DestroyPipelineLayout(_device, _pipelineLayout, null);
        // Step 5
        if (_fragModOk)
            _vk!.DestroyShaderModule(_device, _fragModule, null);
        // Step 6
        if (_vertModOk)
            _vk!.DestroyShaderModule(_device, _vertModule, null);
        // Step 7
        if (_unitBufOk)
        {
            if (_unitBuffer.Handle != 0) _vk!.DestroyBuffer(_device, _unitBuffer, null);
            if (_unitMemory.Handle != 0) _vk!.FreeMemory(_device, _unitMemory, null);
        }
        // Step 8
        if (_gridBufOk)
        {
            if (_gridBuffer.Handle != 0) _vk!.DestroyBuffer(_device, _gridBuffer, null);
            if (_gridMemory.Handle != 0) _vk!.FreeMemory(_device, _gridMemory, null);
        }
        // Step 9
        if (_cursorBufOk)
        {
            if (_cursorBuffer.Handle != 0) _vk!.DestroyBuffer(_device, _cursorBuffer, null);
            if (_cursorMemory.Handle != 0) _vk!.FreeMemory(_device, _cursorMemory, null);
        }

    ReleaseDeviceAndAbove:
        // Step 10
        if (_devOk && _device.Handle != 0)
            _vk!.DestroyDevice(_device, null);

        // Step 11
        if (_surfOk && _fnDestroySurface != 0)
        {
            var fn = Marshal.GetDelegateForFunctionPointer<DestroySurfaceFn>(_fnDestroySurface);
            fn(_instance, _surface, null);
        }

        // Step 12
        if (_debugMessengerScope is not null)
        {
            _debugMessengerScope.Dispose();
            _debugMessengerScope = null;
        }

        // Step 13
        if (_instOk)
            _vk!.DestroyInstance(_instance, null);
    }
}
